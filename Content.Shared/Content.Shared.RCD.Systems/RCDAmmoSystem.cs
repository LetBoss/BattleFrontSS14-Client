using System;
using Content.Shared.Charges.Components;
using Content.Shared.Charges.Systems;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.RCD.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Shared.RCD.Systems;

public sealed class RCDAmmoSystem : EntitySystem
{
	[Dependency]
	private SharedChargesSystem _sharedCharges;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private IGameTiming _timing;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RCDAmmoComponent, ExaminedEvent>((ComponentEventHandler<RCDAmmoComponent, ExaminedEvent>)OnExamine, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RCDAmmoComponent, AfterInteractEvent>((ComponentEventHandler<RCDAmmoComponent, AfterInteractEvent>)OnAfterInteract, (Type[])null, (Type[])null);
	}

	private void OnExamine(EntityUid uid, RCDAmmoComponent comp, ExaminedEvent args)
	{
		if (args.IsInDetailsRange)
		{
			string examineMessage = base.Loc.GetString("rcd-ammo-component-on-examine", (ValueTuple<string, object>)("charges", comp.Charges));
			args.PushText(examineMessage);
		}
	}

	private void OnAfterInteract(EntityUid uid, RCDAmmoComponent comp, AfterInteractEvent args)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || !args.CanReach || !_timing.IsFirstTimePredicted)
		{
			return;
		}
		EntityUid? target = args.Target;
		if (!target.HasValue)
		{
			return;
		}
		EntityUid target2 = target.GetValueOrDefault();
		LimitedChargesComponent charges = default(LimitedChargesComponent);
		if (!((EntityUid)(ref target2)).Valid || !((EntitySystem)this).HasComp<RCDComponent>(target2) || !((EntitySystem)this).TryComp<LimitedChargesComponent>(target2, ref charges))
		{
			return;
		}
		int current = _sharedCharges.GetCurrentCharges(Entity<LimitedChargesComponent, AutoRechargeComponent>.op_Implicit((target2, charges)));
		EntityUid user = args.User;
		((HandledEntityEventArgs)args).Handled = true;
		int count = Math.Min(charges.MaxCharges - current, comp.Charges);
		if (count <= 0)
		{
			_popup.PopupClient(base.Loc.GetString("rcd-ammo-component-after-interact-full"), target2, user);
			return;
		}
		_popup.PopupClient(base.Loc.GetString("rcd-ammo-component-after-interact-refilled"), target2, user);
		_sharedCharges.AddCharges(Entity<LimitedChargesComponent, AutoRechargeComponent>.op_Implicit(target2), count);
		comp.Charges -= count;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)comp, (MetaDataComponent)null);
		if (comp.Charges <= 0)
		{
			((EntitySystem)this).QueueDel((EntityUid?)uid);
		}
	}
}
