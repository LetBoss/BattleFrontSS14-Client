// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.CivCommanderCandidateEntry
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Network;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared._CIV14merka;

[NetSerializable]
[Serializable]
public sealed class CivCommanderCandidateEntry
{
  public NetUserId UserId { get; }

  public string Name { get; }

  public int PlaytimeMinutes { get; }

  public bool IsSelf { get; }

  public int Priority { get; }

  public CivCommanderCandidateEntry(
    NetUserId userId,
    string name,
    int playtimeMinutes,
    bool isSelf,
    int priority = 0)
  {
    this.UserId = userId;
    this.Name = name;
    this.PlaytimeMinutes = playtimeMinutes;
    this.IsSelf = isSelf;
    this.Priority = priority;
  }
}
