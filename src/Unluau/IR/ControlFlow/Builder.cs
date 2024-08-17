using Microsoft.Extensions.Logging;
using Unluau.IR.ControlFlow.Nodes;
using Unluau.IR.ProtoTypes;
using Unluau.IR.ProtoTypes.Instructions;

namespace Unluau.IR.ControlFlow
{
    /// <summary>
    /// Builds the control flow graph from an IR module.
    /// </summary>
    public class Builder(ILoggerFactory loggerFactory, Module module) : Visitor
    {
        /// <summary>
        /// The logger for the builder.
        /// </summary>
        private readonly ILogger<Builder> _logger = loggerFactory.CreateLogger<Builder>();

        /// <summary>
        /// The module to build the control flow graph for.
        /// </summary>
        private readonly Module _module = module;

        /// <summary>
        /// Builds the control flow graphs for all function prototypes in the module.
        /// </summary>
        public void Build() => _module.Accept(this);

        public override bool Visit(ProtoType protoType)
        {
            using (_logger.BeginScope("[{:x}]", protoType.GetHashCode()))
            {
                var name = protoType.IsMain ? (protoType.Name ?? $"prototype_{protoType.GetHashCode():x}") : "main";

                _logger.LogInformation("Building control flow graph for prototype '{}'", name);

                foreach (var instruction in protoType.Instructions)
                    instruction.Accept(this);
            }

            return false;
        }

        public override bool Visit(Instruction instruction)
        {
            return false;
        }
    }
}
