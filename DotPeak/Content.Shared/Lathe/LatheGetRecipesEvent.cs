// Decompiled with JetBrains decompiler
// Type: Content.Shared.Lathe.LatheGetRecipesEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Research.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Lathe;

public sealed class LatheGetRecipesEvent : EntityEventArgs
{
  public readonly EntityUid Lathe;
  public readonly LatheComponent Comp;
  public bool GetUnavailable;
  public HashSet<ProtoId<LatheRecipePrototype>> Recipes = new HashSet<ProtoId<LatheRecipePrototype>>();

  public LatheGetRecipesEvent(Entity<LatheComponent> lathe, bool forced)
  {
    (this.Lathe, this.Comp) = lathe;
    this.GetUnavailable = forced;
  }
}
