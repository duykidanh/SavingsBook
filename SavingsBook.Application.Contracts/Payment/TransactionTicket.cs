using MongoDB.Bson;
using SavingsBook.Application.Contracts.Common;
using SavingsBook.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavingsBook.Application.Contracts.Payment;

public class TransactionTicket : AuditedEntityDto<ObjectId>
{
    public ObjectId SavingBookId { get; set; }
    public DateTime TransactionDate { get; set; }
    public double PaymentAmount { get; set; }
    public string PaymentType { get; set; } = string.Empty;
    public int TermInMonth { get; set; }
    public string Name { get; set; } = string.Empty;
    public double InterestRate { get; set; }
    public string Status {  get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PaymentLink { get; set; } = string.Empty;
    public string PaymentId { get; set; } = string.Empty;
}