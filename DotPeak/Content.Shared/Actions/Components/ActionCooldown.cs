// Decompiled with JetBrains decompiler
// Type: Content.Shared.Actions.Components.ActionCooldown
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable
namespace Content.Shared.Actions.Components;

[DataRecord]
[NetSerializable]
[Serializable]
public record struct ActionCooldown
{
  [DataField(null, false, 1, true, false, typeof (TimeOffsetSerializer))]
  public TimeSpan Start;
  [DataField(null, false, 1, true, false, typeof (TimeOffsetSerializer))]
  public TimeSpan End;

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return EqualityComparer<TimeSpan>.Default.GetHashCode(this.Start) * -1521134295 + EqualityComparer<TimeSpan>.Default.GetHashCode(this.End);
  }

  [CompilerGenerated]
  public readonly bool Equals(ActionCooldown other)
  {
    return EqualityComparer<TimeSpan>.Default.Equals(this.Start, other.Start) && EqualityComparer<TimeSpan>.Default.Equals(this.End, other.End);
  }
}
