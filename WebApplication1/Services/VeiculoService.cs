using Meucombustivel.Dtos.Veiculo;
using Meucombustivel.Models;
using Meucombustivel.Repositories.Interfaces;
using Meucombustivel.Services.Interfaces;
using AutoMapper;



namespace Meucombustivel.Services
{
    public class VeiculoService : IVeiculoService
    {
        private readonly IVeiculoRepository _veiculoRepository;
        private readonly IEmpresaRepository _empresaRepository;
        private readonly IMapper _mapper;

        public VeiculoService(IVeiculoRepository veiculoRepository, IEmpresaRepository empresaRepository, IMapper mapper)
        {
            _veiculoRepository = veiculoRepository;
            _empresaRepository = empresaRepository;
            _mapper = mapper;
        }

        public async Task<Guid> CreateAsync(CreateVeiculoDto dto)
        {
            var existingCompany = await _empresaRepository.GetByIdAsync(dto.EmpresaId);
            if (existingCompany == null)
            {
                throw new InvalidOperationException($"Empresa com ID {dto.EmpresaId} não encontrada. Não é possível cadastrar o veículo.");
            }

            var existingVehicleByPlate = await _veiculoRepository.GetByPlacaAsync(dto.Placa);
            if (existingVehicleByPlate != null)
            {
                throw new InvalidOperationException($"Veículo com a placa {dto.Placa} já cadastrado.");
            }

            var veiculo = _mapper.Map<Veiculo>(dto);
            veiculo.Id = Guid.NewGuid();

            await _veiculoRepository.AddAsync(veiculo);
            return veiculo.Id;

            
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var existingVeiculo = await _veiculoRepository.GetByIdAsync(id);
            if (existingVeiculo == null)
            {
                return false;
            }

            await _veiculoRepository.DeleteAsync(id);
            return true;
        }

        public async Task<IEnumerable<ReadVeiculoDto>> GetAllAsync()
        {
            var veiculos = await _veiculoRepository.GetAllAsync();
            var veiculoDtos = new List<ReadVeiculoDto>();

            foreach (var veiculo in veiculos)
            {
                var empresa = await _empresaRepository.GetByIdAsync(veiculo.EmpresaId);
                var veiculoDto = _mapper.Map<ReadVeiculoDto>(veiculo);
              
                veiculoDtos.Add(veiculoDto);
            }
            return veiculoDtos;
        }

        public async Task<IEnumerable<ReadVeiculoDto>> GetAllByEmpresaAsync(Guid empresaId)
        {
            var veiculos = await _veiculoRepository.GetVeiculosByEmpresaIdAsync(empresaId);
            var veiculoDtos = new List<ReadVeiculoDto>();


            var empresa = await _empresaRepository.GetByIdAsync(empresaId); 
            if (empresa == null)
            {
                throw new InvalidOperationException($"Empresa com ID {empresaId} não encontrada.");
            }

            foreach (var veiculo in veiculos)
            {
                var veiculoDto = _mapper.Map<ReadVeiculoDto>(veiculo);
                
                veiculoDtos.Add(veiculoDto);
            }
            return veiculoDtos;
        }

        public async Task<ReadVeiculoDto?> GetByIdAsync(Guid id)
        {
            var veiculo = await _veiculoRepository.GetByIdAsync(id);
            if (veiculo == null)
            {
                return null;
            }

            var empresa = await _empresaRepository.GetByIdAsync(veiculo.EmpresaId);
            var veiculoDto = _mapper.Map<ReadVeiculoDto>(veiculo);
            

            return veiculoDto;
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateVeiculoDto dto)
        {
            var existingVeiculo = await _veiculoRepository.GetByIdAsync(id);
            if (existingVeiculo == null)
            {
                return false;
            }

         
            if (!string.IsNullOrEmpty(dto.Placa) && dto.Placa != existingVeiculo.Placa)
            {
                var vehicleWithNewPlate = await _veiculoRepository.GetByPlacaAsync(dto.Placa);
                if (vehicleWithNewPlate != null && vehicleWithNewPlate.Id != existingVeiculo.Id)
                {
                    throw new InvalidOperationException($"A placa {dto.Placa} já está em uso por outro veículo.");
                }
            }

            
            if (dto.Empresa_Id != Guid.Empty && dto.Empresa_Id != existingVeiculo.EmpresaId) 
            {
                var newCompany = await _empresaRepository.GetByIdAsync(dto.Empresa_Id);
                if (newCompany == null)
                {
                    throw new InvalidOperationException($"Nova empresa com ID {dto.Empresa_Id} não encontrada.");
                }
            }

            _mapper.Map(dto, existingVeiculo);
            await _veiculoRepository.UpdateAsync(existingVeiculo);
            return true;
        }
    }
}
