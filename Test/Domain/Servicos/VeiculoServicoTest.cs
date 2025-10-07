using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Servicos;
using MinimalApi.Infraestrutura.Db;

namespace Test.Domain.Entidades;

[TestClass]
public class VeiculoServicoTest
{
    private DbContexto? _context;

    [TestInitialize]
    public void Setup()
    {
        _context = CriarContextoDeTeste();

        // Limpa a tabela Veiculos e reinicia AUTO_INCREMENT
        _context.Database.ExecuteSqlRaw("SET FOREIGN_KEY_CHECKS = 0;");
        _context.Database.ExecuteSqlRaw("TRUNCATE TABLE Veiculos;");
        _context.Database.ExecuteSqlRaw("SET FOREIGN_KEY_CHECKS = 1;");
        _context.SaveChanges();
    }

    private DbContexto CriarContextoDeTeste()
    {
        var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var path = Path.GetFullPath(Path.Combine(assemblyPath ?? "", "..", "..", ".."));

        var builder = new ConfigurationBuilder()
            .SetBasePath(path ?? Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

        var configuration = builder.Build();

        return new DbContexto(configuration);
    }

    [TestMethod]
    public void TestandoIncluirVeiculo()
    {
        var veiculo = new Veiculo { Nome = "Fusca", Marca = "Volkswagen" };
        var veiculoServico = new VeiculoServico(_context!);

        veiculoServico.Incluir(veiculo);
        var veiculoDoBanco = veiculoServico.BuscaPorId(veiculo.Id);

        Assert.IsNotNull(veiculoDoBanco, "Veículo não foi encontrado no banco.");
        Assert.AreEqual("Fusca", veiculoDoBanco!.Nome);
        Assert.AreEqual("Volkswagen", veiculoDoBanco.Marca);
    }

    [TestMethod]
    public void TestandoTodosVeiculosComFiltro()
    {
        var veiculo1 = new Veiculo { Nome = "Fusca", Marca = "Volkswagen" };
        var veiculo2 = new Veiculo { Nome = "Gol", Marca = "Volkswagen" };
        var veiculo3 = new Veiculo { Nome = "Civic", Marca = "Honda" };

        var veiculoServico = new VeiculoServico(_context!);
        veiculoServico.Incluir(veiculo1);
        veiculoServico.Incluir(veiculo2);
        veiculoServico.Incluir(veiculo3);

        var todosVolks = veiculoServico.Todos(1, marca: "Volkswagen");

        // Valida que todos retornados são da marca Volkswagen
        Assert.IsTrue(todosVolks.All(v => v.Marca.ToLower() == "volkswagen"),
            "Nem todos os veículos retornados são Volkswagen.");

        // Valida que ao menos 2 veículos foram encontrados
        Assert.IsTrue(todosVolks.Count >= 2,
            "O número de veículos Volkswagen retornados é menor que o esperado.");
    }

    [TestMethod]
    public void TestandoAtualizarVeiculo()
    {
        var veiculo = new Veiculo { Nome = "Uno", Marca = "Fiat" };
        var veiculoServico = new VeiculoServico(_context!);

        veiculoServico.Incluir(veiculo);

        veiculo.Nome = "Uno Mille";
        veiculoServico.Atualizar(veiculo);

        var veiculoAtualizado = veiculoServico.BuscaPorId(veiculo.Id);
        Assert.IsNotNull(veiculoAtualizado);
        Assert.AreEqual("Uno Mille", veiculoAtualizado!.Nome);
    }

    [TestMethod]
    public void TestandoApagarVeiculo()
    {
        var veiculo = new Veiculo { Nome = "Civic", Marca = "Honda" };
        var veiculoServico = new VeiculoServico(_context!);

        veiculoServico.Incluir(veiculo);
        veiculoServico.Apagar(veiculo);

        var veiculoDoBanco = veiculoServico.BuscaPorId(veiculo.Id);
        Assert.IsNull(veiculoDoBanco, "Veículo não foi apagado do banco.");
    }
}
