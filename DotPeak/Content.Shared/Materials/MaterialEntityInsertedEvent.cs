// Decompiled with JetBrains decompiler
// Type: Content.Shared.Materials.MaterialEntityInsertedEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Materials;

[ByRefEvent]
public readonly record struct MaterialEntityInsertedEvent(MaterialComponent MaterialComp)
{
  public readonly MaterialComponent MaterialComp = MaterialComp;

  [CompilerGenerated]
  public override int GetHashCode()
  {
    return EqualityComparer<MaterialComponent>.Default.GetHashCode(this.MaterialComp);
  }

  [CompilerGenerated]
  public bool Equals(MaterialEntityInsertedEvent other)
  {
    return EqualityComparer<MaterialComponent>.Default.Equals(this.MaterialComp, other.MaterialComp);
  }

  [CompilerGenerated]
  public void Deconstruct(out MaterialComponent MaterialComp) => MaterialComp = this.MaterialComp;
}
