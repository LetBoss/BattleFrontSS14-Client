namespace Robust.Shared.Toolshed.Commands.Misc;

[ToolshedCommand]
internal sealed class LocCommand : ToolshedCommand
{
	[CommandImplementation("tryloc")]
	public string? TryLocalize([PipedArgument] string str)
	{
		Loc.TryGetString(str, out string value);
		return value;
	}

	[CommandImplementation("loc")]
	public string Localize([PipedArgument] string str)
	{
		return Loc.GetString(str);
	}
}
