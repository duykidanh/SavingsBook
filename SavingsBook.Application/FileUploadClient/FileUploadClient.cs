using AutoMapper;
using FileUploadServiceGrpc;
using Google.Protobuf;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Http;
using SavingsBook.Application.Contracts.FileUploadClient;

using FileDetails = SavingsBook.Domain.Common.FileDetails;

namespace SavingsBook.Application.FileUploadClient;
public class FileUploadClient : IFileUploadClient
{
    private readonly FileService.FileServiceClient _fileServiceClient;
    private const string GrpcAddress = "https://localhost:9090";

    public FileUploadClient(IMapper mapper)
    {
        _fileServiceClient = new FileService.FileServiceClient(GrpcChannel.ForAddress(GrpcAddress));
    }

    public async Task<FileDetails> UploadImageAsync(IFormFile input)
    {
        try
        {
            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                await input.CopyToAsync(stream);
                bytes = stream.ToArray();
            }

            var request = new ImageUploadRequest() { Content = ByteString.CopyFrom(bytes), FileName = input.FileName };
            var response = await _fileServiceClient.UploadImageFileAsync(request);
            return new FileDetails()
            {
                Name = response.Name, Path = response.Path, Size = response.Size, Type = response.ContentType
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

    }


    public async Task<List<FileDetails>> UploadManyImages(IFormFileCollection input)
    {
        using var stream = _fileServiceClient.UploadManyImagesFiles();
        foreach (var file in input)
        {
            byte[] bytes;
            using (var fileStream = new MemoryStream())
            {
                await file.CopyToAsync(fileStream);
                var request = new ImageUploadRequest()
                {
                    FileName = file.FileName, Content = await ByteString.FromStreamAsync(fileStream)
                };
                await stream.RequestStream.WriteAsync(request);
            }
        }

        await stream.RequestStream.CompleteAsync();

        var response = await stream;

        return response.Files.Select(temp =>
            new FileDetails()
            {
                Name = temp.Name,
                Path = temp.Path,
                Size = temp.Size,
                Type = temp.ContentType
            }).ToList();
    }

    public async Task<FileDetails> UploadVideoChunk(FileChunkDto input)
    {
        try
        {
            var request = new FileChunk()
            {
                Content = ByteString.CopyFrom(input.Content), FileName = input.FileName,
                FileId = input.FileId, ChunkNumber = input.ChunkNumber, IsLastChunk = input.IsLastChunk
            };
            var response = await _fileServiceClient.UploadVideoChunkAsync(request);
            return new FileDetails()
            {
                Name = response.Name, Path = response.Path, Size = response.Size, Type = response.ContentType
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

    }

}
