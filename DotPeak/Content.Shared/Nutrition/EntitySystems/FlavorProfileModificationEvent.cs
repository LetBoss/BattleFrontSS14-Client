// Decompiled with JetBrains decompiler
// Type: Content.Shared.Nutrition.EntitySystems.FlavorProfileModificationEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Nutrition.EntitySystems;

public sealed class FlavorProfileModificationEvent : EntityEventArgs
{
  public FlavorProfileModificationEvent(EntityUid user, HashSet<string> flavors)
  {
    this.User = user;
    this.Flavors = flavors;
  }

  public EntityUid User { get; }

  public HashSet<string> Flavors { get; }
}
