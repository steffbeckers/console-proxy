using MediatR;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleProxy.Commands
{
    public class OpenExplorer : IRequest
    {
        public string Path { get; set; }
    }

    public class OpenExplorerHandler : IRequestHandler<OpenExplorer>
    {
        public Task<Unit> Handle(OpenExplorer command, CancellationToken cancellationToken)
        {
            string process = "explorer";

            if (!string.IsNullOrEmpty(command.Path))
            {
                process += $" {command.Path}";
            }

            new Process()
            {
                StartInfo = new ProcessStartInfo(process)
                {
                    UseShellExecute = true
                }
            }.Start();

            return Unit.Task;
        }
    }
}
