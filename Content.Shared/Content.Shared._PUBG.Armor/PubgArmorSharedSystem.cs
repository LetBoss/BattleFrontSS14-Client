using System;
using Content.Shared.Examine;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Shared._PUBG.Armor;

public sealed class PubgArmorSharedSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<PubgArmorComponent, ExaminedEvent>((EntityEventRefHandler<PubgArmorComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
	}

	private void OnExamined(Entity<PubgArmorComponent> ent, ref ExaminedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		float protectionPercent = MathF.Round(ent.Comp.Protection * 100f);
		args.PushMarkup(base.Loc.GetString("pubg-armor-protection", (ValueTuple<string, object>)("value", protectionPercent)));
		if (!(ent.Comp.MaxDurability <= 0f))
		{
			float durabilityRatio = PubgArmorHelpers.GetDurabilityRatio(ent.Comp);
			float percent = MathF.Round(durabilityRatio * 100f);
			Color durabilityColor = PubgArmorHelpers.GetDurabilityColor(durabilityRatio);
			string color = ((Color)(ref durabilityColor)).ToHexNoAlpha();
			args.PushMarkup($"[color={color}]{base.Loc.GetString("pubg-armor-durability", (ValueTuple<string, object>)("value", percent))}[/color]");
		}
	}
}
