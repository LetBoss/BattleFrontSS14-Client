// Decompiled with JetBrains decompiler
// Type: Content.Client.DamageState.DamageStateVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Mobs;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.DamageState;

public sealed class DamageStateVisualizerSystem : VisualizerSystem<DamageStateVisualsComponent>
{
  protected virtual void OnAppearanceChange(
    EntityUid uid,
    DamageStateVisualsComponent component,
    ref AppearanceChangeEvent args)
  {
    SpriteComponent sprite = args.Sprite;
    MobState key1;
    Dictionary<DamageStateVisualLayers, string> dictionary;
    if (sprite == null || !((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<MobState>(uid, (Enum) MobStateVisuals.State, ref key1, args.Component) || !component.States.TryGetValue(key1, out dictionary))
      return;
    DamageStateVisualLayers[] stateVisualLayersArray = new DamageStateVisualLayers[2]
    {
      DamageStateVisualLayers.Base,
      DamageStateVisualLayers.BaseUnshaded
    };
    int index;
    for (index = 0; index < stateVisualLayersArray.Length; ++index)
    {
      DamageStateVisualLayers stateVisualLayers = stateVisualLayersArray[index];
      int num;
      if (this.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) stateVisualLayers, ref num, false))
        this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) stateVisualLayers, false);
    }
    foreach ((DamageStateVisualLayers key2, string str) in dictionary)
    {
      if (this.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) key2, ref index, false))
      {
        this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) key2, true);
        this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) key2, RSI.StateId.op_Implicit(str));
      }
    }
    if (key1 == MobState.Dead)
    {
      if (sprite.DrawDepth <= -4)
        return;
      component.OriginalDrawDepth = new int?(sprite.DrawDepth);
      this.SpriteSystem.SetDrawDepth(Entity<SpriteComponent>.op_Implicit((uid, sprite)), -4);
    }
    else
    {
      if (!component.OriginalDrawDepth.HasValue)
        return;
      this.SpriteSystem.SetDrawDepth(Entity<SpriteComponent>.op_Implicit((uid, sprite)), component.OriginalDrawDepth.Value);
      component.OriginalDrawDepth = new int?();
    }
  }
}
