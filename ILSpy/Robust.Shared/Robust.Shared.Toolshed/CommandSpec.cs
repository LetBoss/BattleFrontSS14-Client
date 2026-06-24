using Robust.Shared.Console;
using Robust.Shared.Toolshed.TypeParsers;

namespace Robust.Shared.Toolshed;

public readonly record struct CommandSpec(ToolshedCommand Cmd, string? SubCommand) : IAsType<ToolshedCommand>
{
	public ToolshedCommand AsType()
	{
		return Cmd;
	}

	public CompletionOption AsCompletion()
	{
		return new CompletionOption(FullName(), Cmd.Description(SubCommand));
	}

	public string FullName()
	{
		if (SubCommand != null)
		{
			return Cmd.Name + ":" + SubCommand;
		}
		return Cmd.Name;
	}

	public string DescLocStr()
	{
		return Cmd.DescriptionLocKey(SubCommand);
	}

	public override string ToString()
	{
		return FullName();
	}
}
