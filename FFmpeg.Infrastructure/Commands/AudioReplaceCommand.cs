﻿using Ffmpeg.Command.Commands;
using FFmpeg.Core.Models;
using FFmpeg.Infrastructure.Commands;
using FFmpeg.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

public class AudioReplaceCommand : BaseCommand, ICommand<AudioReplaceModel>
{
    private readonly ICommandBuilder _commandBuilder;
    public AudioReplaceCommand(FFmpegExecutor executor, ICommandBuilder commandBuilder)
        : base(executor)
    {
        _commandBuilder = commandBuilder;
    }
    public async Task<CommandResult> ExecuteAsync(AudioReplaceModel model)
    {
        CommandBuilder = _commandBuilder
            .SetInput(model.VideoFile) // input.mp4
            .SetInput(model.NewAudioFile) // new_audio.mp3
            .AddOption("-c:v copy")
            .AddOption("-map 0:v:0")
            .AddOption("-map 1:a:0")
            .SetOutput(model.OutputFile);

        return await RunAsync();
    }
}
