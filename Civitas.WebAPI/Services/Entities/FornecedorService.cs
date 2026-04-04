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
    /// Serviço responsável pelo gerenciamento de Fornecedores (Credores) do sistema.
    /// </summary>
    /// <remarks>
    /// Finalidade:
    /// - Centralizar as operações de cadastro e atualização de empresas prestadoras de serviço.
    ///
    /// Regras de Negócio:
    /// - Atua como validador para garantir que apenas fornecedores com dados fiscais consistentes (CNPJ) sejam mantidos.
    /// - É essencial para a integridade financeira, pois sem um fornecedor válido, não se pode lançar despesas.
    ///
    /// Dependências:
    /// - <see cref="IFornecedorRepository"/>: Persistência de dados.
    /// - <see cref="IMapper"/>: Transformação de objetos (DTO/Entity).
    /// </remarks>
    public class FornecedorService : GenericService<Fornecedor, FornecedorDTO>, IFornecedorService
    {
        private readonly IFornecedorRepository _fornecedorRepository;
        private static readonly HashSet<string> UFsValidas =
        [
            "AC", "AL", "AP", "AM", "BA", "CE", "DF", "ES", "GO", "MA", "MT", "MS", "MG",
            "PA", "PB", "PR", "PE", "PI", "RJ", "RN", "RS", "RO", "RR", "SC", "SP", "SE", "TO"
        ];

        /// <summary>
        /// Inicializa o serviço de Fornecedores com as dependências necessárias.
        /// </summary>
        /// <param name="fornecedorRepository">Repositório de acesso a dados de fornecedores.</param>
        /// <param name="mapper">Mapeador de objetos.</param>
        /// <exception cref="ArgumentNullException">Lançada caso os parâmetros injetados sejam nulos.</exception>
        public FornecedorService(IFornecedorRepository fornecedorRepository, IMapper mapper)
            : base(fornecedorRepository, mapper)
        {
            _fornecedorRepository = fornecedorRepository;
        }

        public async Task ValidarCadastroAsync(FornecedorDTO entityDTO, int? id = null)
        {
            if (entityDTO is null)
            {
                throw new ArgumentException("Dados do fornecedor são obrigatórios.");
            }

            if (id.HasValue && id.Value <= 0)
            {
                throw new ArgumentException("Id do fornecedor inválido.");
            }

            if (id.HasValue)
            {
                var existingEntity = await _fornecedorRepository.GetById(id.Value);
                if (existingEntity is null)
                {
                    throw new KeyNotFoundException($"Entidade com id {id.Value} não encontrada.");
                }
            }

            entityDTO.IdFornecedor = id ?? 0;

            NormalizarCampos(entityDTO);
            ValidarCampos(entityDTO);
            await ValidarUnicidadeAsync(entityDTO, id);
        }

        private async Task ValidarUnicidadeAsync(FornecedorDTO fornecedorDTO, int? ignoreId)
        {
            var cnpjDuplicado = await _fornecedorRepository.ExistsByCnpjAsync(fornecedorDTO.Cnpj, ignoreId);
            if (cnpjDuplicado)
            {
                throw new ArgumentException("CNPJ já cadastrado no sistema.");
            }
        }

        private static void NormalizarCampos(FornecedorDTO fornecedorDTO)
        {
            fornecedorDTO.NomeFantasia = Sanitize(fornecedorDTO.NomeFantasia);
            fornecedorDTO.Nome = Sanitize(fornecedorDTO.Nome);
            fornecedorDTO.Logradouro = Sanitize(fornecedorDTO.Logradouro);
            fornecedorDTO.Numero = Sanitize(fornecedorDTO.Numero);
            fornecedorDTO.Bairro = Sanitize(fornecedorDTO.Bairro);
            fornecedorDTO.Email = Sanitize(fornecedorDTO.Email).ToLowerInvariant();
            fornecedorDTO.Cidade = Sanitize(fornecedorDTO.Cidade);
            fornecedorDTO.Estado = Sanitize(fornecedorDTO.Estado).ToUpperInvariant();

            fornecedorDTO.Cnpj = Sanitize(fornecedorDTO.Cnpj, digitsOnly: true);
            fornecedorDTO.Cep = Sanitize(fornecedorDTO.Cep, digitsOnly: true);
            fornecedorDTO.Telefone = Sanitize(fornecedorDTO.Telefone, digitsOnly: true);
        }

        private static void ValidarCampos(FornecedorDTO fornecedorDTO)
        {
            ValidarObrigatorio(fornecedorDTO.NomeFantasia, nameof(fornecedorDTO.NomeFantasia));
            ValidarObrigatorio(fornecedorDTO.Nome, nameof(fornecedorDTO.Nome));
            ValidarObrigatorio(fornecedorDTO.Cnpj, nameof(fornecedorDTO.Cnpj));
            ValidarObrigatorio(fornecedorDTO.Logradouro, nameof(fornecedorDTO.Logradouro));
            ValidarObrigatorio(fornecedorDTO.Numero, nameof(fornecedorDTO.Numero));
            ValidarObrigatorio(fornecedorDTO.Bairro, nameof(fornecedorDTO.Bairro));
            ValidarObrigatorio(fornecedorDTO.Cidade, nameof(fornecedorDTO.Cidade));
            ValidarObrigatorio(fornecedorDTO.Estado, nameof(fornecedorDTO.Estado));
            ValidarObrigatorio(fornecedorDTO.Cep, nameof(fornecedorDTO.Cep));
            ValidarObrigatorio(fornecedorDTO.Telefone, nameof(fornecedorDTO.Telefone));
            ValidarObrigatorio(fornecedorDTO.Email, nameof(fornecedorDTO.Email));

            ValidarTamanhoMaximo(fornecedorDTO.NomeFantasia, 150, "NomeFantasia");
            ValidarTamanhoMaximo(fornecedorDTO.Nome, 150, "Nome");
            ValidarTamanhoMaximo(fornecedorDTO.Logradouro, 200, "Logradouro");
            ValidarTamanhoMaximo(fornecedorDTO.Numero, 20, "Numero");
            ValidarTamanhoMaximo(fornecedorDTO.Bairro, 100, "Bairro");
            ValidarTamanhoMaximo(fornecedorDTO.Cidade, 100, "Cidade");
            ValidarTamanhoMaximo(fornecedorDTO.Email, 150, "Email");

            if (fornecedorDTO.IdFornecedor < 0)
            {
                throw new ArgumentException("IdFornecedor não pode ser negativo.");
            }

            var valorSituacao = (int)fornecedorDTO.Situacao;
            if (valorSituacao is not ((int)Situacao.ATIVO) and not ((int)Situacao.INATIVO))
            {
                throw new ArgumentException("Situação inválida. Valores permitidos: 1 (Ativo) ou 2 (Inativo).");
            }

            if (fornecedorDTO.Cnpj.Length != 14 || !IsCnpj(fornecedorDTO.Cnpj))
            {
                throw new ArgumentException("CNPJ inválido.");
            }

            if (fornecedorDTO.Cep.Length != 8)
            {
                throw new ArgumentException("CEP deve conter 8 dígitos.");
            }

            if (fornecedorDTO.Telefone.Length != 11)
            {
                throw new ArgumentException("Telefone deve conter 11 dígitos com DDD.");
            }

            if (fornecedorDTO.Estado.Length != 2 || !UFsValidas.Contains(fornecedorDTO.Estado))
            {
                throw new ArgumentException("Estado inválido. Informe uma UF válida.");
            }

            if (!IsEmailValido(fornecedorDTO.Email))
            {
                throw new ArgumentException("E-mail inválido.");
            }
        }

        private static void ValidarObrigatorio(string valor, string campo)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                throw new ArgumentException($"Campo obrigatório não preenchido: {campo}.");
            }
        }

        private static void ValidarTamanhoMaximo(string valor, int maximo, string campo)
        {
            if (valor.Length > maximo)
            {
                throw new ArgumentException($"Campo {campo} deve conter no máximo {maximo} caracteres.");
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
    }
}
