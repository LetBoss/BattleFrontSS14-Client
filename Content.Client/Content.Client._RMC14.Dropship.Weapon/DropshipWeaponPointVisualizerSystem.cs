using System;
using Content.Shared._RMC14.Dropship.AttachmentPoint;
using Content.Shared._RMC14.Dropship.Weapon;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.Utility;

namespace Content.Client._RMC14.Dropship.Weapon;

public sealed class DropshipWeaponPointVisualizerSystem : VisualizerSystem<DropshipWeaponPointComponent>
{
	protected override void OnAppearanceChange(EntityUid uid, DropshipWeaponPointComponent component, ref AppearanceChangeEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Expected O, but got Unknown
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		base.OnAppearanceChange(uid, component, ref args);
		SpriteComponent sprite = args.Sprite;
		string text = default(string);
		string text2 = default(string);
		int num = default(int);
		if (sprite == null || !((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<string>(uid, (Enum)DropshipWeaponVisuals.Sprite, ref text, args.Component) || !((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<string>(uid, (Enum)DropshipWeaponVisuals.State, ref text2, args.Component) || !base.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)DropshipWeaponPointLayers.Layer, ref num, false))
		{
			return;
		}
		if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(text2))
		{
			base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, false);
			return;
		}
		base.SpriteSystem.LayerSetSprite(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, (SpriteSpecifier)new Rsi(new ResPath(text), text2));
		if (Enum.TryParse<DirectionOffset>(component.DirOffset, true, out DirectionOffset result))
		{
			base.SpriteSystem.LayerSetDirOffset(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, result);
		}
		base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, true);
	}
}
