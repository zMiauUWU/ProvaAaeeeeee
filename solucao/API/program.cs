namespace API;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public class program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddDbContext<AppDataContext>();
        var app = builder.Build();

        app.MapPost("/api/usuario/cadastrar", ([FromServices] AppDataContext ctx, [FromBody] Usuario usuario) =>
        {
            Usuario? usuario1 = ctx.Usuarios.FirstOrDefault(u => u.Nome.ToUpper() == usuario.Nome.ToUpper());

            if (usuario1 is null)
            {
                ctx.Usuarios.Add(usuario);
                ctx.SaveChanges();
                return Results.Created("usuario cadastrado com sucesso", usuario);
            }
            return Results.BadRequest("Usuario ja cadastrado");

        });

        app.MapPost("api/imc/cadastrar", ([FromServices] AppDataContext ctx, [FromBody] IMC NovoImc) =>
    {
        Usuario? Usuario = ctx.Usuarios.FirstOrDefault(a => a.Id == NovoImc.UsuarioId);
        
        if (Usuario is null) return Results.NotFound("Usuario não encontrado ou não cadastrado!");
        NovoImc.usuario = Usuario;
        NovoImc.Im = NovoImc.Peso / (NovoImc.Altura * NovoImc.Altura);

        if (NovoImc.Im < 18.5) NovoImc.Classificacao = "Magreza - GRAU 0";
        if (NovoImc.Im >= 18.6) NovoImc.Classificacao = "Normal - GRAU 0";
        if (NovoImc.Im >= 25.0) NovoImc.Classificacao = "Sobrepeso - GRAU I";
        if (NovoImc.Im >= 30.0) NovoImc.Classificacao = "Obesidade - GRAU II";
        if (NovoImc.Im >= 40.0) NovoImc.Classificacao = "Obesidade Grave - GRAU III";

        ctx.IMCs.Add(NovoImc);
        ctx.SaveChanges();
        return Results.Created("", NovoImc);
    });

    app.MapGet("/api/imc/listar", ([FromServices] AppDataContext ctx) =>
{
    return Results.Ok(ctx.IMCs.Include(x => x.usuario).ToList());
});

app.MapGet("api/imc/listarPorUsuario/{id}", ([FromServices] AppDataContext ctx, [FromRoute] int id) =>
{
    IMC? Imc = ctx.IMCs.Include(x => x.usuario).FirstOrDefault(a => a.UsuarioId == id);

    if (Imc is not null) return Results.Ok(Imc);
    
    return Results.NotFound("Nenhum IMC cadastrado");
});

app.MapPut("/api/imc/atualizar/{id}", ([FromServices] AppDataContext ctx, [FromBody] IMC ImcAlterado, [FromRoute] int id) =>
{

    IMC? Imc = ctx.IMCs.FirstOrDefault(a => a.UsuarioId == id);
    
    if (Imc is null) return Results.NotFound("Imc não encontrado ou não cadastrado!");
    Usuario? usuarioAlterado = ctx.Usuarios.Find(Imc.UsuarioId);

    Imc.usuario = usuarioAlterado;
    Imc.Peso = ImcAlterado.Peso;
    Imc.Altura = ImcAlterado.Altura;
    Imc.Im = ImcAlterado.Peso/(ImcAlterado.Altura * ImcAlterado.Altura);
    
    if(Imc.Im < 18.5) Imc.Classificacao = "Magreza - GRAU 0";
    if(Imc.Im >= 18.6) Imc.Classificacao = "Normal - GRAU 0";
    if(Imc.Im >= 25.0) Imc.Classificacao = "Sobrepeso - GRAU I";
    if(Imc.Im >= 30.0) Imc.Classificacao = "Obesidade - GRAU II";
    if(Imc.Im >= 40.0) Imc.Classificacao = "Obesidade Grave - GRAU III";
    
    ctx.IMCs.Update(Imc);
    ctx.SaveChanges();
    return Results.Created("", Imc);
});





        app.Run();
    }
}

