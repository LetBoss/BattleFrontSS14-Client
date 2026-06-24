using System;
using Content.Shared._RMC14.Xenonids;
using Content.Shared._RMC14.Xenonids.Damage;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;

namespace Content.Client._RMC14.Xenonids.Damage;

public sealed class RMCXenoDamageVisualsSystem : VisualizerSystem<RMCXenoDamageVisualsComponent>
{
	protected override void OnAppearanceChange(EntityUid uid, RMCXenoDamageVisualsComponent component, ref AppearanceChangeEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent sprite = args.Sprite;
		int num = default(int);
		int num2 = default(int);
		if (sprite == null || !((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<int>(uid, (Enum)RMCDamageVisuals.State, ref num, (AppearanceComponent)null) || !base.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)RMCDamageVisualLayers.Base, ref num2, false))
		{
			return;
		}
		if (num == 0)
		{
			base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num2, false);
			return;
		}
		base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num2, true);
		int value = component.States - num + 1;
		bool flag = default(bool);
		bool flag2 = default(bool);
		bool flag3 = default(bool);
		if (((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(uid, (Enum)RMCXenoStateVisuals.Downed, ref flag, (AppearanceComponent)null) && flag)
		{
			base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num2, StateId.op_Implicit($"{component.Prefix}_downed_{value}"));
		}
		else if (((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(uid, (Enum)RMCXenoStateVisuals.Fortified, ref flag2, (AppearanceComponent)null) && flag2)
		{
			base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num2, StateId.op_Implicit($"{component.Prefix}_fortify_{value}"));
		}
		else if (((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(uid, (Enum)RMCXenoStateVisuals.Resting, ref flag3, (AppearanceComponent)null) && flag3)
		{
			base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num2, StateId.op_Implicit($"{component.Prefix}_rest_{value}"));
		}
		else
		{
			base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num2, StateId.op_Implicit($"{component.Prefix}_walk_{value}"));
		}
	}
}
