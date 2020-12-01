using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleProxy.Commands
{
    public class Test : IRequest
    {
    }

    public class TestHandler : IRequestHandler<Test>
    {
        private readonly ILogger<TestHandler> _logger;

        public TestHandler(ILogger<TestHandler> logger)
        {
            _logger = logger;
        }

        public Task<Unit> Handle(Test command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Test successfull");

            return Unit.Task;
        }
    }
}
