// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.CollideOnAnchorSystem
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.IoC;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;

#nullable enable
namespace Robust.Shared.GameObjects;

public sealed class CollideOnAnchorSystem : EntitySystem
{
  [Dependency]
  private SharedPhysicsSystem _physics;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<CollideOnAnchorComponent, ComponentStartup>(new ComponentEventHandler<CollideOnAnchorComponent, ComponentStartup>(this.OnStartup));
    this.SubscribeLocalEvent<CollideOnAnchorComponent, AnchorStateChangedEvent>(new ComponentEventRefHandler<CollideOnAnchorComponent, AnchorStateChangedEvent>(this.OnAnchor));
  }

  private void OnStartup(EntityUid uid, CollideOnAnchorComponent component, ComponentStartup args)
  {
    TransformComponent comp;
    if (!this.TryComp(uid, out comp))
      return;
    this.SetCollide(uid, component, comp.Anchored);
  }

  private void OnAnchor(
    EntityUid uid,
    CollideOnAnchorComponent component,
    ref AnchorStateChangedEvent args)
  {
    if (args.Detaching)
      return;
    this.SetCollide(uid, component, args.Anchored);
  }

  private void SetCollide(EntityUid uid, CollideOnAnchorComponent component, bool anchored)
  {
    PhysicsComponent comp;
    if (!this.TryComp<PhysicsComponent>(uid, out comp))
      return;
    bool flag = component.Enable;
    if (!anchored)
      flag = !flag;
    this._physics.SetCanCollide(uid, flag, body: comp);
  }
}
