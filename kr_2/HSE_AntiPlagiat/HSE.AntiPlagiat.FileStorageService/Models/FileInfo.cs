/// <summary>
/// Модель информации о файле.
/// </summary>
public class FileInfo
{
    public Guid Id { get; set; }
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public long Size { get; set; }
    public string StoragePath { get; set; }
    public DateTime UploadDate { get; set; }
    public string Hash { get; set; }
}