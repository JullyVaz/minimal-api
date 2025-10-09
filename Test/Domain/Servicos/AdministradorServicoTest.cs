using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Servicos;
using MinimalApi.Infraestrutura.Db;

namespace Test.Domain.Entidades;

[TestClass]
public class AdministradorServicoTest
{
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

    private void LimparTabelaAdministradores(DbContexto context)
    {
        // Desativa temporariamente foreign keys, limpa a tabela e reseta AUTO_INCREMENT
        context.Database.ExecuteSqlRaw("SET FOREIGN_KEY_CHECKS = 0;");
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores;");
        context.Database.ExecuteSqlRaw("ALTER TABLE Administradores AUTO_INCREMENT = 1;");
        context.Database.ExecuteSqlRaw("SET FOREIGN_KEY_CHECKS = 1;");
    }

    [TestMethod]
    public void TestandoSalvarAdministrador()
    {
        // Arrange
        var context = CriarContextoDeTeste();
        LimparTabelaAdministradores(context);

        var adm = new Administrador
        {
            Email = "teste@teste.com",
            Senha = "teste",
            Perfil = "Adm"
        };

        var administradorServico = new AdministradorServico(context);

        // Act
        administradorServico.Incluir(adm);

        // Assert
        var todos = administradorServico.Todos(1); // Ajuste caso o método Todos aceite outro parâmetro
        Assert.AreEqual(1, todos.Count(), "O número de administradores salvos deve ser 1.");
        Assert.AreEqual(adm.Id, todos.First().Id, "O ID do administrador salvo deve ser igual ao do objeto criado.");
    }

    [TestMethod]
    public void TestandoBuscaPorId()
    {
        // Arrange
        var context = CriarContextoDeTeste();
        LimparTabelaAdministradores(context);

        var adm = new Administrador
        {
            Email = "teste@teste.com",
            Senha = "teste",
            Perfil = "Adm"
        };

        var administradorServico = new AdministradorServico(context);

        // Act
        administradorServico.Incluir(adm);
        var admDoBanco = administradorServico.BuscaPorId(adm.Id);

        // Assert
        Assert.IsNotNull(admDoBanco, "O administrador buscado não deve ser nulo.");
        Assert.AreEqual(adm.Id, admDoBanco.Id, "O ID do administrador buscado deve ser igual ao do objeto criado.");
        Assert.AreEqual(adm.Email, admDoBanco.Email, "O email do administrador buscado deve ser igual ao do objeto criado.");
    }
}
