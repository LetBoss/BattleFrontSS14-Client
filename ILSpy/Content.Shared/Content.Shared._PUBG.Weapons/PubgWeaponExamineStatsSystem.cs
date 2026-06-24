using System;
using Content.Shared._PUBG.Ammo.Components;
using Content.Shared._PUBG.Vision;
using Content.Shared.Examine;
using Content.Shared.Item;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared._PUBG.Weapons;

public sealed class PubgWeaponExamineStatsSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<PubgWeaponExamineStatsComponent, ExaminedEvent>((EntityEventRefHandler<PubgWeaponExamineStatsComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
	}

	private void OnExamined(Entity<PubgWeaponExamineStatsComponent> ent, ref ExaminedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		GunComponent gun = default(GunComponent);
		if (!((EntitySystem)this).TryComp<GunComponent>(Entity<PubgWeaponExamineStatsComponent>.op_Implicit(ent), ref gun))
		{
			return;
		}
		using (args.PushGroup("PubgWeaponExamineStatsComponent"))
		{
			args.PushMarkup(base.Loc.GetString("pubg-weapon-stats-header"));
			args.PushMarkup(base.Loc.GetString("pubg-weapon-stats-fire-rate", (ValueTuple<string, object>)("value", MathF.Round(gun.FireRateModified, 1))));
			float minSpread = MathF.Round((float)(gun.MinAngleModified.Theta * 180.0 / Math.PI), 1);
			float maxSpread = MathF.Round((float)(gun.MaxAngleModified.Theta * 180.0 / Math.PI), 1);
			args.PushMarkup(base.Loc.GetString("pubg-weapon-stats-spread", (ValueTuple<string, object>)("min", minSpread), (ValueTuple<string, object>)("max", maxSpread)));
			PubgAmmoProviderComponent ammoProvider = default(PubgAmmoProviderComponent);
			if (((EntitySystem)this).TryComp<PubgAmmoProviderComponent>(Entity<PubgWeaponExamineStatsComponent>.op_Implicit(ent), ref ammoProvider))
			{
				args.PushMarkup(base.Loc.GetString("pubg-weapon-stats-reload-time", (ValueTuple<string, object>)("value", MathF.Round(ammoProvider.ReloadTime, 2))));
			}
			PubgFocusViewComponent focus = default(PubgFocusViewComponent);
			if (((EntitySystem)this).TryComp<PubgFocusViewComponent>(Entity<PubgWeaponExamineStatsComponent>.op_Implicit(ent), ref focus))
			{
				args.PushMarkup(base.Loc.GetString("pubg-weapon-stats-focus", (ValueTuple<string, object>)("value", MathF.Round(focus.OffsetTiles, 1))));
			}
			HeldSpeedModifierComponent held = default(HeldSpeedModifierComponent);
			if (((EntitySystem)this).TryComp<HeldSpeedModifierComponent>(Entity<PubgWeaponExamineStatsComponent>.op_Implicit(ent), ref held))
			{
				float slowdown = MathF.Round((1f - held.SprintModifier) * 100f, 1);
				if (slowdown > 0f)
				{
					args.PushMarkup(base.Loc.GetString("pubg-weapon-stats-held-slowdown", (ValueTuple<string, object>)("value", slowdown)));
				}
			}
		}
	}
}
