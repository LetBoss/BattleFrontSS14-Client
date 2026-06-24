// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.GridUidChangedEvent
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

#nullable enable
namespace Robust.Shared.GameObjects;

[ByRefEvent]
public readonly record struct GridUidChangedEvent(
  Robust.Shared.GameObjects.Entity<TransformComponent, MetaDataComponent> Entity,
  EntityUid? OldGrid)
{
  public EntityUid? NewGrid => this.Entity.Comp1.GridUid;

  public EntityUid Uid => this.Entity.Owner;

  public TransformComponent Transform => this.Entity.Comp1;

  public MetaDataComponent Meta => this.Entity.Comp2;
}
