using Application.Logic;
using Domain.Contracts;
using Infrastructure.FileHandling;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Presentation.EndPoints;

public static class PerfilsEndPoints
{
    public static void AddPerfilsEndPoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/perfils");

        group.WithOpenApi();

        group.MapPost(string.Empty, Add);
        group.MapPut(string.Empty, Update);
        group.MapPut("UpdateFoto/{id}", UpdateFoto).DisableAntiforgery();
        group.MapGet("{id}", GetById);
        group.MapGet("ByToken/{token}", GetByToken);
        group.MapGet("ByUsername/{username}", GetByUsername);
        group.MapGet("All", GetAll);
        group.MapPost("AllByIds", GetAllByIds);
        group.MapGet("Contains/{keyword}", GetWhenContains);
        group.MapPost("TryCreateOrUpdate", TryCreateOrUpdate);
    }

    public static async Task<IResult> Add(
        [FromServices] IPerfilBusinessLogic _logic,
        [FromBody] CreatePerfilRequest _request
    )
    {
        try
        {
            await _logic.AddPerfil(_request);
            return Results.Ok();
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    public static async Task<IResult> Update(
        [FromServices] IPerfilBusinessLogic _logic,
        [FromBody] UpdatePerfilRequest _request
    )
    {
        try
        {
            await _logic.UpdatePerfil(_request);
            return Results.Ok();
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    public static async Task<IResult> UpdateFoto(
        [FromServices] IPerfilBusinessLogic _logic,
        Guid id,
        IFormFile file,
        ISaveFile saveFile
    )
    {
        if (!file.ContentType.ToLower().StartsWith("image/", StringComparison.Ordinal))
        {
            return Results.BadRequest("O arquivo enviado não é uma imagem.");
        }
        try
        {
            var uniqueFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var filePath = saveFile.BuildPathFileSave(uniqueFileName, "profile");
            await saveFile.SaveImageFile(file, filePath);
            await _logic.UpdateFotoPerfil(id, filePath);
            return Results.Ok();
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    public static async Task<IResult> GetAll([FromServices] IPerfilBusinessLogic _logic)
    {
        try
        {
            return Results.Ok(await _logic.GetAll());
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    public static async Task<IResult> GetById([FromServices] IPerfilBusinessLogic _logic, Guid id)
    {
        try
        {
            return Results.Ok(await _logic.GetById(id));
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    public static async Task<IResult> GetAllByIds(
        [FromServices] IPerfilBusinessLogic _logic,
        [FromBody] List<Guid> ids
    )
    {
        try
        {
            return Results.Ok(await _logic.GetByIds(ids));
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    public static async Task<IResult> GetByUsername(
        [FromServices] IPerfilBusinessLogic _logic,
        string username
    )
    {
        try
        {
            return Results.Ok(await _logic.GetByUsername(username));
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    public static async Task<IResult> GetByToken(
        [FromServices] IPerfilBusinessLogic _logic,
        string token
    )
    {
        try
        {
            return Results.Ok(await _logic.GetByToken(token));
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    public static async Task<IResult> GetWhenContains(
        [FromServices] IPerfilBusinessLogic _logic,
        string keyword
    )
    {
        try
        {
            return Results.Ok(await _logic.GetWhenContainsAsync(keyword));
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    public static async Task<IResult> TryCreateOrUpdate(
        [FromServices] IPerfilBusinessLogic _logic,
        [FromBody] CreateOrUpdatePerfilRequest request
    )
    {
        try
        {
            var response = await _logic.TryCreateOrUpdate(request);
            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }
}
