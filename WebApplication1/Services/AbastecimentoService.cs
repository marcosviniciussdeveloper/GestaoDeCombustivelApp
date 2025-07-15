using Meucombustivel.Dtos.Abastecimento;
using Meucombustivel.Models;
using Meucombustivel.Repositories.Interfaces;
using Meucombustivel.Services.Interfaces;
using AutoMapper;



namespace Meucombustivel.Services
{
    public class AbastecimentoService : IAbastecimentoService
    {
        private readonly IAbastecimentoRepository _abastecimentoRepository;
        private readonly IVeiculoRepository _veiculoRepository;
        private readonly IMotoristaRepository _motoristaRepository;
        private readonly IUsuarioRepository _usuarioRepository; 
        private readonly IMapper _mapper;

        public AbastecimentoService(IAbastecimentoRepository abastecimentoRepository, IVeiculoRepository veiculoRepository, IMotoristaRepository motoristaRepository, IUsuarioRepository usuarioRepository, IMapper mapper) // NOVO: Injetar IUsuarioRepository
        {
            _abastecimentoRepository = abastecimentoRepository;
            _veiculoRepository = veiculoRepository;
            _motoristaRepository = motoristaRepository;
            _usuarioRepository = usuarioRepository; 
            _mapper = mapper;
        }

        public async Task<Guid> CreateAsync(CreateAbastecimentoDto dto)
        {
            var veiculo = await _veiculoRepository.GetByIdAsync(dto.VeiculoId);
            if (veiculo == null)
            {
                throw new InvalidOperationException($"Veículo com ID {dto.VeiculoId} não encontrado. Não é possível registrar o abastecimento.");
            }

            var abastecimento = _mapper.Map<Abastecimento>(dto);
            abastecimento.Id = Guid.NewGuid();

            await _abastecimentoRepository.AddAsync(abastecimento);

            if (veiculo.QuilometragemAtual < abastecimento.KmInicial)
            {
                veiculo.QuilometragemAtual = abastecimento.KmInicial;
                await _veiculoRepository.UpdateAsync(veiculo);
            }

            return abastecimento.Id;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var existingAbastecimento = await _abastecimentoRepository.GetByIdAsync(id);
            if (existingAbastecimento == null)
            {
                return false;
            }

            await _abastecimentoRepository.DeleteAsync(id);
            return true;
        }

        public async Task<IEnumerable<ReadAbastecimentoDto>> GetAllByVeiculoAsync(Guid veiculoId)
        {
            var abastecimentos = await _abastecimentoRepository.GetAbastecimentosByVeiculoIdAsync(veiculoId);
            var abastecimentoDtos = new List<ReadAbastecimentoDto>();

            var veiculo = await _veiculoRepository.GetByIdAsync(veiculoId);
            foreach (var abastecimento in abastecimentos)
            {
                var abastecimentoDto = _mapper.Map<ReadAbastecimentoDto>(abastecimento);
                if (veiculo != null)
                {
                    abastecimentoDto.PlacaVeiculo = veiculo.Placa;
                }

                if (abastecimento.MotoristaId != Guid.Empty)
                {
                    var motorista = await _motoristaRepository.GetByIdAsync(abastecimento.MotoristaId);
                    if (motorista?.UsuarioId != null) 
                    {
                        var usuarioDoMotorista = await _usuarioRepository.GetByIdAsync(motorista.UsuarioId); 
                        if (usuarioDoMotorista != null)
                        {
                            abastecimentoDto.NomeMotorista = usuarioDoMotorista.Nome;
                        }
                    }
                }

                abastecimentoDtos.Add(abastecimentoDto);
            }
            return abastecimentoDtos;
        }

        public async Task<ReadAbastecimentoDto?> GetByIdAsync(Guid id)
        {
            var abastecimento = await _abastecimentoRepository.GetByIdAsync(id);
            if (abastecimento == null)
            {
                return null;
            }

            var veiculo = await _veiculoRepository.GetByIdAsync(abastecimento.VeiculoId);
            var abastecimentoDto = _mapper.Map<ReadAbastecimentoDto>(abastecimento);
            if (veiculo != null)
            {
                abastecimentoDto.PlacaVeiculo = veiculo.Placa;
            }

            if (abastecimento.MotoristaId != Guid.Empty)
            {
                var motorista = await _motoristaRepository.GetByIdAsync(abastecimento.MotoristaId);
                if (motorista != null)
                {
                    var usuarioDoMotorista = await _usuarioRepository.GetByIdAsync(motorista.UsuarioId);
                    if (usuarioDoMotorista != null)
                    {
                        abastecimentoDto.NomeMotorista = usuarioDoMotorista.Nome;
                    }
                }
            }

            return abastecimentoDto;
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateAbastecimentoDto dto)
        {
            var existingAbastecimento = await _abastecimentoRepository.GetByIdAsync(id);
            if (existingAbastecimento == null)
            {
                return false;
            }

            var originalKmInicial = existingAbastecimento.KmInicial;

            _mapper.Map(dto, existingAbastecimento);
            await _abastecimentoRepository.UpdateAsync(existingAbastecimento);

            if (dto.KmInicial.HasValue && dto.KmInicial.Value != originalKmInicial)
            {
                var veiculo = await _veiculoRepository.GetByIdAsync(existingAbastecimento.VeiculoId);
                if (veiculo != null && veiculo.QuilometragemAtual < existingAbastecimento.KmInicial)
                {
                    veiculo.QuilometragemAtual = existingAbastecimento.KmInicial;
                    await _veiculoRepository.UpdateAsync(veiculo);
                }
            }

            return true;
        }
    }
}
