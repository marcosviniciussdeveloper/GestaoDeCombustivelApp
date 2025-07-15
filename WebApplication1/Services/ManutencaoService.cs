using Meucombustivel.Dtos.Manutencao;
using Meucombustivel.Models;
using Meucombustivel.Repositories.Interfaces;
using Meucombustivel.Services.Interfaces;
using AutoMapper;

namespace Meucombustivel.Services
{
    public class ManutencaoService : IManutencaoService
    {
        private readonly IManutencaoRepository _manutencaoRepository;
        private readonly IVeiculoRepository _veiculoRepository;
        private readonly IMapper _mapper;

        public ManutencaoService(IManutencaoRepository manutencaoRepository, IVeiculoRepository veiculoRepository, IMapper mapper)
        {
            _manutencaoRepository = manutencaoRepository;
            _veiculoRepository = veiculoRepository;
            _mapper = mapper;
        }

        public async Task<Guid> CreateAsync(CreateManutencaoDto dto)
        {
            var veiculo = await _veiculoRepository.GetByIdAsync(dto.IdVeiculo);
            if (veiculo == null)
            {
                throw new InvalidOperationException($"Veículo com ID {dto.IdVeiculo} não encontrado. Não é possível registrar a manutenção.");
            }

            var manutencao = _mapper.Map<Manutencoes>(dto);
            manutencao.Id = Guid.NewGuid();

            await _manutencaoRepository.AddAsync(manutencao);
            return manutencao.Id;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var existingManutencao = await _manutencaoRepository.GetByIdAsync(id);
            if (existingManutencao == null)
            {
                return false;
            }

            await _manutencaoRepository.DeleteAsync(id);
            return true;
        }

        public async Task<IEnumerable<ReadManutencaoDto>> GetAllAsync()
        {
            var manutencoes = await _manutencaoRepository.GetAllAsync();
            var manutencaoDtos = new List<ReadManutencaoDto>();

            foreach (var manutencao in manutencoes)
            {
                var veiculo = await _veiculoRepository.GetByIdAsync(manutencao.IdVeiculo);
                var manutencaoDto = _mapper.Map<ReadManutencaoDto>(manutencao);
                if (veiculo != null)
                {
                    manutencaoDto.PlacaVeiculo = veiculo.Placa;
                }
                manutencaoDtos.Add(manutencaoDto);
            }
            return manutencaoDtos;
        }

        public async Task<IEnumerable<ReadManutencaoDto>> GetAllByVeiculoAsync(Guid veiculoId)
        {
            var manutencoes = await _manutencaoRepository.GetManutencoesByVeiculoIdAsync(veiculoId);
            var manutencaoDtos = new List<ReadManutencaoDto>();

            var veiculo = await _veiculoRepository.GetByIdAsync(veiculoId);
            foreach (var manutencao in manutencoes)
            {
                var manutencaoDto = _mapper.Map<ReadManutencaoDto>(manutencao);
                if (veiculo != null)
                {
                    manutencaoDto.PlacaVeiculo = veiculo.Placa;
                }
                manutencaoDtos.Add(manutencaoDto);
            }
            return manutencaoDtos;
        }

        public async Task<ReadManutencaoDto?> GetByIdAsync(Guid id)
        {
            var manutencao = await _manutencaoRepository.GetByIdAsync(id);
            if (manutencao == null)
            {
                return null;
            }

            var veiculo = await _veiculoRepository.GetByIdAsync(manutencao.IdVeiculo);
            var manutencaoDto = _mapper.Map<ReadManutencaoDto>(manutencao);
            if (veiculo != null)
            {
                manutencaoDto.PlacaVeiculo = veiculo.Placa;
            }

            return manutencaoDto;
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateManutencaoDto dto)
        {
            var existingManutencao = await _manutencaoRepository.GetByIdAsync(id);
            if (existingManutencao == null)
            {
                return false;
            }

            _mapper.Map(dto, existingManutencao);
            await _manutencaoRepository.UpdateAsync(existingManutencao);
            return true;

        }
    }
}