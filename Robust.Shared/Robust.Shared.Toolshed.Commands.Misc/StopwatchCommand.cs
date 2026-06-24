using System.Runtime.CompilerServices;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using Robust.Shared.Toolshed.Syntax;

namespace Robust.Shared.Toolshed.Commands.Misc;

[ToolshedCommand]
public sealed class StopwatchCommand : ToolshedCommand
{
	[CommandImplementation(null)]
	public object? Stopwatch(IInvocationContext ctx, CommandRun expr)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		object? result = expr.Invoke(null, ctx);
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(34, 2);
		defaultInterpolatedStringHandler.AppendLiteral("Ran expression in [color=");
		Color aqua = Color.Aqua;
		defaultInterpolatedStringHandler.AppendFormatted(((Color)(ref aqua)).ToHex());
		defaultInterpolatedStringHandler.AppendLiteral("]");
		defaultInterpolatedStringHandler.AppendFormatted(stopwatch.Elapsed, "g");
		defaultInterpolatedStringHandler.AppendLiteral("[/color]");
		ctx.WriteMarkup(defaultInterpolatedStringHandler.ToStringAndClear());
		return result;
	}
}
