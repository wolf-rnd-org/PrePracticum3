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
        public static void MapEndpoints(this WebApplication app)
        {
            app.MapPost("/api/video/replace-audio", ReplaceAudio)
                  .DisableAntiforgery()
                  .WithMetadata(new RequestSizeLimitAttribute(104857600));
        }


        private static async Task<IResult> ReplaceAudio(
            [FromForm] ReplaceAudioDto dto,
            HttpContext context)
        {
            var fileService = context.RequestServices.GetRequiredService<IFileService>();
            var command = context.RequestServices.GetRequiredService<AudioReplaceCommand>();
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
            string videoTempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + Path.GetExtension(dto.VideoFile.FileName));
            string audioTempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + Path.GetExtension(dto.AudioFile.FileName));
            string outputTempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".mp4");

            try
            {
                await using (var videoStream = new FileStream(videoTempFile, FileMode.Create))
                {
                    await dto.VideoFile.CopyToAsync(videoStream);
                }
                await using (var audioStream = new FileStream(audioTempFile, FileMode.Create))
                {
                    await dto.AudioFile.CopyToAsync(audioStream);
                }
                var request = new AudioReplaceModel
                {
                    VideoFile = videoTempFile,
                    NewAudioFile = audioTempFile,
                    OutputFile = outputTempFile
                };
                await command.ExecuteAsync(request);
                byte[] outputBytes = await File.ReadAllBytesAsync(outputTempFile);
                return Results.File(outputBytes, "video/mp4", "output_with_audio.mp4");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in ReplaceAudio endpoint");
                return Results.Problem("Failed to replace audio: " + ex.Message, statusCode: 500);
            }
            finally
            {
                if (File.Exists(videoTempFile)) File.Delete(videoTempFile);
                if (File.Exists(audioTempFile)) File.Delete(audioTempFile);
                if (File.Exists(outputTempFile)) File.Delete(outputTempFile);
            }
        }
    }
}