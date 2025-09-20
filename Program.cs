using Microsoft.EntityFrameworkCore;
using MinimalApi.DTOs;
using MinimalApi.Infraestrutura.Db;
using MinimalApi.Dominio.Servicos;
using MinimalApi.Dominio.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MinimalApi.Dominio.ModelViews;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Enuns;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();
        builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddDbContext<DbContexto>(options =>
        {
            options.UseMySql(
                builder.Configuration.GetConnectionString("mysql"),
                ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
            );
        });

        var app = builder.Build();

        #region Home
        app.MapGet("/", () => Results.Content(
            "<h2>Bem-vindo à API de veículos - Minimal API</h2>" +
            "<p>Documentação: <a href='/swagger' target='_blank'>Swagger</a></p>",
            "text/html; charset=utf-8"
        ))
        .WithTags("Home");
        #endregion

        #region Administradores
        app.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico) =>
        {
            if (administradorServico.Login(loginDTO) != null)
                return Results.Ok("Login com sucesso");
            else
                return Results.Unauthorized();
        }).WithTags("Administradores");

        _ = app.MapPost("/administradores", ([FromBody] AdministradorDTO administradorDTO, IAdministradorServico administradorServico) =>
       {
           var validacao = new ErrosDeValidacao
           {
               Mensagens = new List<string>()
           };

           if (string.IsNullOrEmpty(administradorDTO.Email))
               validacao.Mensagens.Add("Email não pode ser vazio");

           if (string.IsNullOrEmpty(administradorDTO.Senha))
               validacao.Mensagens.Add("Senha não pode ser vazia");

           if (administradorDTO.Perfil == null)
           {
               validacao.Mensagens.Add("Perfil não pode ser vazio");
           }

           if (validacao.Mensagens.Count > 0)
               return Results.BadRequest(validacao);

           var veiculo = new Administrador
           {
               Email = administradorDTO.Email,
               Senha = administradorDTO.Senha,
               Perfil = administradorDTO.Perfil!.Value
           };

           return Results.Ok(veiculo);
       }).WithTags("Veiculos");


        #endregion

        #region Veiculos
        ErrosDeValidacao ValidaDTO(VeiculoDTO veiculoDTO)
        {
            var validacao = new ErrosDeValidacao
            {
                Mensagens = new List<string>()
            };

            if (string.IsNullOrEmpty(veiculoDTO.Nome))
                validacao.Mensagens.Add("O nome não pode ficar em branco");

            if (string.IsNullOrEmpty(veiculoDTO.Marca))
                validacao.Mensagens.Add("A marca não pode ficar em branco");

            if (veiculoDTO.Ano < 1950)
                validacao.Mensagens.Add("Veículo muito antigo, aceito somente anos superiores a 1950");

            return validacao;
        }

        app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
        {
            var validacao = ValidaDTO(veiculoDTO);
            if (validacao.Mensagens.Count > 0)
                return Results.BadRequest(validacao);

            var veiculo = new Veiculo
            {
                Nome = veiculoDTO.Nome,
                Marca = veiculoDTO.Marca,
                Ano = veiculoDTO.Ano,
            };

            veiculoServico.Incluir(veiculo);

            return Results.Created($"/veiculo/{veiculo.Id}", veiculo);
        }).WithTags("Veiculos");

        app.MapGet("/veiculos", ([FromQuery] int? pagina, IVeiculoServico veiculoServico) =>
        {
            var veiculos = veiculoServico.Todos(pagina);
            return Results.Ok(veiculos);
        }).WithTags("Veiculos");

        app.MapGet("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
        {
            var veiculo = veiculoServico.BuscaPorId(id);
            if (veiculo == null) return Results.NotFound();
            return Results.Ok(veiculo);
        }).WithTags("Veiculos");

        app.MapPut("/veiculos/{id}", ([FromRoute] int id, VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
        {
            var veiculo = veiculoServico.BuscaPorId(id);
            if (veiculo == null) return Results.NotFound();

            var validacao = ValidaDTO(veiculoDTO);
            if (validacao.Mensagens.Count > 0)
                return Results.BadRequest(validacao);

            veiculo.Nome = veiculoDTO.Nome;
            veiculo.Marca = veiculoDTO.Marca;
            veiculo.Ano = veiculoDTO.Ano;

            veiculoServico.Atualizar(veiculo);

            return Results.Ok(veiculo);
        }).WithTags("Veiculos");

        app.MapDelete("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
        {
            var veiculo = veiculoServico.BuscaPorId(id);
            if (veiculo == null) return Results.NotFound();

            veiculoServico.Apagar(veiculo);

            return Results.NoContent();
        }).WithTags("Veiculos");
        #endregion

        #region App
        app.UseSwagger();
        app.UseSwaggerUI();

        app.Run();
        #endregion
    }
}
