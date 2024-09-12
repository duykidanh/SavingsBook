using Microsoft.AspNetCore.Http;
using SavingsBook.Domain.Common;

namespace SavingsBook.Application.Contracts.FileUploadClient;

public interface IFileUploadClient
{
    Task<FileDetails> UploadImageAsync(IFormFile input);
    Task<List<FileDetails>> UploadManyImages(IFormFileCollection input);
    Task<FileDetails> UploadVideoChunk(FileChunkDto input);
}