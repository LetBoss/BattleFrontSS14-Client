using System;
using Content.Client._RMC14.Emplacements;
using Content.Client.CombatMode;
using Content.Client.Hands.Systems;
using Content.Shared._RMC14.CombatMode;
using Content.Shared.CCVar;
using Content.Shared.Wieldable.Components;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Content.Client._RMC14.CombatMode;

public sealed class RMCCombatModeUISystem : EntitySystem
{
	[Dependency]
	private IClyde _clyde;

	[Dependency]
	private CombatModeSystem _combatMode;

	[Dependency]
	private IConfigurationManager _config;

	[Dependency]
	private HandsSystem _hands;

	[Dependency]
	private RMCCombatModeSystem _rmcCombatMode;

	[Dependency]
	private IUserInterfaceManager _ui;

	[Dependency]
	private RMCWeaponControllerSystem _rmcSharedWeaponController;

	private bool _crosshairsEnabled;

	private ICursor? _crosshairCursor;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		EntitySystemSubscriptionExt.CVar<bool>(((EntitySystem)this).Subs, _config, CCVars.CombatModeIndicatorsPointShow, (Action<bool>)delegate(bool v)
		{
			_crosshairsEnabled = v;
		}, true);
	}

	public override void FrameUpdate(float frameTime)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		if (!(_ui.CurrentlyHovered is IViewportControl))
		{
			return;
		}
		if (!_crosshairsEnabled || !_combatMode.IsInCombatMode())
		{
			_ui.CurrentlyHovered.CustomCursorShape = null;
			return;
		}
		EntityUid? val = _hands.GetActiveHandEntity();
		if (_rmcSharedWeaponController.TryGetControllingWeapon(out var weapon))
		{
			val = weapon;
		}
		if (!val.HasValue || _rmcCombatMode.GetCrosshair(Entity<WieldedCrosshairComponent, WieldableComponent>.op_Implicit(val.Value)) == null)
		{
			_ui.CurrentlyHovered.CustomCursorShape = null;
			return;
		}
		if (_crosshairCursor == null)
		{
			_crosshairCursor = _clyde.CreateCursor(new Image<Rgba32>(1, 1), Vector2i.Zero);
		}
		_ui.CurrentlyHovered.CustomCursorShape = _crosshairCursor;
	}
}
