// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Calendar.PubgCalendarDayInfo
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared._PUBG.Calendar;

[NetSerializable]
[Serializable]
public sealed class PubgCalendarDayInfo
{
  public int Day { get; set; }

  public string RewardType { get; set; } = string.Empty;

  public string RewardValue { get; set; } = string.Empty;

  public int? DurationDays { get; set; }
}
