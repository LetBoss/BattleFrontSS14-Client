// Decompiled with JetBrains decompiler
// Type: Content.Shared.Explosion.EntitySystems.SharedScatteringGrenadeSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Explosion.Components;
using Content.Shared.Interaction;
using Content.Shared.Whitelist;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Components;
using System;

#nullable enable
namespace Content.Shared.Explosion.EntitySystems;

public abstract class SharedScatteringGrenadeSystem : EntitySystem
{
  [Dependency]
  private EntityWhitelistSystem _whitelistSystem;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedContainerSystem _container;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<ScatteringGrenadeComponent, ComponentInit>(new EntityEventRefHandler<ScatteringGrenadeComponent, ComponentInit>(this.OnScatteringInit));
    this.SubscribeLocalEvent<ScatteringGrenadeComponent, ComponentStartup>(new EntityEventRefHandler<ScatteringGrenadeComponent, ComponentStartup>(this.OnScatteringStartup));
    this.SubscribeLocalEvent<ScatteringGrenadeComponent, InteractUsingEvent>(new EntityEventRefHandler<ScatteringGrenadeComponent, InteractUsingEvent>(this.OnScatteringInteractUsing));
  }

  private void OnScatteringInit(Entity<ScatteringGrenadeComponent> entity, ref ComponentInit args)
  {
    entity.Comp.Container = this._container.EnsureContainer<Container>(entity.Owner, "cluster-payload");
  }

  private void OnScatteringStartup(
    Entity<ScatteringGrenadeComponent> entity,
    ref ComponentStartup args)
  {
    if (!entity.Comp.FillPrototype.HasValue)
      return;
    entity.Comp.UnspawnedCount = Math.Max(0, entity.Comp.Capacity - entity.Comp.Container.ContainedEntities.Count);
    this.UpdateAppearance(entity);
    this.Dirty((EntityUid) entity, (IComponent) entity.Comp);
  }

  private void OnScatteringInteractUsing(
    Entity<ScatteringGrenadeComponent> entity,
    ref InteractUsingEvent args)
  {
    if (entity.Comp.Whitelist == null || entity.Comp.Count >= entity.Comp.Capacity || args.Handled || !this._whitelistSystem.IsValid(entity.Comp.Whitelist, args.Used))
      return;
    this._container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) args.Used, (BaseContainer) entity.Comp.Container);
    this.UpdateAppearance(entity);
    args.Handled = true;
  }

  private void UpdateAppearance(Entity<ScatteringGrenadeComponent> entity)
  {
    AppearanceComponent comp;
    if (!this.TryComp<AppearanceComponent>((EntityUid) entity, out comp))
      return;
    this._appearance.SetData((EntityUid) entity, (Enum) ClusterGrenadeVisuals.GrenadesCounter, (object) entity.Comp.Count, comp);
  }
}
