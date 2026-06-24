// Decompiled with JetBrains decompiler
// Type: Robust.Shared.EntitySerialization.DeserializationOptions
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable
namespace Robust.Shared.EntitySerialization;

public record struct DeserializationOptions
{
  public static readonly DeserializationOptions Default = new DeserializationOptions();
  public bool StoreYamlUids;
  public bool InitializeMaps;
  public bool PauseMaps;
  public bool LogOrphanedGrids;
  public bool LogInvalidEntities;
  public bool AssignMapIds;

  public DeserializationOptions()
  {
    this.StoreYamlUids = false;
    this.InitializeMaps = false;
    this.PauseMaps = false;
    this.LogOrphanedGrids = true;
    this.LogInvalidEntities = true;
    this.AssignMapIds = true;
  }

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return ((((EqualityComparer<bool>.Default.GetHashCode(this.StoreYamlUids) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.InitializeMaps)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.PauseMaps)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.LogOrphanedGrids)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.LogInvalidEntities)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.AssignMapIds);
  }

  [CompilerGenerated]
  public readonly bool Equals(DeserializationOptions other)
  {
    return EqualityComparer<bool>.Default.Equals(this.StoreYamlUids, other.StoreYamlUids) && EqualityComparer<bool>.Default.Equals(this.InitializeMaps, other.InitializeMaps) && EqualityComparer<bool>.Default.Equals(this.PauseMaps, other.PauseMaps) && EqualityComparer<bool>.Default.Equals(this.LogOrphanedGrids, other.LogOrphanedGrids) && EqualityComparer<bool>.Default.Equals(this.LogInvalidEntities, other.LogInvalidEntities) && EqualityComparer<bool>.Default.Equals(this.AssignMapIds, other.AssignMapIds);
  }
}
