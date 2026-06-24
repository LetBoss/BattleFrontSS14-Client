// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Vehicle.Supply.VehicleSupplyLiftSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Vehicle.Supply;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client._RMC14.Vehicle.Supply;

public sealed class VehicleSupplyLiftSystem : EntitySystem
{
  [Dependency]
  private readonly AnimationPlayerSystem _animation;
  private const string AnimationKey = "rmc_vehicle_supply_lift";
  private const string BaseLayerKey = "rmc-vehicle-supply-lift-base";

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<VehicleSupplyLiftComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<VehicleSupplyLiftComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnLiftHandleState)), (Type[]) null, (Type[]) null);
  }

  private void OnLiftHandleState(
    Entity<VehicleSupplyLiftComponent> lift,
    ref AfterAutoHandleStateEvent args)
  {
    SpriteComponent spriteComponent;
    int num;
    if (!this.TryComp<SpriteComponent>(Entity<VehicleSupplyLiftComponent>.op_Implicit(lift), ref spriteComponent) || !spriteComponent.LayerMapTryGet((object) "rmc-vehicle-supply-lift-base", ref num, false))
      return;
    if (lift.Comp.Mode != VehicleSupplyLiftMode.Preparing)
      this._animation.Stop(Entity<AnimationPlayerComponent>.op_Implicit(lift.Owner), "rmc_vehicle_supply_lift");
    switch (lift.Comp.Mode)
    {
      case VehicleSupplyLiftMode.Lowered:
        spriteComponent.LayerSetState(num, RSI.StateId.op_Implicit(lift.Comp.LoweredState));
        break;
      case VehicleSupplyLiftMode.Raised:
        spriteComponent.LayerSetState(num, RSI.StateId.op_Implicit(lift.Comp.RaisedState));
        break;
      case VehicleSupplyLiftMode.Lowering:
        VehicleSupplyLiftComponent comp1 = lift.Comp;
        if (comp1.LoweringAnimation == null)
          comp1.LoweringAnimation = (object) new Animation()
          {
            Length = TimeSpan.FromSeconds(2.0999999046325684),
            AnimationTracks = {
              (AnimationTrack) new AnimationTrackSpriteFlick()
              {
                LayerKey = (object) "rmc-vehicle-supply-lift-base",
                KeyFrames = {
                  new AnimationTrackSpriteFlick.KeyFrame(RSI.StateId.op_Implicit(lift.Comp.LoweringState), 0.0f)
                }
              }
            }
          };
        this._animation.Play(Entity<VehicleSupplyLiftComponent>.op_Implicit(lift), (Animation) lift.Comp.LoweringAnimation, "rmc_vehicle_supply_lift");
        break;
      case VehicleSupplyLiftMode.Raising:
        VehicleSupplyLiftComponent comp2 = lift.Comp;
        if (comp2.RaisingAnimation == null)
          comp2.RaisingAnimation = (object) new Animation()
          {
            Length = TimeSpan.FromSeconds(2.0999999046325684),
            AnimationTracks = {
              (AnimationTrack) new AnimationTrackSpriteFlick()
              {
                LayerKey = (object) "rmc-vehicle-supply-lift-base",
                KeyFrames = {
                  new AnimationTrackSpriteFlick.KeyFrame(RSI.StateId.op_Implicit(lift.Comp.RaisingState), 0.0f)
                }
              }
            }
          };
        this._animation.Play(Entity<VehicleSupplyLiftComponent>.op_Implicit(lift), (Animation) lift.Comp.RaisingAnimation, "rmc_vehicle_supply_lift");
        break;
    }
  }
}
