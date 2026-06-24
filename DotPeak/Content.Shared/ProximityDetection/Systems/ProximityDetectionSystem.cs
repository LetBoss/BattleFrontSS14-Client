// Decompiled with JetBrains decompiler
// Type: Content.Shared.ProximityDetection.Systems.ProximityDetectionSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Item.ItemToggle;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.ProximityDetection.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

#nullable enable
namespace Content.Shared.ProximityDetection.Systems;

public sealed class ProximityDetectionSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private ItemToggleSystem _toggle;
  private Robust.Shared.GameObjects.EntityQuery<TransformComponent> _xformQuery;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<ProximityDetectorComponent, MapInitEvent>(new EntityEventRefHandler<ProximityDetectorComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<ProximityDetectorComponent, ItemToggledEvent>(new EntityEventRefHandler<ProximityDetectorComponent, ItemToggledEvent>(this.OnToggled));
    this._xformQuery = this.GetEntityQuery<TransformComponent>();
  }

  private void OnMapInit(Entity<ProximityDetectorComponent> ent, ref MapInitEvent args)
  {
    ProximityDetectorComponent comp = ent.Comp;
    comp.NextUpdate = this._timing.CurTime + comp.UpdateCooldown;
    this.DirtyField<ProximityDetectorComponent>((EntityUid) ent, comp, "NextUpdate");
  }

  private void OnToggled(Entity<ProximityDetectorComponent> ent, ref ItemToggledEvent args)
  {
    if (args.Activated)
      this.UpdateTarget(ent);
    else
      this.ClearTarget(ent);
  }

  public override void Update(float frameTime)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<ProximityDetectorComponent> entityQueryEnumerator = this.EntityQueryEnumerator<ProximityDetectorComponent>();
    EntityUid uid;
    ProximityDetectorComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (!(comp1.NextUpdate > this._timing.CurTime))
      {
        comp1.NextUpdate += comp1.UpdateCooldown;
        this.DirtyField<ProximityDetectorComponent>(uid, comp1, "NextUpdate");
        if (this._toggle.IsActivated((Entity<ItemToggleComponent>) uid))
          this.UpdateTarget((Entity<ProximityDetectorComponent>) (uid, comp1));
      }
    }
  }

  private void ClearTarget(Entity<ProximityDetectorComponent> ent)
  {
    ProximityDetectorComponent comp = ent.Comp;
    if (!comp.Target.HasValue)
      return;
    comp.Distance = float.PositiveInfinity;
    this.DirtyField<ProximityDetectorComponent>((EntityUid) ent, comp, "Distance");
    comp.Target = new EntityUid?();
    this.DirtyField<ProximityDetectorComponent>((EntityUid) ent, comp, "Target");
    ProximityTargetUpdatedEvent args1 = new ProximityTargetUpdatedEvent(comp.Distance, ent);
    this.RaiseLocalEvent<ProximityTargetUpdatedEvent>((EntityUid) ent, ref args1);
    NewProximityTargetEvent args2 = new NewProximityTargetEvent(comp.Distance, ent);
    this.RaiseLocalEvent<NewProximityTargetEvent>((EntityUid) ent, ref args2);
  }

  private void UpdateTarget(Entity<ProximityDetectorComponent> detector)
  {
    ProximityDetectorComponent comp = detector.Comp;
    TransformComponent component1;
    if (!this._xformQuery.TryGetComponent((EntityUid) detector, out component1))
      return;
    if (this.Deleted(comp.Target))
      this.ClearTarget(detector);
    float Distance = float.PositiveInfinity;
    EntityUid? Target = new EntityUid?();
    CompRegistryEntityEnumerator entityEnumerator = this.EntityManager.CompRegistryQueryEnumerator(comp.Components);
    EntityUid uid;
    while (entityEnumerator.MoveNext(out uid))
    {
      TransformComponent component2;
      float distance;
      if (this._xformQuery.TryGetComponent(uid, out component2) && component1.Coordinates.TryDistance((IEntityManager) this.EntityManager, component2.Coordinates, out distance) && (double) distance <= (double) comp.Range && (double) distance < (double) Distance)
      {
        ProximityDetectionAttemptEvent args = new ProximityDetectionAttemptEvent(distance, detector, uid);
        this.RaiseLocalEvent<ProximityDetectionAttemptEvent>((EntityUid) detector, ref args);
        if (!args.Cancelled)
        {
          Distance = distance;
          Target = new EntityUid?(uid);
        }
      }
    }
    int num = (double) comp.Distance != (double) Distance ? 1 : 0;
    EntityUid? target = comp.Target;
    EntityUid? nullable = Target;
    bool flag = target.HasValue != nullable.HasValue || target.HasValue && target.GetValueOrDefault() != nullable.GetValueOrDefault();
    if (num != 0)
    {
      ProximityTargetUpdatedEvent args = new ProximityTargetUpdatedEvent(Distance, detector, Target);
      this.RaiseLocalEvent<ProximityTargetUpdatedEvent>((EntityUid) detector, ref args);
      comp.Distance = Distance;
      this.DirtyField<ProximityDetectorComponent>((EntityUid) detector, comp, "Distance");
    }
    if (!flag)
      return;
    NewProximityTargetEvent args1 = new NewProximityTargetEvent(Distance, detector, Target);
    this.RaiseLocalEvent<NewProximityTargetEvent>((EntityUid) detector, ref args1);
    comp.Target = Target;
    this.DirtyField<ProximityDetectorComponent>((EntityUid) detector, comp, "Target");
  }
}
