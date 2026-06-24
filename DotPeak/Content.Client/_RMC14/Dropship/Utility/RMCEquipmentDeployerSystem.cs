// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Dropship.Utility.RMCEquipmentDeployerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Dropship.Utility.Components;
using Content.Shared._RMC14.Dropship.Utility.Systems;
using Robust.Client.GameObjects;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client._RMC14.Dropship.Utility;

public sealed class RMCEquipmentDeployerSystem : SharedRMCEquipmentDeployerSystem
{
  [Dependency]
  private SpriteSystem _sprite;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<RMCEquipmentDeployerComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<RMCEquipmentDeployerComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<RMCEquipmentDeployerComponent, AppearanceChangeEvent>(new EntityEventRefHandler<RMCEquipmentDeployerComponent, AppearanceChangeEvent>((object) this, __methodptr(OnAppearanceChange)), (Type[]) null, (Type[]) null);
  }

  private void OnHandleState(
    Entity<RMCEquipmentDeployerComponent> ent,
    ref AfterAutoHandleStateEvent args)
  {
    this.UpdateVisuals(ent);
  }

  private void OnAppearanceChange(
    Entity<RMCEquipmentDeployerComponent> ent,
    ref AppearanceChangeEvent args)
  {
    this.UpdateVisuals(ent);
  }

  private void UpdateVisuals(Entity<RMCEquipmentDeployerComponent> ent)
  {
    EntityUid owner = ent.Owner;
    int num1;
    if (!this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit(owner), (Enum) EquipmentDeployState.UnDeployed, ref num1, false))
      return;
    int num2;
    if (!this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit(owner), (Enum) EquipmentDeployState.Deployed, ref num2, false))
    {
      this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit(owner), num1, true);
    }
    else
    {
      this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit(owner), num1, !ent.Comp.IsDeployed);
      this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit(owner), num2, ent.Comp.IsDeployed);
    }
  }
}
