// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.TransformStartupEvent
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

#nullable enable
namespace Robust.Shared.GameObjects;

[ByRefEvent]
public readonly struct TransformStartupEvent(Robust.Shared.GameObjects.Entity<TransformComponent> entity)
{
  public readonly Robust.Shared.GameObjects.Entity<TransformComponent> Entity = entity;

  public TransformComponent Component => this.Entity.Comp;
}
