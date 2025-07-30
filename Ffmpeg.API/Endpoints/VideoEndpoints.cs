using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FFmpeg.API.DTOs;
using FFmpeg.Core.Interfaces;
using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Services;

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

            app.MapPost("/api/video/convert", ConvertAudio)
                .DisableAntiforgery()
                .WithName("ConvertAudio")
                .Accepts<ConvertAudioDto>("multipart/form-data")
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

            app.MapPost("/api/video/timestamp", AddTimestamp)
                .DisableAntiforgery()
                .WithMetadata(new RequestSizeLimitAttribute(MaxUploadSize));

            app.MapPost("/api/video/createGif", AddCreateGif)
                .DisableAntiforgery()
                .WithMetadata(new RequestSizeLimitAttribute(MaxUploadSizeForGif));

            app.MapPost("/api/video/rotation", RotateVideo)
                .DisableAntiforgery()
                .WithMetadata(new RequestSizeLimitAttribute(MaxUploadSize));

            app.MapPost("/api/video/merge", MergeVideos)
                .DisableAntiforgery()
                .WithMetadata(new RequestSizeLimitAttribute(MaxUploadSize));
        }

        private static async Task<IResult> SetVolume(HttpContext context, [FromForm] SetVolumeDto dto)
        {
            var fileService = context.RequestServices.GetRequiredService<IFileService>();
            var ffmpegService = context.RequestServices.GetRequiredService<IFFmpegServiceFactory>();
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

            if (dto.VideoFile == null || dto.Volume <= 0)
                return Results.BadRequest("Valid video file and volume level are required.");

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

        private static async Task<IResult> AddWatermark(HttpContext context, [FromForm] WatermarkDto dto)
        {
            var fileService = context.RequestServices.GetRequiredService<IFileService>();
            var ffmpegService = context.RequestServices.GetRequiredService<IFFmpegServiceFactory>();
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

            if (dto.VideoFile == null || dto.WatermarkFile == null)
                return Results.BadRequest("Video file and watermark file are required");

            string videoFileName = await fileService.SaveUploadedFileAsync(dto.VideoFile);
            string watermarkFileName = await fileService.SaveUploadedFileAsync(dto.WatermarkFile);
            string extension = Path.GetExtension(dto.VideoFile.FileName);
            string outputFileName = await fileService.GenerateUniqueFileNameAsync(extension);
            List<string> filesToCleanup = new() { videoFileName, watermarkFileName, outputFileName };

            try
            {
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
                _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                return Results.Problem("An error occurred: " + ex.Message, statusCode: 500);
            }
        }

        private static async Task<IResult> CutVideo(HttpContext context, [FromForm] CutVideoDto dto)
        {
            var fileService = context.RequestServices.GetRequiredService<IFileService>();
            var ffmpegService = context.RequestServices.GetRequiredService<IFFmpegServiceFactory>();
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

            if (dto.VideoFile == null || string.IsNullOrEmpty(dto.StartTime) || string.IsNullOrEmpty(dto.EndTime))
                return Results.BadRequest("Video file, start time, and end time are required");

            string videoFileName = await fileService.SaveUploadedFileAsync(dto.VideoFile);
            string extension = Path.GetExtension(dto.VideoFile.FileName);
            string outputFileName = await fileService.GenerateUniqueFileNameAsync(extension);
            List<string> filesToCleanup = new() { videoFileName, outputFileName };

            try
            {
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

            if (dto.VideoFile == null)
                return Results.BadRequest("Video file is required");

            string videoFileName = await fileService.SaveUploadedFileAsync(dto.VideoFile);
            string extension = Path.GetExtension(dto.VideoFile.FileName);
            string outputFileName = await fileService.GenerateUniqueFileNameAsync(extension);
            List<string> filesToCleanup = new() { videoFileName, outputFileName };

            try
            {
                var command = ffmpegService.CreateReverseVideoCommand();
                var result = await command.ExecuteAsync(new ReverseVideoModel
                {
                    InputFile = videoFileName,
                    OutputFile = outputFileName
                });

                if (!result.IsSuccess)
                {
                    logger.LogError("FFmpeg command failed: {ErrorMessage}, CommandExecuted: {CommandExecuted}",
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

        private static async Task<IResult> MergeVideos(HttpContext context, [FromForm] MergeVideosDto dto)
        {
            var fileService = context.RequestServices.GetRequiredService<IFileService>();
            var ffmpegService = context.RequestServices.GetRequiredService<IFFmpegServiceFactory>();
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

            if (dto.VideoFile1 == null || dto.VideoFile2 == null)
                return Results.BadRequest("Both video files are required");

            string file1 = await fileService.SaveUploadedFileAsync(dto.VideoFile1);
            string file2 = await fileService.SaveUploadedFileAsync(dto.VideoFile2);
            string extension = Path.GetExtension(dto.VideoFile1.FileName);
            string output = await fileService.GenerateUniqueFileNameAsync(extension);

            List<string> filesToCleanup = new() { file1, file2, output };

            try
            {
                var command = ffmpegService.CreateMergeVideosCommand();
                var result = await command.ExecuteAsync(new MergeVideosModel
                {
                    InputFile1 = file1,
                    InputFile2 = file2,
                    OutputFile = output,
                    Mode = dto.Mode?.ToLower() == "vertical" ? "vertical" : "horizontal"
                });

                if (!result.IsSuccess)
                {
                    logger.LogError("FFmpeg merge failed: {Error}", result.ErrorMessage);
                    return Results.Problem("Merge failed: " + result.ErrorMessage, statusCode: 500);
                }

                byte[] fileBytes = await fileService.GetOutputFileAsync(output);
                _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                return Results.File(fileBytes, "video/mp4", "merged_" + dto.VideoFile1.FileName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error merging videos");
                _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                return Results.Problem("An error occurred: " + ex.Message, statusCode: 500);
            }
        }

        private static async Task<IResult> AddCreateGif(HttpContext context, [FromForm] CreateGifDto dto)
        {
            var fileService = context.RequestServices.GetRequiredService<IFileService>();
            var ffmpegService = context.RequestServices.GetRequiredService<IFFmpegServiceFactory>();
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

            if (dto.InputFile == null)
                return Results.BadRequest("Video file is required to create a GIF.");

            string videoFileName = await fileService.SaveUploadedFileAsync(dto.InputFile);
            string outputFileName = await fileService.GenerateUniqueFileNameAsync(".gif");
            List<string> filesToCleanup = new() { videoFileName, outputFileName };

            try
            {
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

        private static async Task<IResult> ConvertAudio(HttpContext context, [FromForm] ConvertAudioDto dto)
        {
            var fileService = context.RequestServices.GetRequiredService<IFileService>();
            var ffmpegService = context.RequestServices.GetRequiredService<IFFmpegServiceFactory>();
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

            if (dto.AudioFile == null || string.IsNullOrEmpty(dto.OutputFileName))
                return Results.BadRequest("Audio file and output name are required");

            string inputFileName = await fileService.SaveUploadedFileAsync(dto.AudioFile);
            string extension = Path.GetExtension(dto.OutputFileName);
            if (string.IsNullOrEmpty(extension))
                return Results.BadRequest("Output file name must include extension (e.g., .wav)");

            string outputFileName = dto.OutputFileName;
            List<string> filesToCleanup = new() { inputFileName, outputFileName };

            try
            {
                var command = ffmpegService.CreateConvertAudioCommand();
                var result = await command.ExecuteAsync(new ConvertAudioModel
                {
                    InputFile = inputFileName,
                    OutputFile = outputFileName
                });

                if (!result.IsSuccess)
                {
                    logger.LogError("Audio conversion failed: {Error}", result.ErrorMessage);
                    return Results.Problem("Audio conversion failed: " + result.ErrorMessage);
                }

                byte[] output = await fileService.GetOutputFileAsync(outputFileName);
                _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                return Results.File(output, "audio/wav", outputFileName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error converting audio");
                _ = fileService.CleanupTempFilesAsync(filesToCleanup);
                return Results.Problem("Unexpected error: " + ex.Message);
            }
        }

        private static async Task<IResult> AddTimestamp(HttpContext context, [FromForm] TimestampDto dto)
        {
            var fileService = context.RequestServices.GetRequiredService<IFileService>();
            var ffmpegService = context.RequestServices.GetRequiredService<IFFmpegServiceFactory>();
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

            if (dto.VideoFile == null)
                return Results.BadRequest("Video file is required");

            string inputFile = await fileService.SaveUploadedFileAsync(dto.VideoFile);
            string extension = Path.GetExtension(dto.VideoFile.FileName);
            string outputFile = await fileService.GenerateUniqueFileNameAsync(extension);
            List<string> filesToCleanup = new() { inputFile, outputFile };

            try
            {
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

        private static async Task<IResult> RotateVideo(HttpContext context, [FromForm] RotationDto dto)
        {
            var fileService = context.RequestServices.GetRequiredService<IFileService>();
            var ffmpegService = context.RequestServices.GetRequiredService<IFFmpegServiceFactory>();
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

            if (dto.InputFile == null)
                return Results.BadRequest("Video file is required");

            string inputFile = await fileService.SaveUploadedFileAsync(dto.InputFile);
            string outputFile = await fileService.GenerateUniqueFileNameAsync(Path.GetExtension(dto.InputFile.FileName));

            var filesToCleanup = new List<string> { inputFile, outputFile };

            try
            {
                var command = ffmpegService.CreateRotationCommand();
                var result = await command.ExecuteAsync(new RotationModel
                {
                    InputFile = inputFile,
                    OutputFile = outputFile,
                    Angle = dto.Angle
                });

                if (!result.IsSuccess)
                {
                    logger.LogError("FFmpeg rotation failed: {ErrorMessage}, Command: {Command}", result.ErrorMessage, result.CommandExecuted);
                    return Results.Problem("Failed to rotate video: " + result.ErrorMessage, statusCode: 500);
                }

                var fileBytes = await fileService.GetOutputFileAsync(outputFile);
                _ = fileService.CleanupTempFilesAsync(filesToCleanup);

                return Results.File(fileBytes, "video/mp4", dto.InputFile.FileName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in RotateVideo endpoint");
                return Results.Problem("An error occurred: " + ex.Message, statusCode: 500);
            }
        }
    }
}
