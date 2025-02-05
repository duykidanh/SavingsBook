
using MongoDB.Bson;
using SavingsBook.Application.Contracts.Common;

namespace SavingsBook.Application.Contracts.SavingBook.Dto;

public class SavingBookDto
{
    public ObjectId AccountId { get; set; }
    public Address Address { get; set; }
    public string IdCardNumber { get; set; }
    public List<Regulation> Regulations { get; set; } = [];
    public double Balance { get; set; }
    public string Status { get; set; }

    public class Regulation
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
