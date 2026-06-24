using Robust.Shared.Console;

namespace Robust.Shared.ViewVariables.Commands;

public sealed class ViewVariablesInvokeCommand : ViewVariablesBaseCommand
{
	public const string Comm = "vvinvoke";

	public override string Command => "vvinvoke";

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		if (args.Length == 0)
		{
			shell.WriteError("Not enough arguments!");
			return;
		}
		string text = args[0];
		string arguments = string.Join(string.Empty, args[1..]);
		if (_netMan.IsClient)
		{
			if (!text.StartsWith("/c"))
			{
				_vvm.InvokeRemotePath(text, arguments);
				return;
			}
			string text2 = text;
			text = text2.Substring(2, text2.Length - 2);
		}
		shell.WriteLine(_vvm.InvokePath(text, arguments)?.ToString() ?? "null");
	}
}
