// Decompiled with JetBrains decompiler
// Type: Content.Shared.Materials.GetMaterialWhitelistEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Materials;

[ByRefEvent]
public record struct GetMaterialWhitelistEvent(EntityUid Storage)
{
  public readonly EntityUid Storage = Storage;
  public List<ProtoId<MaterialPrototype>> Whitelist = new List<ProtoId<MaterialPrototype>>();

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return EqualityComparer<EntityUid>.Default.GetHashCode(this.Storage) * -1521134295 + EqualityComparer<List<ProtoId<MaterialPrototype>>>.Default.GetHashCode(this.Whitelist);
  }

  [CompilerGenerated]
  public readonly bool Equals(GetMaterialWhitelistEvent other)
  {
    return EqualityComparer<EntityUid>.Default.Equals(this.Storage, other.Storage) && EqualityComparer<List<ProtoId<MaterialPrototype>>>.Default.Equals(this.Whitelist, other.Whitelist);
  }

  [CompilerGenerated]
  public readonly void Deconstruct(out EntityUid Storage) => Storage = this.Storage;
}
