namespace SavingsBook.Application.Contracts.Common;

public interface ICrudService<TEntityDto, TKey, TGetListInput, TCreateInput, TUpdateInput>
{
    Task<ResponseDto<TEntityDto>> CreateAsync(TCreateInput input);
    Task<ResponseDto<TEntityDto>> UpdateAsync(TKey id, TUpdateInput input);
    Task<ResponseDto<TEntityDto>> GetAsync(TKey id);

    Task<ResponseDto<PageResultDto<TEntityDto>>> GetListAsync(TGetListInput input, CancellationToken cancellationToken
    );

    Task<ResponseDto<bool>> DeleteAsync(TKey id);
}