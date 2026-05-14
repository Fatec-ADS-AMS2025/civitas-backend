using Civitas.WebAPI.Objects.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Civitas.WebAPI.Objects.Models
{
    /// <summary>
    /// Entidade que representa um usuário cadastrado no sistema Civitas.
    /// Mapeia a tabela 'usuario' do banco de dados.
    /// </summary>
    /// <remarks>
    /// Esta classe centraliza as informações de acesso (Login), dados pessoais e endereço.
    /// Diferentes perfis (Visitante, Administrador, Funcionário) compartilham esta mesma estrutura.
    /// </remarks>
    [Table("usuario")]
    public class Usuario : ISoftDeletable
    {
        /// <summary>
        /// Identificador único do usuário (Chave Primária).
        /// </summary>
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// Cadastro de Pessoa Física do usuário.
        /// </summary>
        /// <remarks>
        /// Formato: Deve conter apenas números ou máscara padrão (000.000.000-00).
        /// Regra: Campo único no sistema, não pode haver duplicidade.
        /// </remarks>
        [Column("cpf")]
        public string Cpf { get; set; }

        /// <summary>
        /// Nome completo do usuário.
        /// </summary>
        [Column("nome")]
        public string Nome { get; set; }

        /// <summary>
        /// Registro Geral (Documento de Identidade).
        /// </summary>
        [Column("rg")]
        public string Rg { get; set; }

        /// <summary>
        /// Nome da rua, avenida ou logradouro do endereço.
        /// </summary>
        [Column("logradouro")]
        public string Logradouro { get; set; }

        /// <summary>
        /// Número do imóvel.
        /// </summary>
        [Column("numero")]
        public string Numero { get; set; }

        /// <summary>
        /// Cidade de residência.
        /// </summary>
        [Column("cidade")]
        public string Cidade { get; set; }

        /// <summary>
        /// Unidade Federativa (UF) do endereço.
        /// </summary>
        /// <example>SP, RJ, MG</example>
        [Column("estado")]
        public string Estado { get; set; }

        /// <summary>
        /// Código de Endereçamento Postal.
        /// </summary>
        [Column("cep")]
        public string Cep { get; set; }

        /// <summary>
        /// Bairro ou distrito.
        /// </summary>
        [Column("bairro")]
        public string Bairro { get; set; }

        /// <summary>
        /// Endereço de e-mail para contato e login.
        /// </summary>
        /// <remarks>
        /// Regra: Deve ser um formato de e-mail válido e único no sistema.
        /// </remarks>
        [Column("email")]
        public string Email { get; set; }

        /// <summary>
        /// Credencial de acesso do usuário.
        /// </summary>
        /// <remarks>
        /// Segurança: Deve ser armazenada como Hash encriptado, nunca em texto plano.
        /// </remarks>
        [Column("senha")]
        public string Senha { get; set; }

        /// <summary>
        /// Código de matrícula interna.
        /// </summary>
        /// <remarks>
        /// Relevante principalmente para usuários do tipo <see cref="TipoUsuario.FUNCIONARIO"/> ou <see cref="TipoUsuario.ADMINISTRADOR"/>.
        /// </remarks>
        [Column("matricula")]
        public string Matricula { get; set; }

        /// <summary>
        /// Define o nível de permissão do usuário no sistema.
        /// </summary>
        /// <remarks>
        /// Valores possíveis baseados no Enum <see cref="TipoUsuario"/>:
        /// 1 - VISITANTE (Acesso limitado/público)
        /// 2 - ADMINISTRADOR (Gestão total)
        /// 3 - FUNCIONARIO (Operacional)
        /// </remarks>
        [Column("tipousuario")]
        public TipoUsuario TipoUsuario { get; set; }

        /// <summary>
        /// Estado atual do cadastro do usuário.
        /// </summary>
        /// <remarks>
        /// Valores baseados no Enum <see cref="Situacao"/>:
        /// 1 - ATIVO (Acesso liberado)
        /// 2 - INATIVO (Acesso bloqueado/Soft Delete)
        /// </remarks>
        [Column("situacao")]
        public Situacao Situacao { get; set; }

        /// <summary>
        /// Coleção de despesas vinculadas a este usuário.
        /// </summary>
        public ICollection<Despesa> Despesas { get; set; }

        /// <summary>
        /// Construtor padrão para inicialização completa da entidade Usuario.
        /// </summary>

        [Column("excluido")]
        public bool Excluido { get; set; }

        [Column("dataexclusao")]
        public DateTime? DataExclusao { get; set; }

        public Usuario(int id, string cpf, string nome, string rg, string logradouro, string numero, string cidade, string estado, string cep, string email, string senha
            , Situacao situacao, string matricula, TipoUsuario tipoUsuario, string bairro)
        {
            Id = id;
            Cpf = cpf;
            Nome = nome;
            Rg = rg;
            Logradouro = logradouro;
            Numero = numero;
            Cidade = cidade;
            Estado = estado;
            Cep = cep;
            Email = email;
            Senha = senha;
            Situacao = situacao;
            Matricula = matricula;
            TipoUsuario = tipoUsuario;
            Bairro = bairro;
        }
    }
}
