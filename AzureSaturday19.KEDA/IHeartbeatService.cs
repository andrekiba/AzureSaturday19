using System.Threading;
using System.Threading.Tasks;

namespace AzureSaturday19.KEDA
{
	public interface IHeartbeatService
	{
		Task<string> Heartbeat(CancellationToken cancellationToken);
	}
}
