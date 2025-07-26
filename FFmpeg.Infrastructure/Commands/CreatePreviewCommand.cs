using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ffmpeg.Command.Commands;
using FFmpeg.Infrastructure.Services;

namespace FFmpeg.Infrastructure.Commands
{
    public class CreatePreviewCommand : BaseCommand, ICommand<CreatePreviewModel>
    {
        private readonly ICommandBuilder _commandBuilder;

        public CreatePreviewCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
            : base(executor)
        {
            _commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
        }

        public async Task<CommandResult> ExecuteAsync(CreatePreviewModel model)
        {
            CommandBuilder = _commandBuilder
                .SetInput(model.InputFile)         
                .AddOption($"-ss {model.SeekTime}") 
                .AddOption("-vframes 1")           
                .SetOutput(model.OutputFile);    

            return await RunAsync();
        }
    }
}
