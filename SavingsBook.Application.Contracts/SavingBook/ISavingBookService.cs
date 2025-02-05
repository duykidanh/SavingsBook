using MongoDB.Driver;
﻿using SavingsBook.Application.Contracts.Common;
using SavingsBook.Application.Contracts.SavingBook.Dto;

namespace SavingsBook.Application.Contracts.SavingBook;

public interface ISavingBookService : ICrudService<SavingBookDto, Guid, QuerySavingBookDto, CreateUpdateSavingBookDto, CreateUpdateSavingBookDto>
{
    Task UpdateBalancesAsync();

}