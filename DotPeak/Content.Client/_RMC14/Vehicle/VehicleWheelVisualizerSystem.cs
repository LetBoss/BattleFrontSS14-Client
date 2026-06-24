// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Vehicle.VehicleWheelVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Vehicle;
using Content.Shared.Vehicle.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Client._RMC14.Vehicle;

public sealed class VehicleWheelVisualizerSystem : VisualizerSystem<VehicleWheelSlotsComponent>
{
  [Dependency]
  private readonly SpriteSystem _sprite;
  [Dependency]
  private readonly IGameTiming _timing;

  public virtual void Initialize() => base.Initialize();

  protected virtual void OnAppearanceChange(
    EntityUid uid,
    VehicleWheelSlotsComponent component,
    ref AppearanceChangeEvent args)
  {
    this.UpdateWheelVisuals(uid);
  }

  public virtual void Update(float frameTime)
  {
    ((EntitySystem) this).Update(frameTime);
    EntityQueryEnumerator<VehicleWheelSlotsComponent, SpriteComponent> entityQueryEnumerator = ((EntitySystem) this).EntityQueryEnumerator<VehicleWheelSlotsComponent, SpriteComponent>();
    EntityUid uid;
    VehicleWheelSlotsComponent wheelSlotsComponent;
    SpriteComponent sprite;
    while (entityQueryEnumerator.MoveNext(ref uid, ref wheelSlotsComponent, ref sprite))
      this.UpdateWheelVisuals(uid, sprite);
  }

  private void UpdateWheelVisuals(EntityUid uid, SpriteComponent? sprite = null)
  {
    int num1;
    if (sprite == null && !((EntitySystem) this).TryComp<SpriteComponent>(uid, ref sprite) || !this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), "rmc-wheels", ref num1, false))
      return;
    int num2 = 0;
    int num3;
    if (((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<int>(uid, (Enum) VehicleWheelVisuals.WheelCount, ref num3, (AppearanceComponent) null))
      num2 = num3;
    bool flag1;
    if (((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(uid, (Enum) VehicleWheelVisuals.HasAllWheels, ref flag1, (AppearanceComponent) null) && num2 == 0)
      num2 = flag1 ? 1 : 0;
    if (num2 <= 0)
    {
      this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num1, false);
    }
    else
    {
      bool flag2 = false;
      GridVehicleMoverComponent vehicleMoverComponent;
      if (((EntitySystem) this).TryComp<GridVehicleMoverComponent>(uid, ref vehicleMoverComponent))
        flag2 = (double) Math.Abs(vehicleMoverComponent.CurrentSpeed) > 0.0099999997764825821 || vehicleMoverComponent.TurnInPlace && vehicleMoverComponent.InPlaceTurnBlockUntil > this._timing.CurTime;
      int num4 = num2;
      float num5 = 1f;
      int num6;
      if (((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<int>(uid, (Enum) VehicleWheelVisuals.WheelFunctionalCount, ref num6, (AppearanceComponent) null))
        num4 = num6;
      float num7;
      if (((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<float>(uid, (Enum) VehicleWheelVisuals.WheelIntegrityFraction, ref num7, (AppearanceComponent) null))
        num5 = num7;
      float num8 = num2 > 0 ? (float) num4 / (float) num2 : 1f;
      if ((double) num8 < 0.0)
        num8 = 0.0f;
      else if ((double) num8 > 1.0)
        num8 = 1f;
      float num9 = MathF.Min((float) (0.30000001192092896 + 0.699999988079071 * (double) num5), (float) (0.30000001192092896 + 0.699999988079071 * (double) num8));
      bool flag3 = num2 > 0 && num4 <= 0;
      if (flag3)
        num9 = 1f;
      string str = flag3 ? "wheels_1" : "wheels_0";
      if (RSI.StateId.op_Inequality(sprite.LayerGetState(num1), RSI.StateId.op_Implicit(str)))
      {
        sprite.LayerSetState(num1, RSI.StateId.op_Implicit(str));
        if (flag2 && !flag3)
          this._sprite.LayerSetAnimationTime(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num1, 0.0f);
      }
      this._sprite.LayerSetAutoAnimated(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num1, flag2 && !flag3);
      this._sprite.LayerSetColor(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num1, new Color(num9, num9, num9, sprite.Color.A));
      this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num1, true);
    }
  }
}
