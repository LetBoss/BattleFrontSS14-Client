using System;
using Content.Shared.Inventory;
using Content.Shared.Mobs;
using Content.Shared.Popups;
using Content.Shared.StatusEffect;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Deafness;

public abstract class SharedDeafnessSystem : EntitySystem
{
	[Dependency]
	private StatusEffectsSystem _statusEffect;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private InventorySystem _inventory;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private INetManager _net;

	public ProtoId<StatusEffectPrototype> DeafKey = ProtoId<StatusEffectPrototype>.op_Implicit("Deaf");

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<DeafenWhileCritComponent, StatusEffectEndedEvent>((EntityEventRefHandler<DeafenWhileCritComponent, StatusEffectEndedEvent>)OnCanHear, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DeafenWhileCritComponent, MobStateChangedEvent>((EntityEventRefHandler<DeafenWhileCritComponent, MobStateChangedEvent>)OnDeafenWhileCritMobState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActiveDeafenWhileCritComponent, MobStateChangedEvent>((EntityEventRefHandler<ActiveDeafenWhileCritComponent, MobStateChangedEvent>)OnActiveDeafenWhileCritMobState, (Type[])null, (Type[])null);
	}

	private void OnCanHear(Entity<DeafenWhileCritComponent> ent, ref StatusEffectEndedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (!(ProtoId<StatusEffectPrototype>.op_Implicit(args.Key) != DeafKey))
		{
			DoEarLossPopups(ent.Owner, end: true);
		}
	}

	private void OnDeafenWhileCritMobState(Entity<DeafenWhileCritComponent> ent, ref MobStateChangedEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		if (args.NewMobState == MobState.Critical)
		{
			((EntitySystem)this).EnsureComp<ActiveDeafenWhileCritComponent>(Entity<DeafenWhileCritComponent>.op_Implicit(ent));
		}
	}

	private void OnActiveDeafenWhileCritMobState(Entity<ActiveDeafenWhileCritComponent> ent, ref MobStateChangedEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if (args.NewMobState != MobState.Critical)
		{
			((EntitySystem)this).RemCompDeferred<ActiveDeafenWhileCritComponent>(Entity<ActiveDeafenWhileCritComponent>.op_Implicit(ent));
		}
	}

	public bool TryDeafen(EntityUid uid, TimeSpan time, bool refresh = false, StatusEffectsComponent? status = null, bool ignoreProtection = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<StatusEffectsComponent>(uid, ref status, false))
		{
			return false;
		}
		if (time <= TimeSpan.Zero)
		{
			return false;
		}
		if (!ignoreProtection && HasEarProtection(uid))
		{
			return false;
		}
		if (!((EntitySystem)this).HasComp<DeafComponent>(uid))
		{
			DoEarLossPopups(uid, end: false);
		}
		if (!_statusEffect.TryAddStatusEffect<DeafComponent>(uid, ProtoId<StatusEffectPrototype>.op_Implicit(DeafKey), time, refresh, (StatusEffectsComponent?)null, false))
		{
			return false;
		}
		RMCDeafenedEvent ev = new RMCDeafenedEvent(time);
		((EntitySystem)this).RaiseLocalEvent<RMCDeafenedEvent>(uid, ref ev, false);
		return true;
	}

	public void DoEarLossPopups(EntityUid uid, bool end)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient)
		{
			string msg = base.Loc.GetString(end ? "rmc-deaf-end" : "rmc-deaf-start");
			_popup.PopupEntity(msg, uid, uid, PopupType.MediumCaution);
		}
	}

	public bool HasEarProtection(EntityUid uid)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (_inventory.TryGetContainerSlotEnumerator(Entity<InventoryComponent>.op_Implicit(uid), out var slots))
		{
			EntityUid containedEntity;
			SlotDefinition slot;
			while (slots.NextItem(out containedEntity, out slot))
			{
				if (((EntitySystem)this).HasComp<RMCEarProtectionComponent>(containedEntity))
				{
					return true;
				}
			}
		}
		return ((EntitySystem)this).HasComp<RMCEarProtectionComponent>(uid);
	}

	public override void Update(float frameTime)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<ActiveDeafenWhileCritComponent, StatusEffectsComponent> activeQuery = ((EntitySystem)this).EntityQueryEnumerator<ActiveDeafenWhileCritComponent, StatusEffectsComponent>();
		EntityUid uid = default(EntityUid);
		ActiveDeafenWhileCritComponent comp = default(ActiveDeafenWhileCritComponent);
		StatusEffectsComponent status = default(StatusEffectsComponent);
		while (activeQuery.MoveNext(ref uid, ref comp, ref status))
		{
			if (comp.AddAt < time)
			{
				comp.AddAt = time + comp.Every;
				TryDeafen(uid, comp.Add, refresh: true, status, ignoreProtection: true);
			}
		}
	}
}
