// Decompiled with JetBrains decompiler
// Type: Content.Shared.Forensics.GenerateDnaEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Forensics;

[ByRefEvent]
public record struct GenerateDnaEvent
{
  public EntityUid Owner;
  public required string DNA;

  public GenerateDnaEvent()
  {
    this.Owner = new EntityUid();
    this.DNA = (string) null;
  }

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return EqualityComparer<EntityUid>.Default.GetHashCode(this.Owner) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.DNA);
  }

  [CompilerGenerated]
  public readonly bool Equals(GenerateDnaEvent other)
  {
    return EqualityComparer<EntityUid>.Default.Equals(this.Owner, other.Owner) && EqualityComparer<string>.Default.Equals(this.DNA, other.DNA);
  }
}
