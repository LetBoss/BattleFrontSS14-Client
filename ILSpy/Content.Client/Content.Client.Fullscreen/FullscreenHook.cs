using System;
using Content.Shared.Input;
using Robust.Client.Input;
using Robust.Shared;
using Robust.Shared.Configuration;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Player;

namespace Content.Client.Fullscreen;

public sealed class FullscreenHook
{
	[Dependency]
	private IInputManager _inputManager;

	[Dependency]
	private IConfigurationManager _cfg;

	[Dependency]
	private ILogManager _logManager;

	private ISawmill _sawmill;

	public void Initialize()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		_inputManager.SetInputCommand(ContentKeyFunctions.ToggleFullscreen, InputCmdHandler.FromDelegate(new StateInputCmdDelegate(ToggleFullscreen), (StateInputCmdDelegate)null, true, true));
		_sawmill = _logManager.GetSawmill("fullscreen");
	}

	private void ToggleFullscreen(ICommonSession? session)
	{
		int cVar = _cfg.GetCVar<int>(CVars.DisplayWindowMode);
		switch (cVar)
		{
		case 0:
			_cfg.SetCVar<int>(CVars.DisplayWindowMode, 1, false);
			_sawmill.Info("Switched to Fullscreen mode");
			break;
		case 1:
			_cfg.SetCVar<int>(CVars.DisplayWindowMode, 0, false);
			_sawmill.Info("Switched to Windowed mode");
			break;
		default:
			throw new InvalidOperationException($"Unexpected WindowMode value: {cVar}");
		}
	}
}
