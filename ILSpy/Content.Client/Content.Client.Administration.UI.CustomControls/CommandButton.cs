using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Content.Client.Guidebook.Richtext;
using Robust.Client.Console;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Analyzers;
using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;

namespace Content.Client.Administration.UI.CustomControls;

[Virtual]
public class CommandButton : Button, IDocumentTag
{
	private static readonly ISawmill Sawmill = Logger.GetSawmill("admin.command_button");

	public string? Command { get; set; }

	public CommandButton()
	{
		((BaseButton)this).OnPressed += Execute;
	}

	protected virtual bool CanPress()
	{
		if (!string.IsNullOrEmpty(Command))
		{
			return ((IClientConGroupImplementation)IoCManager.Resolve<IClientConGroupController>()).CanCommand(Command.Split(' ')[0]);
		}
		return true;
	}

	protected override void EnteredTree()
	{
		if (!CanPress())
		{
			((Control)this).Visible = false;
		}
	}

	protected virtual void Execute(ButtonEventArgs obj)
	{
		if (!string.IsNullOrEmpty(Command))
		{
			((IConsoleHost)IoCManager.Resolve<IClientConsoleHost>()).ExecuteCommand(Command);
		}
	}

	public bool TryParseTag(Dictionary<string, string> args, [NotNullWhen(true)] out Control? control)
	{
		if (args.Count != 2 || !args.TryGetValue("Text", out string value) || !args.TryGetValue("Command", out string value2))
		{
			Sawmill.Error("Invalid arguments passed to CommandButton");
			control = null;
			return false;
		}
		Command = value2;
		((Button)this).Text = Loc.GetString(value);
		control = (Control?)(object)this;
		return true;
	}
}
