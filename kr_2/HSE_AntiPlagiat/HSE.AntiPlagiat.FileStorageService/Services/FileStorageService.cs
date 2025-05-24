using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
/// <summary>
/// Сервис для хранения и управления файлами.
/// </summary>
public class FileStorageService : IFileStorageService
{
    private readonly FileDbContext _dbContext;
    private readonly IWebHostEnvironment _environment;
    /// <summary>
    /// Инициализирует новый экземпляр сервиса для хранения файлов.
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="environment"></param>
    public FileStorageService(FileDbContext dbContext, IWebHostEnvironment environment)
    {
        _dbContext = dbContext;
        _environment = environment;
    }
    /// <summary>
    /// Сохраняет файл в хранилище и возвращает информацию о нем.
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public async Task<FileInfo> StoreFileAsync(IFormFile file)
    {
        var fileName = Guid.NewGuid().ToString();
        var storagePath = Path.Combine(_environment.ContentRootPath, "Storage", fileName);

        Directory.CreateDirectory(Path.GetDirectoryName(storagePath));

        using (var stream = new FileStream(storagePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var contentHash = await CalculateFileHashAsync(storagePath);

        var existingFile = await _dbContext.Files.FirstOrDefaultAsync(f => f.Hash == contentHash);
        if (existingFile != null)
        {
            System.IO.File.Delete(storagePath);
            return existingFile;
        }

        var fileInfo = new FileInfo
        {
            Id = Guid.NewGuid(),
            FileName = file.FileName,
            ContentType = string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream": file.ContentType,
            Size = file.Length,
            StoragePath = storagePath,
            UploadDate = DateTime.UtcNow,
            Hash = contentHash
        };


        _dbContext.Files.Add(fileInfo);
        await _dbContext.SaveChangesAsync();

        return fileInfo;
    }
    /// <summary>
    /// Возвращает информацию о файле по его идентификатору.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<FileInfo> GetFileInfoAsync(Guid id)
    {
        return await _dbContext.Files.FindAsync(id);
    }
    /// <summary>
    /// Возвращает содержимое файла по его идентификатору.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<byte[]> GetFileContentAsync(Guid id)
    {
        var fileInfo = await _dbContext.Files.FindAsync(id);
        if (fileInfo == null) return null;

        return await System.IO.File.ReadAllBytesAsync(fileInfo.StoragePath);
    }
    /// <summary>
    /// Проверяет, существует ли файл с указанным хешем в хранилище.
    /// </summary>
    /// <param name="hash"></param>
    /// <returns></returns>
    public async Task<bool> FileExistsAsync(string hash)
    {
        return await _dbContext.Files.AnyAsync(f => f.Hash == hash);
    }
    /// <summary>
    /// Возвращает список всех файлов в хранилище.
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<FileInfo>> GetAllFilesAsync()
    {
        return await _dbContext.Files.ToListAsync();
    }
    /// <summary>
    /// Вычисляет хеш MD5 для файла по указанному пути асинхронно.
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    private async Task<string> CalculateFileHashAsync(string filePath)
    {
        using var md5 = MD5.Create();
        using var stream = System.IO.File.OpenRead(filePath);
        var hash = await md5.ComputeHashAsync(stream);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }
}