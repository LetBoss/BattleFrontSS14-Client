// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Storage.CMStorageVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Inventory;
using Content.Shared._RMC14.Storage;
using Content.Shared.Storage;
using Robust.Client.GameObjects;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client._RMC14.Storage;

public sealed class CMStorageVisualizerSystem : VisualizerSystem<CMStorageVisualizerComponent>
{
  [Dependency]
  private SpriteSystem _sprite;

  protected virtual void OnAppearanceChange(
    EntityUid uid,
    CMStorageVisualizerComponent component,
    ref AppearanceChangeEvent args)
  {
    int num;
    if (args.Sprite == null || !((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<int>(uid, (Enum) StorageVisuals.StorageUsed, ref num, args.Component))
      return;
    StorageComponent storageComponent;
    CMHolsterComponent holsterComponent;
    if (((EntitySystem) this).TryComp<StorageComponent>(uid, ref storageComponent) && ((EntitySystem) this).TryComp<CMHolsterComponent>(uid, ref holsterComponent) && ((BaseContainer) storageComponent.Container).ContainedEntities.Count == holsterComponent.Contents.Count)
      num = 0;
    if (num == 0)
    {
      if (component.StorageOpen != null)
        this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), component.StorageOpen, false);
      if (component.StorageClosed != null)
        this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), component.StorageClosed, false);
      if (component.StorageEmpty == null)
        return;
      this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), component.StorageEmpty, true);
    }
    else
    {
      if (component.StorageEmpty != null)
        this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), component.StorageEmpty, false);
      bool flag;
      if (!((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(uid, (Enum) StorageVisuals.Open, ref flag, args.Component))
        return;
      if (flag)
      {
        if (component.StorageOpen != null)
          this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), component.StorageOpen, true);
        if (component.StorageClosed == null)
          return;
        this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), component.StorageClosed, false);
      }
      else
      {
        if (component.StorageOpen != null)
          this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), component.StorageOpen, false);
        if (component.StorageClosed == null)
          return;
        this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), component.StorageClosed, true);
      }
    }
  }
}
