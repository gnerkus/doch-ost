using System.Threading.Channels;

namespace Dochost.Server.Jobs
{
    public interface IJobQueue
    {
        ValueTask EnqueueAsync(UploadJob job, CancellationToken cancellationToken = default);

        ValueTask<UploadJob> DequeueAsync(
            CancellationToken cancellationToken);
    }
    
    public class JobQueue : IJobQueue
    {
        private readonly Channel<UploadJob> _queue;

        public JobQueue(int capacity)
        {
            var options = new BoundedChannelOptions(capacity)
            {
                FullMode = BoundedChannelFullMode.Wait
            };
            _queue = Channel.CreateBounded<UploadJob>(options);
        }

        public async ValueTask EnqueueAsync(UploadJob job,
            CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(job);

            await _queue.Writer.WriteAsync(job, ct);
        }

        public async ValueTask<UploadJob> DequeueAsync(
            CancellationToken ct = default)
        {
            var job = await _queue.Reader.ReadAsync(ct);
            return job;
        }
    }
}