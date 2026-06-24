using System;
using Content.Shared.Actions.Components;
using Content.Shared.Actions.Events;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Timing;

namespace Content.Shared.Actions;

public sealed class ConfirmableActionSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedPopupSystem _popup;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ConfirmableActionComponent, ActionAttemptEvent>((EntityEventRefHandler<ConfirmableActionComponent, ActionAttemptEvent>)OnAttempt, (Type[])null, (Type[])null);
	}

	public override void Update(float frameTime)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		TimeSpan now = _timing.CurTime;
		EntityQueryEnumerator<ConfirmableActionComponent> query = ((EntitySystem)this).EntityQueryEnumerator<ConfirmableActionComponent>();
		EntityUid uid = default(EntityUid);
		ConfirmableActionComponent comp = default(ConfirmableActionComponent);
		while (query.MoveNext(ref uid, ref comp))
		{
			TimeSpan? nextUnprime = comp.NextUnprime;
			if (nextUnprime.HasValue)
			{
				TimeSpan time = nextUnprime.GetValueOrDefault();
				if (now >= time)
				{
					Unprime(Entity<ConfirmableActionComponent>.op_Implicit((uid, comp)));
				}
			}
		}
	}

	private void OnAttempt(Entity<ConfirmableActionComponent> ent, ref ActionAttemptEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled)
		{
			return;
		}
		TimeSpan? nextConfirm = ent.Comp.NextConfirm;
		if (nextConfirm.HasValue)
		{
			TimeSpan confirm = nextConfirm.GetValueOrDefault();
			if (_timing.CurTime < confirm)
			{
				args.Cancelled = true;
			}
			else
			{
				Unprime(ent);
			}
		}
		else
		{
			Prime(ent, args.User);
			args.Cancelled = true;
		}
	}

	private void Prime(Entity<ConfirmableActionComponent> ent, EntityUid user)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		Entity<ConfirmableActionComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		ConfirmableActionComponent confirmableActionComponent = default(ConfirmableActionComponent);
		val.Deconstruct(ref val2, ref confirmableActionComponent);
		EntityUid uid = val2;
		ConfirmableActionComponent comp = confirmableActionComponent;
		comp.NextConfirm = _timing.CurTime + comp.ConfirmDelay;
		comp.NextUnprime = comp.NextConfirm + comp.PrimeTime;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)comp, (MetaDataComponent)null);
		_popup.PopupClient(base.Loc.GetString(LocId.op_Implicit(comp.Popup)), user, user, PopupType.LargeCaution);
	}

	private void Unprime(Entity<ConfirmableActionComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		Entity<ConfirmableActionComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		ConfirmableActionComponent confirmableActionComponent = default(ConfirmableActionComponent);
		val.Deconstruct(ref val2, ref confirmableActionComponent);
		EntityUid uid = val2;
		ConfirmableActionComponent comp = confirmableActionComponent;
		comp.NextConfirm = null;
		comp.NextUnprime = null;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)comp, (MetaDataComponent)null);
	}
}
