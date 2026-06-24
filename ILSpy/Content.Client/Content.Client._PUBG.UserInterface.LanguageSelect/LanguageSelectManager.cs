using System;
using Content.Client._PUBG.Settings;
using Content.Shared.CCVar;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Maths;

namespace Content.Client._PUBG.UserInterface.LanguageSelect;

public sealed class LanguageSelectManager
{
	[Dependency]
	private IUserInterfaceManager _ui;

	[Dependency]
	private IConfigurationManager _cfg;

	[Dependency]
	private IClyde _clyde;

	private LanguageSelectWindow? _window;

	private ISawmill _sawmill;

	public void Initialize()
	{
		_sawmill = Logger.GetSawmill("language-select");
		TryApplyGeneralAutoSettings();
		TryShowLanguageSelectOnFirstRun();
	}

	public void ShowLanguageSelect(bool forceEnglishPrompt = false)
	{
		if (_window != null)
		{
			_sawmill.Warning("Language select window already open");
			return;
		}
		_window = new LanguageSelectWindow(forceEnglishPrompt);
		((BaseWindow)_window).OnClose += OnWindowClosed;
		((Control)_ui.WindowRoot).AddChild((Control)(object)_window);
		LayoutContainer.SetAnchorPreset((Control)(object)_window, (LayoutPreset)8, false);
		((BaseWindow)_window).Open();
	}

	private void OnWindowClosed()
	{
		_window = null;
	}

	private void TryApplyGeneralAutoSettings()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		int cVar = _cfg.GetCVar<int>(CCVars.PubgGeneralAutoSettingsVersion);
		if (cVar < 3)
		{
			Vector2i screenSize = _clyde.ScreenSize;
			if (screenSize.X > 0 && screenSize.Y > 0)
			{
				PubgGeneralAutoSettings.Apply(_cfg, screenSize, cVar);
				_cfg.SetCVar<int>(CCVars.PubgGeneralAutoSettingsVersion, 3, false);
				_cfg.SaveToFile();
			}
		}
	}

	private void TryShowLanguageSelectOnFirstRun()
	{
		if (string.Equals(_cfg.GetCVar<string>(CCVars.Language), "auto", StringComparison.OrdinalIgnoreCase))
		{
			ShowLanguageSelect(forceEnglishPrompt: true);
		}
	}
}
