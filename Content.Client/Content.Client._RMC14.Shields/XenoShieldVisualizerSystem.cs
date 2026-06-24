using System;
using Content.Shared._RMC14.Shields;
using Content.Shared.FixedPoint;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;

namespace Content.Client._RMC14.Shields;

public sealed class XenoShieldVisualizerSystem : VisualizerSystem<XenoShieldComponent>
{
	protected override void OnAppearanceChange(EntityUid uid, XenoShieldComponent component, ref AppearanceChangeEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		int num = default(int);
		bool flag = default(bool);
		if (((EntitySystem)this).TryComp<SpriteComponent>(uid, ref item) && base.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, item)), (Enum)RMCShieldVisuals.Base, ref num, true) && ((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(uid, (Enum)RMCShieldVisuals.Active, ref flag, (AppearanceComponent)null))
		{
			string text = default(string);
			FixedPoint2 fixedPoint = default(FixedPoint2);
			double num2 = default(double);
			if (!flag)
			{
				base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, item)), num, false);
			}
			else if (((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<string>(uid, (Enum)RMCShieldVisuals.Prefix, ref text, (AppearanceComponent)null) && ((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<FixedPoint2>(uid, (Enum)RMCShieldVisuals.Current, ref fixedPoint, (AppearanceComponent)null) && ((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<double>(uid, (Enum)RMCShieldVisuals.Max, ref num2, (AppearanceComponent)null))
			{
				FixedPoint2 fixedPoint2 = fixedPoint / num2;
				string text2 = text + "-";
				text2 = ((fixedPoint2 > 0.5) ? (text2 + "full") : ((!(fixedPoint2 > 0.25)) ? (text2 + "quarter") : (text2 + "half")));
				base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, item)), num, StateId.op_Implicit(text2));
				base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, item)), num, true);
			}
		}
	}
}
