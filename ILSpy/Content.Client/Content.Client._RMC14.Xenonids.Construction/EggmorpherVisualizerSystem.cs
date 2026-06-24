using System;
using Content.Shared._RMC14.Xenonids.Construction.EggMorpher;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._RMC14.Xenonids.Construction;

public sealed class EggmorpherVisualizerSystem : VisualizerSystem<EggMorpherComponent>
{
	[Dependency]
	private SpriteSystem _sprite;

	protected override void OnAppearanceChange(EntityUid uid, EggMorpherComponent component, ref AppearanceChangeEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent sprite = args.Sprite;
		int num = default(int);
		int num2 = default(int);
		int num3 = default(int);
		if (sprite == null || !((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<int>(uid, (Enum)EggmorpherOverlayVisuals.Number, ref num, (AppearanceComponent)null) || !_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)EggmorpherOverlayLayers.Overlay, ref num2, false) || !_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)EggmorpherOverlayLayers.Base, ref num3, false))
		{
			return;
		}
		int num4 = (int)Math.Min(Math.Ceiling((double)num / (double)component.MaxParasites * (double)component.OverlayCount), component.OverlayCount);
		if (num4 == 0)
		{
			_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num2, false);
			return;
		}
		bool flag = true;
		if (!sprite[num2].Visible)
		{
			_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num2, true);
			flag = false;
		}
		string text = component.OverlayPrefix + "_" + (num4 - 1);
		if (text != _sprite.LayerGetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num2).Name || !flag)
		{
			_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num2, StateId.op_Implicit(text));
			StateId val = _sprite.LayerGetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num3);
			_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num3, StateId.op_Implicit(text));
			_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num3, val);
		}
	}
}
