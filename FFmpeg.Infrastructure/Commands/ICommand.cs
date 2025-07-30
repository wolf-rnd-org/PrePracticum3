
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFmpeg.Infrastructure.Commands;

namespace Ffmpeg.Command.Commands
{
    public interface ICommand<TModel>
    {
        Task<CommandResult> ExecuteAsync(TModel request);
    }
}
