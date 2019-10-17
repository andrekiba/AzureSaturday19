using System.Threading.Tasks;
using AzureSaturday19.Lights.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AzureSaturday19.Lights
{
    public interface ILight
    {
        void On();
        void Off();
        Task<LightState> Get();
        void Color(string hexColor);
        void End();
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class Light : ILight
    {
        readonly ILogger log;
        public Light(ILogger log)
        {
            this.log = log;
        }

        #region Properties

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public LightState State { get; private set; } = LightState.Off;
        [JsonProperty]
        public string HexColor { get; private set; } = "#efebd8";

        #endregion

        #region Methods

        public void On() => State = LightState.On;
        public void Off() => State = LightState.Off;
        public Task<LightState> Get() => Task.FromResult(State);
        public void Color(string hexColor) => HexColor = hexColor;
        public void End() => Entity.Current.DeleteState();

        #endregion

        [FunctionName(nameof(Light))]
        public static Task Run(
            [EntityTrigger] IDurableEntityContext ctx,
            ILogger log)
            => ctx.DispatchAsync<Light>(log);
    }
}
