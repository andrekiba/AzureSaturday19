using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace AzureSaturday19.Cache
{
	public interface ICache<T>
	{
		void Set(T value);
		Task<T> Get();
		void Clear();
	}

	[JsonObject(MemberSerialization.OptIn)]
	public abstract class Cache<T> : ICache<T>
	{
		protected ILogger log;
  
		[JsonProperty]
		public T Value { get; private set; }

		public void Set(T value) => Value = value;

		public Task<T> Get() => Task.FromResult(Value);

		public void Clear() => Entity.Current.DestructOnExit();
	}

	public class ByteCache : Cache<byte[]>
	{
		public ByteCache(ILogger log)
		{
			this.log = log;
		}
  
		[FunctionName(nameof(ByteCache))]
		public static Task Run([EntityTrigger] IDurableEntityContext ctx, ILogger log)
			=> ctx.DispatchAsync<ByteCache>(log);
	}
}
