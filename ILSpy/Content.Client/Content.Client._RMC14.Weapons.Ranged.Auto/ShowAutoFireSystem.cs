using Content.Shared._RMC14.Weapons.Ranged.Auto;
using Robust.Client.Graphics;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._RMC14.Weapons.Ranged.Auto;

public sealed class ShowAutoFireSystem : EntitySystem
{
	[Dependency]
	private IConsoleHost _console;

	[Dependency]
	private GunToggleableAutoFireSystem _autoFire;

	[Dependency]
	private IOverlayManager _overlay;

	public override void Initialize()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		_console.RegisterCommand("showautofire", base.Loc.GetString("cmd-showautofire-desc"), base.Loc.GetString("cmd-showautofire-help"), new ConCommandCallback(ShowAutoFireCommand), false);
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		_console.UnregisterCommand("showautofire");
		_overlay.RemoveOverlay<ShowAutoFireOverlay>();
	}

	private void ShowAutoFireCommand(IConsoleShell shell, string argstr, string[] args)
	{
		if (!_overlay.RemoveOverlay<ShowAutoFireOverlay>())
		{
			_autoFire.Debug = true;
			_overlay.AddOverlay((Overlay)(object)new ShowAutoFireOverlay());
			shell.WriteLine(base.Loc.GetString("cmd-showautofire-enabled"));
		}
		else
		{
			_autoFire.Debug = false;
			shell.WriteLine(base.Loc.GetString("cmd-showautofire-disabled"));
		}
	}
}
