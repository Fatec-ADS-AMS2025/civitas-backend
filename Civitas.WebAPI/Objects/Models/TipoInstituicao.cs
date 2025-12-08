using Civitas.WebAPI.Objects.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Civitas.WebAPI.Objects.Models
{
    /// <summary>
    /// Entidade responsável por categorizar as instituições cadastradas no sistema.
    /// Mapeia a tabela 'tipoinstituicao' do banco de dados.
    /// </summary>
    /// <remarks>
    /// Exemplos de categorias: ONG, Escola, Prefeitura, Posto de Saúde, Associação de Moradores.
    /// </remarks>
    [Table("tipoinstituicao")]
    public class TipoInstituicao
    {
        /// <summary>
        /// Identificador único do tipo de instituição (Chave Primária).
        /// </summary>
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// Nome descritivo da categoria.
        /// </summary>
        /// <example>Organização Não Governamental, Órgão Público.</example>
        [Column("descricao")]
        public string Descricao { get; set; }

        /// <summary>
        /// Define se esta categoria está ativa para ser vinculada a novas instituições.
        /// </summary>
        /// <remarks>
        /// Baseado no Enum <see cref="Situacao"/>:
        /// 1 - ATIVO: Categoria visível em listagens.
        /// 2 - INATIVO: Categoria depreciada.
        /// </remarks>
        [Column("situacao")]
        public Situacao Situacao { get; set; }

        /// <summary>
        /// Relacionamento: Lista de instituições que pertencem a esta categoria.
        /// </summary>
        public ICollection<Instituicao> Instituicoes { get; set; }

        /// <summary>
        /// Construtor para inicialização da entidade TipoInstituicao.
        /// </summary>
        public TipoInstituicao(int id, string descricao, Situacao situacao)
        {
            Id = id;
            Descricao = descricao;
            Situacao = situacao;
        }
    }
}