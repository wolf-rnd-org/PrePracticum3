

using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Commands;
using FFmpeg.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ffmpeg.Command.Commands
{
    public class ExtractAudioCommand : BaseCommand, ICommand<ExtractAudioModel>
    {
        private readonly ICommandBuilder _commandBuilder;

        public ExtractAudioCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }

        public async Task<CommandResult> ExecuteAsync(ExtractAudioModel model)
        {
            if (string.IsNullOrWhiteSpace(model.InputFile) || string.IsNullOrWhiteSpace(model.OutputFile))
            {
                return new CommandResult
                {
                    IsSuccess = false,
                    ErrorMessage = "Input and output file names are required."
                };
            }

            _commandBuilder.SetInput(model.InputFile);
            _commandBuilder.AddOption("-q:a 0");
            _commandBuilder.AddOption("-map a");
            _commandBuilder.SetOutput(model.OutputFile);


            return await RunAsync();
        }
    }
}