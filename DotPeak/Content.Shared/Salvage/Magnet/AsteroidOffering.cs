// Decompiled with JetBrains decompiler
// Type: Content.Shared.Salvage.Magnet.AsteroidOffering
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Procedural;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Salvage.Magnet;

public record struct AsteroidOffering : ISalvageMagnetOffering
{
  public string Id;
  public DungeonConfig DungeonConfig;
  public Dictionary<string, int> MarkerLayers;

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return (EqualityComparer<string>.Default.GetHashCode(this.Id) * -1521134295 + EqualityComparer<DungeonConfig>.Default.GetHashCode(this.DungeonConfig)) * -1521134295 + EqualityComparer<Dictionary<string, int>>.Default.GetHashCode(this.MarkerLayers);
  }

  [CompilerGenerated]
  public readonly bool Equals(AsteroidOffering other)
  {
    return EqualityComparer<string>.Default.Equals(this.Id, other.Id) && EqualityComparer<DungeonConfig>.Default.Equals(this.DungeonConfig, other.DungeonConfig) && EqualityComparer<Dictionary<string, int>>.Default.Equals(this.MarkerLayers, other.MarkerLayers);
  }
}
