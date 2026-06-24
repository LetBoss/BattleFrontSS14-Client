using System.Collections.Generic;

namespace Robust.Shared.Toolshed.Commands.Misc;

[ToolshedCommand]
public sealed class CmdCommand : ToolshedCommand
{
	[CommandImplementation("list")]
	public IEnumerable<CommandSpec> List(IInvocationContext ctx)
	{
		return ctx.Environment.AllCommands();
	}

	[CommandImplementation("moo")]
	public string Moo()
	{
		return "Have you mooed today?";
	}

	[CommandImplementation("descloc")]
	public string GetLocStr([PipedArgument] CommandSpec cmd)
	{
		return cmd.DescLocStr();
	}

	[CommandImplementation("info")]
	public CommandSpec Info(CommandSpec cmd)
	{
		return cmd;
	}
}
