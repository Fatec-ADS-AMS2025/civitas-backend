using AutoMapper;
using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Services.Interfaces;
using Civitas.WebAPI.Services.Validation;
using System.ComponentModel.DataAnnotations;

namespace Civitas.WebAPI.Services.Entities
{
    /// <summary>
    /// Servico especializado na gestao de Instituicoes (Unidades Administrativas) do sistema.
    /// </summary>
    /// <remarks>
    /// Finalidade:
    /// - Centralizar regras de negocio para cadastro de escolas, postos de saude e orgaos publicos.
    /// - Fornecer metodos de busca otimizados, alem do CRUD padrao herdado.
    ///
    /// Dependencias:
    /// - <see cref="IInstituicaoRepository"/>: Acesso a dados com filtros especificos.
    /// - <see cref="IMapper"/>: Transformacao de objetos.
    /// </remarks>
    public class InstituicaoService : GenericService<Instituicao, InstituicaoDTO>, IInstituicaoService
    {
        private static readonly EmailAddressAttribute EmailValidator = new();
        private static readonly HashSet<string> ValidUfs =
        [
            "AC", "AL", "AP", "AM", "BA", "CE", "DF", "ES", "GO",
            "MA", "MT", "MS", "MG", "PA", "PB", "PR", "PE", "PI",
            "RJ", "RN", "RS", "RO", "RR", "SC", "SP", "SE", "TO"
        ];

        private readonly IInstituicaoRepository _instituicaoRepository;
        private readonly ITipoInstituicaoRepository _tipoInstituicaoRepository;
        private readonly ISecretariaRepository _secretariaRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Inicializa o servico de Instituicoes.
        /// </summary>
        /// <param name="instituicaoRepository">Repositorio concreto de instituicoes.</param>
        /// <param name="tipoInstituicaoRepository">Repositorio para validacao do tipo de instituicao.</param>
        /// <param name="secretariaRepository">Repositorio para validacao da secretaria vinculada.</param>
        /// <param name="mapper">Mapeador de objetos.</param>
        public InstituicaoService(
            IInstituicaoRepository instituicaoRepository,
            ITipoInstituicaoRepository tipoInstituicaoRepository,
            ISecretariaRepository secretariaRepository,
            IMapper mapper)
            : base(instituicaoRepository, mapper)
        {
            _instituicaoRepository = instituicaoRepository;
            _tipoInstituicaoRepository = tipoInstituicaoRepository;
            _secretariaRepository = secretariaRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Realiza uma busca textual por instituicoes contendo o termo especificado.
        /// </summary>
        /// <param name="name">Nome ou parte do nome da instituicao para filtragem.</param>
        /// <returns>Uma colecao de DTOs de instituicoes que correspondem ao criterio de busca.</returns>
        /// <remarks>
        /// Utilidade:
        /// - Usado em campos de "Autocomplete" ou barras de pesquisa no front-end.
        /// - A implementacao no repositorio geralmente ignora Case Sensitive (maiusculas/minusculas).
        /// </remarks>
        public async Task<IEnumerable<InstituicaoDTO>> GetInstituicaoByName(string name)
        {
            var instituicao = await _instituicaoRepository.GetInstituicaoByName(name);
            return _mapper.Map<IEnumerable<InstituicaoDTO>>(instituicao);
        }

        public async Task<InstituicaoGastosDTO?> GetGastosByInstituicaoIdAsync(int instituicaoId, int tipoDespesaId)
        {
            return await _instituicaoRepository.GetGastosByInstituicaoIdAsync(instituicaoId, tipoDespesaId);
        }

        public async Task<InstituicaoOrcamentoDisponivelDTO?> GetOrcamentoDisponivelByInstituicaoIdAsync(int instituicaoId)
        {
            return await _instituicaoRepository.GetOrcamentoDisponivelByInstituicaoIdAsync(instituicaoId);
        }

        public override async Task Create(InstituicaoDTO entityDTO)
        {
            ValidateDtoInstance(entityDTO);
            Normalize(entityDTO);
            ValidateFields(entityDTO);
            await ValidateRelationships(entityDTO);
            await ValidateUniqueness(entityDTO);

            var entity = _mapper.Map<Instituicao>(entityDTO);
            await _instituicaoRepository.Add(entity);

            entityDTO.Id = entity.Id;
        }

        public override async Task Update(InstituicaoDTO entityDTO, int id)
        {
            ValidateDtoInstance(entityDTO);

            var existingInstituicao = await _instituicaoRepository.GetById(id);
            if (existingInstituicao is null)
            {
                throw new KeyNotFoundException($"Instituicao com id {id} nao encontrada.");
            }

            Normalize(entityDTO);
            ValidateFields(entityDTO);
            await ValidateRelationships(entityDTO);
            await ValidateStatusTransitionAsync(entityDTO, existingInstituicao);
            await ValidateUniqueness(entityDTO, id);

            var entity = _mapper.Map<Instituicao>(entityDTO);
            entity.Id = id;
            entity.Excluido = existingInstituicao.Excluido;
            entity.DataExclusao = existingInstituicao.DataExclusao;

            await _instituicaoRepository.Update(entity);

            entityDTO.Id = id;
        }

        private static void ValidateDtoInstance(InstituicaoDTO? instituicaoDto)
        {
            if (instituicaoDto is null)
            {
                throw new InstituicaoValidationException(["O corpo da requisicao e obrigatorio."]);
            }
        }

        private static void Normalize(InstituicaoDTO instituicaoDto)
        {
            instituicaoDto.CNPJ = OnlyDigits(instituicaoDto.CNPJ);
            instituicaoDto.CEP = OnlyDigits(instituicaoDto.CEP);
            instituicaoDto.Telefone = OnlyDigits(instituicaoDto.Telefone);
            instituicaoDto.Nome = instituicaoDto.Nome?.Trim() ?? string.Empty;
            instituicaoDto.NomeRazaoSocial = instituicaoDto.NomeRazaoSocial?.Trim() ?? string.Empty;
            instituicaoDto.Logradouro = instituicaoDto.Logradouro?.Trim() ?? string.Empty;
            instituicaoDto.Numero = instituicaoDto.Numero?.Trim() ?? string.Empty;
            instituicaoDto.Bairro = instituicaoDto.Bairro?.Trim() ?? string.Empty;
            instituicaoDto.Cidade = instituicaoDto.Cidade?.Trim() ?? string.Empty;
            instituicaoDto.Estado = instituicaoDto.Estado?.Trim().ToUpperInvariant() ?? string.Empty;
            instituicaoDto.Email = instituicaoDto.Email?.Trim().ToLowerInvariant() ?? string.Empty;
        }

        private static void ValidateFields(InstituicaoDTO instituicaoDto)
        {
            var errors = new List<string>();

            ValidateRequired(instituicaoDto.CNPJ, "CNPJ", errors);
            ValidateRequired(instituicaoDto.Nome, "Nome", errors);
            ValidateRequired(instituicaoDto.Logradouro, "Logradouro", errors);
            ValidateRequired(instituicaoDto.Numero, "Numero", errors);
            ValidateRequired(instituicaoDto.Bairro, "Bairro", errors);
            ValidateRequired(instituicaoDto.CEP, "CEP", errors);
            ValidateRequired(instituicaoDto.NomeRazaoSocial, "NomeRazaoSocial", errors);
            ValidateRequired(instituicaoDto.Telefone, "Telefone", errors);
            ValidateRequired(instituicaoDto.Email, "Email", errors);
            ValidateRequired(instituicaoDto.Cidade, "Cidade", errors);
            ValidateRequired(instituicaoDto.Estado, "Estado", errors);
            ValidatePositiveId(instituicaoDto.IdTipoInstituicao, "Tipo de instituicao", errors);
            ValidatePositiveId(instituicaoDto.IdSecretaria, "Secretaria", errors);

            ValidateNome(instituicaoDto.Nome, errors);
            ValidateNomeRazaoSocial(instituicaoDto.NomeRazaoSocial, errors);
            ValidateLogradouro(instituicaoDto.Logradouro, errors);
            ValidateNumero(instituicaoDto.Numero, errors);
            ValidateBairro(instituicaoDto.Bairro, errors);
            ValidateCidade(instituicaoDto.Cidade, errors);
            ValidateEmail(instituicaoDto.Email, errors);
            ValidateCnpj(instituicaoDto.CNPJ, errors);
            ValidateTelefone(instituicaoDto.Telefone, errors);
            ValidateCep(instituicaoDto.CEP, errors);
            ValidateEstado(instituicaoDto.Estado, errors);
            ValidateSituacao(instituicaoDto.Situacao, errors);

            if (errors.Count > 0)
            {
                throw new InstituicaoValidationException(errors);
            }
        }

        private async Task ValidateRelationships(InstituicaoDTO instituicaoDto)
        {
            var errors = new List<string>();

            if (instituicaoDto.IdTipoInstituicao > 0)
            {
                var tipoInstituicao = await _tipoInstituicaoRepository.GetById(instituicaoDto.IdTipoInstituicao);
                if (tipoInstituicao is null)
                {
                    errors.Add("O tipo de instituicao informado nao foi encontrado.");
                }
            }

            if (instituicaoDto.IdSecretaria > 0)
            {
                var secretaria = await _secretariaRepository.GetById(instituicaoDto.IdSecretaria);
                if (secretaria is null)
                {
                    errors.Add("A secretaria informada nao foi encontrada.");
                }
            }

            if (errors.Count > 0)
            {
                throw new InstituicaoValidationException(errors);
            }
        }

        private async Task ValidateStatusTransitionAsync(InstituicaoDTO instituicaoDto, Instituicao existingInstituicao)
        {
            if (existingInstituicao.Situacao == Situacao.ATIVO &&
                instituicaoDto.Situacao == Situacao.INATIVO &&
                await _instituicaoRepository.HasDespesasPendentesAsync(existingInstituicao.Id))
            {
                throw new InstituicaoValidationException(
                [
                    "A instituicao nao pode ser inativada porque possui despesas pendentes."
                ]);
            }
        }

        private async Task ValidateUniqueness(InstituicaoDTO instituicaoDto, int? ignoredId = null)
        {
            if (await _instituicaoRepository.ExistsByCnpjAsync(instituicaoDto.CNPJ, ignoredId))
            {
                throw new InstituicaoConflictException("cnpj", "Ja existe uma instituicao cadastrada com este CNPJ.");
            }

            if (await _instituicaoRepository.ExistsByEmailAsync(instituicaoDto.Email, ignoredId))
            {
                throw new InstituicaoConflictException("email", "Ja existe uma instituicao cadastrada com este email.");
            }
        }

        private static void ValidateRequired(string? value, string fieldName, ICollection<string> errors)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                errors.Add($"O campo {fieldName} e obrigatorio.");
            }
        }

        private static void ValidatePositiveId(int value, string fieldName, ICollection<string> errors)
        {
            if (value <= 0)
            {
                errors.Add($"O campo {fieldName} e obrigatorio.");
            }
        }

        private static void ValidateNome(string nome, ICollection<string> errors)
        {
            if (string.IsNullOrEmpty(nome))
            {
                return;
            }

            if (nome.Length is < 3 or > 150)
            {
                errors.Add("O nome deve ter entre 3 e 150 caracteres.");
            }
        }

        private static void ValidateNomeRazaoSocial(string nomeRazaoSocial, ICollection<string> errors)
        {
            if (string.IsNullOrEmpty(nomeRazaoSocial))
            {
                return;
            }

            if (nomeRazaoSocial.Length is < 3 or > 200)
            {
                errors.Add("O nome/razao social deve ter entre 3 e 200 caracteres.");
            }
        }

        private static void ValidateLogradouro(string logradouro, ICollection<string> errors)
        {
            if (string.IsNullOrEmpty(logradouro))
            {
                return;
            }

            if (logradouro.Length > 255)
            {
                errors.Add("O logradouro deve ter no maximo 255 caracteres.");
            }
        }

        private static void ValidateNumero(string numero, ICollection<string> errors)
        {
            if (string.IsNullOrEmpty(numero))
            {
                return;
            }

            if (numero.Length > 4)
            {
                errors.Add("O numero deve ter no maximo 4 caracteres.");
            }
        }

        private static void ValidateBairro(string bairro, ICollection<string> errors)
        {
            if (string.IsNullOrEmpty(bairro))
            {
                return;
            }

            if (bairro.Length > 100)
            {
                errors.Add("O bairro deve ter no maximo 100 caracteres.");
            }
        }

        private static void ValidateCidade(string cidade, ICollection<string> errors)
        {
            if (string.IsNullOrEmpty(cidade))
            {
                return;
            }

            if (cidade.Length > 100)
            {
                errors.Add("A cidade deve ter no maximo 100 caracteres.");
            }
        }

        private static void ValidateEmail(string email, ICollection<string> errors)
        {
            if (string.IsNullOrEmpty(email))
            {
                return;
            }

            if (email.Length > 255)
            {
                errors.Add("O email deve ter no maximo 255 caracteres.");
            }

            if (!EmailValidator.IsValid(email))
            {
                errors.Add("O email informado e invalido.");
            }
        }

        private static void ValidateCnpj(string cnpj, ICollection<string> errors)
        {
            if (string.IsNullOrEmpty(cnpj))
            {
                return;
            }

            if (cnpj.Length != 14)
            {
                errors.Add("O CNPJ deve conter 14 digitos.");
                return;
            }

            if (!IsCnpj(cnpj))
            {
                errors.Add("O CNPJ informado e invalido.");
            }
        }

        private static void ValidateTelefone(string telefone, ICollection<string> errors)
        {
            if (string.IsNullOrEmpty(telefone))
            {
                return;
            }

            if (telefone.Length is < 10 or > 11)
            {
                errors.Add("O telefone deve conter entre 10 e 11 digitos com DDD.");
            }
        }

        private static void ValidateCep(string cep, ICollection<string> errors)
        {
            if (string.IsNullOrEmpty(cep))
            {
                return;
            }

            if (cep.Length != 8)
            {
                errors.Add("O CEP deve conter 8 digitos.");
            }
        }

        private static void ValidateEstado(string estado, ICollection<string> errors)
        {
            if (string.IsNullOrEmpty(estado))
            {
                return;
            }

            if (estado.Length != 2 || !ValidUfs.Contains(estado))
            {
                errors.Add("O estado deve conter uma UF valida.");
            }
        }

        private static void ValidateSituacao(Situacao situacao, ICollection<string> errors)
        {
            if (!Enum.IsDefined(situacao) || situacao is not (Situacao.ATIVO or Situacao.INATIVO))
            {
                errors.Add("A situacao informada e invalida.");
            }
        }

        private static string OnlyDigits(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            return new string(value.Where(char.IsDigit).ToArray());
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
