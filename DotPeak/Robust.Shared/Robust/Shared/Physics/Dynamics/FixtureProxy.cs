// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Dynamics.FixtureProxy
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;

#nullable enable
namespace Robust.Shared.Physics.Dynamics;

public sealed class FixtureProxy
{
  public EntityUid Entity;
  [Robust.Shared.ViewVariables.ViewVariables]
  public Box2 AABB;
  [Robust.Shared.ViewVariables.ViewVariables]
  public int ChildIndex;
  public string FixtureId;
  public Fixture Fixture;
  [Robust.Shared.ViewVariables.ViewVariables]
  public DynamicTree.Proxy ProxyId = DynamicTree.Proxy.Free;

  public PhysicsComponent Body { get; internal set; }

  public TransformComponent Xform { get; internal set; }

  internal FixtureProxy(
    EntityUid uid,
    PhysicsComponent body,
    TransformComponent xform,
    Box2 aabb,
    string fixtureId,
    Fixture fixture,
    int childIndex)
  {
    this.Entity = uid;
    this.Body = body;
    this.Xform = xform;
    this.AABB = aabb;
    this.FixtureId = fixtureId;
    this.Fixture = fixture;
    this.ChildIndex = childIndex;
  }
}
