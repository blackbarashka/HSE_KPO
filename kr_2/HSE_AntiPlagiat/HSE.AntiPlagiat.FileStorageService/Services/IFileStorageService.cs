// Интерфейс для сервиса хранения файлов.
public interface IFileStorageService
{
    Task<FileInfo> StoreFileAsync(IFormFile file);
    Task<FileInfo> GetFileInfoAsync(Guid id);
    Task<byte[]> GetFileContentAsync(Guid id);
    Task<bool> FileExistsAsync(string hash);
    Task<IEnumerable<FileInfo>> GetAllFilesAsync();
}