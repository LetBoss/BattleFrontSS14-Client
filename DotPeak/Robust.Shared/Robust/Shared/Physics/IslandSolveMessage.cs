// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.IslandSolveMessage
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Physics.Components;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Physics;

internal sealed class IslandSolveMessage : EntityEventArgs
{
  public List<Entity<PhysicsComponent, TransformComponent>> Bodies { get; }

  public IslandSolveMessage(
    List<Entity<PhysicsComponent, TransformComponent>> bodies)
  {
    this.Bodies = bodies;
  }
}
