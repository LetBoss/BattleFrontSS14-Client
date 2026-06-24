// Decompiled with JetBrains decompiler
// Type: Content.Shared.Prying.Components.BeforePryEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Prying.Components;

[ByRefEvent]
public record struct BeforePryEvent(EntityUid User, bool PryPowered, bool Force, bool StrongPry)
{
  public readonly EntityUid User = User;
  public readonly bool PryPowered = PryPowered;
  public readonly bool Force = Force;
  public readonly bool StrongPry = StrongPry;
  public string? Message = (string) null;
  public bool Cancelled = false;

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return ((((EqualityComparer<EntityUid>.Default.GetHashCode(this.User) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.PryPowered)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.Force)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.StrongPry)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.Message)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.Cancelled);
  }

  [CompilerGenerated]
  public readonly bool Equals(BeforePryEvent other)
  {
    return EqualityComparer<EntityUid>.Default.Equals(this.User, other.User) && EqualityComparer<bool>.Default.Equals(this.PryPowered, other.PryPowered) && EqualityComparer<bool>.Default.Equals(this.Force, other.Force) && EqualityComparer<bool>.Default.Equals(this.StrongPry, other.StrongPry) && EqualityComparer<string>.Default.Equals(this.Message, other.Message) && EqualityComparer<bool>.Default.Equals(this.Cancelled, other.Cancelled);
  }

  [CompilerGenerated]
  public readonly void Deconstruct(
    out EntityUid User,
    out bool PryPowered,
    out bool Force,
    out bool StrongPry)
  {
    User = this.User;
    PryPowered = this.PryPowered;
    Force = this.Force;
    StrongPry = this.StrongPry;
  }
}
