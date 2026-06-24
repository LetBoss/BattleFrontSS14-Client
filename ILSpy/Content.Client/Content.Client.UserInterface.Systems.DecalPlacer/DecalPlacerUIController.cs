using System.Collections.Generic;
using Content.Client._PUBG.Sponsor;
using Content.Client.Decals.UI;
using Content.Client.Gameplay;
using Content.Client.Sandbox;
using Content.Shared.Decals;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Client.UserInterface.Systems.DecalPlacer;

public sealed class DecalPlacerUIController : UIController, IOnStateExited<GameplayState>, IOnSystemChanged<SandboxSystem>, IOnSystemLoaded<SandboxSystem>, IOnSystemUnloaded<SandboxSystem>
{
	[Dependency]
	private IPrototypeManager _prototypes;

	[UISystemDependency]
	private readonly SandboxSystem _sandbox;

	private DecalPlacerWindow? _window;

	public void ToggleWindow()
	{
		EnsureWindow();
		if (((BaseWindow)_window).IsOpen)
		{
			((BaseWindow)_window).Close();
		}
		else if (_sandbox.SandboxAllowed)
		{
			((BaseWindow)_window).Open();
		}
		else if (base.EntityManager.System<SponsorSandboxSystem>().State.AllowSpawnDecals)
		{
			((BaseWindow)_window).Open();
		}
	}

	public void OnStateExited(GameplayState state)
	{
		if (_window != null)
		{
			if (!((Control)_window).Disposed)
			{
				((Control)_window).Orphan();
			}
			_window = null;
		}
	}

	public void OnSystemLoaded(SandboxSystem system)
	{
		_sandbox.SandboxDisabled += CloseWindow;
		_prototypes.PrototypesReloaded += OnPrototypesReloaded;
	}

	public void OnSystemUnloaded(SandboxSystem system)
	{
		_sandbox.SandboxDisabled -= CloseWindow;
		_prototypes.PrototypesReloaded -= OnPrototypesReloaded;
	}

	private void OnPrototypesReloaded(PrototypesReloadedEventArgs obj)
	{
		if (obj.WasModified<DecalPrototype>())
		{
			ReloadPrototypes();
		}
	}

	private void ReloadPrototypes()
	{
		if (_window != null && !((Control)_window).Disposed)
		{
			IEnumerable<DecalPrototype> prototypes = _prototypes.EnumeratePrototypes<DecalPrototype>();
			_window.Populate(prototypes);
		}
	}

	private void EnsureWindow()
	{
		DecalPlacerWindow window = _window;
		if (window == null || ((Control)window).Disposed)
		{
			_window = base.UIManager.CreateWindow<DecalPlacerWindow>();
			LayoutContainer.SetAnchorPreset((Control)(object)_window, (LayoutPreset)4, false);
			ReloadPrototypes();
		}
	}

	private void CloseWindow()
	{
		if (_window != null && !((Control)_window).Disposed)
		{
			((BaseWindow)_window).Close();
		}
	}
}
