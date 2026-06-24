// Decompiled with JetBrains decompiler
// Type: Content.Shared.Movement.Events.GetFootstepSoundEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Movement.Events;

[ByRefEvent]
public record struct GetFootstepSoundEvent(EntityUid User)
{
  public readonly EntityUid User = User;
  public SoundSpecifier? Sound = (SoundSpecifier) null;

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return EqualityComparer<EntityUid>.Default.GetHashCode(this.User) * -1521134295 + EqualityComparer<SoundSpecifier>.Default.GetHashCode(this.Sound);
  }

  [CompilerGenerated]
  public readonly bool Equals(GetFootstepSoundEvent other)
  {
    return EqualityComparer<EntityUid>.Default.Equals(this.User, other.User) && EqualityComparer<SoundSpecifier>.Default.Equals(this.Sound, other.Sound);
  }

  [CompilerGenerated]
  public readonly void Deconstruct(out EntityUid User) => User = this.User;
}
