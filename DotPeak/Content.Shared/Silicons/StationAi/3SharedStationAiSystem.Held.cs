// Decompiled with JetBrains decompiler
// Type: Content.Shared.Silicons.StationAi.GetStationAiRadialEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Silicons.StationAi;

[ByRefEvent]
public record struct GetStationAiRadialEvent
{
  public List<StationAiRadial> Actions;

  public GetStationAiRadialEvent() => this.Actions = new List<StationAiRadial>();

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return EqualityComparer<List<StationAiRadial>>.Default.GetHashCode(this.Actions);
  }

  [CompilerGenerated]
  public readonly bool Equals(GetStationAiRadialEvent other)
  {
    return EqualityComparer<List<StationAiRadial>>.Default.Equals(this.Actions, other.Actions);
  }
}
