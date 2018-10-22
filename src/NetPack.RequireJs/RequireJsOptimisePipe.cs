using Dazinator.AspNet.Extensions.FileProviders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.Extensions.Logging;
using NetPack.Extensions;
using NetPack.Pipeline;
using NetPack.Utils;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace NetPack.RequireJs
{
    public class RequireJsOptimisePipe : IPipe
    {
        private INetPackNodeServices _nodeServices;
        private readonly RequireJsOptimisationPipeOptions _options;
        private IEmbeddedResourceProvider _embeddedResourceProvider;
        private readonly ILogger<RequireJsOptimisePipe> _logger;
        private Lazy<StringAsTempFile> _script = null;

        public RequireJsOptimisePipe(INetPackNodeServices nodeServices, IEmbeddedResourceProvider embeddedResourceProvider, ILogger<RequireJsOptimisePipe> logger) : this(nodeServices, embeddedResourceProvider, logger, new RequireJsOptimisationPipeOptions())
        {

        }

        public RequireJsOptimisePipe(INetPackNodeServices nodeServices, IEmbeddedResourceProvider embeddedResourceProvider, ILogger<RequireJsOptimisePipe> logger, RequireJsOptimisationPipeOptions options)
        {
            _nodeServices = nodeServices;
            _embeddedResourceProvider = embeddedResourceProvider;
            _options = options;
            _script = new Lazy<StringAsTempFile>(() =>
            {
                Assembly assy = GetType().GetAssemblyFromType();
                Microsoft.Extensions.FileProviders.IFileInfo script = _embeddedResourceProvider.GetResourceFile(assy, "Embedded/netpack-requirejs-optimise.js");
                string scriptContent = script.ReadAllContent();
                return _nodeServices.CreateStringAsTempFile(scriptContent);
            });
        }


        public async Task ProcessAsync(PipeContext context, CancellationToken cancelationToken)
        {

            // var pipeContext = context.PipeContext;


            RequireJsOptimiseRequestDto optimiseRequest = new RequireJsOptimiseRequestDto();

            foreach (FileWithDirectory file in context.InputFiles)
            {
                string fileContent = file.FileInfo.ReadAllContent();
                //  var dir = file.Directory;
                // var name = file.FileInfo.Name;

                // expose all input files to the node process, so r.js can see them using fs.
                optimiseRequest.Files.Add(new NodeInMemoryFile()
                {
                    Contents = fileContent,
                    Path = file.UrlPath.ToString().TrimStart(new char[] { '/' })
                });
            }

            optimiseRequest.Options = _options;

            try
            {
                RequireJsOptimiseResult result = await _nodeServices.InvokeAsync<RequireJsOptimiseResult>(_script.Value.FileName, optimiseRequest);
                foreach (NodeInMemoryFile file in result.Files)
                {
                    string filePath = file.Path.Replace('\\', '/');
                    SubPathInfo subPathInfo = SubPathInfo.Parse(filePath);
                    PathString dir = subPathInfo.Directory.ToPathString();
                    context.AddOutput(dir, new StringFileInfo(file.Contents, subPathInfo.Name));
                }

                //if (!string.IsNullOrWhiteSpace(result.Error))
                //{
                //    throw new RequireJsOptimiseException(result.Error);
                //}
            }
            catch (Exception e)
            {

                //var jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(optimiseRequest, new JsonSerializerSettings()
                //{
                //    ContractResolver = new CamelCasePropertyNamesContractResolver()
                //});

                throw;
            }


        }



    }
}