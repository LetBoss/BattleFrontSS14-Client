using Robust.Shared.Console;

namespace Robust.Shared.ViewVariables.Commands;

public sealed class ViewVariablesWriteCommand : ViewVariablesBaseCommand
{
	public const string Comm = "vvwrite";

	public override string Command => "vvwrite";

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		if (args.Length != 2)
		{
			shell.WriteError("Incorrect number of arguments!");
			return;
		}
		string text = args[0];
		string value = args[1];
		if (_netMan.IsClient)
		{
			if (!text.StartsWith("/c"))
			{
				_vvm.WriteRemotePath(text, value);
				return;
			}
			string text2 = text;
			text = text2.Substring(2, text2.Length - 2);
		}
		_vvm.WritePath(text, value);
	}
}
