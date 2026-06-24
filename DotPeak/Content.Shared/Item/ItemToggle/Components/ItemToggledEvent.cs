// Decompiled with JetBrains decompiler
// Type: Content.Shared.Item.ItemToggle.Components.ItemToggledEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable
namespace Content.Shared.Item.ItemToggle.Components;

[ByRefEvent]
public readonly record struct ItemToggledEvent(bool Predicted, bool Activated, EntityUid? User)
{
  public readonly bool Predicted = Predicted;
  public readonly bool Activated = Activated;
  public readonly EntityUid? User = User;

  [CompilerGenerated]
  public override int GetHashCode()
  {
    return (EqualityComparer<bool>.Default.GetHashCode(this.Predicted) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.Activated)) * -1521134295 + EqualityComparer<EntityUid?>.Default.GetHashCode(this.User);
  }

  [CompilerGenerated]
  public bool Equals(ItemToggledEvent other)
  {
    return EqualityComparer<bool>.Default.Equals(this.Predicted, other.Predicted) && EqualityComparer<bool>.Default.Equals(this.Activated, other.Activated) && EqualityComparer<EntityUid?>.Default.Equals(this.User, other.User);
  }

  [CompilerGenerated]
  public void Deconstruct(out bool Predicted, out bool Activated, out EntityUid? User)
  {
    Predicted = this.Predicted;
    Activated = this.Activated;
    User = this.User;
  }
}
