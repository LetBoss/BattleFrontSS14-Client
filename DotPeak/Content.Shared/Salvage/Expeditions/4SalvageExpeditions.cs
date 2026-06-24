// Decompiled with JetBrains decompiler
// Type: Content.Shared.Salvage.Expeditions.SalvageMissionParams
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Salvage.Expeditions;

[NetSerializable]
[Serializable]
public sealed record SalvageMissionParams
{
  [Robust.Shared.ViewVariables.ViewVariables]
  public ushort Index;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public int Seed;
  public string Difficulty = string.Empty;

  [CompilerGenerated]
  public override int GetHashCode()
  {
    return ((EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<ushort>.Default.GetHashCode(this.Index)) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(this.Seed)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.Difficulty);
  }

  [CompilerGenerated]
  public bool Equals(SalvageMissionParams? other)
  {
    if ((object) this == (object) other)
      return true;
    return (object) other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<ushort>.Default.Equals(this.Index, other.Index) && EqualityComparer<int>.Default.Equals(this.Seed, other.Seed) && EqualityComparer<string>.Default.Equals(this.Difficulty, other.Difficulty);
  }
}
