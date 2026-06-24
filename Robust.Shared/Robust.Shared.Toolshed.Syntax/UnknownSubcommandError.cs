using System.Diagnostics;
using Robust.Shared.Maths;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Utility;

namespace Robust.Shared.Toolshed.Syntax;

public record UnknownSubcommandError(string SubCmd, ToolshedCommand Command) : IConError
{
	public string? Expression { get; set; }

	public Vector2i? IssueSpan { get; set; }

	public StackTrace? Trace { get; set; }

	public FormattedMessage DescribeInner()
	{
		FormattedMessage formattedMessage = new FormattedMessage();
		formattedMessage.AddText($"The command group {Command.Name} doesn't have command {SubCmd}.");
		formattedMessage.PushNewline();
		formattedMessage.AddText("The valid commands are: " + string.Join(", ", Command.Subcommands) + ".");
		return formattedMessage;
	}
}
