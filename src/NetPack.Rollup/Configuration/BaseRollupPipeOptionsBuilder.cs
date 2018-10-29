﻿using NetPack.Pipeline;
using NetPack.Rollup;
using System;

// ReSharper disable once CheckNamespace
// Extension method put in root namespace for discoverability purposes.
namespace NetPack
{
    public class BaseRollupPipeOptionsBuilder<TInputOptions, TOutputOptions, TBuilder>
        where TInputOptions : BaseRollupInputOptions, new()
        where TOutputOptions : BaseRollupOutputOptions, new()
        where TBuilder: BaseRollupPipeOptionsBuilder<TInputOptions, TOutputOptions, TBuilder>
    {
        private readonly IPipelineBuilder _builder;
        private readonly TInputOptions _inputOptions;
        private readonly TOutputOptions _outputOptions;


        public BaseRollupPipeOptionsBuilder(IPipelineBuilder builder) : this(builder, new TInputOptions())
        {
        }

        public BaseRollupPipeOptionsBuilder(IPipelineBuilder builder, TInputOptions inputOptions) : this(builder, new TInputOptions(), new TOutputOptions())
        {
        }

        public BaseRollupPipeOptionsBuilder(IPipelineBuilder builder, TInputOptions inputOptions, TOutputOptions outputOptions)
        {
            _builder = builder;
            _inputOptions = inputOptions;
            _outputOptions = outputOptions;
        }

        public TBuilder AddPlugin(Action<IRollupPluginOptionsBuilder> configurePlugin)
        {
            RollupPluginOptionsBuilder builder = new RollupPluginOptionsBuilder();
            configurePlugin(builder);
            IPipelineBuilder.IncludeRequirement(builder.ModuleRequirement);
            InputOptions.AddPlugin(builder.ModuleRequirement.PackageName, builder.Options, builder.DefaultExportName1, builder.IsImportOnly);
            return (TBuilder)this;
        }

        public IPipelineBuilder IPipelineBuilder => _builder;

        public TInputOptions InputOptions => _inputOptions;

        public TOutputOptions OutputOptions => _outputOptions;

    }
}