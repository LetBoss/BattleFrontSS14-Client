using System;
using System.Collections.Generic;
using Content.Shared.Damage;
using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Vehicle;

public sealed class RMCVehicleDamageProfileSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<RMCVehicleDamageProfileComponent, BeforeDamageChangedEvent>((EntityEventRefHandler<RMCVehicleDamageProfileComponent, BeforeDamageChangedEvent>)OnBeforeDamageChanged, new Type[1] { typeof(VehicleSystem) }, (Type[])null);
	}

	private void OnBeforeDamageChanged(Entity<RMCVehicleDamageProfileComponent> ent, ref BeforeDamageChangedEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled || ent.Comp.Rules.Count == 0 || !args.Damage.AnyPositive())
		{
			return;
		}
		foreach (string damageType in new List<string>(args.Damage.DamageDict.Keys))
		{
			if (!args.Damage.DamageDict.TryGetValue(damageType, out var value) || value <= 0)
			{
				continue;
			}
			foreach (RMCVehicleDamageScaleRule rule in ent.Comp.Rules)
			{
				if (rule.DamageTypes.Contains(damageType) && !(value > rule.MaxDamage))
				{
					args.Damage.DamageDict[damageType] = value * rule.Multiplier;
					break;
				}
			}
		}
	}
}
