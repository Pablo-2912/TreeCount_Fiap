using TreeCount.Application.ViewModels;

namespace TreeCount.API.Interfaces
{
    public interface IControllerBase<TCreateDto, TUpdateDto, TListDto, TId, TCreateResponse, TUpdateResponse, TDeleteResponse, TGetByIdResponse, TListResponse>
    {
        Task<TCreateResponse> CreateAsync(TCreateDto dto);

        Task<TUpdateResponse> UpdateAsync(TUpdateDto dto);

        Task<TDeleteResponse> DeleteAsync(TId id);

        Task<IEnumerable<TListResponse>> ListPaginatedAsync(TListDto dto);

        Task<IEnumerable<TGetByIdResponse>> ListPaginatedAsync(TId id);
    }

}
