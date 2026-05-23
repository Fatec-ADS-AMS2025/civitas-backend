using AutoMapper;
using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Services.Interfaces;
using Civitas.WebAPI.Services.Security;
using Civitas.WebAPI.Services.Validation;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Civitas.WebAPI.Services.Entities
{
    /// <summary>
    /// Serviço responsável pela gestão de usuários, credenciais e perfis de acesso.
    /// </summary>
    /// <remarks>
    /// Finalidade:
    /// - Centralizar a lógica de cadastro, atualização e consulta de usuários do sistema.
    /// - Gerenciar os perfis de acesso (Visitante, Administrador, Funcionário).
    /// 
    /// Regras de Negócio:
    /// - O CPF deve ser único no sistema.
    /// - Este serviço é a base para as rotinas de Autenticação e Autorização.
    /// 
    /// Dependências:
    /// - <see cref="IUsuarioRepository"/>: Camada de acesso a dados.
    /// - <see cref="IMapper"/>: Transformação de objetos (DTO/Entity).
    /// </remarks>
    public class UsuarioService : GenericService<Usuario, UsuarioDTO>, IUsuarioService
    {
        private static readonly EmailAddressAttribute EmailValidator = new();
        private static readonly HashSet<string> ValidUfs =
        [
            "AC", "AL", "AP", "AM", "BA", "CE", "DF", "ES", "GO",
            "MA", "MT", "MS", "MG", "PA", "PB", "PR", "PE", "PI",
            "RJ", "RN", "RS", "RO", "RR", "SC", "SP", "SE", "TO"
        ];

        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordHashService _passwordHashService;

        /// <summary>
        /// Inicializa o serviço de Usuários.
        /// </summary>
        /// <param name="usuarioRepository">Repositório de usuários injetado.</param>
        /// <param name="mapper">Mapeador de objetos.</param>
        public UsuarioService(
            IUsuarioRepository usuarioRepository,
            IMapper mapper,
            IPasswordHashService passwordHashService)
            : base(usuarioRepository, mapper)
        {
            _usuarioRepository = usuarioRepository;
            _mapper = mapper;
            _passwordHashService = passwordHashService;
        }

        /// <summary>
        /// Realiza a busca de usuários através do número de CPF.
        /// </summary>
        /// <param name="cpf">O número do Cadastro de Pessoa Física a ser pesquisado.</param>
        /// <returns>Uma coleção de usuários encontrados (geralmente contendo 0 ou 1 registro).</returns>
        /// <remarks>
        /// Utilidade:
        /// - Verificação de duplicidade no momento do cadastro (impedir dois usuários com mesmo CPF).
        /// - Recuperação de conta ou login alternativo.
        /// </remarks>
        public async Task<IEnumerable<UsuarioDTO>> GetUsuarioByCpf(string cpf)
        {
            var usuario = await _usuarioRepository.GetUsuarioByCpf(OnlyDigits(cpf));
            return _mapper.Map<IEnumerable<UsuarioDTO>>(usuario);
        }

        public override async Task Create(UsuarioDTO entityDTO)
        {
            ValidateDtoInstance(entityDTO);
            Normalize(entityDTO);
            ValidateCommonRules(entityDTO, requirePassword: true);
            await ValidateUniqueness(entityDTO);

            var entity = _mapper.Map<Usuario>(entityDTO);
            entity.Senha = _passwordHashService.Hash(entityDTO.Senha!);

            await _usuarioRepository.Add(entity);

            entityDTO.Id = entity.Id;
            entityDTO.Senha = null;
        }

        public override async Task Update(UsuarioDTO entityDTO, int id)
        {
            ValidateDtoInstance(entityDTO);

            var existingUsuario = await _usuarioRepository.GetById(id);
            if (existingUsuario is null)
            {
                throw new KeyNotFoundException($"Usuário com id {id} não encontrado.");
            }

            Normalize(entityDTO);
            ValidateCommonRules(entityDTO, requirePassword: false);
            await ValidateUniqueness(entityDTO, id);

            var entity = _mapper.Map<Usuario>(entityDTO);
            entity.Id = id;
            entity.Senha = string.IsNullOrWhiteSpace(entityDTO.Senha)
                ? existingUsuario.Senha
                : _passwordHashService.Hash(entityDTO.Senha);
            entity.Excluido = existingUsuario.Excluido;
            entity.DataExclusao = existingUsuario.DataExclusao;

            await _usuarioRepository.Update(entity);

            entityDTO.Id = id;
            entityDTO.Senha = null;
        }

        private static void ValidateDtoInstance(UsuarioDTO? usuarioDto)
        {
            if (usuarioDto is null)
            {
                throw new UsuarioValidationException(["O corpo da requisição é obrigatório."]);
            }
        }

        private static void Normalize(UsuarioDTO usuarioDto)
        {
            usuarioDto.Cpf = OnlyDigits(usuarioDto.Cpf);
            usuarioDto.Rg = OnlyDigits(usuarioDto.Rg);
            usuarioDto.Cep = OnlyDigits(usuarioDto.Cep);
            usuarioDto.Nome = usuarioDto.Nome?.Trim() ?? string.Empty;
            usuarioDto.Logradouro = usuarioDto.Logradouro?.Trim() ?? string.Empty;
            usuarioDto.Numero = usuarioDto.Numero?.Trim() ?? string.Empty;
            usuarioDto.Bairro = usuarioDto.Bairro?.Trim() ?? string.Empty;
            usuarioDto.Cidade = usuarioDto.Cidade?.Trim() ?? string.Empty;
            usuarioDto.Estado = usuarioDto.Estado?.Trim().ToUpperInvariant() ?? string.Empty;
            usuarioDto.Email = usuarioDto.Email?.Trim().ToLowerInvariant() ?? string.Empty;
            usuarioDto.Matricula = usuarioDto.Matricula?.Trim() ?? string.Empty;
            usuarioDto.Senha = string.IsNullOrWhiteSpace(usuarioDto.Senha)
                ? null
                : usuarioDto.Senha.Trim();
        }

        private static void ValidateCommonRules(UsuarioDTO usuarioDto, bool requirePassword)
        {
            var errors = new List<string>();

            ValidateRequired(usuarioDto.Cpf, "CPF", errors);
            ValidateRequired(usuarioDto.Nome, "Nome", errors);
            ValidateRequired(usuarioDto.Rg, "RG", errors);
            ValidateRequired(usuarioDto.Email, "Email", errors);
            ValidateRequired(usuarioDto.Logradouro, "Logradouro", errors);
            ValidateRequired(usuarioDto.Numero, "Número", errors);
            ValidateRequired(usuarioDto.Bairro, "Bairro", errors);
            ValidateRequired(usuarioDto.Cidade, "Cidade", errors);
            ValidateRequired(usuarioDto.Estado, "Estado", errors);
            ValidateRequired(usuarioDto.Cep, "CEP", errors);
            ValidateRequired(usuarioDto.Matricula, "Matrícula", errors);

            if (requirePassword || !string.IsNullOrWhiteSpace(usuarioDto.Senha))
            {
                ValidateRequired(usuarioDto.Senha, "Senha", errors);
                ValidatePassword(usuarioDto.Senha, errors);
            }

            ValidateCpf(usuarioDto.Cpf, errors);
            ValidateNome(usuarioDto.Nome, errors);
            ValidateRg(usuarioDto.Rg, errors);
            ValidateEmail(usuarioDto.Email, errors);
            ValidateEndereco(usuarioDto, errors);
            ValidateSituacao(usuarioDto.Situacao, errors);
            ValidateTipoUsuario(usuarioDto.TipoUsuario, errors);

            if (errors.Count > 0)
            {
                throw new UsuarioValidationException(errors);
            }
        }

        private async Task ValidateUniqueness(UsuarioDTO usuarioDto, int? ignoredId = null)
        {
            if (await _usuarioRepository.ExistsByCpf(usuarioDto.Cpf, ignoredId))
            {
                throw new UsuarioConflictException("cpf", "Já existe um usuário cadastrado com este CPF.");
            }

            if (await _usuarioRepository.ExistsByEmail(usuarioDto.Email, ignoredId))
            {
                throw new UsuarioConflictException("email", "Já existe um usuário cadastrado com este email.");
            }

            if (await _usuarioRepository.ExistsByMatricula(usuarioDto.Matricula, ignoredId))
            {
                throw new UsuarioConflictException("matricula", "Já existe um usuário cadastrado com esta matrícula.");
            }
        }

        private static void ValidateRequired(string? value, string fieldName, ICollection<string> errors)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                errors.Add($"O campo {fieldName} é obrigatório.");
            }
        }

        private static void ValidateCpf(string cpf, ICollection<string> errors)
        {
            if (cpf.Length != 11)
            {
                errors.Add("O CPF deve conter 11 dígitos.");
                return;
            }

            if (!IsValidCpf(cpf))
            {
                errors.Add("O CPF informado é inválido.");
            }
        }

        private static void ValidateNome(string nome, ICollection<string> errors)
        {
            if (nome.Length is < 3 or > 150)
            {
                errors.Add("O nome deve ter entre 3 e 150 caracteres.");
            }
        }

        private static void ValidateRg(string rg, ICollection<string> errors)
        {
            if (string.IsNullOrEmpty(rg))
            {
                return;
            }

            if (!Regex.IsMatch(rg, @"^\d+$"))
            {
                errors.Add("O RG deve conter apenas números após a normalização.");
            }

            if (rg.Length > 20)
            {
                errors.Add("O RG deve ter no máximo 20 caracteres.");
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
                errors.Add("O email deve ter no máximo 255 caracteres.");
            }

            if (!EmailValidator.IsValid(email))
            {
                errors.Add("O email informado é inválido.");
            }
        }

        private static void ValidatePassword(string? password, ICollection<string> errors)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return;
            }

            if (password.Length < 8)
            {
                errors.Add("A senha deve ter no mínimo 8 caracteres.");
            }

            if (!password.Any(char.IsLetter) || !password.Any(char.IsDigit))
            {
                errors.Add("A senha deve conter pelo menos uma letra e um número.");
            }
        }

        private static void ValidateEndereco(UsuarioDTO usuarioDto, ICollection<string> errors)
        {
            if (usuarioDto.Logradouro.Length > 200)
            {
                errors.Add("O logradouro deve ter no máximo 200 caracteres.");
            }

            if (usuarioDto.Numero.Length > 10)
            {
                errors.Add("O número deve ter no máximo 10 caracteres.");
            }

            if (usuarioDto.Cep.Length != 8)
            {
                errors.Add("O CEP deve conter 8 dígitos.");
            }

            if (usuarioDto.Estado.Length != 2 || !ValidUfs.Contains(usuarioDto.Estado))
            {
                errors.Add("O estado deve conter uma UF válida.");
            }
        }

        private static void ValidateSituacao(Situacao situacao, ICollection<string> errors)
        {
            if (!Enum.IsDefined(situacao) || situacao is not (Situacao.ATIVO or Situacao.INATIVO))
            {
                errors.Add("A situação informada é inválida.");
            }
        }

        private static void ValidateTipoUsuario(TipoUsuario tipoUsuario, ICollection<string> errors)
        {
            if (!Enum.IsDefined(tipoUsuario))
            {
                errors.Add("O tipo de usuário informado é inválido.");
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

        private static bool IsValidCpf(string cpf)
        {
            if (cpf.Distinct().Count() == 1)
            {
                return false;
            }

            var numbers = cpf.Select(character => character - '0').ToArray();

            var firstDigit = CalculateCpfDigit(numbers, 9, 10);
            var secondDigit = CalculateCpfDigit(numbers, 10, 11);

            return numbers[9] == firstDigit && numbers[10] == secondDigit;
        }

        private static int CalculateCpfDigit(IReadOnlyList<int> numbers, int length, int multiplier)
        {
            var sum = 0;

            for (var index = 0; index < length; index++)
            {
                sum += numbers[index] * (multiplier - index);
            }

            var remainder = sum % 11;
            return remainder < 2 ? 0 : 11 - remainder;
        }
    }
}
