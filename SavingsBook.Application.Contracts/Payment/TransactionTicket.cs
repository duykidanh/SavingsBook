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
    public ObjectId AccountId { get; set; }
    public double Balance { get; set; }
    public string IdCardNumber { get; set; }
    public Address Address { get; set; }
    public string Status { get; set; }
    public DateTime NextScheduleMonth { get; set; }
    public List<SavingRegulation> Regulations { get; set; } = [];

    public class SavingRegulation
    {
        public ObjectId RegulationIdRef { get; set; }
        public DateTime ApplyDate { get; set; }
        public string Name { get; set; }
        public int TermInMonth { get; set; }
        public double InterestRate { get; set; }
        public double MinWithDrawValue { get; set; }
        public int MinWithDrawDay { get; set; }
        public DateTime ExpiredDate { get; set; }
        public DateTime UpdatedBalanceDate { get; set; }
    }

}