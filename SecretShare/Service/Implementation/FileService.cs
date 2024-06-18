using Microsoft.EntityFrameworkCore;
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
        private readonly ApplicationDbContext _context;

        public FileService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<UploadedFile>> GetFileById(string id)
        {
            // Implementation to get the file by ID
            var file = await _context.UploadedFiles.FindAsync(id);
            if (file == null)
            {
                return Result<UploadedFile>.Fail("File not found");
            }

            return Result<UploadedFile>.Success(file);
        }
        public async Task<Result<UploadedText>> GetTextById(string id)
        {
            // Implementation to get the text by ID
            var file = await _context.UploadedTexts.FindAsync(id);
            if (file == null)
            {
                return Result<UploadedText>.Fail("File not found");
            }

            return Result<UploadedText>.Success(file);
        }
        public async Task<Result<List<UploadFileVm>>> GetAllUploadedFiles()
        {
            try
            {
                var files = await _context.UploadedFiles.ToListAsync();
                var fileViewModels = files.Select(file => new UploadFileVm
                {
                    Id = file.Id,
                    UserId = file.UserId,
                    AutoDelete = file.AutoDelete,
                    UploadDate = file.UploadDate,
                    HasbeenDowloaded= file.HasbeenDowloaded
                    
                }).ToList();
                return Result<List<UploadFileVm>>.Success(fileViewModels);
            }
            catch (Exception ex)
            {
                return Result<List<UploadFileVm>>.Fail($"Failed to retrieve files: {ex.Message}");
            }
        }

        public async Task<Result<List<UploadTextVm>>> GetAllUploadedTexts()
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
                    HasbeenDowloaded = text.HasbeenDowloaded
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
                var files = await _context.UploadedFiles.Where(file => file.UserId == userId).ToListAsync();
                var fileViewModels = files.Select(file => new UploadFileVm
                {
                    Id = file.Id,
                    UserId = file.UserId,
                    AutoDelete = file.AutoDelete,
                    UploadDate = file.UploadDate,
                    HasbeenDowloaded = file.HasbeenDowloaded
                }).ToList();
                return Result<List<UploadFileVm>>.Success(fileViewModels);
            }
            catch (Exception ex)
            {
                return Result<List<UploadFileVm>>.Fail($"Failed to retrieve files: {ex.Message}");
            }
        }

        public async Task<Result<List<UploadTextVm>>> GetTextsByUserId(string userId)
        {
            try
            {
                var texts = await _context.UploadedTexts.Where(text => text.UserId == userId).ToListAsync();
                var textViewModels = texts.Select(text => new UploadTextVm
                {
                    Id = text.Id,
                    UserId = text.UserId,
                    Content = text.Content,
                    AutoDelete = text.AutoDelete,
                    UploadDate = text.UploadDate,
                    HasbeenDowloaded = text.HasbeenDowloaded
                }).ToList();
                return Result<List<UploadTextVm>>.Success(textViewModels);
            }
            catch (Exception ex)
            {
                return Result<List<UploadTextVm>>.Fail($"Failed to retrieve texts: {ex.Message}");
            }
        }

    }
}
