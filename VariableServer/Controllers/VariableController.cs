using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
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
                version = Assembly.GetAssembly(GetType()).GetName().Version.ToString(),
                varlist = "GET ~/variables",
                readvar = "GET ~/variables/{varname}",
                readprop = "GET ~/variables/{varname}/{propname}"
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
            var readInfo = GetVarReadInfo(varname);

            if (readInfo.HttpResponse == null)
            {
                readInfo.HttpResponse = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(readInfo, JsonSerializerSettings), Encoding.UTF8, "application/json")
                };
            }
            var result = new ResponseMessageResult(readInfo.HttpResponse);
            return await Task.FromResult(result);
        }


        private VarReadInfo GetVarReadInfo(string varname)
        { 
            var config = Request.GetOwinContext().Get<PiConfiguration>("PiConfig");
            var control = Request.GetOwinContext().Get<PiControl>("PiControl");

            var readInfo = new VarReadInfo();

            var varInfo = config.Devices
                .SelectMany(d => d.Variables)
                .FirstOrDefault(v => string.Compare(v.Name, varname, StringComparison.InvariantCultureIgnoreCase) == 0);

            if (varInfo == null)
            {
                var error = new
                {
                    error = "Variable not found",
                    name = varname
                };
                readInfo.HttpResponse = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(error, JsonSerializerSettings), Encoding.UTF8, "application/json")
                };
            }
            else
            {
                var varData = control.ReadVariable(varInfo);
                if (varData == null)
                {
                    var error = new
                    {
                        error = "Could not read variable",
                        name = varname
                    };
                    readInfo.HttpResponse = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(error, JsonSerializerSettings), Encoding.UTF8, "application/json")
                    };
                }
                else
                {
                    readInfo = new VarReadInfo
                    {
                        Name = varInfo.Name,
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
                        Data = varData.Raw.Select(d => (int)d).ToArray(),
                        Value = varData.Value
                    };
                }
            }

            return readInfo;
        }

        [Route("variables/{varname}/{propname}")]
        [HttpGet]
        public async Task<IHttpActionResult> ReadVariableProperty(string varname, string propname)
        {
            var readInfo = GetVarReadInfo(varname);

            if (readInfo.HttpResponse == null)
            {
                var prop = readInfo.GetType().GetProperties()
                    .FirstOrDefault(p => string.Compare(p.Name, propname, StringComparison.InvariantCultureIgnoreCase) == 0);
                if (prop == null)
                {
                    var error = new
                    {
                        error = "Unknown roperty",
                        name = propname
                    };
                    readInfo.HttpResponse = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(error, JsonSerializerSettings), Encoding.UTF8, "application/json")
                    };
                }
                else
                {
                    var propval = prop.GetValue(readInfo);

                    readInfo.HttpResponse = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(propval, JsonSerializerSettings), Encoding.UTF8, "application/json")
                    };
                }
            }
            var result = new ResponseMessageResult(readInfo.HttpResponse);
            return await Task.FromResult(result);
        }

    }
}
