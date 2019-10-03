using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace AzureSaturday19.Cache
{
	public interface ICache<T>
	{
		void Set(T value);
		T Get();
		void Clear();
	}

	[JsonObject(MemberSerialization.OptIn)]
	public abstract class Cache<T> : ICache<T>
	{
		protected ILogger log;
  
		[JsonProperty]
		public T Value { get; private set; } = default;

		public void Set(T value) => Value = value;

		public T Get() => Value;

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
