using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using SecretShare.Models.Domains;
using SecretShare.Models.Service.Abstract;
using System;
using System.IO;
using System.Threading.Tasks;

public class CloudinaryService : ICloudinaryService
{
    private readonly Cloudinary _cloudinary;
    private readonly ApplicationDbContext _context;

    public CloudinaryService(Cloudinary cloudinary, ApplicationDbContext context)
    {
        _cloudinary = cloudinary;
        _context = context;
    }

    public async Task<Result<UploadedFile>> UploadFile(IFormFile file, string userId, bool autoDelete,bool PublicFile) //Upload file to cloud
    {
        try
        {
            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream)
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams); // Upload the file to cloudinary

                var uploadedFile = new UploadedFile  // Create a new UploadedFile object
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userId,
                    FilePath = uploadResult.Url.ToString(),
                    AutoDelete = autoDelete,
                    UploadDate = DateTime.UtcNow,
                    PublicFile = PublicFile
                };

                _context.UploadedFiles.Add(uploadedFile); // Add the new UploadFile object to database
                await _context.SaveChangesAsync();

                return Result<UploadedFile>.Success(uploadedFile);
            }
        }
        catch (Exception ex)
        {
            return Result<UploadedFile>.Fail($"Failed to upload file: {ex.Message}");
        }
    }

    public async Task<Result<UploadedText>> UploadText(string text, string userId, bool autoDelete, bool PublicText)
    {
        try
        {
            var id = Guid.NewGuid().ToString();
            var tempFilePath = Path.Combine(Path.GetTempPath(), id + ".txt");

            await System.IO.File.WriteAllTextAsync(tempFilePath, text);

            using (var stream = new FileStream(tempFilePath, FileMode.Open))
            {
                var uploadParams = new RawUploadParams
                {
                    File = new FileDescription(tempFilePath, stream)
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams); // Upload the text to cloudinary
                System.IO.File.Delete(tempFilePath); // Clean up the temp file

                var uploadedText = new UploadedText  // Create a new UploadedText object
                {
                    Id = id,
                    UserId = userId,
                    Content = text,
                    FilePath = uploadResult.Url.ToString(),
                    AutoDelete = autoDelete,
                    UploadDate = DateTime.UtcNow,
                    PublicText = PublicText
                };

                _context.UploadedTexts.Add(uploadedText); // Add the new UploadedText object to database
                await _context.SaveChangesAsync();

                return Result<UploadedText>.Success(uploadedText);
            }
        }
        catch (Exception ex)
        {
            return Result<UploadedText>.Fail($"Failed to upload text: {ex.Message}");
        }
    }

    public async Task<Result<string>> DeleteFile(string filePath) // Remove the file using the url leading to it on cloudinary
    {
        try
        {
            var publicId = Path.GetFileNameWithoutExtension(filePath);
            var deletionParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deletionParams);
          
            if (result.Result == "ok")
            {
                return Result<string>.Success("Delete successfully");
            }

            return Result<string>.Fail("Failed to delete file");
        }
        catch (Exception ex)
        {
            return Result<string>.Fail($"Failed to delete file: {ex.Message}");
        }
    }

}
