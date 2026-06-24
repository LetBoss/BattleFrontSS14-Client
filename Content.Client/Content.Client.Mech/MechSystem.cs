using System;
using Content.Shared.DrawDepth;
using Content.Shared.Mech;
using Content.Shared.Mech.Components;
using Content.Shared.Mech.EntitySystems;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Mech;

public sealed class MechSystem : SharedMechSystem
{
	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<MechComponent, AppearanceChangeEvent>((ComponentEventRefHandler<MechComponent, AppearanceChangeEvent>)OnAppearanceChanged, (Type[])null, (Type[])null);
	}

	private void OnAppearanceChanged(EntityUid uid, MechComponent component, ref AppearanceChangeEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		if (args.Sprite != null && _sprite.LayerExists(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)MechVisualLayers.Base))
		{
			string text = component.BaseState;
			DrawDepth drawDepth = DrawDepth.Mobs;
			bool flag = default(bool);
			bool flag2 = default(bool);
			if (component.BrokenState != null && _appearance.TryGetData<bool>(uid, (Enum)MechVisuals.Broken, ref flag, args.Component) && flag)
			{
				text = component.BrokenState;
				drawDepth = DrawDepth.SmallMobs;
			}
			else if (component.OpenState != null && _appearance.TryGetData<bool>(uid, (Enum)MechVisuals.Open, ref flag2, args.Component) && flag2)
			{
				text = component.OpenState;
				drawDepth = DrawDepth.SmallMobs;
			}
			_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)MechVisualLayers.Base, StateId.op_Implicit(text));
			_sprite.SetDrawDepth(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (int)drawDepth);
		}
	}
}
