// Decompiled with JetBrains decompiler
// Type: Content.Client.Clothing.Systems.PilotedByClothingSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Clothing.Components;
using Robust.Client.Physics;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Clothing.Systems;

public sealed class PilotedByClothingSystem : EntitySystem
{
  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<PilotedByClothingComponent, UpdateIsPredictedEvent>(new EntityEventRefHandler<PilotedByClothingComponent, UpdateIsPredictedEvent>((object) this, __methodptr(OnUpdatePredicted)), (Type[]) null, (Type[]) null);
  }

  private void OnUpdatePredicted(
    Entity<PilotedByClothingComponent> entity,
    ref UpdateIsPredictedEvent args)
  {
    args.BlockPrediction = true;
  }
}
