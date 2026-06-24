using System;
using System.Collections.Generic;

namespace Robust.Shared.Toolshed.Commands.Misc;

[ToolshedCommand]
internal sealed class TypesCommand : ToolshedCommand
{
	[CommandImplementation("consumers")]
	public void Consumers(IInvocationContext ctx, [PipedArgument] object? input)
	{
		Type type = ((input is Type) ? ((Type)input) : input.GetType());
		ctx.WriteLine("Valid intakers for " + type.PrettyName() + ":");
		CommandSpec[] array = ctx.Environment.CommandsTakingType(type);
		for (int i = 0; i < array.Length; i++)
		{
			CommandSpec commandSpec = array[i];
			var (toolshedCommand2, text2) = (CommandSpec)(ref commandSpec);
			if (text2 == null)
			{
				ctx.WriteLine(toolshedCommand2.Name ?? "");
			}
			else
			{
				ctx.WriteLine(toolshedCommand2.Name + ":" + text2);
			}
		}
	}

	[CommandImplementation("tree")]
	public IEnumerable<Type> Tree(IInvocationContext ctx, [PipedArgument] object? input)
	{
		Type t = ((input is Type) ? ((Type)input) : input.GetType());
		return Toolshed.AllSteppedTypes(t);
	}

	[CommandImplementation("gettype")]
	public Type GetType([PipedArgument] object? input)
	{
		return input?.GetType() ?? typeof(void);
	}

	[CommandImplementation("fullname")]
	public string FullName([PipedArgument] Type input)
	{
		return input.FullName;
	}
}
