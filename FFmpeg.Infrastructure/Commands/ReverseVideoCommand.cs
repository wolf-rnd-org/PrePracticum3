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
    public class ReverseVideoCommand:BaseCommand,ICommand<ReverseVideoModel>
    {
        private readonly ICommandBuilder _commandBuilder;

        public ReverseVideoCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }
        public async Task<CommandResult> ExecuteAsync(ReverseVideoModel model)
        {
            Console.WriteLine(model.InputFile+"+======================");
            CommandBuilder = _commandBuilder
                .SetInput(model.InputFile)
                .AddOption($"-vf reverse")
                .SetOutput(model.OutputFile);        
            Console.WriteLine(model.OutputFile+"98765555555555555555555555555555");

            return await RunAsync();
        }
    }
}
