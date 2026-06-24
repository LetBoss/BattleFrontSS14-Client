// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Aircraft.AircraftVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._CIV14merka.Aircraft;
using Content.Shared.Vehicle.Components;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using System.Numerics;

#nullable enable
namespace Content.Client._CIV14merka.Aircraft;

public sealed class AircraftVisualizerSystem : EntitySystem
{
  [Dependency]
  private readonly ISharedPlayerManager _player;

  public virtual void FrameUpdate(float frameTime)
  {
    bool flag = false;
    EntityUid? localEntity = this._player.LocalEntity;
    EntityUid entityUid;
    if (localEntity.HasValue)
    {
      EntityUid valueOrDefault = localEntity.GetValueOrDefault();
      EntityQueryEnumerator<AircraftComponent, VehicleComponent> entityQueryEnumerator = this.EntityQueryEnumerator<AircraftComponent, VehicleComponent>();
      AircraftComponent aircraftComponent;
      VehicleComponent vehicleComponent;
      while (entityQueryEnumerator.MoveNext(ref entityUid, ref aircraftComponent, ref vehicleComponent))
      {
        if (aircraftComponent.Altitude > 0)
        {
          EntityUid? nullable = vehicleComponent.Operator;
          entityUid = valueOrDefault;
          if ((nullable.HasValue ? (EntityUid.op_Equality(nullable.GetValueOrDefault(), entityUid) ? 1 : 0) : 0) != 0)
          {
            flag = true;
            break;
          }
        }
      }
    }
    EntityQueryEnumerator<AircraftComponent, SpriteComponent> entityQueryEnumerator1 = this.EntityQueryEnumerator<AircraftComponent, SpriteComponent>();
    AircraftComponent aircraftComponent1;
    SpriteComponent spriteComponent1;
    while (entityQueryEnumerator1.MoveNext(ref entityUid, ref aircraftComponent1, ref spriteComponent1))
    {
      if (aircraftComponent1.Altitude <= 0)
      {
        if (Color.op_Inequality(spriteComponent1.Color, Color.White))
          spriteComponent1.Color = Color.White;
        if (spriteComponent1.Scale != Vector2.One)
          spriteComponent1.Scale = Vector2.One;
        if (spriteComponent1.DrawDepth != 6)
          spriteComponent1.DrawDepth = 6;
      }
      else
      {
        float num = aircraftComponent1.AirborneScale + (float) (aircraftComponent1.Altitude - 1) * aircraftComponent1.ScalePerLevel;
        Vector2 vector2 = new Vector2(num, num);
        if (spriteComponent1.Scale != vector2)
          spriteComponent1.Scale = vector2;
        if (spriteComponent1.DrawDepth != 13)
          spriteComponent1.DrawDepth = 13;
        Color color1;
        if (!flag)
        {
          Color black = Color.Black;
          color1 = ((Color) ref black).WithAlpha(aircraftComponent1.ShadowAlpha);
        }
        else
          color1 = Color.White;
        Color color2 = color1;
        if (Color.op_Inequality(spriteComponent1.Color, color2))
          spriteComponent1.Color = color2;
      }
    }
    EntityQueryEnumerator<HighAltitudeProjectileComponent, SpriteComponent> entityQueryEnumerator2 = this.EntityQueryEnumerator<HighAltitudeProjectileComponent, SpriteComponent>();
    HighAltitudeProjectileComponent projectileComponent;
    SpriteComponent spriteComponent2;
    while (entityQueryEnumerator2.MoveNext(ref entityUid, ref projectileComponent, ref spriteComponent2))
    {
      if (spriteComponent2.Visible != flag)
        spriteComponent2.Visible = flag;
    }
  }
}
