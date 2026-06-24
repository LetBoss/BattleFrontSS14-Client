// Decompiled with JetBrains decompiler
// Type: Content.Client.Materials.MaterialStorageSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Materials;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.Materials;

public sealed class MaterialStorageSystem : SharedMaterialStorageSystem
{
  [Dependency]
  private AppearanceSystem _appearance;
  [Dependency]
  private TransformSystem _transform;
  [Dependency]
  private SpriteSystem _sprite;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<MaterialStorageComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<MaterialStorageComponent, AppearanceChangeEvent>((object) this, __methodptr(OnAppearanceChange)), (Type[]) null, (Type[]) null);
  }

  private void OnAppearanceChange(
    EntityUid uid,
    MaterialStorageComponent component,
    ref AppearanceChangeEvent args)
  {
    int num;
    bool flag;
    if (args.Sprite == null || !this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) MaterialStorageVisualLayers.Inserting, ref num, false) || !((SharedAppearanceSystem) this._appearance).TryGetData<bool>(uid, (Enum) MaterialStorageVisuals.Inserting, ref flag, args.Component))
      return;
    InsertingMaterialStorageComponent storageComponent;
    if (flag && this.TryComp<InsertingMaterialStorageComponent>(uid, ref storageComponent))
    {
      this._sprite.LayerSetAnimationTime(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, 0.0f);
      this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, true);
      if (!storageComponent.MaterialColor.HasValue)
        return;
      this._sprite.LayerSetColor(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, storageComponent.MaterialColor.Value);
    }
    else
      this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, false);
  }

  public override bool TryInsertMaterialEntity(
    EntityUid user,
    EntityUid toInsert,
    EntityUid receiver,
    MaterialStorageComponent? storage = null,
    MaterialComponent? material = null,
    PhysicalCompositionComponent? composition = null)
  {
    if (!base.TryInsertMaterialEntity(user, toInsert, receiver, storage, material, composition))
      return false;
    ((SharedTransformSystem) this._transform).DetachEntity(toInsert, this.Transform(toInsert));
    return true;
  }
}
