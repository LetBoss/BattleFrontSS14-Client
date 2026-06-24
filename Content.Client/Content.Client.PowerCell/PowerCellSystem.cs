using System;
using Content.Shared.PowerCell;
using Content.Shared.PowerCell.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.PowerCell;

public sealed class PowerCellSystem : SharedPowerCellSystem
{
	private enum PowerCellVisualLayers : byte
	{
		Base,
		Unshaded
	}

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<PowerCellVisualsComponent, AppearanceChangeEvent>((ComponentEventRefHandler<PowerCellVisualsComponent, AppearanceChangeEvent>)OnPowerCellVisualsChange, (Type[])null, (Type[])null);
	}

	public override bool HasActivatableCharge(EntityUid uid, PowerCellDrawComponent? battery = null, PowerCellSlotComponent? cell = null, EntityUid? user = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<PowerCellDrawComponent, PowerCellSlotComponent>(uid, ref battery, ref cell, false))
		{
			return true;
		}
		return battery.CanUse;
	}

	public override bool HasDrawCharge(EntityUid uid, PowerCellDrawComponent? battery = null, PowerCellSlotComponent? cell = null, EntityUid? user = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<PowerCellDrawComponent, PowerCellSlotComponent>(uid, ref battery, ref cell, false))
		{
			return true;
		}
		return battery.CanDraw;
	}

	private void OnPowerCellVisualsChange(EntityUid uid, PowerCellVisualsComponent component, ref AppearanceChangeEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		byte b = default(byte);
		if (args.Sprite != null && _sprite.LayerExists(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)PowerCellVisualLayers.Unshaded) && _appearance.TryGetData<byte>(uid, (Enum)PowerCellVisuals.ChargeLevel, ref b, args.Component))
		{
			bool flag = b > 0;
			_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)PowerCellVisualLayers.Unshaded, flag);
			if (flag)
			{
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)PowerCellVisualLayers.Unshaded, StateId.op_Implicit($"o{b}"));
			}
		}
	}
}
