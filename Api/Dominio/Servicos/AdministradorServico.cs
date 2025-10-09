using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.DTOs;
using MinimalApi.Infraestrutura.Db;

namespace MinimalApi.Dominio.Servicos;

public class AdministradorServico : IAdministradorServico
{
    private readonly DbContexto _contexto;

    public AdministradorServico(DbContexto contexto)
    {
        _contexto = contexto;
    }

    public Administrador? BuscaPorId(int id)
    {
        return _contexto.Administradores
                        .Where(v => v.Id == id)
                        .FirstOrDefault();
    }

    public Administrador Incluir(Administrador administrador)
    {
        _contexto.Administradores.Add(administrador);
        _contexto.SaveChanges();
        return administrador;
    }

    public Administrador? Login(LoginDTO loginDTO)
    {
        return _contexto.Administradores
                        .Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha)
                        .FirstOrDefault();
    }

    /// <summary>
    /// Retorna todos os administradores com opção de paginação.
    /// Remove a senha antes de devolver os dados.
    /// </summary>

    public List<Administrador> Todos(int? pagina = null)
    {
        var query = _contexto.Administradores.AsQueryable();

        if (pagina.HasValue)
        {
            int itensPorPagina = 10;
            query = query.Skip((pagina.Value - 1) * itensPorPagina)
                         .Take(itensPorPagina);
        }

        var administradores = query.ToList();

        // Remove a senha para evitar exposição de dados sensíveis
        administradores.ForEach(adm => adm.Senha = string.Empty);

        return administradores;
    }
}



