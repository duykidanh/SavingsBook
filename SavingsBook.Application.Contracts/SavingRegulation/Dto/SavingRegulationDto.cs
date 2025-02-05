using MongoDB.Bson;
using SavingsBook.Application.Contracts.Common;

namespace SavingsBook.Application.Contracts.SavingRegulation.Dto;

public class SavingRegulationDto : AuditedEntityDto<ObjectId>
{
    public double MinWithDrawValue { get; set; }
    public List<SavingType> SavingTypes { get; set; } = [];
    public class SavingType
    {
        public string Name { get; set; }
        public int Term { get; set; }
        public double InterestRate { get; set; }
    }
    public int  MinWithDrawDay { get; set; }
}