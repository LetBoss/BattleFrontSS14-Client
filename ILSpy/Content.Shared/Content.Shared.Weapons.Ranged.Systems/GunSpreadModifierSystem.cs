using System;
using Content.Shared.Examine;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Shared.Weapons.Ranged.Systems;

public sealed class GunSpreadModifierSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<GunSpreadModifierComponent, GunGetAmmoSpreadEvent>((ComponentEventRefHandler<GunSpreadModifierComponent, GunGetAmmoSpreadEvent>)OnGunGetAmmoSpread, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunSpreadModifierComponent, ExaminedEvent>((ComponentEventHandler<GunSpreadModifierComponent, ExaminedEvent>)OnExamine, (Type[])null, (Type[])null);
	}

	private void OnGunGetAmmoSpread(EntityUid uid, GunSpreadModifierComponent comp, ref GunGetAmmoSpreadEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		args.Spread = Angle.op_Implicit(Angle.op_Implicit(args.Spread) * (double)comp.Spread);
	}

	private void OnExamine(EntityUid uid, GunSpreadModifierComponent comp, ExaminedEvent args)
	{
		double percentage = Math.Round(comp.Spread * 100f);
		string loc = ((percentage < 100.0) ? "examine-gun-spread-modifier-reduction" : "examine-gun-spread-modifier-increase");
		percentage = ((percentage < 100.0) ? (100.0 - percentage) : (percentage - 100.0));
		string msg = base.Loc.GetString(loc, (ValueTuple<string, object>)("percentage", percentage));
		args.PushMarkup(msg);
	}
}
