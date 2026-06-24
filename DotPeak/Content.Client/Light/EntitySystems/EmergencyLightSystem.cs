// Decompiled with JetBrains decompiler
// Type: Content.Client.Light.EntitySystems.EmergencyLightSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Light.Components;
using Content.Shared.Light.Components;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Client.Light.EntitySystems;

public sealed class EmergencyLightSystem : VisualizerSystem<EmergencyLightComponent>
{
  protected virtual void OnAppearanceChange(
    EntityUid uid,
    EmergencyLightComponent comp,
    ref AppearanceChangeEvent args)
  {
    if (args.Sprite == null)
      return;
    bool flag;
    if (!((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(uid, (Enum) EmergencyLightVisuals.On, ref flag, args.Component))
      flag = false;
    this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) EmergencyLightVisualLayers.LightOff, !flag);
    this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) EmergencyLightVisualLayers.LightOn, flag);
    Color color;
    if (!((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<Color>(uid, (Enum) EmergencyLightVisuals.Color, ref color, args.Component))
      return;
    this.SpriteSystem.LayerSetColor(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) EmergencyLightVisualLayers.LightOn, color);
    this.SpriteSystem.LayerSetColor(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) EmergencyLightVisualLayers.LightOff, color);
  }
}
