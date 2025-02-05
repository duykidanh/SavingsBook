using MongoDB.Driver;
using SavingsBook.Application.Contracts.Common;
using SavingsBook.Application.Contracts.SavingBook.Dto;

namespace SavingsBook.Application.Contracts.SavingBook;

public class SavingBookService : ISavingBookService
{
    private readonly IMongoCollection<SavingBook> _savingBooks;

    public SavingBookService(IMongoDatabase database)
    {
        _savingBooks = database.GetCollection<SavingBook>("SavingBooks");
    }

    public Task<ResponseDto<SavingBookDto>> CreateAsync(CreateUpdateSavingBookDto input)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto<bool>> DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto<SavingBookDto>> GetAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto<PageResultDto<SavingBookDto>>> GetListAsync(QuerySavingBookDto input, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto<SavingBookDto>> UpdateAsync(Guid id, CreateUpdateSavingBookDto input)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateBalancesAsync()
    {
        var savingBooks = _savingBooks.Find(_ => true).ToList();

        foreach (var savingBook in savingBooks)
        {
            if (savingBook.Status.Equals("active"))
            {
                var regulation = savingBook.Regulations.LastOrDefault();

                if (regulation != null)
                {
                    var daysPassed = (DateTime.UtcNow - regulation.UpdatedBalanceDate).TotalDays;
                    double interest = 0;
                    if (regulation.TermInMonth == 0)
                    {
                        for (int i = 0; i < Convert.ToInt32(daysPassed); ++i)
                        {
                            interest = savingBook.Balance * regulation.InterestRate;
                            savingBook.Balance += interest;
                        }
                        regulation.UpdatedBalanceDate = DateTime.UtcNow;
                    }
                    else
                    {
                        if (DateTime.UtcNow >= regulation.ExpiredDate)
                        {
                            if (savingBook.Balance > 0)
                                daysPassed = (regulation.ExpiredDate - regulation.UpdatedBalanceDate).TotalDays;
                            savingBook.Status = "expired";
                        }
                        for (int i = 0; i < Convert.ToInt32(daysPassed); ++i)
                        {
                            interest = savingBook.Balance * regulation.InterestRate;
                            savingBook.Balance += interest;
                        }
                        regulation.UpdatedBalanceDate = DateTime.UtcNow;
                    }
                }
                await _savingBooks.ReplaceOneAsync(
                    Builders<SavingBook>.Filter.Eq(s => s.Id, savingBook.Id),
                    savingBook
                );
            }
        }
    }
}