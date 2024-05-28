using System;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

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
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RollupOutputFormat Format { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public SourceMapType? Sourcemap { get; set; }

        /// <summary>
        /// The variable name, representing your iife/umd bundle, by which other scripts on the same page can access it.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Configure Object of id: name pairs, used for umd/iife bundles.
        /// Used to tell Rollup which module ids are mapped to global variables.
        /// </summary>
        public void ConfigureGlobals(Action<JsonObject> globals)
        {
            if (Globals == null)
            {
                Globals = new JsonObject();
            }
            globals?.Invoke(Globals);
        }

        /// <summary>
        /// Object of id: name pairs, used for umd/iife bundles.
        /// Used to tell Rollup which module ids are mapped to global variables.
        /// </summary>
        public JsonObject Globals { get; set; }

        /// <summary>
        /// Configure Object of id: path pairs. Where supplied, these paths will be used in the generated bundle instead of the module ID, allowing you to (for example) load dependencies from a CDN.
        /// </summary>
        public void ConfigurePaths(Action<JsonObject> paths)
        {
            if (Paths == null)
            {
                Paths = new JsonObject();
            }
            paths?.Invoke(Paths);
        }

        /// <summary>
        /// Object of id: path pairs. Where supplied, these paths will be used in the generated bundle instead of the module ID, allowing you to (for example) load dependencies from a CDN.
        /// </summary>
        public JsonObject Paths { get; set; }

        /// <summary>
        /// Configure options for when the bundle output format is AMD.
        /// </summary>
        public void ConfigureAmd(Action<JsonObject> amd)
        {
            if (Amd == null)
            {
                Amd = new JsonObject();
            }
            amd?.Invoke(Amd);
        }

        /// <summary>
        /// Configure options for when the bundle output format is AMD.
        /// </summary>
        public JsonObject Amd { get; set; }
    }
}