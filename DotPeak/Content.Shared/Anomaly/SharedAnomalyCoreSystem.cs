// Decompiled with JetBrains decompiler
// Type: Content.Shared.Anomaly.SharedAnomalyCoreSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Anomaly.Components;
using Content.Shared.Construction.Components;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Examine;
using Content.Shared.Weapons.Melee.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Components;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared.Anomaly;

public sealed class SharedAnomalyCoreSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _gameTiming;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private ItemSlotsSystem _itemSlots;

  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AnomalyCoreComponent, MapInitEvent>(new EntityEventRefHandler<AnomalyCoreComponent, MapInitEvent>((object) this, __methodptr(OnMapInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CorePoweredThrowerComponent, AttemptMeleeThrowOnHitEvent>(new EntityEventRefHandler<CorePoweredThrowerComponent, AttemptMeleeThrowOnHitEvent>((object) this, __methodptr(OnAttemptMeleeThrowOnHit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CorePoweredThrowerComponent, ExaminedEvent>(new EntityEventRefHandler<CorePoweredThrowerComponent, ExaminedEvent>((object) this, __methodptr(OnCorePoweredExamined)), (Type[]) null, (Type[]) null);
  }

  private void OnMapInit(Entity<AnomalyCoreComponent> core, ref MapInitEvent args)
  {
    core.Comp.DecayMoment = this._gameTiming.CurTime + TimeSpan.FromSeconds(core.Comp.TimeToDecay);
    this.Dirty(Entity<AnomalyCoreComponent>.op_Implicit(core), (IComponent) core.Comp, (MetaDataComponent) null);
  }

  private void OnAttemptMeleeThrowOnHit(
    Entity<CorePoweredThrowerComponent> ent,
    ref AttemptMeleeThrowOnHitEvent args)
  {
    EntityUid entityUid;
    CorePoweredThrowerComponent throwerComponent1;
    ent.Deconstruct(ref entityUid, ref throwerComponent1);
    EntityUid uid = entityUid;
    CorePoweredThrowerComponent throwerComponent2 = throwerComponent1;
    PhysicsComponent physicsComponent;
    if (!this.HasComp<AnomalyComponent>(args.Target) && !this.HasComp<AnchorableComponent>(args.Target) && this.TryComp<PhysicsComponent>(args.Target, ref physicsComponent) && physicsComponent.BodyType == 4)
      return;
    args.Cancelled = true;
    args.Handled = true;
    ItemSlot itemSlot;
    AnomalyCoreComponent anomalyCoreComponent;
    if (!this._itemSlots.TryGetSlot(uid, throwerComponent2.CoreSlotId, out itemSlot) || !this.TryComp<AnomalyCoreComponent>(itemSlot.Item, ref anomalyCoreComponent))
      return;
    if (anomalyCoreComponent.IsDecayed)
    {
      if (anomalyCoreComponent.Charge <= 0)
        return;
      args.Cancelled = false;
      --anomalyCoreComponent.Charge;
    }
    else
      args.Cancelled = false;
  }

  private void OnCorePoweredExamined(
    Entity<CorePoweredThrowerComponent> ent,
    ref ExaminedEvent args)
  {
    EntityUid entityUid;
    CorePoweredThrowerComponent throwerComponent1;
    ent.Deconstruct(ref entityUid, ref throwerComponent1);
    EntityUid uid = entityUid;
    CorePoweredThrowerComponent throwerComponent2 = throwerComponent1;
    if (!args.IsInDetailsRange)
      return;
    ItemSlot itemSlot;
    AnomalyCoreComponent anomalyCoreComponent;
    if (!this._itemSlots.TryGetSlot(uid, throwerComponent2.CoreSlotId, out itemSlot) || !this.TryComp<AnomalyCoreComponent>(itemSlot.Item, ref anomalyCoreComponent))
      args.PushMarkup(this.Loc.GetString("anomaly-gorilla-charge-none"));
    else if (anomalyCoreComponent.IsDecayed)
      args.PushMarkup(this.Loc.GetString("anomaly-gorilla-charge-limit", ("count", (object) anomalyCoreComponent.Charge)));
    else
      args.PushMarkup(this.Loc.GetString("anomaly-gorilla-charge-infinite"));
  }

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    EntityQueryEnumerator<AnomalyCoreComponent> entityQueryEnumerator = this.EntityQueryEnumerator<AnomalyCoreComponent>();
    EntityUid uid;
    AnomalyCoreComponent component;
    while (entityQueryEnumerator.MoveNext(ref uid, ref component))
    {
      if (!component.IsDecayed && component.DecayMoment < this._gameTiming.CurTime)
        this.Decay(uid, component);
    }
  }

  private void Decay(EntityUid uid, AnomalyCoreComponent component)
  {
    this._appearance.SetData(uid, (Enum) AnomalyCoreVisuals.Decaying, (object) false, (AppearanceComponent) null);
    component.IsDecayed = true;
    this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
  }
}
