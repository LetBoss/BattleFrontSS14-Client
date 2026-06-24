// Decompiled with JetBrains decompiler
// Type: Content.Client.Kudzu.KudzuVisualsSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Spreader;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Kudzu;

public sealed class KudzuVisualsSystem : VisualizerSystem<KudzuVisualsComponent>
{
  protected virtual void OnAppearanceChange(
    EntityUid uid,
    KudzuVisualsComponent component,
    ref AppearanceChangeEvent args)
  {
    int num1;
    int num2;
    if (args.Sprite == null || !((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<int>(uid, (Enum) KudzuVisuals.Variant, ref num1, args.Component) || !((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<int>(uid, (Enum) KudzuVisuals.GrowthLevel, ref num2, args.Component))
      return;
    int num3 = this.SpriteSystem.LayerMapReserve(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), $"{component.Layer}");
    this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num3, RSI.StateId.op_Implicit($"kudzu_{num2}{num1}"));
  }
}
