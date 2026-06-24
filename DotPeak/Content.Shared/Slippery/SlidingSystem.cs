// Decompiled with JetBrains decompiler
// Type: Content.Shared.Slippery.SlidingSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Standing;
using Robust.Shared.GameObjects;
using Robust.Shared.Physics.Events;

#nullable enable
namespace Content.Shared.Slippery;

public sealed class SlidingSystem : EntitySystem
{
  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<SlidingComponent, StoodEvent>(new ComponentEventRefHandler<SlidingComponent, StoodEvent>(this.OnStand));
    this.SubscribeLocalEvent<SlidingComponent, StartCollideEvent>(new ComponentEventRefHandler<SlidingComponent, StartCollideEvent>(this.OnStartCollide));
    this.SubscribeLocalEvent<SlidingComponent, EndCollideEvent>(new ComponentEventRefHandler<SlidingComponent, EndCollideEvent>(this.OnEndCollide));
  }

  private void OnStand(EntityUid uid, SlidingComponent component, ref StoodEvent args)
  {
    this.RemComp<SlidingComponent>(uid);
  }

  private void OnStartCollide(
    EntityUid uid,
    SlidingComponent component,
    ref StartCollideEvent args)
  {
    SlipperyComponent comp;
    if (!this.TryComp<SlipperyComponent>(args.OtherEntity, out comp) || !comp.SlipData.SuperSlippery)
      return;
    component.CollidingEntities.Add(args.OtherEntity);
    this.Dirty(uid, (IComponent) component);
  }

  private void OnEndCollide(EntityUid uid, SlidingComponent component, ref EndCollideEvent args)
  {
    if (!component.CollidingEntities.Remove(args.OtherEntity))
      return;
    if (component.CollidingEntities.Count == 0)
      this.RemComp<SlidingComponent>(uid);
    this.Dirty(uid, (IComponent) component);
  }
}
