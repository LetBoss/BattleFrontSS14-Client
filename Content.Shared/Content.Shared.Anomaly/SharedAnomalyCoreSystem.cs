using System;
using Content.Shared.Anomaly.Components;
using Content.Shared.Construction.Components;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Examine;
using Content.Shared.Weapons.Melee.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Components;
using Robust.Shared.Timing;

namespace Content.Shared.Anomaly;

public sealed class SharedAnomalyCoreSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _gameTiming;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private ItemSlotsSystem _itemSlots;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<AnomalyCoreComponent, MapInitEvent>((EntityEventRefHandler<AnomalyCoreComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CorePoweredThrowerComponent, AttemptMeleeThrowOnHitEvent>((EntityEventRefHandler<CorePoweredThrowerComponent, AttemptMeleeThrowOnHitEvent>)OnAttemptMeleeThrowOnHit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CorePoweredThrowerComponent, ExaminedEvent>((EntityEventRefHandler<CorePoweredThrowerComponent, ExaminedEvent>)OnCorePoweredExamined, (Type[])null, (Type[])null);
	}

	private void OnMapInit(Entity<AnomalyCoreComponent> core, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		core.Comp.DecayMoment = _gameTiming.CurTime + TimeSpan.FromSeconds(core.Comp.TimeToDecay);
		((EntitySystem)this).Dirty(Entity<AnomalyCoreComponent>.op_Implicit(core), (IComponent)(object)core.Comp, (MetaDataComponent)null);
	}

	private void OnAttemptMeleeThrowOnHit(Entity<CorePoweredThrowerComponent> ent, ref AttemptMeleeThrowOnHitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Invalid comparison between Unknown and I4
		Entity<CorePoweredThrowerComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		CorePoweredThrowerComponent corePoweredThrowerComponent = default(CorePoweredThrowerComponent);
		val.Deconstruct(ref val2, ref corePoweredThrowerComponent);
		EntityUid uid = val2;
		CorePoweredThrowerComponent comp = corePoweredThrowerComponent;
		PhysicsComponent body = default(PhysicsComponent);
		if (!((EntitySystem)this).HasComp<AnomalyComponent>(args.Target) && !((EntitySystem)this).HasComp<AnchorableComponent>(args.Target) && ((EntitySystem)this).TryComp<PhysicsComponent>(args.Target, ref body) && (int)body.BodyType == 4)
		{
			return;
		}
		args.Cancelled = true;
		args.Handled = true;
		AnomalyCoreComponent coreComponent = default(AnomalyCoreComponent);
		if (!_itemSlots.TryGetSlot(uid, comp.CoreSlotId, out ItemSlot slot) || !((EntitySystem)this).TryComp<AnomalyCoreComponent>(slot.Item, ref coreComponent))
		{
			return;
		}
		if (coreComponent.IsDecayed)
		{
			if (coreComponent.Charge > 0)
			{
				args.Cancelled = false;
				coreComponent.Charge--;
			}
		}
		else
		{
			args.Cancelled = false;
		}
	}

	private void OnCorePoweredExamined(Entity<CorePoweredThrowerComponent> ent, ref ExaminedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		Entity<CorePoweredThrowerComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		CorePoweredThrowerComponent corePoweredThrowerComponent = default(CorePoweredThrowerComponent);
		val.Deconstruct(ref val2, ref corePoweredThrowerComponent);
		EntityUid uid = val2;
		CorePoweredThrowerComponent comp = corePoweredThrowerComponent;
		if (args.IsInDetailsRange)
		{
			AnomalyCoreComponent coreComponent = default(AnomalyCoreComponent);
			if (!_itemSlots.TryGetSlot(uid, comp.CoreSlotId, out ItemSlot slot) || !((EntitySystem)this).TryComp<AnomalyCoreComponent>(slot.Item, ref coreComponent))
			{
				args.PushMarkup(base.Loc.GetString("anomaly-gorilla-charge-none"));
			}
			else if (coreComponent.IsDecayed)
			{
				args.PushMarkup(base.Loc.GetString("anomaly-gorilla-charge-limit", (ValueTuple<string, object>)("count", coreComponent.Charge)));
			}
			else
			{
				args.PushMarkup(base.Loc.GetString("anomaly-gorilla-charge-infinite"));
			}
		}
	}

	public override void Update(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		EntityQueryEnumerator<AnomalyCoreComponent> query = ((EntitySystem)this).EntityQueryEnumerator<AnomalyCoreComponent>();
		EntityUid uid = default(EntityUid);
		AnomalyCoreComponent component = default(AnomalyCoreComponent);
		while (query.MoveNext(ref uid, ref component))
		{
			if (!component.IsDecayed && component.DecayMoment < _gameTiming.CurTime)
			{
				Decay(uid, component);
			}
		}
	}

	private void Decay(EntityUid uid, AnomalyCoreComponent component)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		_appearance.SetData(uid, (Enum)AnomalyCoreVisuals.Decaying, (object)false, (AppearanceComponent)null);
		component.IsDecayed = true;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
	}
}
