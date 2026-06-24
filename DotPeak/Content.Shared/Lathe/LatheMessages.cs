// Decompiled with JetBrains decompiler
// Type: Content.Shared.Lathe.LatheUpdateState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Research.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Lathe;

[NetSerializable]
[Serializable]
public sealed class LatheUpdateState : BoundUserInterfaceState
{
  public List<ProtoId<LatheRecipePrototype>> Recipes;
  public ProtoId<LatheRecipePrototype>[] Queue;
  public ProtoId<LatheRecipePrototype>? CurrentlyProducing;

  public LatheUpdateState(
    List<ProtoId<LatheRecipePrototype>> recipes,
    ProtoId<LatheRecipePrototype>[] queue,
    ProtoId<LatheRecipePrototype>? currentlyProducing = null)
  {
    this.Recipes = recipes;
    this.Queue = queue;
    this.CurrentlyProducing = currentlyProducing;
  }
}
