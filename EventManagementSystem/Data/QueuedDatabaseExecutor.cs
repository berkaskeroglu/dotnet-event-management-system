using System;
using System.Threading;
using System.Threading.Tasks;


namespace EventManagementSystem.Data
{
    public class QueuedDatabaseExecutor
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public async Task ExecuteAsync(Func<Task> databaseAction)
        {
            await _semaphore.WaitAsync();

            try
            {
                await databaseAction();
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
