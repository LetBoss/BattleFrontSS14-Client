// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.CollisionWakeSystem
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;

#nullable enable
namespace Robust.Shared.GameObjects;

public sealed class CollisionWakeSystem : EntitySystem
{
  [Dependency]
  private readonly SharedPhysicsSystem _physics;
  private Robust.Shared.GameObjects.EntityQuery<CollisionWakeComponent> _query;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<CollisionWakeComponent, ComponentShutdown>(new ComponentEventHandler<CollisionWakeComponent, ComponentShutdown>(this.OnRemove));
    this.SubscribeLocalEvent<CollisionWakeComponent, JointAddedEvent>(new ComponentEventHandler<CollisionWakeComponent, JointAddedEvent>(this.OnJointAdd));
    this.SubscribeLocalEvent<CollisionWakeComponent, JointRemovedEvent>(new ComponentEventHandler<CollisionWakeComponent, JointRemovedEvent>(this.OnJointRemove));
    this.SubscribeLocalEvent<CollisionWakeComponent, EntParentChangedMessage>(new ComponentEventRefHandler<CollisionWakeComponent, EntParentChangedMessage>(this.OnParentChange));
    this._query = this.GetEntityQuery<CollisionWakeComponent>();
  }

  public void SetEnabled(EntityUid uid, bool enabled, CollisionWakeComponent? component = null)
  {
    if (!this._query.Resolve(uid, ref component, false) || component.Enabled == enabled)
      return;
    component.Enabled = enabled;
    if (component.Enabled)
    {
      this.UpdateCanCollide(uid, component);
    }
    else
    {
      PhysicsComponent comp;
      if (this.TryComp<PhysicsComponent>(uid, out comp))
        this._physics.SetCanCollide(uid, true, body: comp);
    }
    this.Dirty(uid, (IComponent) component);
  }

  private void OnRemove(EntityUid uid, CollisionWakeComponent component, ComponentShutdown args)
  {
    PhysicsComponent comp;
    if (!component.Enabled || this.Terminating(uid) || !this.TryComp<PhysicsComponent>(uid, out comp))
      return;
    this._physics.SetCanCollide(uid, true, body: comp);
  }

  private void OnParentChange(
    EntityUid uid,
    CollisionWakeComponent component,
    ref EntParentChangedMessage args)
  {
    if (component.LifeStage < ComponentLifeStage.Initialized)
      return;
    this.UpdateCanCollide(uid, component, xform: args.Transform);
  }

  private void OnJointRemove(
    EntityUid uid,
    CollisionWakeComponent component,
    JointRemovedEvent args)
  {
    this.UpdateCanCollide(uid, component, args.OurBody);
  }

  private void OnJointAdd(EntityUid uid, CollisionWakeComponent component, JointAddedEvent args)
  {
    if (!component.Enabled)
      return;
    this._physics.SetCanCollide(uid, true);
  }

  internal void UpdateCanCollide(
    Entity<PhysicsComponent> entity,
    bool checkTerminating = true,
    bool dirty = true)
  {
    CollisionWakeComponent component;
    if (!this._query.TryGetComponent((EntityUid) entity, out component))
      return;
    this.UpdateCanCollide(entity.Owner, component, entity.Comp, checkTerminating: checkTerminating, dirty: dirty);
  }

  internal void UpdateCanCollide(
    EntityUid uid,
    CollisionWakeComponent component,
    PhysicsComponent? body = null,
    TransformComponent? xform = null,
    bool checkTerminating = true,
    bool dirty = true)
  {
    if (!component.Enabled || checkTerminating && this.Terminating(uid) || !this.Resolve<PhysicsComponent>(uid, ref body, false) || !this.Resolve(uid, ref xform) || xform.MapID == MapId.Nullspace)
      return;
    JointComponent comp;
    bool flag = body.Awake || body.ContactCount > 0 || this.TryComp<JointComponent>(uid, out comp) && comp.JointCount > 0 || !xform.GridUid.HasValue;
    this._physics.SetCanCollide(uid, flag, dirty, body: body);
  }
}
