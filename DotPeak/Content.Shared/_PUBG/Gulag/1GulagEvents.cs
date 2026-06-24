// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Gulag.GulagFightStartEvent
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
public sealed class GulagFightStartEvent : EntityEventArgs
{
  public string OpponentUsername { get; }

  public string OpponentRank { get; }

  public GulagFightStartEvent(string opponentUsername, string opponentRank)
  {
    this.OpponentUsername = opponentUsername;
    this.OpponentRank = opponentRank;
  }
}
