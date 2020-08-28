using Onyx.Binding.Nodes;
using Onyx.Binding.Nodes.Expressions;
using Onyx.Binding.Nodes.Statements;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Onyx.Binding
{
    internal sealed class ControlFlowGraph
    {
        public BasicBlock Start { get; }
        public BasicBlock End { get; }
        public List<BasicBlock> Blocks { get; }
        public List<BasicBlockBranch> Branches { get; }

        private ControlFlowGraph(BasicBlock start, BasicBlock end, List<BasicBlock> blocks, List<BasicBlockBranch> branches)
        {
            Start = start;
            End = end;
            Blocks = blocks;
            Branches = branches;
        }

        public sealed class BasicBlock
        {
            public bool IsStart { get; }
            public bool IsEnd { get; }
            public List<BoundStatement> Statements { get; } = new List<BoundStatement>();
            public List<BasicBlockBranch> Incoming { get; } = new List<BasicBlockBranch>();
            public List<BasicBlockBranch> Outgoing { get; } = new List<BasicBlockBranch>();

            public BasicBlock() { }
            public BasicBlock(bool isStart)
            {
                IsStart = isStart;
                IsEnd = !isStart;
            }

            public override string ToString()
            {
                if (IsStart)
                    return "<Start>";

                if (IsEnd)
                    return "<End>";

                using (var writer = new StringWriter())
                using (var indentedWriter = new IndentedTextWriter(writer))
                {
                    foreach (var statement in Statements)
                        statement.WriteTo(indentedWriter);

                    return writer.ToString();
                }
            }
        }
        public sealed class BasicBlockBranch
        {
            public BasicBlock From { get; }
            public BasicBlock To { get; }
            public BoundExpression? Condition { get; }

            public BasicBlockBranch(BasicBlock from, BasicBlock to, BoundExpression? condition)
            {
                From = from;
                To = to;
                Condition = condition;
            }

            public override string ToString()
            {
                if (Condition == null)
                    return string.Empty;

                return Condition.ToString();
            }
        }
        public sealed class BasicBlockBuilder
        {
            private List<BoundStatement> _statements = new List<BoundStatement>();
            private List<BasicBlock> blocks = new List<BasicBlock>();

            public List<BasicBlock> Build(BoundBlockStatement block)
            {
                foreach (var statement in block.Statements)
                {
                    switch (statement.Type)
                    {
                        case BoundNodeType.LabelStatement:
                            StartBlock();
                            _statements.Add(statement);
                            break;
                        case BoundNodeType.GotoStatement:
                        case BoundNodeType.ConditionalGotoStatement:
                        case BoundNodeType.ReturnStatement:
                            _statements.Add(statement);
                            StartBlock();
                            break;
                        case BoundNodeType.NopStatement:
                        case BoundNodeType.VariableDeclaration:
                        case BoundNodeType.ExpressionStatement:
                            _statements.Add(statement);
                            break;
                        default:
                            throw new Exception($"Unexpected statement: {statement.Type}");
                    }
                }

                EndBlock();

                return blocks.ToList();
            }

            private void StartBlock() => EndBlock();
            private void EndBlock()
            {
                if (_statements.Count > 0)
                {
                    var block = new BasicBlock();
                    block.Statements.AddRange(_statements);
                    blocks.Add(block);
                    _statements.Clear();
                }
            }
        }

        public sealed class GraphBuilder
        {
            private Dictionary<BoundStatement, BasicBlock> blockFromStatement = new Dictionary<BoundStatement, BasicBlock>();
            private Dictionary<BoundLabel, BasicBlock> blockFromLabel = new Dictionary<BoundLabel, BasicBlock>();
            private List<BasicBlockBranch> branches = new List<BasicBlockBranch>();
            private BasicBlock start = new BasicBlock(isStart: true);
            private BasicBlock end = new BasicBlock(isStart: false);

            public ControlFlowGraph Build(List<BasicBlock> blocks)
            {
                if (!blocks.Any())
                    Connect(start, end);
                else
                    Connect(start, blocks.First());

                foreach (var block in blocks)
                {
                    foreach (var statement in block.Statements)
                    {
                        blockFromStatement.Add(statement, block);

                        if (statement is BoundLabelStatement labelStatement)
                            blockFromLabel.Add(labelStatement.Label, block);
                    }
                }

                for (int i = 0; i < blocks.Count; i++)
                {
                    var current = blocks[i];
                    var next = i == blocks.Count - 1 ? end : blocks[i + 1];

                    foreach (var statement in current.Statements)
                    {
                        var isLastStatementInBlock = statement == current.Statements.Last();

                        switch (statement.Type)
                        {
                            case BoundNodeType.GotoStatement:
                                var gs = (BoundGotoStatement)statement;
                                var toBlock = blockFromLabel[gs.Label];
                                Connect(current, toBlock);
                                break;
                            case BoundNodeType.ConditionalGotoStatement:
                                var cgs = (BoundConditionalGotoStatement)statement;
                                var thenBlock = blockFromLabel[cgs.Label];
                                var elseBlock = next;
                                var negatedCondition = Negate(cgs.Condition);
                                var thenCondition = cgs.JumpIfTrue ? cgs.Condition : negatedCondition;
                                var elseCondition = cgs.JumpIfTrue ? negatedCondition : cgs.Condition;

                                Connect(current, thenBlock, thenCondition);
                                Connect(current, elseBlock, elseCondition);

                                break;
                            case BoundNodeType.ReturnStatement:
                                Connect(current, end);
                                break;
                            case BoundNodeType.NopStatement:
                            case BoundNodeType.VariableDeclaration:
                            case BoundNodeType.LabelStatement:
                            case BoundNodeType.ExpressionStatement:
                                if (isLastStatementInBlock)
                                    Connect(current, next);
                                break;
                            default:
                                throw new Exception($"Unexpected statement: {statement.Type}");
                        }
                    }
                }

            ScanAgain:
                foreach (var block in blocks)
                {
                    if (!block.Incoming.Any())
                    {
                        RemoveBlock(blocks, block);
                        goto ScanAgain;
                    }
                }

                blocks.Insert(0, start);
                blocks.Add(end);

                return new ControlFlowGraph(start, end, blocks, branches);
            }
            private void Connect(BasicBlock from, BasicBlock to, BoundExpression? condition = null)
            {
                if (condition is BoundLiteralExpression l)
                {
                    var value = (bool)l.Value;

                    if (value)
                        condition = null;
                    else
                        return;
                }

                var branch = new BasicBlockBranch(from, to, condition);

                from.Outgoing.Add(branch);
                to.Incoming.Add(branch);
                branches.Add(branch);
            }
            private void RemoveBlock(List<BasicBlock> blocks, BasicBlock block)
            {
                foreach (var branch in block.Incoming)
                {
                    branch.From.Outgoing.Remove(branch);
                    branches.Remove(branch);
                }

                foreach (var branch in block.Outgoing)
                {
                    branch.To.Incoming.Remove(branch);
                    branches.Remove(branch);
                }

                blocks.Remove(block);
            }
            private BoundExpression Negate(BoundExpression condition)
            {
                var negated = BoundNodeFactory.Not(condition.Syntax, condition);

                if (negated.ConstantValue != null)
                    return new BoundLiteralExpression(condition.Syntax, negated.ConstantValue.Value);

                return negated;
            }
        }

        public void WriteTo(TextWriter writer)
        {
            string Quote(string text)
            {
                return "\"" + text.TrimEnd().Replace("\\", "\\\\").Replace("\"", "\\\"").Replace(Environment.NewLine, "\\l") + "\"";
            }

            writer.WriteLine("digraph G {");

            var blockIds = new Dictionary<BasicBlock, string>();

            for (int i = 0; i < Blocks.Count; i++)
            {
                var id = $"N{i}";
                blockIds.Add(Blocks[i], id);
            }

            foreach (var block in Blocks)
            {
                var id = blockIds[block];
                var label = Quote(block.ToString());
                writer.WriteLine($"    {id} [label = {label}, shape = box]");
            }

            foreach (var branch in Branches)
            {
                var fromId = blockIds[branch.From];
                var toId = blockIds[branch.To];
                var label = Quote(branch.ToString());
                writer.WriteLine($"    {fromId} -> {toId} [label = {label}]");
            }

            writer.WriteLine("}");
        }
        public static ControlFlowGraph Create(BoundBlockStatement body)
        {
            var basicBlockBuilder = new BasicBlockBuilder();
            var blocks = basicBlockBuilder.Build(body);

            var graphBuilder = new GraphBuilder();
            return graphBuilder.Build(blocks);
        }
        public static bool AllPathsReturn(BoundBlockStatement body)
        {
            var graph = Create(body);

            foreach (var branch in graph.End.Incoming)
            {
                var lastStatement = branch.From.Statements.LastOrDefault();
                if (lastStatement == null || lastStatement.Type != BoundNodeType.ReturnStatement)
                    return false;
            }

            return true;
        }
    }
}
