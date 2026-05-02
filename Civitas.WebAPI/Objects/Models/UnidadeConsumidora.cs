using System.ComponentModel.DataAnnotations.Schema;

namespace Civitas.WebAPI.Objects.Models
{
    /// <summary>
    /// Entidade que representa uma Unidade Consumidora (UC) vinculada ao controle de despesas da prefeitura.
    /// Mapeia a tabela 'unidadeconsumidora' do banco de dados.
    /// </summary>
    /// <remarks>
    /// Usada para identificar pontos de consumo (energia, agua, telefonia, frota etc.) por meio de identificadores
    /// como RENAVAM, HIDROMETRO, MEDIDOR. Vincula-se a Instituicao, TipoDespesa, Secretaria, Orcamento e Fornecedor.
    /// Suporta soft delete via <see cref="Excluido"/> e <see cref="DataExclusao"/>.
    /// </remarks>
    [Table("unidadeconsumidora")]
    public class UnidadeConsumidora
    {
        /// <summary>
        /// Identificador unico da unidade consumidora (Chave Primaria).
        /// </summary>
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// Codigo identificador externo da unidade consumidora.
        /// </summary>
        /// <example>RENAVAM, HIDROMETRO, MEDIDOR.</example>
        [Column("identificador")]
        public string Identificador { get; set; } = string.Empty;

        /// <summary>
        /// Chave estrangeira para a Instituicao vinculada.
        /// </summary>
        [Column("idinstituicao")]
        public int IdInstituicao { get; set; }

        /// <summary>
        /// Objeto de navegacao da Instituicao vinculada.
        /// </summary>
        public Instituicao? Instituicao { get; set; }

        /// <summary>
        /// Chave estrangeira para o TipoDespesa vinculado.
        /// </summary>
        [Column("idtipodespesa")]
        public int IdTipoDespesa { get; set; }

        /// <summary>
        /// Objeto de navegacao do TipoDespesa vinculado.
        /// </summary>
        public TipoDespesa? TipoDespesa { get; set; }

        /// <summary>
        /// Chave estrangeira para a Secretaria vinculada.
        /// </summary>
        [Column("idsecretaria")]
        public int IdSecretaria { get; set; }

        /// <summary>
        /// Objeto de navegacao da Secretaria vinculada.
        /// </summary>
        public Secretaria? Secretaria { get; set; }

        /// <summary>
        /// Chave estrangeira para o Orcamento vinculado.
        /// </summary>
        [Column("idorcamento")]
        public int IdOrcamento { get; set; }

        /// <summary>
        /// Objeto de navegacao do Orcamento vinculado.
        /// </summary>
        public Orcamento? Orcamento { get; set; }

        /// <summary>
        /// Chave estrangeira para o Fornecedor vinculado.
        /// </summary>
        [Column("idfornecedor")]
        public int IdFornecedor { get; set; }

        /// <summary>
        /// Objeto de navegacao do Fornecedor vinculado.
        /// </summary>
        public Fornecedor? Fornecedor { get; set; }

        /// <summary>
        /// Indica se o registro esta marcado como excluido (soft delete).
        /// </summary>
        /// <remarks>
        /// false = ativo; true = excluido logicamente. Por padrao, novos registros sao criados como false.
        /// </remarks>
        [Column("excluido")]
        public bool Excluido { get; set; }

        /// <summary>
        /// Data e hora em que o registro foi marcado como excluido.
        /// </summary>
        /// <remarks>
        /// Permanece null em registros ativos. E preenchida no momento da exclusao logica
        /// e retorna a null se o registro for restaurado.
        /// </remarks>
        [Column("dataexclusao")]
        public DateTime? DataExclusao { get; set; }

        public UnidadeConsumidora() { }

        public UnidadeConsumidora(
            int id,
            string identificador,
            int idInstituicao,
            int idTipoDespesa,
            int idSecretaria,
            int idOrcamento,
            int idFornecedor,
            bool excluido,
            DateTime? dataExclusao)
        {
            Id = id;
            Identificador = identificador;
            IdInstituicao = idInstituicao;
            IdTipoDespesa = idTipoDespesa;
            IdSecretaria = idSecretaria;
            IdOrcamento = idOrcamento;
            IdFornecedor = idFornecedor;
            Excluido = excluido;
            DataExclusao = dataExclusao;
        }
    }
}
