using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleProxy.Commands
{
    public class OpenExplorer : IRequest
    {
        public OpenExplorer()
        {
        }

        public OpenExplorer(JsonElement options)
        {
            Path = options.GetProperty("path").ToString();
        }

        public string Path { get; set; }
    }

    public class OpenExplorerHandler : IRequestHandler<OpenExplorer>
    {
        private readonly ILogger<OpenExplorerHandler> _logger;

        public OpenExplorerHandler(ILogger<OpenExplorerHandler> logger)
        {
            _logger = logger;
        }

        public Task<Unit> Handle(OpenExplorer command, CancellationToken cancellationToken)
        {
            string process = @"explorer.exe";

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

            _logger.LogInformation("Explorer opened");

            return Unit.Task;
        }
    }
}
