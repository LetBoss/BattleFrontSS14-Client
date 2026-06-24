using System.Linq;
using System.Reflection;
using Robust.Shared.Utility;

namespace Robust.Shared.Toolshed;

internal sealed class CommandMethod
{
	public readonly MethodInfo Info;

	public readonly ParameterInfo? PipeArg;

	public readonly bool Generic;

	public readonly bool Invertible;

	public readonly bool PipeGeneric;

	public readonly CommandArgument[] Arguments;

	public CommandMethod(MethodInfo info, ToolshedCommandImplementor impl)
	{
		Info = info;
		PipeArg = info.ConsoleGetPipedArgument();
		Invertible = info.ConsoleHasInvertedArgument();
		Arguments = (from x in info.GetParameters()
			where x.IsCommandArgument()
			select x).Select(impl.GetCommandArgument).ToArray();
		if (info.IsGenericMethodDefinition)
		{
			Generic = true;
			PipeGeneric = info.HasCustomAttribute<TakesPipedTypeAsGenericAttribute>();
		}
	}
}
