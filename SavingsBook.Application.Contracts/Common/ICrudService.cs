namespace SavingsBook.Application.Contracts.Common;

public interface ICrudService<TEntityDto, TKey,  TGetListInput,  TCreateInput, TUpdateInput>
{
    Task<TEntityDto> CreateAsync(TCreateInput input);
    Task<TEntityDto> UpdateAsync(TKey id, TUpdateInput input);
    Task<TEntityDto> GetAsync(TKey id);
    Task<PageResultDto<TEntityDto>> GetListAsync(TGetListInput input);
    Task<TEntityDto> DeleteAsync(TKey id);
}