// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Player.PlayerAttachedEvent
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;

#nullable enable
namespace Robust.Shared.Player;

public sealed class PlayerAttachedEvent : EntityEventArgs
{
  public readonly EntityUid Entity;
  public readonly ICommonSession Player;

  public PlayerAttachedEvent(EntityUid entity, ICommonSession player)
  {
    this.Entity = entity;
    this.Player = player;
  }
}
