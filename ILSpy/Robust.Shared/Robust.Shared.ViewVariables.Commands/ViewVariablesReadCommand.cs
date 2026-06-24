using Robust.Shared.Console;

namespace Robust.Shared.ViewVariables.Commands;

public sealed class ViewVariablesReadCommand : ViewVariablesBaseCommand
{
	public const string Comm = "vvread";

	public override string Command => "vvread";

	public override async void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		if (args.Length == 0)
		{
			shell.WriteError("Not enough arguments!");
			return;
		}
		string text = args[0];
		if (_netMan.IsClient)
		{
			if (!text.StartsWith("/c"))
			{
				shell.WriteLine((await _vvm.ReadRemotePath(text)) ?? "null");
				return;
			}
			string text2 = text;
			text = text2.Substring(2, text2.Length - 2);
		}
		string text3 = _vvm.ReadPathSerialized(text);
		shell.WriteLine(text3 ?? "null");
	}
}
