// PhotoProcessor.Api/Controllers/ImagesController.cs
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/images")]
public class ImagesController : ControllerBase
{
    [HttpPost]
    public IActionResult UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("Nenhum arquivo enviado.");
        }

        // POR ENQUANTO: Salvar o arquivo localmente para testar
        var tempPath = Path.Combine("uploads"); // Cria uma pasta 'uploads' no seu projeto
        if (!Directory.Exists(tempPath))
        {
            Directory.CreateDirectory(tempPath);
        }

        var filePath = Path.Combine(tempPath, file.FileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            file.CopyTo(stream);
        }

        // No futuro, aqui enviaremos para uma fila.
        // Por enquanto, retornamos um OK.
        var response = new {
            File = file.FileName,
            Size = file.Length,
            Status = "Upload concluído com sucesso."
        };

        return Ok(response);
    }
}