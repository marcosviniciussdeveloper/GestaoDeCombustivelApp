namespace Meucombustivel.Dtos.Veiculo
{
 

        public class CreateVeiculoDto
        {

            public Guid EmpresaId { get; set; }
            public string Placa { get; set; }

            public string Modelo { get; set; }

            public string TipoCombustivel { get; set; }

            public double Quilometragem_Atual { get; set; }

            public string Categoria { get; set; }

            public int Ano { get; set; }

            public string Marca { get; set; }

        }

        public class UpdateVeiculoDto
        {
              public Guid Empresa_Id { get; set; }

            public string? Placa { get; set; }
            public string? Modelo { get; set; }
            public string? TipoCombustivel { get; set; }
            public double? QuilometragemAtual { get; set; }
            public string? Categoria { get; set; }
            public int? Ano { get; set; }
            public string? Marca { get; set; }
        }

        public class ReadVeiculoDto
        {
            public Guid Id { get; set; }
            public string Placa { get; set; }
            public string Modelo { get; set; }
            public string TipoCombustivel { get; set; }
            public double QuilometragemAtual { get; set; }
            public string Categoria { get; set; }
            public int Ano { get; set; }
            public string Marca { get; set; }
            public DateTime CreatedAt { get; set; }
        }

    }

