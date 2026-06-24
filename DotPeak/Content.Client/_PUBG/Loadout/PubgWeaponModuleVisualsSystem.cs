// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.Loadout.PubgWeaponModuleVisualsSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._PUBG.Loadout;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client._PUBG.Loadout;

public sealed class PubgWeaponModuleVisualsSystem : 
  VisualizerSystem<PubgWeaponModuleVisualsComponent>
{
  protected virtual void OnAppearanceChange(
    EntityUid uid,
    PubgWeaponModuleVisualsComponent component,
    ref AppearanceChangeEvent args)
  {
    if (args.Sprite == null)
      return;
    this.UpdateLayer(uid, args, component.OpticLayer, PubgWeaponModuleVisuals.Optic);
    this.UpdateLayer(uid, args, component.MuzzleLayer, PubgWeaponModuleVisuals.Muzzle);
    this.UpdateLayer(uid, args, component.MagazineLayer, PubgWeaponModuleVisuals.Magazine);
  }

  private void UpdateLayer(
    EntityUid uid,
    AppearanceChangeEvent args,
    string layerKey,
    PubgWeaponModuleVisuals visualKey)
  {
    int num;
    bool flag;
    if (string.IsNullOrWhiteSpace(layerKey) || args.Sprite == null || !this.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), layerKey, ref num, false) || !((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(uid, (Enum) visualKey, ref flag, args.Component))
      return;
    this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), layerKey, flag);
  }
}
