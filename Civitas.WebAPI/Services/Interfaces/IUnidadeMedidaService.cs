using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Services.Interfaces
{
    /// <summary>
    /// Interface que define o contrato para o serviço de gerenciamento de Unidades de Medida.
    /// </summary>
    /// <remarks>
    /// Herda as operações padrão de <see cref="IGenericService{UnidadeMedida, UnidadeMedidaDTO}"/>.
    /// Serve como ponto de injeção de dependência para a lógica que provê as grandezas físicas (kWh, m³, etc)
    /// utilizadas nos cálculos de consumo e eficiência.
    /// </remarks>
    public interface IUnidadeMedidaService : IGenericService<UnidadeMedida, UnidadeMedidaDTO>
    {
    }
}