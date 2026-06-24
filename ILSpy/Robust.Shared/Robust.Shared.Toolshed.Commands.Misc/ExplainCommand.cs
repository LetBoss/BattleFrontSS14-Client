using System.Reflection;
using System.Text;
using Robust.Shared.Toolshed.Syntax;

namespace Robust.Shared.Toolshed.Commands.Misc;

[ToolshedCommand]
public sealed class ExplainCommand : ToolshedCommand
{
	[CommandImplementation(null)]
	public void Explain(IInvocationContext ctx, CommandRun expr)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (var command in expr.Commands)
		{
			ParsedCommand item = command.Item1;
			stringBuilder.AppendLine();
			string fullName = item.Implementor.FullName;
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder3 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(3, 2, stringBuilder2);
			handler.AppendFormatted(fullName);
			handler.AppendLiteral(" - ");
			handler.AppendFormatted(item.Implementor.Description());
			stringBuilder3.AppendLine(ref handler);
			string value = item.PipedType?.PrettyName() ?? "[none]";
			stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder4 = stringBuilder2;
			handler = new StringBuilder.AppendInterpolatedStringHandler(12, 1, stringBuilder2);
			handler.AppendLiteral("Pipe input: ");
			handler.AppendFormatted(value);
			stringBuilder4.AppendLine(ref handler);
			stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder5 = stringBuilder2;
			handler = new StringBuilder.AppendInterpolatedStringHandler(13, 1, stringBuilder2);
			handler.AppendLiteral("Pipe output: ");
			handler.AppendFormatted(item.ReturnType.PrettyName());
			stringBuilder5.AppendLine(ref handler);
			stringBuilder.Append("Signature:\n  ");
			if (item.PipedType != null)
			{
				ParameterInfo pipeArg = item.Method.Base.PipeArg;
				string messageId = "command-arg-sig-" + item.Implementor.LocName + "-" + pipeArg?.Name;
				if (Loc.TryGetString(messageId, out string value2))
				{
					stringBuilder.Append(value2);
					stringBuilder.Append(" → ");
				}
				else
				{
					stringBuilder2 = stringBuilder;
					StringBuilder stringBuilder6 = stringBuilder2;
					handler = new StringBuilder.AppendInterpolatedStringHandler(5, 1, stringBuilder2);
					handler.AppendLiteral("<");
					handler.AppendFormatted(pipeArg?.Name);
					handler.AppendLiteral("> → ");
					stringBuilder6.Append(ref handler);
				}
			}
			if (item.Bundle.Inverted)
			{
				stringBuilder.Append("not ");
			}
			item.Implementor.AddMethodSignature(stringBuilder, item.Method.Args, item.Bundle.TypeArguments);
			stringBuilder.AppendLine();
		}
		ctx.WriteLine(stringBuilder.ToString().TrimEnd());
	}
}
