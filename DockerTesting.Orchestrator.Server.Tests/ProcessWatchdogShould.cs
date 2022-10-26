using System.Threading;
using System.Threading.Tasks;
using DockerTesting.Orchestrator.Server.Processes;
using NSubstitute;
using Xunit;

namespace DockerTesting.Orchestrator.Server.Tests
{
    public class ProcessWatchdogShould
    {
        [Fact]
        public async Task Free_Resources_Spawned_By_Process()
        {
            var fakeProcesses = Substitute.For<SystemProcesses>();
            var fakeClientProcess = new FakeClientProcess();
            fakeProcesses.GetProcessById(fakeClientProcess.ProcessId).Returns(fakeClientProcess);

            var watchdog = new ProcessWatchdog(new ProcessRegistry(), fakeProcesses);
            var testProcessResource = Substitute.For<IResource>();

            watchdog.WatchResource(fakeClientProcess.ProcessId, testProcessResource);

            await fakeClientProcess.Exit();

            await testProcessResource.Received(1).FreeAsync();
        }

        private class FakeClientProcess : IProcess
        {
            public int ProcessId = 1234;
            private bool _timeToExit;
            private readonly TaskCompletionSource<bool> _tcs;

            public FakeClientProcess()
            {
                _tcs = new TaskCompletionSource<bool>();
            }

            public Task<bool> Exit()
            {
                _timeToExit = true;
                return _tcs.Task;
            }

            public void WaitForExit()
            {
                SpinWait.SpinUntil(() => _timeToExit);
                _tcs.SetResult(true);
            }
        }
    }

}