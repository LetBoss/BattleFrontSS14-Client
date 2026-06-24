// Decompiled with JetBrains decompiler
// Type: Content.Shared.Nutrition.AfterFullyEatenEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable
namespace Content.Shared.Nutrition;

[ByRefEvent]
public readonly record struct AfterFullyEatenEvent(EntityUid User)
{
  public readonly EntityUid User = User;

  [CompilerGenerated]
  public override int GetHashCode() => EqualityComparer<EntityUid>.Default.GetHashCode(this.User);

  [CompilerGenerated]
  public bool Equals(AfterFullyEatenEvent other)
  {
    return EqualityComparer<EntityUid>.Default.Equals(this.User, other.User);
  }

  [CompilerGenerated]
  public void Deconstruct(out EntityUid User) => User = this.User;
}
