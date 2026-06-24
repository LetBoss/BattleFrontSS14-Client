// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.CivRoundEndPlayerEntry
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._CIV14merka.Stats;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared._CIV14merka;

[NetSerializable]
[Serializable]
public sealed class CivRoundEndPlayerEntry
{
  public string PlayerName { get; }

  public string RoleText { get; }

  public string SquadText { get; }

  public string StatusText { get; }

  public bool IsCommander { get; }

  public CivPlayerRoundStats? Stats { get; }

  public CivRoundEndPlayerEntry(
    string playerName,
    string roleText,
    string squadText,
    string statusText,
    bool isCommander,
    CivPlayerRoundStats? stats = null)
  {
    this.PlayerName = playerName;
    this.RoleText = roleText;
    this.SquadText = squadText;
    this.StatusText = statusText;
    this.IsCommander = isCommander;
    this.Stats = stats;
  }
}
