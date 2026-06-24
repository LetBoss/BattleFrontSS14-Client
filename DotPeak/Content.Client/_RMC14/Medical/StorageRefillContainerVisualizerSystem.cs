// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Medical.StorageRefillContainerVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Medical.Refill;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Robust.Client.GameObjects;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client._RMC14.Medical;

public sealed class StorageRefillContainerVisualizerSystem : 
  VisualizerSystem<RMCRefillSolutionFromContainerOnStoreComponent>
{
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private SharedSolutionContainerSystem _solution;
  [Dependency]
  private CMRefillableSolutionSystem _refillable;

  protected virtual void OnAppearanceChange(
    EntityUid uid,
    RMCRefillSolutionFromContainerOnStoreComponent component,
    ref AppearanceChangeEvent args)
  {
    SpriteComponent sprite = args.Sprite;
    Color color;
    int num;
    if (sprite == null || !((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<Color>(uid, (Enum) SolutionContainerStoreVisuals.Color, ref color, (AppearanceComponent) null) || !this.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) SolutionContainerStoreVisuals.Base, ref num, false))
      return;
    BaseContainer baseContainer;
    EntityUid? nullable;
    Entity<SolutionComponent>? soln;
    Solution solution;
    if (!this._container.TryGetContainer(uid, component.ContainerId, ref baseContainer, (ContainerManagerComponent) null) || !Extensions.TryFirstOrNull<EntityUid>((IEnumerable<EntityUid>) baseContainer.ContainedEntities, ref nullable) || !this._solution.TryGetDrainableSolution(Entity<DrainableSolutionComponent, SolutionContainerManagerComponent>.op_Implicit(nullable.Value), out soln, out solution) && !this._refillable.TryGetPressurizedSolution(Entity<RMCPressurizedSolutionComponent, SolutionContainerManagerComponent>.op_Implicit(nullable.Value), out soln, out solution) || solution.Volume == 0)
    {
      this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, false);
    }
    else
    {
      this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, true);
      this.SpriteSystem.LayerSetColor(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, ((Color) ref color).WithAlpha(component.LayerOpacity));
    }
  }
}
