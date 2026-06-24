using System.Linq;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Robust.Shared.Console.Commands;

internal sealed class HelpCommand : LocalizedCommands
{
	private static readonly string Gold;

	private static readonly string Aqua;

	public override string Command => "help";

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		ExecuteStatic(shell, argStr, args, base.Loc);
	}

	public static void ExecuteStatic(IConsoleShell shell, string argStr, string[] args, ILocalizationManager loc)
	{
		switch (args.Length)
		{
		case 0:
			shell.WriteLine("\n  TOOLSHED\n /.\\\\\\\\\\\\\\\\\n/___\\\\\\\\\\\\\\\\\n|''''|'''''|\n| 8  | === |\n|_0__|_____|");
			shell.WriteMarkup($"\nFor a list of commands, run [color={Gold}]cmd:list[/color].\nTo search for commands, run [color={Gold}]cmd:list search \"[color={Aqua}]query[/color]\"[/color].\nFor a breakdown of how a string of commands flows, run [color={Gold}]explain [color={Aqua}]commands here[/color][/color].\nFor help with old console commands, run [color={Gold}]oldhelp[/color].\n");
			break;
		case 1:
		{
			string text = args[0];
			if (!shell.ConsoleHost.AvailableCommands.TryGetValue(text, out IConsoleCommand value))
			{
				shell.WriteError(loc.GetString("cmd-help-unknown", ("command", text)));
				break;
			}
			shell.WriteLine(loc.GetString("cmd-help-top", ("command", value.Command), ("description", value.Description)));
			shell.WriteLine(value.Help);
			break;
		}
		default:
			shell.WriteError(loc.GetString("cmd-help-invalid-args"));
			break;
		}
	}

	public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
	{
		return GetCompletionStatic(shell, args, base.Loc);
	}

	public static CompletionResult GetCompletionStatic(IConsoleShell shell, string[] args, ILocalizationManager loc)
	{
		if (args.Length == 1)
		{
			return CompletionResult.FromHintOptions((from c in shell.ConsoleHost.AvailableCommands.Values
				orderby c.Command
				select new CompletionOption(c.Command, c.Description)).ToArray(), loc.GetString("cmd-help-arg-cmdname"));
		}
		return CompletionResult.Empty;
	}

	static HelpCommand()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		Color val = Color.Gold;
		Gold = ((Color)(ref val)).ToHex();
		val = Color.Aqua;
		Aqua = ((Color)(ref val)).ToHex();
	}
}
