using AutoMapper;
using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Services.Interfaces;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Civitas.WebAPI.Services.Entities
{
    /// <summary>
    /// Serviço responsável pela administração das Secretarias e órgãos gestores superiores.
    /// </summary>
    /// <remarks>
    /// Finalidade:
    /// - Gerenciar o cadastro das entidades que agrupam as instituições (ex: Secretaria de Educação).
    /// - Centralizar a gestão de dados fiscais (CNPJ) e contatos dos órgãos públicos.
    /// 
    /// Regras de Negócio:
    /// - Uma Secretaria atua como "pai" na hierarquia, sendo mandatória para a criação de Instituições.
    /// 
    /// Dependências:
    /// - <see cref="ISecretariaRepository"/>: Camada de persistência.
    /// - <see cref="IMapper"/>: Conversão de dados.
    /// </remarks>
    public class SecretariaService : GenericService<Secretaria, SecretariaDTO>, ISecretariaService
    {
        private readonly ISecretariaRepository _secretariaRepository;
        private static readonly HashSet<string> UFsValidas =
        [
            "AC", "AL", "AP", "AM", "BA", "CE", "DF", "ES", "GO", "MA", "MT", "MS", "MG",
            "PA", "PB", "PR", "PE", "PI", "RJ", "RN", "RS", "RO", "RR", "SC", "SP", "SE", "TO"
        ];

        /// <summary>
        /// Inicializa o serviço de Secretarias.
        /// </summary>
        /// <param name="secretariaRepository">Repositório injetado para acesso a dados.</param>
        /// <param name="mapper">Mapeador de objetos.</param>
        /// <exception cref="ArgumentNullException">Lançada se as dependências não forem resolvidas.</exception>
        public SecretariaService(ISecretariaRepository secretariaRepository, IMapper mapper)
            : base(secretariaRepository, mapper)
        {
            _secretariaRepository = secretariaRepository;
        }

        public async Task ValidarCadastroAsync(SecretariaDTO entityDTO, int? id = null)
        {
            if (entityDTO is null)
            {
                throw new ArgumentException("Dados da secretaria são obrigatórios.");
            }

            if (id.HasValue && id.Value <= 0)
            {
                throw new ArgumentException("Id da secretaria inválido.");
            }

            if (id.HasValue)
            {
                var existingEntity = await _secretariaRepository.GetById(id.Value);
                if (existingEntity is null)
                {
                    throw new KeyNotFoundException($"Entidade com id {id.Value} não encontrada.");
                }
            }

            entityDTO.IdSecretaria = id ?? 0;

            NormalizarCampos(entityDTO);
            ValidarCampos(entityDTO);
            await ValidarUnicidade(entityDTO, id);
        }

        private async Task ValidarUnicidade(SecretariaDTO secretariaDTO, int? ignoreId)
        {
            var cnpjDuplicado = await _secretariaRepository.ExistsByCnpjAsync(secretariaDTO.Cnpj, ignoreId);
            if (cnpjDuplicado)
            {
                throw new ArgumentException("CNPJ já cadastrado no sistema.");
            }

            var emailDuplicado = await _secretariaRepository.ExistsByEmailAsync(secretariaDTO.Email, ignoreId);
            if (emailDuplicado)
            {
                throw new ArgumentException("E-mail já cadastrado no sistema.");
            }
        }

        private static void NormalizarCampos(SecretariaDTO secretariaDTO)
        {
            secretariaDTO.Descricao = Sanitize(secretariaDTO.Descricao);
            secretariaDTO.Nome = Sanitize(secretariaDTO.Nome);
            secretariaDTO.Logradouro = Sanitize(secretariaDTO.Logradouro);
            secretariaDTO.Numero = Sanitize(secretariaDTO.Numero);
            secretariaDTO.Bairro = Sanitize(secretariaDTO.Bairro);
            secretariaDTO.NomeRazaoSocial = Sanitize(secretariaDTO.NomeRazaoSocial);
            secretariaDTO.Email = Sanitize(secretariaDTO.Email).ToLowerInvariant();
            secretariaDTO.Cidade = Sanitize(secretariaDTO.Cidade);
            secretariaDTO.Estado = Sanitize(secretariaDTO.Estado).ToUpperInvariant();

            secretariaDTO.Cnpj = Sanitize(secretariaDTO.Cnpj, digitsOnly: true);
            secretariaDTO.Cep = Sanitize(secretariaDTO.Cep, digitsOnly: true);
            secretariaDTO.Telefone = Sanitize(secretariaDTO.Telefone, digitsOnly: true);
        }

        private static void ValidarCampos(SecretariaDTO secretariaDTO)
        {
            ValidarObrigatorio(secretariaDTO.Nome, nameof(secretariaDTO.Nome));
            ValidarObrigatorio(secretariaDTO.Descricao, nameof(secretariaDTO.Descricao));
            ValidarObrigatorio(secretariaDTO.Cnpj, nameof(secretariaDTO.Cnpj));
            ValidarObrigatorio(secretariaDTO.NomeRazaoSocial, nameof(secretariaDTO.NomeRazaoSocial));
            ValidarObrigatorio(secretariaDTO.Email, nameof(secretariaDTO.Email));
            ValidarObrigatorio(secretariaDTO.Telefone, nameof(secretariaDTO.Telefone));
            ValidarObrigatorio(secretariaDTO.Logradouro, nameof(secretariaDTO.Logradouro));
            ValidarObrigatorio(secretariaDTO.Numero, nameof(secretariaDTO.Numero));
            ValidarObrigatorio(secretariaDTO.Bairro, nameof(secretariaDTO.Bairro));
            ValidarObrigatorio(secretariaDTO.Cidade, nameof(secretariaDTO.Cidade));
            ValidarObrigatorio(secretariaDTO.Estado, nameof(secretariaDTO.Estado));
            ValidarObrigatorio(secretariaDTO.Cep, nameof(secretariaDTO.Cep));

            ValidarTamanho(secretariaDTO.Nome, 3, 150, "Nome");
            ValidarTamanho(secretariaDTO.NomeRazaoSocial, 3, 200, "NomeRazaoSocial");
            ValidarTamanho(secretariaDTO.Descricao, 1, 500, "Descricao");
            ValidarTamanho(secretariaDTO.Logradouro, 1, 200, "Logradouro");

            if (secretariaDTO.IdSecretaria < 0)
            {
                throw new ArgumentException("IdSecretaria não pode ser negativo.");
            }

            var valorSituacao = (int)secretariaDTO.Situacao;
            if (valorSituacao is not ((int)Situacao.ATIVO) and not ((int)Situacao.INATIVO))
            {
                throw new ArgumentException("Situação inválida. Valores permitidos: 1 (Ativo) ou 2 (Inativo).");
            }

            if (secretariaDTO.Cnpj.Length != 14 || !IsCnpj(secretariaDTO.Cnpj))
            {
                throw new ArgumentException("CNPJ inválido.");
            }

            if (secretariaDTO.Cep.Length != 8)
            {
                throw new ArgumentException("CEP deve conter 8 dígitos.");
            }

            if (secretariaDTO.Telefone.Length is < 10 or > 11)
            {
                throw new ArgumentException("Telefone deve conter entre 10 e 11 dígitos com DDD.");
            }

            if (secretariaDTO.Email.Length > 255 || !IsEmailValido(secretariaDTO.Email))
            {
                throw new ArgumentException("E-mail inválido.");
            }

            if (!UFsValidas.Contains(secretariaDTO.Estado))
            {
                throw new ArgumentException("Estado inválido. Informe uma UF válida.");
            }
        }

        private static void ValidarObrigatorio(string valor, string campo)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                throw new ArgumentException($"Campo obrigatório não preenchido: {campo}.");
            }
        }

        private static void ValidarTamanho(string valor, int minimo, int maximo, string campo)
        {
            if (valor.Length < minimo || valor.Length > maximo)
            {
                throw new ArgumentException($"Campo {campo} deve conter entre {minimo} e {maximo} caracteres.");
            }
        }

        private static bool IsEmailValido(string email)
        {
            try
            {
                var mailAddress = new MailAddress(email);
                return mailAddress.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private static string Sanitize(string? valor, bool digitsOnly = false)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return string.Empty;
            }

            var sanitized = valor.Trim();
            return digitsOnly
                ? Regex.Replace(sanitized, "[^0-9]", string.Empty)
                : sanitized;
        }

        private static bool IsCnpj(string cnpj)
        {
            if (cnpj.Length != 14)
            {
                return false;
            }

            if (cnpj.All(caractere => caractere == cnpj[0]))
            {
                return false;
            }

            int[] pesoPrimeiroDigito = [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
            int[] pesoSegundoDigito = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];

            var somaPrimeiroDigito = 0;
            for (var i = 0; i < 12; i++)
            {
                somaPrimeiroDigito += (cnpj[i] - '0') * pesoPrimeiroDigito[i];
            }

            var restoPrimeiroDigito = somaPrimeiroDigito % 11;
            var digito13 = restoPrimeiroDigito < 2 ? 0 : 11 - restoPrimeiroDigito;

            var somaSegundoDigito = 0;
            for (var i = 0; i < 13; i++)
            {
                var numero = i == 12 ? digito13 : cnpj[i] - '0';
                somaSegundoDigito += numero * pesoSegundoDigito[i];
            }

            var restoSegundoDigito = somaSegundoDigito % 11;
            var digito14 = restoSegundoDigito < 2 ? 0 : 11 - restoSegundoDigito;

            return cnpj[12] - '0' == digito13 && cnpj[13] - '0' == digito14;
        }

        public async Task<SecretariaGastosDTO?> GetGastosBySecretariaIdAsync(int secretariaId)
        {
            return await _secretariaRepository.GetGastosBySecretariaIdAsync(secretariaId);
        }

        public async Task<SecretariaOrcamentoDisponivelDTO?> GetOrcamentoDisponivelBySecretariaIdAsync(int secretariaId)
        {
            return await _secretariaRepository.GetOrcamentoDisponivelBySecretariaIdAsync(secretariaId);
        }
    }
}