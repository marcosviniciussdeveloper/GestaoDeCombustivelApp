using Moq;
using AutoMapper;

using Meucombustivel.Dtos.Abastecimento;
using Meucombustivel.Models;
using Meucombustivel.Repositories.Interfaces;
using Meucombustivel.Services;


namespace MeucombustivelTeste.Services
{
    public class AbastecimentoServiceTests
    {
        private readonly Mock<IAbastecimentoRepository> _mockAbastecimentoRepository;
        private readonly Mock<IVeiculoRepository> _mockVeiculoRepository;
        private readonly Mock<IMotoristaRepository> _mockMotoristaRepository;
        private readonly Mock<IUsuarioRepository> _mockUsuarioRepository;
        private readonly Mock<IMapper> _mockMapper;

        private readonly AbastecimentoService _abastecimentoService;

        public AbastecimentoServiceTests()
        {
            _mockAbastecimentoRepository = new Mock<IAbastecimentoRepository>();
            _mockVeiculoRepository = new Mock<IVeiculoRepository>();
            _mockMotoristaRepository = new Mock<IMotoristaRepository>();
            _mockUsuarioRepository = new Mock<IUsuarioRepository>();
            _mockMapper = new Mock<IMapper>();

            _abastecimentoService = new AbastecimentoService(
                _mockAbastecimentoRepository.Object,
                _mockVeiculoRepository.Object,
                _mockMotoristaRepository.Object,
                _mockUsuarioRepository.Object,
                _mockMapper.Object
            );
            

        }

        [Fact]
        public async Task CreateAsync_DeveRetornarGuidQuandoAbastecimentoCriadoComSucesso()
        {
            var createDto = new CreateAbastecimentoDto
            {
                VeiculoId = Guid.NewGuid(),
                KmInicial = 10000,
                Litros = 10.0m,
                TipoCombustivel = "Gasolina",
                Data = DateTime.Now,
                Custo = 50.0,
                NotaFiscalUrl = "http://nota.com/123",
                Localização = "Posto X"
            };
            var veiculo = new Veiculo { Id = createDto.VeiculoId, QuilometragemAtual = 900 };
            
            Abastecimento abastecimentoCapturadoPeloAdd = null;

            _mockVeiculoRepository.Setup(repo => repo.GetByIdAsync(createDto.VeiculoId)).ReturnsAsync(veiculo);

            _mockMapper.Setup(mapper => mapper.Map<Abastecimento>(createDto))
                       .Returns((CreateAbastecimentoDto src) => new Abastecimento
                       {
                           VeiculoId = src.VeiculoId,
                           KmInicial = (int)src.KmInicial,
                           Litros = src.Litros,
                           TipoCombustivel = src.TipoCombustivel,
                           Data = src.Data,
                           Custo = (decimal)src.Custo,
                           NotaFiscalUrl = src.NotaFiscalUrl,
                           Localizacao = src.Localização
                       });

            _mockAbastecimentoRepository.Setup(repo => repo.AddAsync(It.IsAny<Abastecimento>()))
                                       .ReturnsAsync((Abastecimento a) => { abastecimentoCapturadoPeloAdd = a; return a; });
            
            _mockVeiculoRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Veiculo>()))
                                 .Returns(Task.CompletedTask);

            var resultGuid = await _abastecimentoService.CreateAsync(createDto);

            Assert.NotEqual(Guid.Empty, resultGuid);
            _mockVeiculoRepository.Verify(repo => repo.GetByIdAsync(createDto.VeiculoId), Times.Once);
            _mockMapper.Verify(mapper => mapper.Map<Abastecimento>(createDto), Times.Once);
            _mockAbastecimentoRepository.Verify(repo => repo.AddAsync(It.Is<Abastecimento>(a => a.Id == resultGuid)), Times.Once);
            _mockVeiculoRepository.Verify(repo => repo.UpdateAsync(It.Is<Veiculo>(v => v.QuilometragemAtual == abastecimentoCapturadoPeloAdd.KmInicial)), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_DeveLancarInvalidOperationExceptionQuandoVeiculoNaoEncontrado()
        {
            var createDto = new CreateAbastecimentoDto { VeiculoId = Guid.NewGuid() };
            _mockVeiculoRepository.Setup(repo => repo.GetByIdAsync(createDto.VeiculoId)).ReturnsAsync((Veiculo)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _abastecimentoService.CreateAsync(createDto));
            _mockVeiculoRepository.Verify(repo => repo.GetByIdAsync(createDto.VeiculoId), Times.Once);
            _mockMapper.Verify(mapper => mapper.Map<Abastecimento>(It.IsAny<CreateAbastecimentoDto>()), Times.Never);
            _mockAbastecimentoRepository.Verify(repo => repo.AddAsync(It.IsAny<Abastecimento>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_NaoDeveAtualizarQuilometragemSeKmInicialNaoForMaior()
        {
            var createDto = new CreateAbastecimentoDto { VeiculoId = Guid.NewGuid(), KmInicial = 1000 };
            var veiculo = new Veiculo { Id = createDto.VeiculoId, QuilometragemAtual = 1100 };
            
            _mockVeiculoRepository.Setup(repo => repo.GetByIdAsync(createDto.VeiculoId)).ReturnsAsync(veiculo);
            
            _mockMapper.Setup(mapper => mapper.Map<Abastecimento>(It.IsAny<CreateAbastecimentoDto>()))
                       .Returns((CreateAbastecimentoDto src) => new Abastecimento
                       {
                           VeiculoId = src.VeiculoId,
                           KmInicial = (int)src.KmInicial,
                           Litros = 0.0m, 
                           TipoCombustivel = "N/A",
                           Data = DateTime.Now,
                           Custo = 0.0m,
                           NotaFiscalUrl = "",
                           Localizacao = ""
                       });

            _mockAbastecimentoRepository.Setup(repo => repo.AddAsync(It.IsAny<Abastecimento>())).ReturnsAsync(new Abastecimento()); 

            await _abastecimentoService.CreateAsync(createDto);

            _mockVeiculoRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Veiculo>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_DeveRetornarTrueQuandoAbastecimentoEncontradoEDeletado()
        {
            var abastecimentoId = Guid.NewGuid();
            var existingAbastecimento = new Abastecimento { Id = abastecimentoId };

            _mockAbastecimentoRepository.Setup(repo => repo.GetByIdAsync(abastecimentoId)).ReturnsAsync(existingAbastecimento);
            _mockAbastecimentoRepository.Setup(repo => repo.DeleteAsync(abastecimentoId)).Returns(Task.CompletedTask);

            var result = await _abastecimentoService.DeleteAsync(abastecimentoId);

            Assert.True(result);
            _mockAbastecimentoRepository.Verify(repo => repo.GetByIdAsync(abastecimentoId), Times.Once);
            _mockAbastecimentoRepository.Verify(repo => repo.DeleteAsync(abastecimentoId), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_DeveRetornarFalseQuandoAbastecimentoNaoEncontrado()
        {
            var abastecimentoId = Guid.NewGuid();
            _mockAbastecimentoRepository.Setup(repo => repo.GetByIdAsync(abastecimentoId)).ReturnsAsync((Abastecimento)null);

            var result = await _abastecimentoService.DeleteAsync(abastecimentoId);

            Assert.False(result);
            _mockAbastecimentoRepository.Verify(repo => repo.GetByIdAsync(abastecimentoId), Times.Once);
            _mockAbastecimentoRepository.Verify(repo => repo.DeleteAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task GetByIdAsync_DeveRetornarDtoQuandoAbastecimentoEncontrado() 
        {
            var abastecimentoId = Guid.NewGuid();
            var veiculoId = Guid.NewGuid();
            var motoristaId = Guid.NewGuid();
            var usuarioId = Guid.NewGuid();

            var abastecimentoModel = new Abastecimento { Id = abastecimentoId, VeiculoId = veiculoId, MotoristaId = motoristaId };
            var veiculoModel = new Veiculo { Id = veiculoId, Placa = "ABC-1234" };
            var motoristaModel = new Motorista { UsuarioId = usuarioId }; 
            var usuarioModel = new Usuarios { Id = usuarioId, Nome = "João Motorista" };

            var readDto = new ReadAbastecimentoDto { Id = abastecimentoId };

            _mockAbastecimentoRepository.Setup(repo => repo.GetByIdAsync(abastecimentoId)).ReturnsAsync(abastecimentoModel);
            _mockVeiculoRepository.Setup(repo => repo.GetByIdAsync(veiculoId)).ReturnsAsync(veiculoModel);
            _mockMotoristaRepository.Setup(repo => repo.GetByIdAsync(motoristaId)).ReturnsAsync(motoristaModel);
            _mockUsuarioRepository.Setup(repo => repo.GetByIdAsync(usuarioId)).ReturnsAsync(usuarioModel);
            _mockMapper.Setup(mapper => mapper.Map<ReadAbastecimentoDto>(abastecimentoModel)).Returns(readDto);

            var result = await _abastecimentoService.GetByIdAsync(abastecimentoId); 

            Assert.NotNull(result);
            Assert.Equal(abastecimentoId, result.Id);
            Assert.Equal("ABC-1234", result.PlacaVeiculo);
            Assert.Equal("João Motorista", result.NomeMotorista);

            _mockAbastecimentoRepository.Verify(repo => repo.GetByIdAsync(abastecimentoId), Times.Once);
            _mockVeiculoRepository.Verify(repo => repo.GetByIdAsync(veiculoId), Times.Once);
            _mockMotoristaRepository.Verify(repo => repo.GetByIdAsync(motoristaId), Times.Once);
            _mockUsuarioRepository.Verify(repo => repo.GetByIdAsync(usuarioId), Times.Once);
            _mockMapper.Verify(mapper => mapper.Map<ReadAbastecimentoDto>(abastecimentoModel), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_DeveRetornarNullQuandoAbastecimentoNaoEncontrado() 
        {
            var abastecimentoId = Guid.NewGuid();
            _mockAbastecimentoRepository.Setup(repo => repo.GetByIdAsync(abastecimentoId)).ReturnsAsync((Abastecimento)null);

            var result = await _abastecimentoService.GetByIdAsync(abastecimentoId); 

            Assert.Null(result);
            _mockAbastecimentoRepository.Verify(repo => repo.GetByIdAsync(abastecimentoId), Times.Once);
            _mockVeiculoRepository.Verify(repo => repo.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
            _mockMapper.Verify(mapper => mapper.Map<ReadAbastecimentoDto>(It.IsAny<Abastecimento>()), Times.Never);
        }

        [Fact]
        public async Task GetByIdAsync_DeveRetornarDtoSemPlacaOuNomeMotoristaSeNaoEncontrados() 
        {
            var abastecimentoId = Guid.NewGuid();
            var veiculoId = Guid.NewGuid();
            var motoristaId = Guid.NewGuid();

            var abastecimentoModel = new Abastecimento { Id = abastecimentoId, VeiculoId = veiculoId, MotoristaId = motoristaId };
            var readDto = new ReadAbastecimentoDto { Id = abastecimentoId };

            _mockAbastecimentoRepository.Setup(repo => repo.GetByIdAsync(abastecimentoId)).ReturnsAsync(abastecimentoModel);
            _mockVeiculoRepository.Setup(repo => repo.GetByIdAsync(veiculoId)).ReturnsAsync((Veiculo)null);
            _mockMotoristaRepository.Setup(repo => repo.GetByIdAsync(motoristaId)).ReturnsAsync((Motorista)null);
            _mockMapper.Setup(mapper => mapper.Map<ReadAbastecimentoDto>(abastecimentoModel)).Returns(readDto);

            var result = await _abastecimentoService.GetByIdAsync(abastecimentoId); 

            Assert.NotNull(result);
            Assert.Equal(abastecimentoId, result.Id);
            Assert.Null(result.PlacaVeiculo);
            Assert.Null(result.NomeMotorista);
        }

        [Fact]
        public async Task GetAllByVeiculoAsync_DeveRetornarListaDeDtosComDadosCompletos() 
        {
            var veiculoId = Guid.NewGuid();
            var motoristaId1 = Guid.NewGuid();
            var motoristaId2 = Guid.Empty;
            var usuarioId1 = Guid.NewGuid();

            var abastecimentosModels = new List<Abastecimento>
            {
                new Abastecimento { Id = Guid.NewGuid(), VeiculoId = veiculoId, MotoristaId = motoristaId1, KmInicial = 100 },
                new Abastecimento { Id = Guid.NewGuid(), VeiculoId = veiculoId, MotoristaId = motoristaId2, KmInicial = 200 }
            };
            var veiculoModel = new Veiculo { Id = veiculoId, Placa = "XYZ-5678" };
            var motoristaModel1 = new Motorista { UsuarioId = usuarioId1 }; 
            var usuarioModel1 = new Usuarios { Id = usuarioId1, Nome = "Maria Motorista" };

            _mockAbastecimentoRepository.Setup(repo => repo.GetAbastecimentosByVeiculoIdAsync(veiculoId)).ReturnsAsync(abastecimentosModels);
            _mockVeiculoRepository.Setup(repo => repo.GetByIdAsync(veiculoId)).ReturnsAsync(veiculoModel);
            _mockMotoristaRepository.Setup(repo => repo.GetByIdAsync(motoristaId1)).ReturnsAsync(motoristaModel1);
            _mockUsuarioRepository.Setup(repo => repo.GetByIdAsync(usuarioId1)).ReturnsAsync(usuarioModel1);
            
            _mockMapper.Setup(mapper => mapper.Map<ReadAbastecimentoDto>(abastecimentosModels[0]))
                       .Returns(new ReadAbastecimentoDto { Id = abastecimentosModels[0].Id });
            _mockMapper.Setup(mapper => mapper.Map<ReadAbastecimentoDto>(abastecimentosModels[1]))
                       .Returns(new ReadAbastecimentoDto { Id = abastecimentosModels[1].Id });

            var result = await _abastecimentoService.GetAllByVeiculoAsync(veiculoId); 

            Assert.NotNull(result);
            var resultList = result.ToList();
            Assert.Equal(2, resultList.Count);

            Assert.Equal("XYZ-5678", resultList[0].PlacaVeiculo);
            Assert.Equal("Maria Motorista", resultList[0].NomeMotorista);

            Assert.Equal("XYZ-5678", resultList[1].PlacaVeiculo);
            Assert.Null(resultList[1].NomeMotorista);

            _mockAbastecimentoRepository.Verify(repo => repo.GetAbastecimentosByVeiculoIdAsync(veiculoId), Times.Once);
            _mockVeiculoRepository.Verify(repo => repo.GetByIdAsync(veiculoId), Times.Once);
            _mockMotoristaRepository.Verify(repo => repo.GetByIdAsync(motoristaId1), Times.Once);
            _mockMotoristaRepository.Verify(repo => repo.GetByIdAsync(Guid.Empty), Times.Never); 
            _mockUsuarioRepository.Verify(repo => repo.GetByIdAsync(usuarioId1), Times.Once);
         
            _mockUsuarioRepository.Verify(repo => repo.GetByIdAsync(It.Is<Guid>(g => g == Guid.Empty)), Times.Never);
        }

        [Fact]
        public async Task GetAllByVeiculoAsync_DeveRetornarListaVaziaQuandoNenhumAbastecimentoEncontrado() 
        {
            var veiculoId = Guid.NewGuid();
            _mockAbastecimentoRepository.Setup(repo => repo.GetAbastecimentosByVeiculoIdAsync(veiculoId)).ReturnsAsync(new List<Abastecimento>());
            _mockVeiculoRepository.Setup(repo => repo.GetByIdAsync(veiculoId)).ReturnsAsync(new Veiculo { Id = veiculoId, Placa = "TESTE" });

            var result = await _abastecimentoService.GetAllByVeiculoAsync(veiculoId); 

            Assert.NotNull(result);
            Assert.Empty(result);
            _mockAbastecimentoRepository.Verify(repo => repo.GetAbastecimentosByVeiculoIdAsync(veiculoId), Times.Once);
            _mockVeiculoRepository.Verify(repo => repo.GetByIdAsync(veiculoId), Times.Once);
            _mockMotoristaRepository.Verify(repo => repo.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
            _mockUsuarioRepository.Verify(repo => repo.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
            _mockMapper.Verify(mapper => mapper.Map<ReadAbastecimentoDto>(It.IsAny<Abastecimento>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_DeveRetornarTrueEAtualizarAbastecimentoQuandoEncontrado()
        {
            var abastecimentoId = Guid.NewGuid();
            var originalAbastecimento = new Abastecimento { Id = abastecimentoId, KmInicial = 500, VeiculoId = Guid.NewGuid() };
            var updateDto = new UpdateAbastecimentoDto { KmInicial = 600, Custo = 150m };

            _mockAbastecimentoRepository.Setup(repo => repo.GetByIdAsync(abastecimentoId)).ReturnsAsync(originalAbastecimento);
            _mockMapper.Setup(mapper => mapper.Map(updateDto, originalAbastecimento));
            _mockAbastecimentoRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Abastecimento>())).Returns(Task.CompletedTask);
            
            _mockVeiculoRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Veiculo { QuilometragemAtual = 700 });
            _mockVeiculoRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Veiculo>()), Times.Never);

            var result = await _abastecimentoService.UpdateAsync(abastecimentoId, updateDto);

            Assert.True(result);
            _mockAbastecimentoRepository.Verify(repo => repo.GetByIdAsync(abastecimentoId), Times.Once);
            _mockMapper.Verify(mapper => mapper.Map(updateDto, originalAbastecimento), Times.Once);
            _mockAbastecimentoRepository.Verify(repo => repo.UpdateAsync(originalAbastecimento), Times.Once);
            _mockVeiculoRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Veiculo>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_DeveRetornarFalseQuandoAbastecimentoNaoEncontrado()
        {
            var abastecimentoId = Guid.NewGuid();
            var updateDto = new UpdateAbastecimentoDto { KmInicial = 700 };

            _mockAbastecimentoRepository.Setup(repo => repo.GetByIdAsync(abastecimentoId)).ReturnsAsync((Abastecimento)null);

            var result = await _abastecimentoService.UpdateAsync(abastecimentoId, updateDto);

            Assert.False(result);
            _mockAbastecimentoRepository.Verify(repo => repo.GetByIdAsync(abastecimentoId), Times.Once);
            _mockMapper.Verify(mapper => mapper.Map(It.IsAny<UpdateAbastecimentoDto>(), It.IsAny<Abastecimento>()), Times.Never);
            _mockAbastecimentoRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Abastecimento>()), Times.Never);
            _mockVeiculoRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Veiculo>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_DeveAtualizarQuilometragemDoVeiculoSeKmInicialAumentar()
        {
            var abastecimentoId = Guid.NewGuid();
            var veiculoId = Guid.NewGuid();
            var originalAbastecimento = new Abastecimento { Id = abastecimentoId, KmInicial = 500, VeiculoId = veiculoId };
            var updateDto = new UpdateAbastecimentoDto { KmInicial = 600 };
            var veiculoModel = new Veiculo { Id = veiculoId, QuilometragemAtual = 550 };

            _mockAbastecimentoRepository.Setup(repo => repo.GetByIdAsync(abastecimentoId)).ReturnsAsync(originalAbastecimento);
            _mockMapper.Setup(mapper => mapper.Map(updateDto, originalAbastecimento))
                       .Callback<UpdateAbastecimentoDto, Abastecimento>((dto, model) => { 
                           if (dto.KmInicial.HasValue) 
                           {
                               model.KmInicial = (int)dto.KmInicial.Value; 
                           }
                       });
            _mockAbastecimentoRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Abastecimento>())).Returns(Task.CompletedTask);
            _mockVeiculoRepository.Setup(repo => repo.GetByIdAsync(veiculoId)).ReturnsAsync(veiculoModel);
            _mockVeiculoRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Veiculo>())).Returns(Task.CompletedTask);

            var result = await _abastecimentoService.UpdateAsync(abastecimentoId, updateDto);

            Assert.True(result);
            _mockAbastecimentoRepository.Verify(repo => repo.UpdateAsync(originalAbastecimento), Times.Once);
            _mockVeiculoRepository.Verify(repo => repo.GetByIdAsync(veiculoId), Times.Once);
            _mockVeiculoRepository.Verify(repo => repo.UpdateAsync(It.Is<Veiculo>(v => v.QuilometragemAtual == originalAbastecimento.KmInicial)), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_NaoDeveAtualizarQuilometragemDoVeiculoSeKmInicialNaoAumentar()
        {
            var abastecimentoId = Guid.NewGuid();
            var veiculoId = Guid.NewGuid();
            var originalAbastecimento = new Abastecimento { Id = abastecimentoId, KmInicial = 500, VeiculoId = veiculoId };
            var updateDto = new UpdateAbastecimentoDto { KmInicial = 450 };
            var veiculoModel = new Veiculo { Id = veiculoId, QuilometragemAtual = 550 };

            _mockAbastecimentoRepository.Setup(repo => repo.GetByIdAsync(abastecimentoId)).ReturnsAsync(originalAbastecimento);
            _mockMapper.Setup(mapper => mapper.Map(updateDto, originalAbastecimento))
                       .Callback<UpdateAbastecimentoDto, Abastecimento>((dto, model) => { 
                           if (dto.KmInicial.HasValue) 
                           {
                               model.KmInicial = (int)dto.KmInicial.Value; 
                           }
                       });
            _mockAbastecimentoRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Abastecimento>())).Returns(Task.CompletedTask);
            _mockVeiculoRepository.Setup(repo => repo.GetByIdAsync(veiculoId)).ReturnsAsync(veiculoModel); 
            
            var result = await _abastecimentoService.UpdateAsync(abastecimentoId, updateDto);

            Assert.True(result);
            _mockAbastecimentoRepository.Verify(repo => repo.UpdateAsync(originalAbastecimento), Times.Once);
            _mockVeiculoRepository.Verify(repo => repo.GetByIdAsync(veiculoId), Times.Once);
            _mockVeiculoRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Veiculo>()), Times.Never);
        }
    }
}
