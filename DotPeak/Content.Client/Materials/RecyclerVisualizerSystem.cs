// Decompiled with JetBrains decompiler
// Type: Content.Client.Materials.RecyclerVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Conveyor;
using Content.Shared.Materials;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Materials;

public sealed class RecyclerVisualizerSystem : VisualizerSystem<RecyclerVisualsComponent>
{
  protected virtual void OnAppearanceChange(
    EntityUid uid,
    RecyclerVisualsComponent component,
    ref AppearanceChangeEvent args)
  {
    int num1;
    if (args.Sprite == null || !this.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) RecyclerVisualLayers.Main, ref num1, false))
      return;
    ConveyorState conveyorState;
    ((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<ConveyorState>(uid, (Enum) ConveyorVisuals.State, ref conveyorState, (AppearanceComponent) null);
    bool flag1;
    ((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(uid, (Enum) RecyclerVisuals.Bloody, ref flag1, (AppearanceComponent) null);
    bool flag2;
    ((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(uid, (Enum) RecyclerVisuals.Broken, ref flag2, (AppearanceComponent) null);
    int num2 = conveyorState != 0 ? 1 : 0;
    if (flag2)
      num2 = 2;
    string str1 = flag1 ? component.BloodyKey : string.Empty;
    string str2 = $"{component.BaseKey}{num2}{str1}";
    this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num1, RSI.StateId.op_Implicit(str2));
  }
}
