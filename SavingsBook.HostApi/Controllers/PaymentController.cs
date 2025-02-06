using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using SavingsBook.Application.Contracts.Payment;
using SavingsBook.Application.Contracts.Payment.Dto;
using SavingsBook.Application.Contracts.SavingBook;

namespace SavingsBook.HostApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly IMongoCollection<SavingBook> _savingBook;
    private readonly IMongoCollection<TransactionTicket> _transactionTicket;

    public PaymentController(IMongoDatabase database)
    {
        _savingBook = database.GetCollection<SavingBook>("SavingBooks");
        _transactionTicket = database.GetCollection<TransactionTicket>("TransactionTickets");
    }

    [HttpGet]
    public async Task<IActionResult> GetTransactionTickets()
    {
        var transactionTickets = _transactionTicket.Find(_ => true).ToList();

        return Ok(transactionTickets);
    }

    [HttpPost("{savingBookId}/deposit")]
    public async Task<IActionResult> CreateDepositPayment(string savingBookId, [FromBody] CreatePayment input)
    {
        var bookId = ObjectId.Parse(savingBookId);

        var savingBook = _savingBook.Find(m => m.Id == bookId).FirstOrDefault();
        if(savingBook == null)
        {
            return StatusCode(400, new { message = "Cannot find Saving Book" });
        }
  
        var regulation = savingBook.Regulations.LastOrDefault();
        if(regulation == null)
        {
            return StatusCode(400, new { message = "Cannot find Regulation" });
        }

        if(regulation.TermInMonth != 0)
        {
            return StatusCode(400, new { message = "Cannot deposit this Saving Book" });
        }

        savingBook.Balance = savingBook.Balance + input.PaymentAmount;

        var transactionTicket = new TransactionTicket
        {
            Id = ObjectId.GenerateNewId(),
            TransactionDate = DateTime.UtcNow,
            PaymentAmount = input.PaymentAmount,
            PaymentType = "deposit",
            TermInMonth = regulation.TermInMonth,
            Name = regulation.Name,
            InterestRate = regulation.InterestRate,
            Status = "success",
            SavingBookId = savingBook.Id
        };
        await _savingBook.ReplaceOneAsync(m => m.Id == bookId, savingBook);
        await _transactionTicket.InsertOneAsync(transactionTicket);

        return Ok(new {message = "Create Transaction Ticket Successfully", transactionTicket});
    }

    [HttpPost("{savingBookId}/withdraw")]
    public async Task<IActionResult> CreateWithdrawPayment(string savingBookId, [FromBody] CreatePayment input)
    {
        var bookId = ObjectId.Parse(savingBookId);

        var savingBook = _savingBook.Find(m => m.Id == bookId).FirstOrDefault();
        if (savingBook == null)
        {
            return StatusCode(400, new { message = "Cannot find Saving Book" });
        }

        var regulation = savingBook.Regulations.LastOrDefault();
        if (regulation == null)
        {
            return StatusCode(400, new { message = "Cannot find Regulation" });
        }

        if (regulation.TermInMonth != 0)
        {
            if(savingBook.Status.Equals("expired"))
            {
                savingBook.Balance = 0;
                savingBook.Status = "closed";
            }
            else
            {
                return StatusCode(400, new { message = "The withdrawal deadline has not yet reached" });
            }
        }
        else
        {
            savingBook.Balance = savingBook.Balance - input.PaymentAmount;
            if(savingBook.Balance <= 0)
            {
                savingBook.Status = "closed";
            }
        }

        var transactionTicket = new TransactionTicket
        {
            Id = ObjectId.GenerateNewId(),
            TransactionDate = DateTime.UtcNow,
            PaymentAmount = input.PaymentAmount,
            PaymentType = "withdraw",
            TermInMonth = regulation.TermInMonth,
            Name = regulation.Name,
            InterestRate = regulation.InterestRate,
            Status = "success",
            SavingBookId = savingBook.Id
        };

        await _savingBook.ReplaceOneAsync(m => m.Id == bookId, savingBook);
        await _transactionTicket.InsertOneAsync(transactionTicket);
        return Ok(new { message = "Create Transaction Ticket Successfully", transactionTicket });
    }
}