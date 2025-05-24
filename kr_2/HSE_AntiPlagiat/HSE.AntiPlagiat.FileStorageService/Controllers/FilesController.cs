using Microsoft.AspNetCore.Mvc;
/// <summary>
/// Контроллер для управления файлами в системе хранения.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly IFileStorageService _fileService;
    /// <summary>
    /// Инициализирует новый экземпляр контроллера для управления файлами.
    /// </summary>
    /// <param name="fileService"></param>
    public FilesController(IFileStorageService fileService)
    {
        _fileService = fileService;
    }
    /// <summary>
    /// Загружает файл в систему хранения и возвращает информацию о нем.
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file == null) return BadRequest("No file uploaded");
        if (!file.FileName.EndsWith(".txt")) return BadRequest("Only .txt files are supported");

        try
        {
            var fileInfo = await _fileService.StoreFileAsync(file);
            return Ok(fileInfo);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
    /// <summary>
    /// Получает файл по его идентификатору и возвращает его содержимое.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetFile(Guid id)
    {
        var fileInfo = await _fileService.GetFileInfoAsync(id);
        if (fileInfo == null) return NotFound();

        var content = await _fileService.GetFileContentAsync(id);

        string contentType = string.IsNullOrWhiteSpace(fileInfo.ContentType)
        ? "application/octet-stream"
        : fileInfo.ContentType;
        return File(content, contentType, fileInfo.FileName);
    }
    /// <summary>
    /// Получает информацию о файле по его идентификатору.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("info/{id}")]
    public async Task<IActionResult> GetFileInfo(Guid id)
    {
        var fileInfo = await _fileService.GetFileInfoAsync(id);
        if (fileInfo == null) return NotFound();

        return Ok(fileInfo);
    }
    /// <summary>
    /// Проверяет, существует ли файл с указанным хэшем в системе хранения.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAllFiles()
    {
        var files = await _fileService.GetAllFilesAsync();
        return Ok(files);
    }
}