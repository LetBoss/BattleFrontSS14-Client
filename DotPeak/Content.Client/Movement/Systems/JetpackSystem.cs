// Decompiled with JetBrains decompiler
// Type: Content.Client.Movement.Systems.JetpackSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Clothing.Components;
using Content.Shared.Clothing.EntitySystems;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Robust.Client.GameObjects;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Physics.Components;
using Robust.Shared.Timing;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.Movement.Systems;

public sealed class JetpackSystem : SharedJetpackSystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private ClothingSystem _clothing;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedMapSystem _mapSystem;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<JetpackComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<JetpackComponent, AppearanceChangeEvent>((object) this, __methodptr(OnJetpackAppearance)), (Type[]) null, (Type[]) null);
  }

  protected override bool CanEnable(EntityUid uid, JetpackComponent component) => false;

  private void OnJetpackAppearance(
    EntityUid uid,
    JetpackComponent component,
    ref AppearanceChangeEvent args)
  {
    bool flag;
    this.Appearance.TryGetData<bool>(uid, (Enum) JetpackVisuals.Enabled, ref flag, args.Component);
    ClothingComponent clothing;
    if (!this.TryComp<ClothingComponent>(uid, ref clothing))
      return;
    this._clothing.SetEquippedPrefix(uid, flag ? "on" : (string) null, clothing);
  }

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    if (!this._timing.IsFirstTimePredicted)
      return;
    EntityQueryEnumerator<ActiveJetpackComponent, TransformComponent> entityQueryEnumerator = this.EntityQueryEnumerator<ActiveJetpackComponent, TransformComponent>();
    EntityUid uid;
    ActiveJetpackComponent jetpackComponent;
    TransformComponent transformComponent;
    while (entityQueryEnumerator.MoveNext(ref uid, ref jetpackComponent, ref transformComponent))
    {
      if (!this._transform.InRange(transformComponent.Coordinates, jetpackComponent.LastCoordinates, jetpackComponent.MaxDistance) || !(this._timing.CurTime < jetpackComponent.TargetTime))
      {
        jetpackComponent.LastCoordinates = this._transform.GetMoverCoordinates(transformComponent.Coordinates);
        jetpackComponent.TargetTime = this._timing.CurTime + TimeSpan.FromSeconds((double) jetpackComponent.EffectCooldown);
        this.CreateParticles(uid);
      }
    }
  }

  private void CreateParticles(EntityUid uid)
  {
    TransformComponent transformComponent = this.Transform(uid);
    BaseContainer baseContainer;
    PhysicsComponent physicsComponent;
    if (this.Container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((uid, transformComponent, (MetaDataComponent) null)), ref baseContainer) && this.TryComp<PhysicsComponent>(baseContainer.Owner, ref physicsComponent) && (double) physicsComponent.LinearVelocity.LengthSquared() < 1.0)
      return;
    EntityCoordinates coordinates = transformComponent.Coordinates;
    EntityUid? grid = this._transform.GetGrid(coordinates);
    MapGridComponent mapGridComponent;
    if (this.TryComp<MapGridComponent>(grid, ref mapGridComponent))
    {
      // ISSUE: explicit constructor call
      ((EntityCoordinates) ref coordinates).\u002Ector(grid.Value, this._mapSystem.WorldToLocal(grid.Value, mapGridComponent, this._transform.ToMapCoordinates(coordinates, true).Position));
    }
    else
    {
      EntityUid? mapUid = transformComponent.MapUid;
      if (!mapUid.HasValue)
        return;
      ref EntityCoordinates local = ref coordinates;
      mapUid = transformComponent.MapUid;
      EntityUid entityUid = mapUid.Value;
      Vector2 worldPosition = this._transform.GetWorldPosition(transformComponent);
      // ISSUE: explicit constructor call
      ((EntityCoordinates) ref local).\u002Ector(entityUid, worldPosition);
    }
    this.Spawn("JetpackEffect", coordinates);
  }
}
