using Content.Client.Options.UI;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Client.UserInterface.Systems.EscapeMenu;

public sealed class OptionsUIController : UIController
{
	[Dependency]
	private IConsoleHost _con;

	private OptionsMenu _optionsWindow;

	public override void Initialize()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		_con.RegisterCommand("options", Loc.GetString("cmd-options-desc"), Loc.GetString("cmd-options-help"), new ConCommandCallback(OptionsCommand), false);
	}

	private void OptionsCommand(IConsoleShell shell, string argStr, string[] args)
	{
		if (args.Length == 0)
		{
			ToggleWindow();
			return;
		}
		OpenWindow();
		if (!int.TryParse(args[0], out var result))
		{
			shell.WriteError(Loc.GetString("cmd-parse-failure-integer", new(string, object)[1] { ("arg", args[0]) }));
		}
		else
		{
			_optionsWindow.Tabs.CurrentTab = result;
		}
	}

	private void EnsureWindow()
	{
		OptionsMenu optionsWindow = _optionsWindow;
		if (optionsWindow == null || ((Control)optionsWindow).Disposed)
		{
			_optionsWindow = base.UIManager.CreateWindow<OptionsMenu>();
		}
	}

	public void OpenWindow()
	{
		EnsureWindow();
		_optionsWindow.UpdateTabs();
		((BaseWindow)_optionsWindow).OpenCentered();
		((BaseWindow)_optionsWindow).MoveToFront();
	}

	public void ToggleWindow()
	{
		EnsureWindow();
		if (((BaseWindow)_optionsWindow).IsOpen)
		{
			((BaseWindow)_optionsWindow).Close();
		}
		else
		{
			OpenWindow();
		}
	}
}
