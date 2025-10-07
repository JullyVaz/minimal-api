using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Interfaces;

namespace Test.Helpers.Mocks;

public class VeiculoServicoMock : IVeiculoServico
{
    private static List<Veiculo> veiculos = new List<Veiculo>()
    {
        new Veiculo
        {
            Id = 1,
            Marca = "Toyota",
            Nome = "Corolla",
            Ano = 2020
        },
        new Veiculo
        {
            Id = 2,
            Marca = "Honda",
            Nome = "Civic",
            Ano = 2021
        }
    };

    public Veiculo? BuscaPorId(int id)
    {
        return veiculos.FirstOrDefault(v => v.Id == id);
    }

    public void Incluir(Veiculo veiculo)
    {
        veiculo.Id = veiculos.Count + 1;
        veiculos.Add(veiculo);
    }

    public void Atualizar(Veiculo veiculo)
    {
        var existente = veiculos.FirstOrDefault(v => v.Id == veiculo.Id);
        if (existente != null)
        {
            existente.Marca = veiculo.Marca;
            existente.Nome = veiculo.Nome;
            existente.Ano = veiculo.Ano;
        }
    }

    public void Apagar(Veiculo veiculo)
    {
        veiculos.RemoveAll(v => v.Id == veiculo.Id);
    }

    public List<Veiculo> Todos(int? pagina, string? marca, string? nome)
    {
        IEnumerable<Veiculo> query = veiculos;

        if (!string.IsNullOrWhiteSpace(marca))
            query = query.Where(v => v.Marca.Equals(marca, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(nome))
            query = query.Where(v => v.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase));

        // Exemplo simples de paginação (5 registros por página)
        if (pagina.HasValue && pagina.Value > 0)
            query = query.Skip((pagina.Value - 1) * 5).Take(5);

        return query.ToList();
    }
}
