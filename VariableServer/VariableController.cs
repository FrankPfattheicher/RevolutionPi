using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using IctBaden.RevolutionPi;
using IctBaden.RevolutionPi.Configuration;
using Newtonsoft.Json;

namespace VariableServer
{
    public class VariableController : ApiController
    {
        [Route("")]
        [HttpGet]
        public async Task<IHttpActionResult> Root()
        {
            var root = new
            {
                service = "RevolutionPi Variable Server",
                varlist = "GET ~/variables",
                readvar = "GET ~/variables/{varname}"
            };
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(root, Formatting.Indented), Encoding.UTF8, "application/json")
            };

            var result = new ResponseMessageResult(httpResponse);
            return await Task.FromResult(result);
        }

        [Route("variables")]
        [HttpGet]
        public async Task<IHttpActionResult> GetVariableNames()
        {
            var config = Request.GetOwinContext().Get<PiConfiguration>("PiConfig");
            var varlist = config.Devices
                .SelectMany(d => d.Variables)
                .Select(v => new
                {
                    name = v.Name,
                    type = v.Type.ToString(),
                    length = v.LengthText,
                });

            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(varlist, Formatting.Indented), Encoding.UTF8, "application/json")
            };

            var result = new ResponseMessageResult(httpResponse);
            return await Task.FromResult(result);
        }

        [Route("variables/{varname}")]
        [HttpGet]
        public async Task<IHttpActionResult> ReadVariable(string varname)
        {
            var config = Request.GetOwinContext().Get<PiConfiguration>("PiConfig");
            var control = Request.GetOwinContext().Get<PiControl>("PiControl");

            HttpResponseMessage httpResponse;
            var varInfo = config.Devices
                .SelectMany(d => d.Variables)
                .FirstOrDefault(v => v.Name == varname);

            if (varInfo == null)
            {
                var error = new
                {
                    error = "Variable not found",
                    name = varname
                };
                httpResponse = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(error, Formatting.Indented), Encoding.UTF8, "application/json")
                };
            }
            else
            {
                var data = control.Read(varInfo.Address, varInfo.Length);
                if (data == null)
                {
                    var error = new
                    {
                        error = "Could not read variable",
                        name = varname
                    };
                    httpResponse = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(error, Formatting.Indented), Encoding.UTF8, "application/json")
                    };
                }
                else
                {
                    var read = new
                    {
                        name = varname,
                        type = varInfo.Type.ToString(),
                        length = varInfo.LengthText,
                        value = control.ConvertDataToValue(data, varInfo.Length)
                    };
                    httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(read, Formatting.Indented), Encoding.UTF8, "application/json")
                    };
                }
            }

            var result = new ResponseMessageResult(httpResponse);
            return await Task.FromResult(result);
        }
    }
}
