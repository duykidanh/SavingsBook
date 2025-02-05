using MongoDB.Bson;
using SavingsBook.Application.Contracts.Common;

namespace SavingsBook.Application.Contracts.SavingRegulation.Dto;

public class RegulationDto : AuditedEntityDto<ObjectId>
{
    public double MinWithdrawValue { get; set; }
    public List<SavingType> SavingTypes { get; set; } = [];
    public class SavingType
    {
        public string Name { get; set; }
        public int Term { get; set; }
        public double InterestRate { get; set; }
    }
    public int MinWithdrawDay { get; set; }
}