// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Skin.SponsorTierInfo
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared._PUBG.Skin;

[NetSerializable]
[Serializable]
public sealed class SponsorTierInfo
{
  public string TierKey { get; set; } = string.Empty;

  public string TierName { get; set; } = string.Empty;

  public string? Badge { get; set; }

  public string? Color { get; set; }

  public int Priority { get; set; }
}
