using Ffmpeg.Command;
using Ffmpeg.Command.Commands;
using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Commands;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg.Infrastructure.Services
{
    public interface IFFmpegServiceFactory
    {
        ICommand<WatermarkModel> CreateWatermarkCommand();
        ICommand<ReverseVideoModel> CreateReverseVideoCommand();
        ICommand<ReplaceGreenScreenModal> CreateReplaceGreenScreenCommand();
        ICommand<TimestampModel> CreateTimestampCommand();
        ICommand<CreateGifModel> CreateGifCommand();
        ICommand<ConvertAudioModel> CreateConvertAudioCommand();
        ICommand<CutVideoModel> CreateCutVideoCommand();
        ICommand<RotationModel> CreateRotationCommand();
        ICommand<SetVolumeModel> CreateSetVolumeCommand();
    }

    public class FFmpegServiceFactory : IFFmpegServiceFactory
    {
        private readonly FFmpegExecutor _executor;
        private readonly ICommandBuilder _commandBuilder;

        public FFmpegServiceFactory(IConfiguration configuration, ILogger logger = null)
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string ffmpegPath = Path.Combine(baseDirectory, "external", "ffmpeg.exe");
            bool logOutput = bool.TryParse(configuration["FFmpeg:LogOutput"], out bool log) && log;
            _executor = new FFmpegExecutor(ffmpegPath, logOutput, logger);
            _commandBuilder = new CommandBuilder(configuration);
        }

        public ICommand<WatermarkModel> CreateWatermarkCommand()
        {
            return new WatermarkCommand(_executor, _commandBuilder);
        }
        public ICommand<ReverseVideoModel> CreateReverseVideoCommand()
        {
            return new ReverseVideoCommand(_executor, _commandBuilder);
        }
        public ICommand<ReplaceGreenScreenModal> CreateReplaceGreenScreenCommand()
        {
            return new ReplaceGreenScreenCommand(_executor, _commandBuilder);
        }

        public ICommand<ConvertAudioModel> CreateConvertAudioCommand()
        {
            return new ConvertAudioCommand(_executor, _commandBuilder);
        }
        public ICommand<CutVideoModel> CreateCutVideoCommand()
        {
            return new CutVideoCommand(_executor, _commandBuilder);
        }

        public ICommand<RotationModel> CreateRotationCommand()
        {
            return new RotationCommand(_executor, _commandBuilder);
        }

        public ICommand<TimestampModel> CreateTimestampCommand()
        {
            return new TimestampCommand(_executor, _commandBuilder);
        }

        public ICommand<SetVolumeModel> CreateSetVolumeCommand()
        {
            return new SetVolumeCommand(_executor, _commandBuilder);
        }

        public ICommand<CreateGifModel> CreateGifCommand()
        {
            return new CreateGifCommand(_executor, _commandBuilder);
        }
    }
}
