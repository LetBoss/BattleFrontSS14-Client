using System.Linq;
using System.Text;

namespace Robust.Shared.Console.Commands;

internal sealed class ListCommands : LocalizedCommands
{
	public override string Command => "list";

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		string filter = "";
		if (args.Length == 1)
		{
			filter = args[0];
		}
		IConsoleHostInternal consoleHostInternal = (IConsoleHostInternal)shell.ConsoleHost;
		StringBuilder stringBuilder = new StringBuilder(base.Loc.GetString("cmd-list-heading"));
		foreach (IConsoleCommand item in from c in consoleHostInternal.AvailableCommands.Values
			where c.Command.Contains(filter)
			orderby c.Command
			select c)
		{
			string value = (consoleHostInternal.IsCmdServer(item) ? "S" : "C");
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(1, 3, stringBuilder2);
			handler.AppendFormatted(value);
			handler.AppendLiteral(" ");
			handler.AppendFormatted<string>(item.Command, -32);
			handler.AppendFormatted(item.Description);
			stringBuilder2.AppendLine(ref handler);
		}
		string text = stringBuilder.ToString().Trim(new char[2] { ' ', '\n' });
		shell.WriteLine(text);
	}

	public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
	{
		if (args.Length == 1)
		{
			return CompletionResult.FromHint(base.Loc.GetString("cmd-list-arg-filter"));
		}
		return CompletionResult.Empty;
	}
}
