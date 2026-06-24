// Decompiled with JetBrains decompiler
// Type: Content.Client.Storage.Systems.StorageContainerVisualsSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Storage.Components;
using Content.Shared.Rounding;
using Content.Shared.Storage;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Storage.Systems;

public sealed class StorageContainerVisualsSystem : 
  VisualizerSystem<StorageContainerVisualsComponent>
{
  protected virtual void OnAppearanceChange(
    EntityUid uid,
    StorageContainerVisualsComponent component,
    ref AppearanceChangeEvent args)
  {
    int num1;
    int num2;
    if (args.Sprite == null || !((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<int>(uid, (Enum) StorageVisuals.StorageUsed, ref num1, args.Component) || !((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<int>(uid, (Enum) StorageVisuals.Capacity, ref num2, args.Component))
      return;
    float actual = (float) num1 / (float) num2;
    int num3;
    if (!this.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) component.FillLayer, ref num3, false))
      return;
    int num4 = Math.Min(ContentHelpers.RoundToNearestLevels((double) actual, 1.0, component.MaxFillLevels + 1), component.MaxFillLevels);
    if (num4 > 0)
    {
      if (component.FillBaseName == null)
        return;
      this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num3, true);
      string str = component.FillBaseName + num4.ToString();
      this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num3, RSI.StateId.op_Implicit(str));
    }
    else
      this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num3, false);
  }
}
