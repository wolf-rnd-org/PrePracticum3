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
    public class CutVideoCommand : BaseCommand, ICommand<CutVideoModel>
    {
        private readonly ICommandBuilder _commandBuilder;

        public CutVideoCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }

        public async Task<CommandResult> ExecuteAsync(CutVideoModel model)
        {
            CommandBuilder = _commandBuilder
               .SetInput(model.InputFile)
               .AddOption($"-ss {model.StartTime}")   
               .AddOption($"-to {model.EndTime}")    
               .AddOption("-c copy")                   
               .SetOutput(model.OutputFile);

            return await RunAsync();
        }
    }
}
