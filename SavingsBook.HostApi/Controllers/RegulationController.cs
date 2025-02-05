using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using SavingsBook.Application.Contracts.SavingRegulation;
using SavingsBook.Application.Contracts.SavingRegulation.Dto;
using SavingsBook.HostApi.Utility;

namespace SavingsBook.HostApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RegulationController : ControllerBase
{
    private readonly IMongoCollection<Regulation> _savingRegulationCollection;

    public RegulationController(IMongoDatabase database)
    {
        _savingRegulationCollection = database.GetCollection<Regulation>("Regulations");
    }

    [HttpPost]
    public async Task<IActionResult> CreateSavingRegulation([FromBody] CreateUpdateRegulationDto input)
    {
        if (input == null || input.SavingTypes == null || input.SavingTypes.Count == 0)
        {
            return BadRequest(new { message = "Invalid input data." });
        }

        var savingRegulation = new Regulation
        {
            Id = ObjectId.GenerateNewId(),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            CreationTime = DateTime.UtcNow,
            MinWithdrawValue = input.MinWithdrawValue,
            MinWithdrawDay = input.MinWithdrawDay,
            SavingTypes = input.SavingTypes.Select(type => new Regulation.SavingType
            {
                Name = type.Name,
                Term = type.Term,
                InterestRate = type.InterestRate
            }).ToList(),
            IsActive = input.IsActive
        };

        // Insert the new saving regulation into the database
        await _savingRegulationCollection.InsertOneAsync(savingRegulation);

        return Ok(new { message = "Saving regulation created successfully.", id = savingRegulation.Id });
    }

    [HttpGet]
    public async Task<IActionResult> GetSavingRegulations()
    {
        var response = await _savingRegulationCollection.Find(_ => true).ToListAsync();
        return Ok(response);
    }

    [HttpPost("{id}")]
    public async Task<IActionResult> UpdateSavingRegulation(string id, [FromBody] CreateUpdateRegulationDto input)
    {
        if (input == null || input.SavingTypes == null || input.SavingTypes.Count == 0)
        {
            return BadRequest(new { message = "Invalid input data." });
        }

        // Parse the ID to ObjectId
        if (!ObjectId.TryParse(id, out var objectId))
        {
            return BadRequest(new { message = "Invalid regulation ID." });
        }

        // Retrieve the existing regulation
        var existingRegulation = await _savingRegulationCollection.Find(r => r.Id == objectId).FirstOrDefaultAsync();

        if (existingRegulation == null)
        {
            return NotFound(new { message = "Regulation not found." });
        }

        // Update the properties
        existingRegulation.ConcurrencyStamp = Guid.NewGuid().ToString(); // Update the concurrency stamp
        existingRegulation.MinWithdrawValue = input.MinWithdrawValue;
        existingRegulation.MinWithdrawDay = input.MinWithdrawDay;
        existingRegulation.IsActive = input.IsActive;
        existingRegulation.LastModificationTime = DateTime.UtcNow;
        existingRegulation.SavingTypes = input.SavingTypes.Select(type => new Regulation.SavingType
        {
            Name = type.Name,
            Term = type.Term,
            InterestRate = type.InterestRate
        }).ToList();

        // Update the regulation in the database
        await _savingRegulationCollection.ReplaceOneAsync(r => r.Id == objectId, existingRegulation);

        return Ok(new { message = "Regulation updated successfully." });
    }

    [HttpGet("latest")]
    public async Task<IActionResult> GetLatestRegulation()
    {
        // Fetch all regulations
        var regulations = await _savingRegulationCollection
            .Find(_ => true)
            .ToListAsync();

        if (regulations == null || !regulations.Any())
        {
            return NotFound(new { message = "No regulations found." });
        }

        // Determine the latest regulation based on LastModificationTime or CreationTime
        var latestRegulation = regulations
            .OrderByDescending(r => r.LastModificationTime ?? r.CreationTime)
            .FirstOrDefault();

        return Ok(latestRegulation);
    }
    /*    [HttpPut]
        public async Task<IActionResult> PutSavingRegulation([FromQuery] Guid id,
            [FromBody] CreateUpdateSavingRegulationDto input)
        {
            var response = await _savingRegulationService.UpdateAsync(id, input);
            return ResponseHelper.FormatResponse(response);
        }*/

    /*    [HttpGet]
        public async Task<IActionResult> GetSavingRegulationList([FromQuery] QuerySavingRegulationDto input, CancellationToken cancellationToken)
        {
            var response = await _savingRegulationService.GetListAsync(input, cancellationToken);
            return ResponseHelper.FormatResponse(response);
        }*/

    /*    [HttpDelete]
        public async Task<IActionResult> DeleteSavingRegulation([FromQuery] Guid id)
        {
            var response = await _savingRegulationService.DeleteAsync(id);
            return ResponseHelper.FormatResponse(response);
        }*/
}