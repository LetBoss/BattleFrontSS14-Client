// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Dropship.Utility.DropshipPointVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Dropship.AttachmentPoint;
using Content.Shared._RMC14.Dropship.Utility.Components;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Client._RMC14.Dropship.Utility;

public sealed class DropshipPointVisualizerSystem : VisualizerSystem<DropshipPointVisualsComponent>
{
  [Dependency]
  private SpriteSystem _sprite;

  protected virtual void OnAppearanceChange(
    EntityUid uid,
    DropshipPointVisualsComponent component,
    ref AppearanceChangeEvent args)
  {
    base.OnAppearanceChange(uid, component, ref args);
    SpriteComponent sprite = args.Sprite;
    string str1;
    string str2;
    int num1;
    if (sprite == null || !((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<string>(uid, (Enum) DropshipUtilityVisuals.Sprite, ref str1, args.Component) || !((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<string>(uid, (Enum) DropshipUtilityVisuals.State, ref str2, args.Component) || !this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) DropshipPointVisualsLayers.AttachmentBase, ref num1, false))
      return;
    int num2;
    if (!this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) DropshipPointVisualsLayers.AttachedUtility, ref num2, false))
      this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num1, true);
    else if (string.IsNullOrWhiteSpace(str1) || string.IsNullOrWhiteSpace(str2))
    {
      this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num1, true);
      this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num2, false);
    }
    else
    {
      this._sprite.LayerSetSprite(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num2, (SpriteSpecifier) new SpriteSpecifier.Rsi(new ResPath(str1), str2));
      this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num1, false);
      this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num2, true);
    }
  }
}
