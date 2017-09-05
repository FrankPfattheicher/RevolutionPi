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
using Newtonsoft.Json.Serialization;
using VariableServer.Model;

namespace VariableServer.Controllers
{
    public class VariableController : ApiController
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };


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
                Content = new StringContent(JsonConvert.SerializeObject(root, JsonSerializerSettings), Encoding.UTF8, "application/json")
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
                Content = new StringContent(JsonConvert.SerializeObject(varlist, JsonSerializerSettings), Encoding.UTF8, "application/json")
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
                    Content = new StringContent(JsonConvert.SerializeObject(error, JsonSerializerSettings), Encoding.UTF8, "application/json")
                };
            }
            else
            {
                var deviceOffset = varInfo.Device.Offset;
                int byteLen;

                switch (varInfo.Length)
                {
                    case 1: byteLen = 0; break;        // Bit
                    case 8: byteLen = 1; break;
                    case 16: byteLen = 2; break;
                    case 32: byteLen = 4; break;
                    default:                            // strings, z.B. IP-Adresse
                        byteLen = -varInfo.Length / 8; 
                        break;      
                }

                byte[] data;

                if (byteLen > 0)
                {
                    data = control.Read(deviceOffset + varInfo.Address, byteLen);
                }
                else if (byteLen == 0)
                {
                    var address = (ushort) (deviceOffset + varInfo.Address);
                    data = new[]
                    {
                        (byte) (control.GetBitValue(address, varInfo.BitOffset) ? 1 : 0)
                    };
                }
                else  // iByteLen < 0
                {
                    data = control.Read(deviceOffset + varInfo.Address, -byteLen);
                }

                if (data == null)
                {
                    var error = new
                    {
                        error = "Could not read variable",
                        name = varname
                    };
                    httpResponse = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(error, JsonSerializerSettings), Encoding.UTF8, "application/json")
                    };
                }
                else
                {
                    var read = new VarReadInfo
                    {
                        Name = varname,
                        DefaultValue = varInfo.DefaultValue,
                        Comment = varInfo.Comment,
                        Type = varInfo.Type.ToString(),
                        LengthText = varInfo.LengthText,
                        Length = varInfo.Length,
                        Address = varInfo.Address,
                        Device = new VarDeviceInfo
                        { 
                            Name = varInfo.Device.Name,
                            Offset = varInfo.Device.Offset
                        },
                        Data = data.Select(d => (int)d).ToArray(),
                        Value = control.ConvertDataToValue(data)
                    };
                    httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(read, JsonSerializerSettings), Encoding.UTF8, "application/json")
                    };
                }
            }

            var result = new ResponseMessageResult(httpResponse);
            return await Task.FromResult(result);
        }
    }
}
