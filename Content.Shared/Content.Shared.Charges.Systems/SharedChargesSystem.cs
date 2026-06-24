using System;
using Content.Shared.Actions.Events;
using Content.Shared.Charges.Components;
using Content.Shared.Examine;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Shared.Charges.Systems;

public abstract class SharedChargesSystem : EntitySystem
{
	[Dependency]
	protected IGameTiming _timing;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<LimitedChargesComponent, ExaminedEvent>((ComponentEventHandler<LimitedChargesComponent, ExaminedEvent>)OnExamine, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LimitedChargesComponent, ActionAttemptEvent>((EntityEventRefHandler<LimitedChargesComponent, ActionAttemptEvent>)OnChargesAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LimitedChargesComponent, MapInitEvent>((EntityEventRefHandler<LimitedChargesComponent, MapInitEvent>)OnChargesMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LimitedChargesComponent, ActionPerformedEvent>((EntityEventRefHandler<LimitedChargesComponent, ActionPerformedEvent>)OnChargesPerformed, (Type[])null, (Type[])null);
	}

	private void OnExamine(EntityUid uid, LimitedChargesComponent comp, ExaminedEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		if (!args.IsInDetailsRange)
		{
			return;
		}
		Entity<LimitedChargesComponent, AutoRechargeComponent> rechargeEnt = default(Entity<LimitedChargesComponent, AutoRechargeComponent>);
		rechargeEnt._002Ector(uid, comp, (AutoRechargeComponent)null);
		int charges = GetCurrentCharges(rechargeEnt);
		using (args.PushGroup("LimitedChargesComponent"))
		{
			args.PushMarkup(base.Loc.GetString("limited-charges-charges-remaining", (ValueTuple<string, object>)("charges", charges)));
			if (charges == comp.MaxCharges)
			{
				args.PushMarkup(base.Loc.GetString("limited-charges-max-charges"));
			}
			if (charges != comp.MaxCharges && ((EntitySystem)this).Resolve<AutoRechargeComponent>(uid, ref rechargeEnt.Comp2, false))
			{
				TimeSpan timeRemaining = GetNextRechargeTime(rechargeEnt);
				args.PushMarkup(base.Loc.GetString("limited-charges-recharging", (ValueTuple<string, object>)("seconds", timeRemaining.TotalSeconds.ToString("F1"))));
			}
		}
	}

	private void OnChargesAttempt(Entity<LimitedChargesComponent> ent, ref ActionAttemptEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && GetCurrentCharges(Entity<LimitedChargesComponent, AutoRechargeComponent>.op_Implicit((ValueTuple<EntityUid, LimitedChargesComponent, AutoRechargeComponent>)(ent.Owner, ent.Comp, null))) <= 0)
		{
			args.Cancelled = true;
		}
	}

	private void OnChargesPerformed(Entity<LimitedChargesComponent> ent, ref ActionPerformedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		AddCharges(Entity<LimitedChargesComponent, AutoRechargeComponent>.op_Implicit((ent.Owner, ent.Comp)), -1);
	}

	private void OnChargesMapInit(Entity<LimitedChargesComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.LastCharges == 0)
		{
			ent.Comp.LastCharges = ent.Comp.MaxCharges;
		}
		else if (ent.Comp.LastCharges < 0)
		{
			ent.Comp.LastCharges = 0;
		}
		ent.Comp.LastUpdate = _timing.CurTime;
		((EntitySystem)this).Dirty<LimitedChargesComponent>(ent, (MetaDataComponent)null);
	}

	public bool HasCharges(Entity<LimitedChargesComponent?> action, int charges)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return GetCurrentCharges(Entity<LimitedChargesComponent, AutoRechargeComponent>.op_Implicit(action)) >= charges;
	}

	public void AddCharges(Entity<LimitedChargesComponent?, AutoRechargeComponent?> action, int addCharges)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		if (addCharges == 0)
		{
			return;
		}
		ref LimitedChargesComponent comp = ref action.Comp1;
		if (comp == null)
		{
			comp = ((EntitySystem)this).EnsureComp<LimitedChargesComponent>(action.Owner);
		}
		int lastCharges = GetCurrentCharges(action);
		int charges = lastCharges + addCharges;
		if (lastCharges != charges)
		{
			if (charges == action.Comp1.MaxCharges || lastCharges == action.Comp1.MaxCharges)
			{
				action.Comp1.LastUpdate = _timing.CurTime;
				action.Comp1.LastCharges = action.Comp1.MaxCharges;
			}
			else if (((EntitySystem)this).Resolve<AutoRechargeComponent>(action.Owner, ref action.Comp2, false))
			{
				TimeSpan duration = action.Comp2.RechargeDuration;
				int remainder = (int)((_timing.CurTime - action.Comp1.LastUpdate) / duration);
				action.Comp1.LastCharges += remainder;
				action.Comp1.LastUpdate += remainder * duration;
			}
			action.Comp1.LastCharges = Math.Clamp(action.Comp1.LastCharges + addCharges, 0, action.Comp1.MaxCharges);
			((EntitySystem)this).Dirty(action.Owner, (IComponent)(object)action.Comp1, (MetaDataComponent)null);
		}
	}

	public bool TryUseCharge(Entity<LimitedChargesComponent?> entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return TryUseCharges(entity, 1);
	}

	public bool TryUseCharges(Entity<LimitedChargesComponent?> entity, int amount)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (GetCurrentCharges(Entity<LimitedChargesComponent, AutoRechargeComponent>.op_Implicit(entity)) < amount)
		{
			return false;
		}
		AddCharges(Entity<LimitedChargesComponent, AutoRechargeComponent>.op_Implicit(entity), -amount);
		return true;
	}

	public bool IsEmpty(Entity<LimitedChargesComponent?> entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return GetCurrentCharges(Entity<LimitedChargesComponent, AutoRechargeComponent>.op_Implicit(entity)) == 0;
	}

	public void ResetCharges(Entity<LimitedChargesComponent?> action)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<LimitedChargesComponent>(action.Owner, ref action.Comp, false) && GetCurrentCharges(Entity<LimitedChargesComponent, AutoRechargeComponent>.op_Implicit((ValueTuple<EntityUid, LimitedChargesComponent, AutoRechargeComponent>)(action.Owner, action.Comp, null))) != action.Comp.MaxCharges)
		{
			action.Comp.LastCharges = action.Comp.MaxCharges;
			action.Comp.LastUpdate = _timing.CurTime;
			((EntitySystem)this).Dirty<LimitedChargesComponent>(action, (MetaDataComponent)null);
		}
	}

	public void SetCharges(Entity<LimitedChargesComponent?> action, int value)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		ref LimitedChargesComponent comp = ref action.Comp;
		if (comp == null)
		{
			comp = ((EntitySystem)this).EnsureComp<LimitedChargesComponent>(action.Owner);
		}
		int adjusted = Math.Clamp(value, 0, action.Comp.MaxCharges);
		if (action.Comp.LastCharges != adjusted)
		{
			action.Comp.LastCharges = adjusted;
			action.Comp.LastUpdate = _timing.CurTime;
			((EntitySystem)this).Dirty<LimitedChargesComponent>(action, (MetaDataComponent)null);
		}
	}

	public TimeSpan GetNextRechargeTime(Entity<LimitedChargesComponent?, AutoRechargeComponent?> entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<LimitedChargesComponent, AutoRechargeComponent>(entity.Owner, ref entity.Comp1, ref entity.Comp2, false))
		{
			return TimeSpan.Zero;
		}
		TimeSpan timeRemaining = (entity.Comp1.MaxCharges - entity.Comp1.LastCharges) * entity.Comp2.RechargeDuration + entity.Comp1.LastUpdate - _timing.CurTime;
		if (timeRemaining < TimeSpan.Zero)
		{
			return TimeSpan.Zero;
		}
		return TimeSpan.FromSeconds(timeRemaining.TotalSeconds % entity.Comp2.RechargeDuration.TotalSeconds);
	}

	public int GetCurrentCharges(Entity<LimitedChargesComponent?, AutoRechargeComponent?> entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<LimitedChargesComponent>(entity.Owner, ref entity.Comp1, false))
		{
			return -1;
		}
		int calculated = 0;
		if (((EntitySystem)this).Resolve<AutoRechargeComponent>(entity.Owner, ref entity.Comp2, false) && entity.Comp2.RechargeDuration.TotalSeconds != 0.0)
		{
			calculated = (int)((_timing.CurTime - entity.Comp1.LastUpdate).TotalSeconds / entity.Comp2.RechargeDuration.TotalSeconds);
		}
		return Math.Clamp(entity.Comp1.LastCharges + calculated, 0, entity.Comp1.MaxCharges);
	}
}
