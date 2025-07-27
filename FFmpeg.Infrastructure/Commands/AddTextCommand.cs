using FFmpeg.Core.Interfaces;
using FFmpeg.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Infrastructure.Commands
{
    public class AddTextCommand : ICommandRunner
    {
        public async Task<Result> RunAsync(AddTextRequest request)
        {
            try
            {
                var args = $"-i \"{request.InputPath}\" -vf " +
                           $"\"drawtext=text='{request.Text}':x={request.X}:y={request.Y}:" +
                           $"fontsize={request.FontSize}:fontcolor={request.FontColor}\" " +
                           $"\"{request.OutputPath}\"";

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "ffmpeg",
                        Arguments = args,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                string stderr = await process.StandardError.ReadToEndAsync();
                await process.WaitForExitAsync();

                if (process.ExitCode != 0)
                {
                    return Result.Fail($"FFmpeg error: {stderr}");
                }

                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail($"Exception: {ex.Message}");
            }
        }
    }
}
