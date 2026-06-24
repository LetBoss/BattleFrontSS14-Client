// Decompiled with JetBrains decompiler
// Type: Content.Shared.Salvage.Magnet.DebrisOffering
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Salvage.Magnet;

public record struct DebrisOffering : ISalvageMagnetOffering
{
  public string Id;

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return EqualityComparer<string>.Default.GetHashCode(this.Id);
  }

  [CompilerGenerated]
  public readonly bool Equals(DebrisOffering other)
  {
    return EqualityComparer<string>.Default.Equals(this.Id, other.Id);
  }
}
