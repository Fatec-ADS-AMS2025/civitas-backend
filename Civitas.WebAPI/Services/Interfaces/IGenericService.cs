namespace Civitas.WebAPI.Services.Interfaces
{
    public interface IGenericService<T, TDto> where T : class where TDto : class
    {
        Task<IEnumerable<TDto>> GetAll();
        Task<TDto> GetById(int id);
        Task Create(TDto entityDTO);
        Task Update(TDto entityDTO, int id);
        Task Remove(int id);
    }
}
