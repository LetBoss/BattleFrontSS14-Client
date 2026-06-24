using System;
using Content.Client.UserInterface.Systems.Hotbar.Widgets;
using Content.Shared._PUBG.Vision;
using Content.Shared.CombatMode;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Player;

namespace Content.Client._PUBG.Vision;

public sealed class PubgAimHideHudSystem : EntitySystem
{
	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private IInputManager _inputManager;

	[Dependency]
	private IClyde _clyde;

	[Dependency]
	private IUserInterfaceManager _ui;

	[Dependency]
	private SharedHandsSystem _hands;

	private bool _hidden;

	public override void FrameUpdate(float frameTime)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		HotbarGui activeUIWidgetOrNull = _ui.GetActiveUIWidgetOrNull<HotbarGui>();
		if (activeUIWidgetOrNull == null)
		{
			_hidden = false;
			return;
		}
		bool flag = ShouldHide();
		if (flag != _hidden)
		{
			_hidden = flag;
			if (flag)
			{
				((Control)activeUIWidgetOrNull).Visible = false;
				return;
			}
			EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
			((Control)activeUIWidgetOrNull).Visible = localEntity.HasValue && ((EntitySystem)this).HasComp<HandsComponent>(localEntity.Value);
		}
	}

	private bool ShouldHide()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (!localEntity.HasValue)
		{
			return false;
		}
		CombatModeComponent combatModeComponent = default(CombatModeComponent);
		if (!((EntitySystem)this).TryComp<CombatModeComponent>(localEntity.Value, ref combatModeComponent) || !combatModeComponent.IsInCombatMode)
		{
			return false;
		}
		if (!_hands.TryGetActiveItem(Entity<HandsComponent>.op_Implicit((ValueTuple<EntityUid, HandsComponent>)(localEntity.Value, null)), out var item) || !item.HasValue)
		{
			return false;
		}
		PubgFocusViewComponent pubgFocusViewComponent = default(PubgFocusViewComponent);
		if (!((EntitySystem)this).TryComp<PubgFocusViewComponent>(item.Value, ref pubgFocusViewComponent) || !pubgFocusViewComponent.Active)
		{
			return false;
		}
		ScreenCoordinates mouseScreenPosition = _inputManager.MouseScreenPosition;
		if (mouseScreenPosition.Window == WindowId.Invalid)
		{
			return false;
		}
		int y = _clyde.MainWindow.Size.Y;
		return ((ScreenCoordinates)(ref mouseScreenPosition)).Y > (float)y / 2f;
	}
}
