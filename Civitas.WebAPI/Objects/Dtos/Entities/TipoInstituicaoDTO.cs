using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Objects.Dtos.Entities
{
    /// <summary>
    /// DTO utilizado para transportar dados referentes às categorias de instituições (TipoInstituicao).
    /// </summary>
    /// <remarks>
    /// Finalidade:
    /// - Gerenciar o cadastro de categorias (CRUD).
    /// - Listagem para seleção no formulário de cadastro de Instituições.
    /// </remarks>
    public class TipoInstituicaoDTO
    {
        /// <summary>
        /// Identificador único do tipo de instituição.
        /// </summary>
        /// <remarks>
        /// Input: Nulo/Ignorado na criação. Obrigatório na edição.
        /// Output: Identificador gerado pelo banco.
        /// </remarks>
        public int Id { get; set; }

        /// <summary>
        /// Nome descritivo da categoria.
        /// </summary>
        /// <remarks>
        /// Obrigatório. Ex: "Escola", "Posto de Saúde", "Prefeitura", "ONG".
        /// </remarks>
        public string Descricao { get; set; }

        /// <summary>
        /// Situação do registro.
        /// </summary>
        /// <remarks>
        /// Valores: <see cref="Situacao"/> (1-Ativo, 2-Inativo).
        /// Regra: Categorias inativas não podem ser vinculadas a novas Instituições.
        /// </remarks>
        public Situacao Situacao { get; set; }
    }
}