using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NamedPipesRpc.Core
{
    public static class PipeExtensions
    {

        public static async Task SendMessageAsync(this PipeStream clientStream, object message)
        {
            await clientStream.WriteLineAsync(JsonConvert.SerializeObject(message));
        }

        #region SEND MESSAGE routines

        private static async Task WriteLineAsync(this PipeStream pipe, string text)
        {
            using (var sw = AttachWriter(pipe))
            {
                sw.AutoFlush = true;
                await sw.WriteLineAsync(text);
                pipe.WaitForPipeDrain();
            }
        }

        private static StreamWriter AttachWriter(Stream pipe)
        {
            return new StreamWriter(pipe, Encoding.UTF8, 2048, true);
        }

        #endregion

        public static async Task<JObject> ReadMessageAsync(this PipeStream clientStream)
        {
            return JObject.Parse(await clientStream.ReadLineAsync());
        }

        #region READ MESSAGE routines

        private static async Task<string> ReadLineAsync(this Stream pipe)
        {
            using (var sr = AttachReader(pipe))
                return await sr.ReadLineAsync();
        }

        private static StreamReader AttachReader(Stream pipe)
        {
            return new StreamReader(pipe, Encoding.UTF8, false, 2048, true);
        }

        #endregion
    }
}