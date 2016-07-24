﻿namespace NetPack.Pipes
{
    public class TypeScriptPipeOptions
    {

        public TypeScriptPipeOptions()
        {
            SourceMap = true;
            NoImplicitAny = true;
            Module = ModuleKind.Amd;
            Target = ScriptTarget.Es6;
        }

        public ScriptTarget Target { get; set; }

        public ModuleKind Module { get; set; }
        public bool SourceMap { get; set; }
        public bool RemoveComments { get; set; }
        public bool NoImplicitAny { get; set; }

        public enum ScriptTarget
        {
            Es5,
            Es6
        }

        public enum ModuleKind
        {
            CommonJs,
            Amd
        }


    }
}