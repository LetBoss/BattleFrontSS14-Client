// Decompiled with JetBrains decompiler
// Type: Content.Shared.Salvage.Magnet.SalvageOffering
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Salvage.Magnet;

public record struct SalvageOffering : ISalvageMagnetOffering
{
  public SalvageMapPrototype SalvageMap;

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return EqualityComparer<SalvageMapPrototype>.Default.GetHashCode(this.SalvageMap);
  }

  [CompilerGenerated]
  public readonly bool Equals(SalvageOffering other)
  {
    return EqualityComparer<SalvageMapPrototype>.Default.Equals(this.SalvageMap, other.SalvageMap);
  }
}
