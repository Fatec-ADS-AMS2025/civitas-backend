using Civitas.WebAPI.Objects.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Civitas.WebAPI.Objects.Models
{
    /// <summary>
    /// Entidade que representa um prestador de serviços ou vendedor de produtos para o sistema (Fornecedor).
    /// Mapeia a tabela 'fornecedor' do banco de dados.
    /// </summary>
    /// <remarks>
    /// O Fornecedor é a contraparte externa nas transações financeiras. 
    /// É obrigatório que possua dados fiscais válidos (CNPJ) para a emissão de despesas.
    /// </remarks>
    [Table("fornecedor")]
    public class Fornecedor
    {
        /// <summary>
        /// Identificador único do fornecedor (Chave Primária).
        /// </summary>
        [Column("idfornecedor")]
        public int IdFornecedor { get; set; }

        /// <summary>
        /// Nome comercial ou marca pela qual a empresa é conhecida publicamente.
        /// </summary>
        /// <example>Kalunga, Copel, Microsoft.</example>
        [Column("nomefantasia")]
        public string NomeFantasia { get; set; }

        /// <summary>
        /// Situação cadastral do fornecedor.
        /// </summary>
        /// <remarks>
        /// Controlado pelo Enum <see cref="Situacao"/>:
        /// 1 - ATIVO: Apto a receber pagamentos e ser vinculado a novas despesas.
        /// 2 - INATIVO: Bloqueado para novas operações (ex: empresa faliu ou foi descredenciada).
        /// </remarks>
        [Column("situacao")]
        public Situacao Situacao { get; set; }

        /// <summary>
        /// Cadastro Nacional da Pessoa Jurídica.
        /// </summary>
        /// <remarks>
        /// Regra: Deve ser validado via algoritmo de módulo 11 e ser único na base de fornecedores.
        /// </remarks>
        [Column("cnpj")]
        public string Cnpj { get; set; }

        /// <summary>
        /// Razão Social ou nome oficial registrado no contrato social da empresa.
        /// </summary>
        /// <remarks>
        /// Utilizado para emissão de notas fiscais e contratos formais.
        /// </remarks>
        [Column("nome")]
        public string Nome { get; set; }

        /// <summary>
        /// Nome da rua, avenida ou logradouro da sede da empresa.
        /// </summary>
        [Column("logradouro")]
        public string Logradouro { get; set; }

        /// <summary>
        /// Número do imóvel comercial.
        /// </summary>
        [Column("numero")]
        public string Numero { get; set; }

        /// <summary>
        /// Bairro ou distrito.
        /// </summary>
        [Column("bairro")]
        public string Bairro { get; set; }

        /// <summary>
        /// Código de Endereçamento Postal.
        /// </summary>
        [Column("cep")]
        public string Cep { get; set; }

        /// <summary>
        /// Telefone comercial para contato.
        /// </summary>
        [Column("telefone")]
        public string Telefone { get; set; }

        /// <summary>
        /// Endereço de e-mail para envio de comprovantes e notificações.
        /// </summary>
        [Column("email")]
        public string Email { get; set; }

        /// <summary>
        /// Cidade sede do fornecedor.
        /// </summary>
        [Column("cidade")]
        public string Cidade { get; set; }

        /// <summary>
        /// Unidade Federativa (UF) do fornecedor.
        /// </summary>
        [Column("estado")]
        public string Estado { get; set; }

        /// <summary>
        /// Coleção de documentos legais (contratos, certidões negativas) vinculados ao fornecedor.
        /// </summary>
        public ICollection<Documento> Documentos { get; set; }

        /// <summary>
        /// Histórico de despesas e pagamentos realizados para este fornecedor.
        /// </summary>
        public ICollection<Despesa> Despesas { get; set; }

        /// <summary>
        /// Construtor para inicialização completa da entidade Fornecedor.
        /// </summary>
        public Fornecedor(int idFornecedor, string nomeFantasia, Situacao situacao, string cnpj, string nome, string logradouro, string numero, string bairro, string cep
            , string telefone, string email, string cidade, string estado)
        {
            IdFornecedor = idFornecedor;
            NomeFantasia = nomeFantasia;
            Situacao = situacao;
            Cnpj = cnpj;
            Nome = nome;
            Logradouro = logradouro;
            Numero = numero;
            Bairro = bairro;
            Cep = cep;
            Telefone = telefone;
            Email = email;
            Cidade = cidade;
            Estado = estado;
        }
    }
}