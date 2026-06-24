// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Pointing.RMCIgnorePointingPointerHideVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Pointing;
using Content.Shared.Ghost;
using Content.Shared.Mobs.Components;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using System;

#nullable enable
namespace Content.Client._RMC14.Pointing;

public sealed class RMCIgnorePointingPointerHideVisualizerSystem : 
  VisualizerSystem<RMCPointingArrowComponent>
{
  [Dependency]
  private IPlayerManager _player;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<RMCPointingArrowComponent, ComponentStartup>(new EntityEventRefHandler<RMCPointingArrowComponent, ComponentStartup>((object) this, __methodptr(OnPointSpawn)), (Type[]) null, (Type[]) null);
  }

  private void OnPointSpawn(Entity<RMCPointingArrowComponent> arrow, ref ComponentStartup args)
  {
    SpriteComponent spriteComponent;
    if (!((EntitySystem) this).TryComp<SpriteComponent>(Entity<RMCPointingArrowComponent>.op_Implicit(arrow), ref spriteComponent))
      return;
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    RMCIgnorePointingComponent pointingComponent;
    if (!((EntitySystem) this).TryComp<RMCIgnorePointingComponent>(localEntity, ref pointingComponent) || !arrow.Comp.Source.HasValue)
      return;
    EntityUid? entity = ((EntitySystem) this).GetEntity(arrow.Comp.Source);
    EntityUid? nullable1 = localEntity;
    EntityUid? nullable2 = entity;
    if ((nullable1.HasValue == nullable2.HasValue ? (nullable1.HasValue ? (EntityUid.op_Equality(nullable1.GetValueOrDefault(), nullable2.GetValueOrDefault()) ? 1 : 0) : 1) : 0) != 0 || (!pointingComponent.IgnoreMobs || !((EntitySystem) this).HasComp<MobStateComponent>(entity)) && (!pointingComponent.IgnoreGhosts || !((EntitySystem) this).HasComp<GhostComponent>(entity)))
      return;
    this.SpriteSystem.SetVisible(Entity<SpriteComponent>.op_Implicit((arrow.Owner, spriteComponent)), false);
  }
}
