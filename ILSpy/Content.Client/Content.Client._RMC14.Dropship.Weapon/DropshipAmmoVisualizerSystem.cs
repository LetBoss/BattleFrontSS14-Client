using System;
using Content.Shared._RMC14.Dropship.Weapon;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;

namespace Content.Client._RMC14.Dropship.Weapon;

public sealed class DropshipAmmoVisualizerSystem : VisualizerSystem<DropshipAmmoComponent>
{
	protected override void OnAppearanceChange(EntityUid uid, DropshipAmmoComponent component, ref AppearanceChangeEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		base.OnAppearanceChange(uid, component, ref args);
		SpriteComponent sprite = args.Sprite;
		int num = default(int);
		int num2 = default(int);
		if (sprite != null && ((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<int>(uid, (Enum)DropshipAmmoVisuals.Fill, ref num, args.Component) && base.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)DropshipAmmoVisuals.Fill, ref num2, false) && component.AmmoType != null)
		{
			int num3 = Math.Clamp(num / component.RoundsPerShot, 0, component.MaxRounds / component.RoundsPerShot);
			string text = component.AmmoType + "_" + num3;
			base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num2, StateId.op_Implicit(text));
		}
	}
}
