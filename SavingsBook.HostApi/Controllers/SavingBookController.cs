using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

using SavingsBook.Application.Contracts.SavingBook;
using SavingsBook.Application.Contracts.SavingBook.Dto;
using SavingsBook.Application.Contracts.SavingRegulation;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

﻿using Microsoft.AspNetCore.Http.HttpResults;

namespace SavingsBook.HostApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SavingBookController : ControllerBase
{
    private readonly IMongoCollection<SavingBook> _savingBooks;
    private readonly IMongoCollection<Regulation> _regulations;

    public SavingBookController(IMongoDatabase database)
    {
        _savingBooks = database.GetCollection<SavingBook>("SavingBooks");
        _regulations = database.GetCollection<Regulation>("Regulations");
    }

    [HttpGet("tokens")]
    [Authorize]
    public async Task<IActionResult> GetToken()
    {
        // Lấy "Id" từ token (nếu có)
        var userIdString = User.FindFirstValue("Id");

        if (string.IsNullOrEmpty(userIdString))
        {
            return Unauthorized(new { message = "User ID not found in token." });
        }

        return Ok(new { UserId = userIdString });  // Trả về giá trị từ claim
    }

    [HttpPost]
    public async Task<IActionResult> CreateSavingBook([FromBody] CreateUpdateSavingBookDto input)
    {
        if (input == null)
        {
            return BadRequest(new { message = "Invalid input data." });
        }

        var userIdString = User.FindFirstValue("Id");
        if (string.IsNullOrEmpty(userIdString) || !ObjectId.TryParse(userIdString, out var userId))
        {
            return Unauthorized(new { message = "Invalid or missing user ID in token." });
        }
        var regulations = await _regulations
        .Find(_ => true)
        .ToListAsync();

        if (regulations == null || !regulations.Any())
        {
            return NotFound(new { message = "No regulations found." });
        }

        var latestRegulation = regulations
            .OrderByDescending(r => r.LastModificationTime ?? r.CreationTime)
            .FirstOrDefault();

        var savingType = latestRegulation.SavingTypes.Find(r => r.Term == input.Term);

        var savingBook = new SavingBook
        {
            AccountId = userId,
            Id = ObjectId.GenerateNewId(),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            CreationTime = DateTime.UtcNow,
            Balance = input.NewPaymentAmount,
            Address = input.Address,
            Regulations = new List<SavingBook.Regulation>(),
            IsActive = true,
            Status = "active"
        };

        savingBook.Regulations.Add(new SavingBook.Regulation
        {
            RegulationIdRef = latestRegulation.Id,
            ApplyDate = DateTime.UtcNow,
            Name = savingType.Name,
            TermInMonth = savingType.Term,
            MinWithDrawDay = latestRegulation.MinWithdrawDay,
            MinWithDrawValue = latestRegulation.MinWithdrawValue,
            InterestRate = savingType.InterestRate,
            ExpiredDate = DateTime.UtcNow.AddMonths(savingType.Term),
            UpdatedBalanceDate = DateTime.UtcNow
        });

        await _savingBooks.InsertOneAsync(savingBook);

        return Ok(new { message = "Saving book created successfully.", id = savingBook.Id });
        
    }

    [HttpGet]
    public async Task<IActionResult> GetSavingBooks()
    {
        var savingBooks = _savingBooks.Find(_ => true).ToList();

        return Ok(savingBooks);

    }

    public async Task UpdateBalancesAsync()
    {
        var savingBooks = _savingBooks.Find(_ => true).ToList();

        foreach (var savingBook in savingBooks)
        {
            foreach (var regulation in savingBook.Regulations)
            {
                var daysPassed = (DateTime.UtcNow - regulation.ApplyDate).TotalDays;

                if (daysPassed >= regulation.TermInMonth * 30)
                {
                    double interest = savingBook.Balance * regulation.InterestRate / 100 * (daysPassed / 365);
                    savingBook.Balance += interest;

                    regulation.ApplyDate = DateTime.UtcNow;
                }
            }

            await _savingBooks.ReplaceOneAsync(
                Builders<SavingBook>.Filter.Eq(s => s.Id, savingBook.Id),
                savingBook
            );
        }
    }
}

