﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using System;

namespace NetPack.BrowserReload
{
    internal class BrowserReloadOptionsConfigureOptions : IPostConfigureOptions<StaticFileOptions>
    {
        private readonly IHostingEnvironment _environment;

        public BrowserReloadOptionsConfigureOptions(IHostingEnvironment environment)
        {
            _environment = environment;
        }

        public void PostConfigure(string name, StaticFileOptions options)
        {

            // Basic initialization in case the options weren't initialized by any other component
            options.ContentTypeProvider = options.ContentTypeProvider ?? new FileExtensionContentTypeProvider();

            if (options.FileProvider == null && _environment.WebRootFileProvider == null)
            {
                throw new InvalidOperationException("Missing FileProvider.");
            }

            options.FileProvider = options.FileProvider ?? _environment.WebRootFileProvider;


            // Add our provider
            var filesProvider = new ManifestEmbeddedFileProvider(GetType().Assembly);
            options.FileProvider = new CompositeFileProvider(options.FileProvider, filesProvider);
        }
    }
}
