// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Events.CollisionChangeEvent
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Physics.Components;

#nullable enable
namespace Robust.Shared.Physics.Events;

[ByRefEvent]
public readonly struct CollisionChangeEvent(
  EntityUid bodyUid,
  PhysicsComponent body,
  bool canCollide)
{
  public readonly EntityUid BodyUid = bodyUid;
  public readonly PhysicsComponent Body = body;
  public readonly bool CanCollide = canCollide;
}
