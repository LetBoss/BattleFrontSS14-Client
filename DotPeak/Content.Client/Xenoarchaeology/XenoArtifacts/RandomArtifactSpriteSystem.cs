// Decompiled with JetBrains decompiler
// Type: Content.Client.Xenoarchaeology.XenoArtifacts.RandomArtifactSpriteSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Xenoarchaeology.XenoArtifacts;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Xenoarchaeology.XenoArtifacts;

public sealed class RandomArtifactSpriteSystem : VisualizerSystem<RandomArtifactSpriteComponent>
{
  protected virtual void OnAppearanceChange(
    EntityUid uid,
    RandomArtifactSpriteComponent component,
    ref AppearanceChangeEvent args)
  {
    int num1;
    if (args.Sprite == null || !((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<int>(uid, (Enum) SharedArtifactsVisuals.SpriteIndex, ref num1, args.Component))
      return;
    bool flag1;
    if (!((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(uid, (Enum) SharedArtifactsVisuals.IsUnlocking, ref flag1, args.Component))
      flag1 = false;
    bool flag2;
    if (!((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(uid, (Enum) SharedArtifactsVisuals.IsActivated, ref flag2, args.Component))
      flag2 = false;
    string str1 = num1.ToString("D2");
    string str2 = flag1 ? "_on" : "";
    int num2;
    if (this.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) ArtifactsVisualLayers.UnlockingEffect, ref num2, false))
    {
      string str3 = "ano" + str1;
      this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) ArtifactsVisualLayers.Base, RSI.StateId.op_Implicit(str3));
      this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num2, RSI.StateId.op_Implicit(str3 + "_on"));
      this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num2, flag1);
      int num3;
      if (!this.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) ArtifactsVisualLayers.ActivationEffect, ref num3, false))
        return;
      this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num3, RSI.StateId.op_Implicit("artifact-activation"));
      this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num3, flag2);
    }
    else
    {
      string str4 = $"ano{str1}{str2}";
      this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) ArtifactsVisualLayers.Base, RSI.StateId.op_Implicit(str4));
    }
  }
}
