using System.Diagnostics;
using System.Net;
using System.Net.Http.Extensions.Compression.Core.Compressors;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Cors;
using Microsoft.AspNet.WebApi.Extensions.Compression.Server;
using Owin;
// ReSharper disable UnusedMember.Global

namespace VariableServer
{
    public class Startup
    {
        private static long _index;

        public void Configuration(IAppBuilder app)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            if (app.Properties.TryGetValue(typeof(HttpListener).FullName, out object httpListener)
                && httpListener is HttpListener listener)
            {
                // HttpListener should not return exceptions that occur
                // when sending the response to the client
                listener.IgnoreWriteExceptions = true;
            }

            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            config.EnsureInitialized();
            config.MessageHandlers.Add(new ServerCompressionHandler(new GZipCompressor(), new DeflateCompressor()));
            config.EnableCors(new EnableCorsAttribute("*", "*", "*"));
            app.Use((context, next) =>
            {
                var rqIndex = Interlocked.Increment(ref _index);
                var duration = new Stopwatch();
                duration.Start();
                Trace.TraceInformation($"Variable Request({rqIndex}) {context.Request.Method}  {context.Request.Path}");
                context.Environment.Add("PiControl", Program.Control);
                context.Environment.Add("PiConfig", Program.Config);
                var result = next.Invoke();
                duration.Stop();
                Trace.TraceInformation($"Variable Response({rqIndex}) {context.Response.StatusCode} {context.Response.ReasonPhrase} {duration.ElapsedMilliseconds}ms");
                return result;
            });
            app.UseWebApi(config);
        }
    }
}
