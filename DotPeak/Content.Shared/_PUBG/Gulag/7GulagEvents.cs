// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Gulag.GulagSpectatorUpdateEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared._PUBG.Gulag;

[NetSerializable]
[Serializable]
public sealed class GulagSpectatorUpdateEvent : EntityEventArgs
{
  public string Fighter1Username { get; }

  public string Fighter1Rank { get; }

  public string Fighter2Username { get; }

  public string Fighter2Rank { get; }

  public float TimeRemaining { get; }

  public int QueueSize { get; }

  public GulagSpectatorUpdateEvent(
    string fighter1Username,
    string fighter1Rank,
    string fighter2Username,
    string fighter2Rank,
    float timeRemaining,
    int queueSize)
  {
    this.Fighter1Username = fighter1Username;
    this.Fighter1Rank = fighter1Rank;
    this.Fighter2Username = fighter2Username;
    this.Fighter2Rank = fighter2Rank;
    this.TimeRemaining = timeRemaining;
    this.QueueSize = queueSize;
  }
}
