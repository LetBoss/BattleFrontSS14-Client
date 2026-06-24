// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Vehicle.VehicleFrameDamageVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Vehicle;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Client._RMC14.Vehicle;

public sealed class VehicleFrameDamageVisualizerSystem : 
  VisualizerSystem<HardpointIntegrityComponent>
{
  private const float ShowThreshold = 0.9f;
  private const float MinAlpha = 0.1f;

  protected virtual void OnAppearanceChange(
    EntityUid uid,
    HardpointIntegrityComponent component,
    ref AppearanceChangeEvent args)
  {
    SpriteComponent sprite = args.Sprite;
    int num1;
    if (sprite == null || !sprite.LayerMapTryGet((object) "damaged_frame", ref num1, false))
      return;
    float num2;
    if (!((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<float>(uid, (Enum) VehicleFrameDamageVisuals.IntegrityFraction, ref num2, (AppearanceComponent) null))
    {
      float num3 = (double) component.MaxIntegrity > 0.0 ? component.MaxIntegrity : 1f;
      num2 = Math.Clamp(component.Integrity / num3, 0.0f, 1f);
    }
    if ((double) num2 >= 0.89999997615814209)
    {
      sprite.LayerSetVisible(num1, false);
    }
    else
    {
      float num4 = (float) (0.10000000149011612 + 0.89999997615814209 * (1.0 - (double) (num2 / 0.9f)));
      sprite.LayerSetVisible(num1, true);
      SpriteComponent spriteComponent = sprite;
      int num5 = num1;
      Color color1 = sprite.Color;
      Color color2 = ((Color) ref color1).WithAlpha(num4);
      spriteComponent.LayerSetColor(num5, color2);
    }
  }
}
