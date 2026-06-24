// Decompiled with JetBrains decompiler
// Type: Content.Shared.Item.ItemToggle.Components.ItemToggleDeactivateAttemptEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Item.ItemToggle.Components;

[ByRefEvent]
public record struct ItemToggleDeactivateAttemptEvent(EntityUid? User)
{
  public bool Silent = false;
  public bool Cancelled = false;
  public readonly EntityUid? User = User;
  public string? Popup = (string) null;

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return ((EqualityComparer<bool>.Default.GetHashCode(this.Silent) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.Cancelled)) * -1521134295 + EqualityComparer<EntityUid?>.Default.GetHashCode(this.User)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.Popup);
  }

  [CompilerGenerated]
  public readonly bool Equals(ItemToggleDeactivateAttemptEvent other)
  {
    return EqualityComparer<bool>.Default.Equals(this.Silent, other.Silent) && EqualityComparer<bool>.Default.Equals(this.Cancelled, other.Cancelled) && EqualityComparer<EntityUid?>.Default.Equals(this.User, other.User) && EqualityComparer<string>.Default.Equals(this.Popup, other.Popup);
  }

  [CompilerGenerated]
  public readonly void Deconstruct(out EntityUid? User) => User = this.User;
}
