using MongoDB.Bson;
using SavingsBook.Application.Contracts.Common;
using SavingsBook.Domain.Common;

namespace SavingsBook.Application.Contracts.SavingRegulation;

public class Regulation : AuditedEntityDto<ObjectId>
{
    public double MinWithdrawValue { get; set; }
    public int MinWithdrawDay { get; set; }
    public List<SavingType> SavingTypes { get; set; } = [];
    public class SavingType
    {
        public string Name { get; set; }
        public int Term { get; set; }
        public double InterestRate { get; set; }
    }
}