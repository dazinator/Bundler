using System.Text.Json.Nodes;

namespace NetPack.Rollup
{
    public class RollupResult
    {
        public JsonValue Code { get; set; }

        public SourceMap SourceMap { get; set; }

        public string FileName { get; set; }

        public string[] Exports { get; set; }

        public string[] Imports { get; set; }

        public bool IsEntry { get; set; }

        public RollupModuleResult[] Modules { get; set; }

        public string Id { get; set; }
    }
}