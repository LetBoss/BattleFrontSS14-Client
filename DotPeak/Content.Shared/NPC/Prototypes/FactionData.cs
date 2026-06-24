// Decompiled with JetBrains decompiler
// Type: Content.Shared.NPC.Prototypes.FactionData
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.NPC.Prototypes;

[NetSerializable]
[Serializable]
public record struct FactionData
{
  [Robust.Shared.ViewVariables.ViewVariables]
  public HashSet<ProtoId<NpcFactionPrototype>> Friendly;
  [Robust.Shared.ViewVariables.ViewVariables]
  public HashSet<ProtoId<NpcFactionPrototype>> Hostile;

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return EqualityComparer<HashSet<ProtoId<NpcFactionPrototype>>>.Default.GetHashCode(this.Friendly) * -1521134295 + EqualityComparer<HashSet<ProtoId<NpcFactionPrototype>>>.Default.GetHashCode(this.Hostile);
  }

  [CompilerGenerated]
  public readonly bool Equals(FactionData other)
  {
    return EqualityComparer<HashSet<ProtoId<NpcFactionPrototype>>>.Default.Equals(this.Friendly, other.Friendly) && EqualityComparer<HashSet<ProtoId<NpcFactionPrototype>>>.Default.Equals(this.Hostile, other.Hostile);
  }
}
