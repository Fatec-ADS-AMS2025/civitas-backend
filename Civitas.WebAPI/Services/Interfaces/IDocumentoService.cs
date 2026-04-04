using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Services.Interfaces
{
    /// <summary>
    /// Interface que define o contrato para o serviço de gerenciamento de documentos e arquivos digitais.
    /// </summary>
    /// <remarks>
    /// Esta interface herda as operações básicas de <see cref="IGenericService{Documento, DocumentoDTO}"/>.
    /// É utilizada para injetar a lógica de manipulação de arquivos (comprovantes, notas fiscais) nas controllers e outros serviços.
    /// </remarks>
    public interface IDocumentoService : IGenericService<Documento, DocumentoDTO>
    {

    }
}