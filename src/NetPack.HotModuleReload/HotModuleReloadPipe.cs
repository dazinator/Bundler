using Dazinator.AspNet.Extensions.FileProviders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetPack.Extensions;
using NetPack.Node.Dto;
using NetPack.Pipeline;
using NetPack.Utils;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace NetPack.HotModuleReload
{
    public class HotModuleReloadPipe : BasePipe
    {
        private INetPackNodeServices _nodeServices;
        private readonly IOptions<HotModuleReloadOptions> _options;
        private IEmbeddedResourceProvider _embeddedResourceProvider;
        private readonly ILogger<HotModuleReloadPipe> _logger;
        private Lazy<StringAsTempFile> _script = null;



        public HotModuleReloadPipe(INetPackNodeServices nodeServices, IEmbeddedResourceProvider embeddedResourceProvider, ILogger<HotModuleReloadPipe> logger, IOptions<HotModuleReloadOptions> options, string name = "RequireJs Optimise"):base(name)
        {
            _nodeServices = nodeServices;
            _embeddedResourceProvider = embeddedResourceProvider;
            _options = options;
            _script = new Lazy<StringAsTempFile>(() =>
            {
                Assembly assy = GetType().GetAssemblyFromType();
                Microsoft.Extensions.FileProviders.IFileInfo script = _embeddedResourceProvider.GetResourceFile(assy, "Embedded/netpack-madge-entry.js");
                string scriptContent = script.ReadAllContent();
                return _nodeServices.CreateStringAsTempFile(scriptContent);
            });
        }


        public override async Task ProcessAsync(PipeState context, CancellationToken cancelationToken)
        {

            // var pipeContext = context.PipeContext;


            //RequireJsOptimiseRequestDto optimiseRequest = new RequireJsOptimiseRequestDto();

            //var inputFiles = context.GetInputFiles();
            //foreach (FileWithDirectory file in inputFiles)
            //{
            //    string fileContent = file.FileInfo.ReadAllContent();
            //    //  var dir = file.Directory;
            //    // var name = file.FileInfo.Name;

            //    // expose all input files to the node process, so r.js can see them using fs.
            //    optimiseRequest.Files.Add(new NodeInMemoryFile()
            //    {
            //        Contents = fileContent,
            //        Path = file.UrlPath.ToString().TrimStart(new char[] { '/' })
            //    });
            //}

            //optimiseRequest.Options = _options;

           
            //    cancelationToken.ThrowIfCancellationRequested();
            //    RequireJsOptimiseResult result = await _nodeServices.InvokeAsync<RequireJsOptimiseResult>(_script.Value.FileName, optimiseRequest);
            //    foreach (NodeInMemoryFile file in result.Files)
            //    {
            //        string filePath = file.Path.Replace('\\', '/');
            //        SubPathInfo subPathInfo = SubPathInfo.Parse(filePath);
            //        PathString dir = subPathInfo.Directory.ToPathString();
            //        context.AddOutput(dir, new StringFileInfo(file.Contents, subPathInfo.Name));
            //    }

            //    //if (!string.IsNullOrWhiteSpace(result.Error))
            //    //{
            //    //    throw new RequireJsOptimiseException(result.Error);
            //    //}
           


        }



    }
}