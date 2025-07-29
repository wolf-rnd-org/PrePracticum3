using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FFmpeg.API.DTOs;
using FFmpeg.Core.Interfaces;
using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace FFmpeg.API.Endpoints
{
    public static class VideoEndpoints
    {
        private const int MaxUploadSize = 104_857_600; // 100 MB
        private const int MaxUploadSizeForGif = 52_428_800; // 50 MB

        public static void MapEndpoints(this WebApplication app)
        {
            app.MapPost("/api/video/watermark", AddWatermark)
                .DisableAntiforgery()
                .WithMetadata(new RequestSizeLimitAttribute(MaxUploadSize));

            app.MapPost("/api/video/cut", CutVideo)
               .DisableAntiforgery()
               .WithMetadata(new RequestSizeLimitAttribute(MaxUploadSize));

            app.MapPost("/api/video/volume", SetVolume)
                 .DisableAntiforgery()
                 .WithMetadata(new RequestSizeLimitAttribute(MaxUploadSize));

            app.MapPost("/api/video/reverse", ReverseVideo)
                .DisableAntiforgery()
                .WithMetadata(new RequestSizeLimitAttribute(MaxUploadSize));

            app.MapPost("/api/video/change-resolution", ChangeResolution)
                .DisableAntiforgery()
                .WithMetadata(new RequestSizeLimitAttribute(MaxUploadSize));

            app.MapPost("/api/video/timestamp", AddTimestamp)
                 .DisableAntiforgery()
                 .WithMetadata(new RequestSizeLimitAttribute(MaxUploadSize));

            app.MapPost("/api/video/createGif", AddCreateGif)
               .DisableAntiforgery()
               .WithMetadata(new RequestSizeLimitAttribute(MaxUploadSizeForGif));
        }

        private static async Task<IResult> SetVolume(HttpContext context, [FromForm] SetVolumeDto dto)
        {
            var fileService = context.RequestServices.GetRequiredService<IFileService>();
            var ffmpegService = context.RequestServices.GetRequiredService<IFFmpegServiceFactory>();
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

            if (dto.VideoFile == null || dto.Volume <= 0)
            {
                return Results.BadRequest("Valid video file and volume level are required.");
            }

            string inputFile = await fileService.SaveUploadedFileAsync(dto.VideoFile);
            string extension = Path.GetExtension(dto.VideoFile.FileName);
            string outputFile = await fileService.GenerateUniqueFileNameAsync(extension);
            var filesToCleanup = new List<string> { inputFile, outputFile };

            try
            {
                var command = ffmpegService.CreateSetVolumeCommand();
                var result = await command.ExecuteAsync(new SetVolumeModel
                {
                    InputFile = inputFile,
                    OutputFile = outputFile,
                    Volume = dto.Volume
                });

                if (!result.IsSuccess)
                {
                    logger.LogError("FFmpeg volume command failed: {ErrorMessage}, Command: {Command}",
                        result.ErrorMessage, result.CommandExecuted);
                    return Results.Problem("Failed to adjust volume: " + result.ErrorMessage, statusCode: 500);
                }

                byte[] outputBytes = await fileService.GetOutputFileAsync(outputFile);
                _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                return Results.File(outputBytes, "video/mp4", dto.VideoFile.FileName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing SetVolume");
                _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                return Results.Problem("An error occurred: " + ex.Message, statusCode: 500);
            }
        }

        private static async Task<IResult> ChangeResolution(
            HttpContext context,
            [FromForm] ChangeResolutionDto dto)
        {
            var fileService = context.RequestServices.GetRequiredService<IFileService>();
            var ffmpegService = context.RequestServices.GetRequiredService<IFFmpegServiceFactory>();
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>(); 
            try
            {
                if (dto.VideoFile == null)
                {
                    return Results.BadRequest("Video file is required");
                }
                string videoFileName = await fileService.SaveUploadedFileAsync(dto.VideoFile);
                string extension = Path.GetExtension(dto.VideoFile.FileName);
                string outputFileName = await fileService.GenerateUniqueFileNameAsync(extension);
                List<string> filesToCleanup = new List<string> { videoFileName, outputFileName };
                try
                {
                    var command = ffmpegService.CreateChangeResolutionCommand();
                    var result = await command.ExecuteAsync(new ChangeResolutionModel
                    {
                        InputFile = videoFileName,
                        Width = dto.Width,
                        Height = dto.Height,
                        OutputFile = outputFileName,
                        VideoCodec = "libx264"
                    });
                    if (!result.IsSuccess)
                    {
                        logger.LogError("FFmpeg command failed: {ErrorMessage}, Command: {CommandExecuted}",
                            result.ErrorMessage, result.CommandExecuted);
                        return Results.Problem("Failed to change resolution: " + result.ErrorMessage, statusCode: 500);
                    }
                    byte[] fileBytes = await fileService.GetOutputFileAsync(outputFileName);
                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                    return Results.File(fileBytes, "video/mp4", dto.VideoFile.FileName);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing change resolution request");
                    _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                    throw;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in ChangeResolution endpoint");
                return Results.Problem("An error occurred: " + ex.Message, statusCode: 500);
            }
        }
        private static async Task<IResult> AddWatermark(
            HttpContext context,
            [FromForm] WatermarkDto dto)
        {
            var fileService = context.RequestServices.GetRequiredService<IFileService>();
            var ffmpegService = context.RequestServices.GetRequiredService<IFFmpegServiceFactory>();
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

            try
            {
                if (dto.VideoFile == null || dto.WatermarkFile == null)
                {
                    return Results.BadRequest("Video file and watermark file are required");
                }

                string videoFileName = await fileService.SaveUploadedFileAsync(dto.VideoFile);
                string watermarkFileName = await fileService.SaveUploadedFileAsync(dto.WatermarkFile);
                string extension = Path.GetExtension(dto.VideoFile.FileName);
                string outputFileName = await fileService.GenerateUniqueFileNameAsync(extension);

                List<string> filesToCleanup = new List<string> { videoFileName, watermarkFileName, outputFileName };

                var command = ffmpegService.CreateWatermarkCommand();
                var result = await command.ExecuteAsync(new WatermarkModel
                {
                    InputFile = videoFileName,
                    WatermarkFile = watermarkFileName,
                    OutputFile = outputFileName,
                    XPosition = dto.XPosition,
                    YPosition = dto.YPosition,
                    IsVideo = true,
                    VideoCodec = "libx264"
                });

                if (!result.IsSuccess)
                {
                    logger.LogError("FFmpeg command failed: {ErrorMessage}, Command: {Command}",
                        result.ErrorMessage, result.CommandExecuted);
                    return Results.Problem("Failed to add watermark: " + result.ErrorMessage, statusCode: 500);
                }

                byte[] fileBytes = await fileService.GetOutputFileAsync(outputFileName);
                _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                return Results.File(fileBytes, "video/mp4", dto.VideoFile.FileName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in AddWatermark endpoint");
                return Results.Problem("An error occurred: " + ex.Message, statusCode: 500);
            }
        }

        private static async Task<IResult> CutVideo(HttpContext context, [FromForm] CutVideoDto dto)
        {
            var fileService = context.RequestServices.GetRequiredService<IFileService>();
            var ffmpegService = context.RequestServices.GetRequiredService<IFFmpegServiceFactory>();
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

            try
            {
                if (dto.VideoFile == null)
                    return Results.BadRequest("Video file is required");

                if (string.IsNullOrEmpty(dto.StartTime) || string.IsNullOrEmpty(dto.EndTime))
                    return Results.BadRequest("Start time and end time are required");

                string videoFileName = await fileService.SaveUploadedFileAsync(dto.VideoFile);
                string extension = Path.GetExtension(dto.VideoFile.FileName);
                string outputFileName = await fileService.GenerateUniqueFileNameAsync(extension);
                var filesToCleanup = new List<string> { videoFileName, outputFileName };

                var command = ffmpegService.CreateCutVideoCommand();
                var result = await command.ExecuteAsync(new CutVideoModel
                {
                    InputFile = videoFileName,
                    OutputFile = outputFileName,
                    StartTime = dto.StartTime,
                    EndTime = dto.EndTime
                });

                if (!result.IsSuccess)
                {
                    logger.LogError("FFmpeg command failed: {ErrorMessage}, Command: {Command}",
                        result.ErrorMessage, result.CommandExecuted);
                    return Results.Problem("Failed to cut video: " + result.ErrorMessage, statusCode: 500);
                }

                byte[] fileBytes = await fileService.GetOutputFileAsync(outputFileName);
                _ = fileService.CleanupTempFilesAsync(filesToCleanup);

                return Results.File(fileBytes, "video/mp4", dto.VideoFile.FileName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in CutVideo endpoint");
                return Results.Problem("An error occurred: " + ex.Message, statusCode: 500);
            }
        }

        private static async Task<IResult> ReverseVideo(HttpContext context, [FromForm] ReverseVideoDto dto)
        {
            var fileService = context.RequestServices.GetRequiredService<IFileService>();
            var ffmpegService = context.RequestServices.GetRequiredService<IFFmpegServiceFactory>();
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

            try
            {
                if (dto.VideoFile == null)
                {
                    return Results.BadRequest("Video file is required");
                }

                string videoFileName = await fileService.SaveUploadedFileAsync(dto.VideoFile);
                string extension = Path.GetExtension(dto.VideoFile.FileName);
                string outputFileName = await fileService.GenerateUniqueFileNameAsync(extension);
                List<string> filesToCleanup = new List<string> { videoFileName, outputFileName };

                var command = ffmpegService.CreateReverseVideoCommand();
                var result = await command.ExecuteAsync(new ReverseVideoModel
                {
                    InputFile = videoFileName,
                    OutputFile = outputFileName
                });

                if (!result.IsSuccess)
                {
                    logger.LogError("FFmpeg command failed: {ErrorMessage}, Command: {CommandExecuted}",
                        result.ErrorMessage, result.CommandExecuted);
                    return Results.Problem("Failed to reverse video: " + result.ErrorMessage, statusCode: 500);
                }

                byte[] fileBytes = await fileService.GetOutputFileAsync(outputFileName);
                _ = fileService.CleanupTempFilesAsync(filesToCleanup);

                return Results.File(fileBytes, "video/mp4", dto.VideoFile.FileName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in ReverseVideo endpoint");
                return Results.Problem("An error occurred: " + ex.Message, statusCode: 500);
            }
        }

        private static async Task<IResult> AddCreateGif(HttpContext context, [FromForm] CreateGifDto dto)
        {
            var fileService = context.RequestServices.GetRequiredService<IFileService>();
            var ffmpegService = context.RequestServices.GetRequiredService<IFFmpegServiceFactory>();
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

            try
            {
                if (dto.InputFile == null)
                {
                    return Results.BadRequest("Video file is required to create a GIF.");
                }

                string videoFileName = await fileService.SaveUploadedFileAsync(dto.InputFile);
                string outputFileName = await fileService.GenerateUniqueFileNameAsync(".gif");
                List<string> filesToCleanup = new() { videoFileName, outputFileName };

                var command = ffmpegService.CreateGifCommand();
                var result = await command.ExecuteAsync(new CreateGifModel
                {
                    InputFile = videoFileName,
                    OutputFile = outputFileName,
                    Fps = dto.Fps,
                    Width = dto.Width
                });

                if (!result.IsSuccess)
                {
                    logger.LogError("FFmpeg command failed: {ErrorMessage}, Command: {Command}",
                        result.ErrorMessage, result.CommandExecuted);
                    return Results.Problem("Failed to create GIF: " + result.ErrorMessage, statusCode: 500);
                }

                byte[] fileBytes = await fileService.GetOutputFileAsync(outputFileName);
                _ = fileService.CleanupTempFilesAsync(filesToCleanup);

                return Results.File(fileBytes, "image/gif", Path.GetFileName(outputFileName));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in AddCreateGif endpoint");
                return Results.Problem("An error occurred: " + ex.Message, statusCode: 500);
            }
        }

        private static async Task<IResult> AddTimestamp(HttpContext context, [FromForm] TimestampDto dto)
        {
            var fileService = context.RequestServices.GetRequiredService<IFileService>();
            var ffmpegService = context.RequestServices.GetRequiredService<IFFmpegServiceFactory>();
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

            try
            {
                if (dto.VideoFile == null)
                    return Results.BadRequest("Video file is required");

                string inputFile = await fileService.SaveUploadedFileAsync(dto.VideoFile);
                string extension = Path.GetExtension(dto.VideoFile.FileName);
                string outputFile = await fileService.GenerateUniqueFileNameAsync(extension);

                List<string> filesToCleanup = new List<string> { inputFile, outputFile };

                var command = ffmpegService.CreateTimestampCommand();
                var result = await command.ExecuteAsync(new TimestampModel
                {
                    InputFile = inputFile,
                    OutputFile = outputFile
                });

                if (!result.IsSuccess)
                {
                    logger.LogError("FFmpeg timestamp failed: {ErrorMessage}, Command: {Command}",
                        result.ErrorMessage, result.CommandExecuted);
                    return Results.Problem("Failed to add timestamp: " + result.ErrorMessage, statusCode: 500);
                }

                byte[] fileBytes = await fileService.GetOutputFileAsync(outputFile);
                _ = fileService.CleanupTempFilesAsync(filesToCleanup);

                return Results.File(fileBytes, "video/mp4", dto.VideoFile.FileName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in AddTimestamp endpoint");
                return Results.Problem("An error occurred: " + ex.Message, statusCode: 500);
            }
        }
    }
}
