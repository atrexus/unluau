using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unluau.IR;
using Unluau.IR.ControlFlow.Collectors;
using Unluau.IR.ControlFlow.Nodes;
using Unluau.IR.ControlFlow.Reductions;
using Unluau.IR.ProtoTypes;

namespace Unluau.IR.ControlFlow
{
    /// <summary>
    /// Analyzes the control flow of the program. It creates various reductions in the control flow graph by grouping
    /// basic blocks together into larger, abstract blocks.
    /// </summary>
    public class ControlFlowAnalyzer : Visitor
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;

        private readonly List<ReductionPass> _reductionPasses;

        /// <summary>
        /// Creates a new instance of the <see cref="ControlFlowAnalyzer"/> class.
        /// </summary>
        private ControlFlowAnalyzer(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger("ControlFlowAnalyzer");

            _reductionPasses =
            [
                new SequentialReduction(_loggerFactory),
                new IfThenReduction(_loggerFactory)
            ];
        }

        /// <summary>
        /// Analyzes the control flow of the module.
        /// </summary>
        /// <param name="module">The target module.</param>
        public static void Analyze(ILoggerFactory loggerFactory, Module module)
        {
            var analyzer = new ControlFlowAnalyzer(loggerFactory);
            module.Accept(analyzer);
        }

        public override bool Visit(ProtoType protoType)
        {
            var name = protoType.IsMain ? "main" : protoType.Name ?? $"prototype_{protoType.GetHashCode():x}";

            _logger.LogInformation("Analyzing control flow graph for prototype '{}'", name);

            // Collect the final nodes of the control flow graph. We will start reducing the graph from these nodes.
            var finalNodes = FinalNodeCollector.Collect(protoType);

            _logger.LogDebug("Found {} final node(s) => {}", finalNodes.Count, string.Join(',', finalNodes));

            // Run all reduction passes on the final nodes.
            foreach (var pass in _reductionPasses)
            {
                _logger.LogInformation("Running reduction pass '{}'", pass.GetType().Name);
                pass.Run(finalNodes, protoType);
            }

            return false;
        }
    }
}
