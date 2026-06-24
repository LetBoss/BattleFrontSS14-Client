// Decompiled with JetBrains decompiler
// Type: Content.Shared.Ninja.Components.NinjaGloveAbility
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Objectives.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Ninja.Components;

[DataRecord]
public record struct NinjaGloveAbility
{
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId<ObjectiveComponent>? Objective;
  [DataField(null, false, 1, true, false, null)]
  public ComponentRegistry Components;

  public NinjaGloveAbility()
  {
    this.Objective = new EntProtoId<ObjectiveComponent>?();
    this.Components = new ComponentRegistry();
  }

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return EqualityComparer<EntProtoId<ObjectiveComponent>?>.Default.GetHashCode(this.Objective) * -1521134295 + EqualityComparer<ComponentRegistry>.Default.GetHashCode(this.Components);
  }

  [CompilerGenerated]
  public readonly bool Equals(NinjaGloveAbility other)
  {
    return EqualityComparer<EntProtoId<ObjectiveComponent>?>.Default.Equals(this.Objective, other.Objective) && EqualityComparer<ComponentRegistry>.Default.Equals(this.Components, other.Components);
  }
}
