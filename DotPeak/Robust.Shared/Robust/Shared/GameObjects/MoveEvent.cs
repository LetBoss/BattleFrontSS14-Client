// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.MoveEvent
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Map;
using Robust.Shared.Maths;

#nullable enable
namespace Robust.Shared.GameObjects;

[ByRefEvent]
public readonly struct MoveEvent(
  Robust.Shared.GameObjects.Entity<TransformComponent, MetaDataComponent> entity,
  EntityCoordinates oldPos,
  EntityCoordinates newPos,
  Angle oldRotation,
  Angle newRotation)
{
  public readonly Robust.Shared.GameObjects.Entity<TransformComponent, MetaDataComponent> Entity = entity;
  public readonly EntityCoordinates OldPosition = oldPos;
  public readonly EntityCoordinates NewPosition = newPos;
  public readonly Angle OldRotation = oldRotation;
  public readonly Angle NewRotation = newRotation;

  public EntityUid Sender => this.Entity.Owner;

  public TransformComponent Component => this.Entity.Comp1;

  public bool ParentChanged => this.NewPosition.EntityId != this.OldPosition.EntityId;
}
