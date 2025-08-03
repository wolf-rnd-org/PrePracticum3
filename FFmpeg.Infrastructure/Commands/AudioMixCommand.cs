using Ffmpeg.Command.Commands;
using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Infrastructure.Commands
{
    public class AudioMixCommand : BaseCommand, ICommand<AudioMixModel>
    {
        private readonly ICommandBuilder _commandBuilder;

        public AudioMixCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }

        public async Task<CommandResult> ExecuteAsync(AudioMixModel model)
        {
            // בדיקת תקינות קלט
            if (string.IsNullOrWhiteSpace(model.InputFile1) || string.IsNullOrWhiteSpace(model.InputFile2) || string.IsNullOrWhiteSpace(model.OutputFile))
            {
                return new CommandResult
                {
                    IsSuccess = false,
                    ErrorMessage = "Both input files and output file are required."
                };
            }
            CommandBuilder = _commandBuilder
                .SetInput(model.InputFile1)
                .SetInput(model.InputFile2)
                .AddFilterComplex("amix=inputs=2:duration=longest")
                .AddOption("-c:a libmp3lame")
                .SetOutput(model.OutputFile);

            return await RunAsync();
        }
    }
}
