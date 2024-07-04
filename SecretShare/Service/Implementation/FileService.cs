using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SecretShare.Models.Domains;
using SecretShare.Models.Service.Abstract;
using SecretShare.Models.ViewModel;
using SecretShare.Service.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecretShare.Models.Service.Implementation
{
    public class FileService : IFileService
    {
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(10);
        private readonly ApplicationDbContext _context;
        private readonly ICacheService _cacheService;

        public FileService(ApplicationDbContext context, ICacheService cache)
        {
            _context = context;
            _cacheService = cache;
        }

        public async Task<Result<UploadedFile>> GetFileById(string id, string userId)
        {
            var file = await _context.UploadedFiles.FindAsync(id);
            if (file == null)
            {
                return Result<UploadedFile>.Fail("File not found");
            }

            if (!file.PublicFile && file.UserId != userId) //Incase some random user got the id and try to access a private file
            {
                return Result<UploadedFile>.Fail("You do not have permission to access this file");
            }

            return Result<UploadedFile>.Success(file);
        }
        public async Task<Result<UploadedText>> GetTextById(string id, string userId)
        {
            // Implementation to get the text by ID
            var file = await _context.UploadedTexts.FindAsync(id);
            if (file == null)
            {
                return Result<UploadedText>.Fail("File not found");
            }
            if (!file.PublicText && file.UserId != userId) //Incase some random user got the id and try to access a private text
            {
                return Result<UploadedText>.Fail("You do not have permission to access this file");
            }


            return Result<UploadedText>.Success(file);
        }
        public async Task<Result<List<UploadFileVm>>> GetAllUploadedFiles() // Get all files. This is only for testing
        {
            try
            {
                string cacheKey = "GetAllUploadedFiles";

                // Sử dụng GetOrCreateAsync để lấy dữ liệu từ cache hoặc thêm mới vào cache
                var cachedFiles = await _cacheService.GetOrCreateAsync(cacheKey, async () =>
                {
                    var files = await _context.UploadedFiles.ToListAsync();
                    var fileViewModels = files.Select(file => new UploadFileVm
                    {
                        Id = file.Id,
                        UserId = file.UserId,
                        AutoDelete = file.AutoDelete,
                        UploadDate = file.UploadDate,
                        HasbeenDowloaded = file.HasbeenDowloaded,
                        PublicFile = file.PublicFile
                    }).ToList();
                    return fileViewModels;
                });

                return Result<List<UploadFileVm>>.Success(cachedFiles);
            }
            catch (Exception ex)
            {
                return Result<List<UploadFileVm>>.Fail($"Failed to retrieve files: {ex.Message}");
            }
        }



        public async Task<Result<List<UploadTextVm>>> GetAllUploadedTexts() // Get all texts. This is only for testing
        {
            try
            {
                var texts = await _context.UploadedTexts.ToListAsync();
                var textViewModels = texts.Select(text => new UploadTextVm
                {
                    Id=text.Id,
                    UserId = text.UserId,
                    Content = text.Content,
                    AutoDelete = text.AutoDelete,
                    UploadDate = text.UploadDate,
                    HasbeenDowloaded = text.HasbeenDowloaded,
                    PublicText = text.PublicText,
                }).ToList();
                return Result<List<UploadTextVm>>.Success(textViewModels);
            }
            catch (Exception ex)
            {
                return Result<List<UploadTextVm>>.Fail($"Failed to retrieve texts: {ex.Message}");
            }
        }
        public async Task<Result<List<UploadFileVm>>> GetFilesByUserId(string userId)
        {
            try
            {
                string cacheKey = $"GetFilesByUserId_{userId}";

                var cachedFiles = await _cacheService.GetOrCreateAsync(cacheKey, async () =>
                {
                    var files = await _context.UploadedFiles
                        .Where(file => file.UserId == userId && file.PublicFile)
                        .ToListAsync();

                    var fileViewModels = files.Select(file => new UploadFileVm
                    {
                        Id = file.Id,
                        UserId = file.UserId,
                        AutoDelete = file.AutoDelete,
                        UploadDate = file.UploadDate,
                        HasbeenDowloaded = file.HasbeenDowloaded,
                        PublicFile = file.PublicFile
                    }).ToList();

                    return fileViewModels;
                });

                return Result<List<UploadFileVm>>.Success(cachedFiles);
            }
            catch (Exception ex)
            {
                return Result<List<UploadFileVm>>.Fail($"Failed to retrieve files: {ex.Message}");
            }
        }

        public async Task<Result<List<UploadTextVm>>> GetTextsByUserId(string userId) // Get text from the current logged-in user
        {
            try
            {
                var texts = await _context.UploadedTexts.Where(text => text.UserId == userId).ToListAsync();
                var textViewModels = texts.Select(text => new UploadTextVm            // inject data from linq into viewmodel
                {
                    Id = text.Id,
                    UserId = text.UserId,
                    Content = text.Content,
                    AutoDelete = text.AutoDelete,
                    UploadDate = text.UploadDate,
                    HasbeenDowloaded = text.HasbeenDowloaded,
                    PublicText=text.PublicText
                }).ToList();
                return Result<List<UploadTextVm>>.Success(textViewModels);
            }
            catch (Exception ex)
            {
                return Result<List<UploadTextVm>>.Fail($"Failed to retrieve texts: {ex.Message}");
            }
        }

        public async Task<Result<List<UploadFileVm>>> GetPublicFilesByUserId(string userId)  // Get public files from a specific user
        {
            try
            {
                var publicFiles = await _context.UploadedFiles
                    .Where(file => file.UserId == userId && file.PublicFile)
                    .ToListAsync();

                var fileViewModels = publicFiles.Select(file => new UploadFileVm
                {
                    Id = file.Id,
                    UserId = file.UserId,
                    AutoDelete = file.AutoDelete,
                    UploadDate = file.UploadDate,
                    HasbeenDowloaded = file.HasbeenDowloaded,
                    PublicFile = file.PublicFile
                }).ToList();

                return Result<List<UploadFileVm>>.Success(fileViewModels);
            }
            catch (Exception ex)
            {
                return Result<List<UploadFileVm>>.Fail($"Failed to retrieve public files: {ex.Message}");
            }
        }
        public async Task<Result<List<UploadTextVm>>> GetPublicTextsByUserId(string userId)  // Get public texts from a specific user
        {
            try
            {
                var publicTexts = await _context.UploadedTexts
                    .Where(text => text.UserId == userId && text.PublicText)
                    .ToListAsync();

                var textViewModels = publicTexts.Select(text => new UploadTextVm
                {
                    Id = text.Id,
                    UserId = text.UserId,
                    Content = text.Content,
                    AutoDelete = text.AutoDelete,
                    UploadDate = text.UploadDate,
                    HasbeenDowloaded = text.HasbeenDowloaded,
                    PublicText = text.PublicText
                }).ToList();

                return Result<List<UploadTextVm>>.Success(textViewModels);
            }
            catch (Exception ex)
            {
                return Result<List<UploadTextVm>>.Fail($"Failed to retrieve public texts: {ex.Message}");
            }
        }
    }
}
