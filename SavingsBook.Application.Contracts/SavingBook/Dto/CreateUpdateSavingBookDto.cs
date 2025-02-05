
﻿using MongoDB.Bson;
using SavingsBook.Application.Contracts.SavingRegulation;
using SavingsBook.Domain.Common;

﻿using SavingsBook.Application.Contracts.Common;


namespace SavingsBook.Application.Contracts.SavingBook.Dto;

public class CreateUpdateSavingBookDto
{
    public double NewPaymentAmount { get; set; }
    public string IdCardNumber { get; set; }
    public Address Address { get; set; }
    public int Term { get; set; }

}