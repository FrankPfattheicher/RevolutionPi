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
                var iDeviceOffset = (ushort)varInfo.Device.Offset;
                int iByteLen;

                switch (varInfo.Length)
                {
                    case 1: iByteLen = 0; break;        // Bit
                    case 8: iByteLen = 1; break;
                    case 16: iByteLen = 2; break;
                    case 32: iByteLen = 3; break;
                    default:                            // strings, z.B. IP-Adresse
                        iByteLen = -varInfo.Length / 8; 
                        break;      
                }

                byte[] data;

                if (iByteLen > 0)
                {
                    data = control.Read(iDeviceOffset + varInfo.Address, iByteLen);
                }
                else if (iByteLen == 0)
                {
                    var address = (ushort) (iDeviceOffset + varInfo.Address);
                    data = new[]
                    {
                        (byte) (control.GetBitValue(address, varInfo.BitOffset) ? 1 : 0)
                    };
                }
                else  // iByteLen < 0
                {
                    data = control.Read(iDeviceOffset + varInfo.Address, -iByteLen);
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
                        Content = new StringContent(JsonConvert.SerializeObject(error, Formatting.Indented), Encoding.UTF8, "application/json")
                    };
                }
                else
                {
                    var read = new
                    {
                        name = varname,
                        default_value = varInfo.DefaultValue,
                        comment = varInfo.Comment,
                        type = varInfo.Type.ToString(),
                        lengthText = varInfo.LengthText,
                        length = varInfo.Length,
                        device_offset = iDeviceOffset,
                        var_offset = varInfo.Address,
                        var_dev_name = varInfo.Device.Name,
                        var_dev_offset = varInfo.Device.Offset,
                        data,
                        value = control.ConvertDataToValue(data, iByteLen)
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
