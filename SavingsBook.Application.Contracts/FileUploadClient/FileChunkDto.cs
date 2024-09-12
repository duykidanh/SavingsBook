namespace SavingsBook.Application.Contracts.FileUploadClient;

public class FileChunkDto
{
    public byte[] Content { get; set; }
    public string FileId { get; set; }
    public string FileName { get; set; }
    public int ChunkNumber { get; set; }
    public bool IsLastChunk { get; set; }
}