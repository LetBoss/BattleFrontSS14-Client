using System.Collections;
using System.Linq;

namespace Robust.Shared.Toolshed.Commands.Generic.Variables;

[ToolshedCommand]
public sealed class VarsCommand : ToolshedCommand
{
	[CommandImplementation(null)]
	public void Vars(IInvocationContext ctx)
	{
		ctx.WriteLine(Toolshed.PrettyPrintType(from x in ctx.GetVars()
			select x + " = " + Toolshed.PrettyPrintType(ctx.ReadVar(x), out IEnumerable _), out IEnumerable _));
	}
}
