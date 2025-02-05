namespace SavingsBook.Application.Contracts.SavingRegulation.Dto;

public class CreateUpdateRegulationDto
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
    public bool IsActive { get; set; }

}