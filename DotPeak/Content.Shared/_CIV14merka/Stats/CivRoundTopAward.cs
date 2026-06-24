// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Stats.CivRoundTopAward
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Network;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared._CIV14merka.Stats;

[NetSerializable]
[Serializable]
public sealed class CivRoundTopAward
{
  public string AwardId { get; set; } = string.Empty;

  public string Title { get; set; } = string.Empty;

  public NetUserId UserId { get; set; }

  public string PlayerName { get; set; } = string.Empty;

  public string ValueText { get; set; } = string.Empty;
}
