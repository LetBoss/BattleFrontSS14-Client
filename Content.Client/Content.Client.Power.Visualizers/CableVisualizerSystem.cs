using System;
using Content.Client.SubFloor;
using Content.Shared.Wires;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Power.Visualizers;

public sealed class CableVisualizerSystem : EntitySystem
{
	[Dependency]
	private AppearanceSystem _appearanceSystem;

	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<CableVisualizerComponent, AppearanceChangeEvent>((ComponentEventRefHandler<CableVisualizerComponent, AppearanceChangeEvent>)OnAppearanceChange, (Type[])null, new Type[1] { typeof(SubFloorHideSystem) });
	}

	private void OnAppearanceChange(EntityUid uid, CableVisualizerComponent component, ref AppearanceChangeEvent args)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		if (args.Sprite != null && args.Sprite.Visible)
		{
			WireVisDirFlags value = default(WireVisDirFlags);
			if (!((SharedAppearanceSystem)_appearanceSystem).TryGetData<WireVisDirFlags>(uid, (Enum)WireVisVisuals.ConnectedMask, ref value, args.Component))
			{
				value = WireVisDirFlags.None;
			}
			_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), 0, StateId.op_Implicit($"{component.StatePrefix}{(int)value}"));
			if (component.ExtraLayerPrefix != null)
			{
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), 1, StateId.op_Implicit($"{component.ExtraLayerPrefix}{(int)value}"));
			}
		}
	}
}
