using System;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Bed.Components;
using Content.Shared.Bed.Sleep;
using Content.Shared.Body.Events;
using Content.Shared.Body.Systems;
using Content.Shared.Buckle.Components;
using Content.Shared.Emag.Systems;
using Content.Shared.Power;
using Content.Shared.Power.Components;
using Content.Shared.Power.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared.Bed;

public abstract class SharedBedSystem : EntitySystem
{
	[Dependency]
	protected IGameTiming Timing;

	[Dependency]
	private ActionContainerSystem _actConts;

	[Dependency]
	private SharedActionsSystem _actionsSystem;

	[Dependency]
	private EmagSystem _emag;

	[Dependency]
	private SharedMetabolizerSystem _metabolizer;

	[Dependency]
	private SharedPowerReceiverSystem _powerReceiver;

	[Dependency]
	private SleepingSystem _sleepingSystem;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<HealOnBuckleComponent, MapInitEvent>((EntityEventRefHandler<HealOnBuckleComponent, MapInitEvent>)OnHealMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HealOnBuckleComponent, StrappedEvent>((EntityEventRefHandler<HealOnBuckleComponent, StrappedEvent>)OnStrapped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HealOnBuckleComponent, UnstrappedEvent>((EntityEventRefHandler<HealOnBuckleComponent, UnstrappedEvent>)OnUnstrapped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StasisBedComponent, StrappedEvent>((EntityEventRefHandler<StasisBedComponent, StrappedEvent>)OnStasisStrapped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StasisBedComponent, UnstrappedEvent>((EntityEventRefHandler<StasisBedComponent, UnstrappedEvent>)OnStasisUnstrapped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StasisBedComponent, GotEmaggedEvent>((EntityEventRefHandler<StasisBedComponent, GotEmaggedEvent>)OnStasisEmagged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StasisBedComponent, PowerChangedEvent>((EntityEventRefHandler<StasisBedComponent, PowerChangedEvent>)OnPowerChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StasisBedBuckledComponent, GetMetabolicMultiplierEvent>((EntityEventRefHandler<StasisBedBuckledComponent, GetMetabolicMultiplierEvent>)OnStasisGetMetabolicMultiplier, (Type[])null, (Type[])null);
	}

	private void OnHealMapInit(Entity<HealOnBuckleComponent> ent, ref MapInitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		_actConts.EnsureAction(ent.Owner, ref ent.Comp.SleepAction, EntProtoId.op_Implicit(SleepingSystem.SleepActionId));
		((EntitySystem)this).Dirty<HealOnBuckleComponent>(ent, (MetaDataComponent)null);
	}

	private void OnStrapped(Entity<HealOnBuckleComponent> bed, ref StrappedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).EnsureComp<HealOnBuckleHealingComponent>(Entity<HealOnBuckleComponent>.op_Implicit(bed));
		bed.Comp.NextHealTime = Timing.CurTime + TimeSpan.FromSeconds(bed.Comp.HealTime);
		_actionsSystem.AddAction(Entity<BuckleComponent>.op_Implicit(args.Buckle), ref bed.Comp.SleepAction, EntProtoId.op_Implicit(SleepingSystem.SleepActionId), Entity<HealOnBuckleComponent>.op_Implicit(bed));
		((EntitySystem)this).Dirty<HealOnBuckleComponent>(bed, (MetaDataComponent)null);
	}

	private void OnUnstrapped(Entity<HealOnBuckleComponent> bed, ref UnstrappedEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		SharedActionsSystem actionsSystem = _actionsSystem;
		Entity<ActionsComponent> performer = Entity<ActionsComponent>.op_Implicit(args.Buckle.Owner);
		EntityUid? sleepAction = bed.Comp.SleepAction;
		actionsSystem.RemoveAction(performer, sleepAction.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(sleepAction.GetValueOrDefault())) : ((Entity<ActionComponent>?)null));
		_sleepingSystem.TryWaking(Entity<SleepingComponent>.op_Implicit(args.Buckle.Owner));
		((EntitySystem)this).RemComp<HealOnBuckleHealingComponent>(Entity<HealOnBuckleComponent>.op_Implicit(bed));
	}

	private void OnStasisStrapped(Entity<StasisBedComponent> ent, ref StrappedEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).EnsureComp<StasisBedBuckledComponent>(Entity<BuckleComponent>.op_Implicit(args.Buckle));
		_metabolizer.UpdateMetabolicMultiplier(Entity<BuckleComponent>.op_Implicit(args.Buckle));
	}

	private void OnStasisUnstrapped(Entity<StasisBedComponent> ent, ref UnstrappedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RemComp<StasisBedBuckledComponent>(Entity<StasisBedComponent>.op_Implicit(ent));
		_metabolizer.UpdateMetabolicMultiplier(Entity<BuckleComponent>.op_Implicit(args.Buckle));
	}

	private void OnStasisEmagged(Entity<StasisBedComponent> ent, ref GotEmaggedEvent args)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		if (_emag.CompareFlag(args.Type, EmagType.Interaction) && !_emag.CheckFlag(Entity<StasisBedComponent>.op_Implicit(ent), EmagType.Interaction))
		{
			ent.Comp.Multiplier = 1f / ent.Comp.Multiplier;
			UpdateMetabolisms(Entity<StrapComponent>.op_Implicit(ent.Owner));
			((EntitySystem)this).Dirty<StasisBedComponent>(ent, (MetaDataComponent)null);
			args.Handled = true;
		}
	}

	private void OnPowerChanged(Entity<StasisBedComponent> ent, ref PowerChangedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		UpdateMetabolisms(Entity<StrapComponent>.op_Implicit(ent.Owner));
	}

	private void OnStasisGetMetabolicMultiplier(Entity<StasisBedBuckledComponent> ent, ref GetMetabolicMultiplierEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		BuckleComponent buckle = default(BuckleComponent);
		if (!((EntitySystem)this).TryComp<BuckleComponent>(Entity<StasisBedBuckledComponent>.op_Implicit(ent), ref buckle))
		{
			return;
		}
		EntityUid? buckledTo = buckle.BuckledTo;
		if (buckledTo.HasValue)
		{
			EntityUid buckledTo2 = buckledTo.GetValueOrDefault();
			StasisBedComponent stasis = default(StasisBedComponent);
			if (((EntitySystem)this).TryComp<StasisBedComponent>(buckledTo2, ref stasis) && _powerReceiver.IsPowered(Entity<SharedApcPowerReceiverComponent>.op_Implicit(buckledTo2)))
			{
				args.Multiplier *= stasis.Multiplier;
			}
		}
	}

	protected void UpdateMetabolisms(Entity<StrapComponent?> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<StrapComponent>(Entity<StrapComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			return;
		}
		foreach (EntityUid buckledEntity in ent.Comp.BuckledEntities)
		{
			_metabolizer.UpdateMetabolicMultiplier(buckledEntity);
		}
	}
}
