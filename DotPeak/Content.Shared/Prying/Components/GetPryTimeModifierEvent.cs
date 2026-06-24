// Decompiled with JetBrains decompiler
// Type: Content.Shared.Prying.Components.GetPryTimeModifierEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable
namespace Content.Shared.Prying.Components;

[ByRefEvent]
public record struct GetPryTimeModifierEvent(EntityUid user)
{
  public readonly EntityUid User = user;
  public float PryTimeModifier = 1f;
  public float BaseTime = 5f;

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return (EqualityComparer<EntityUid>.Default.GetHashCode(this.User) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.PryTimeModifier)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.BaseTime);
  }

  [CompilerGenerated]
  public readonly bool Equals(GetPryTimeModifierEvent other)
  {
    return EqualityComparer<EntityUid>.Default.Equals(this.User, other.User) && EqualityComparer<float>.Default.Equals(this.PryTimeModifier, other.PryTimeModifier) && EqualityComparer<float>.Default.Equals(this.BaseTime, other.BaseTime);
  }
}
