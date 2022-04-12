using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;

namespace NetPack.Rollup
{
    public class BaseRollupOutputOptions
    {
        public BaseRollupOutputOptions()
        {
            Format = RollupOutputFormat.System;
        }

        /// <summary>
        /// The format of the generated bundle.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public RollupOutputFormat Format { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public SourceMapType? Sourcemap { get; set; }

        /// <summary>
        /// The variable name, representing your iife/umd bundle, by which other scripts on the same page can access it.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Configure Object of id: name pairs, used for umd/iife bundles.
        /// Used to tell Rollup which module ids are mapped to global variables.
        /// </summary>
        public void ConfigureGlobals(Action<dynamic> globals)
        {
            if (Globals == null)
            {
                Globals = new JObject();
            }
            globals?.Invoke(Globals);
        }

        /// <summary>
        /// Object of id: name pairs, used for umd/iife bundles.
        /// Used to tell Rollup which module ids are mapped to global variables.
        /// </summary>
        public JObject Globals { get; set; }

        /// <summary>
        ///  Configure Object of id: path pairs. Where supplied, these paths will be used in the generated bundle instead of the module ID, allowing you to (for example) load dependencies from a CDN
        /// </summary>
        public void ConfigurePaths(Action<dynamic> paths)
        {
            if (Paths == null)
            {
                Paths = new JObject();
            }
            paths?.Invoke(Paths);
        }

        /// <summary>
        ///  Object of id: path pairs. Where supplied, these paths will be used in the generated bundle instead of the module ID, allowing you to (for example) load dependencies from a CDN
        /// </summary>
        public JObject Paths { get; set; }

        /// <summary>
        ///  Configure options for when the bundle output format is AMD.
        /// </summary>
        public void ConfigureAmd(Action<dynamic> amd)
        {
            if (Amd == null)
            {
                Amd = new JObject();
            }
            amd?.Invoke(Amd);
        }

        /// <summary>
        ///  Configure options for when the bundle output format is AMD.
        /// </summary>
        public JObject Amd { get; set; }

    }
}