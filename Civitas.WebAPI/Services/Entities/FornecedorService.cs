using AutoMapper;
using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Services.Interfaces;

namespace Civitas.WebAPI.Services.Entities
{
    public class FornecedorService : GenericService<Fornecedor, FornecedorDTO>, IFornecedorService
    {
        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IMapper _mapper;

        public FornecedorService(IFornecedorRepository fornecedorRepository, IMapper mapper)
            : base(fornecedorRepository, mapper)
        {
            _fornecedorRepository = fornecedorRepository;
            _mapper = mapper;
        }
    }
}
