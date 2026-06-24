// Decompiled with JetBrains decompiler
// Type: Content.Client.Electrocution.ElectrocutionHUDVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Electrocution;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using System;

#nullable enable
namespace Content.Client.Electrocution;

public sealed class ElectrocutionHUDVisualizerSystem : 
  VisualizerSystem<ElectrocutionHUDVisualsComponent>
{
  [Dependency]
  private IPlayerManager _playerMan;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<ShowElectrocutionHUDComponent, ComponentInit>(new EntityEventRefHandler<ShowElectrocutionHUDComponent, ComponentInit>((object) this, __methodptr(OnInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<ShowElectrocutionHUDComponent, ComponentShutdown>(new EntityEventRefHandler<ShowElectrocutionHUDComponent, ComponentShutdown>((object) this, __methodptr(OnShutdown)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<ShowElectrocutionHUDComponent, LocalPlayerAttachedEvent>(new EntityEventRefHandler<ShowElectrocutionHUDComponent, LocalPlayerAttachedEvent>((object) this, __methodptr(OnPlayerAttached)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<ShowElectrocutionHUDComponent, LocalPlayerDetachedEvent>(new EntityEventRefHandler<ShowElectrocutionHUDComponent, LocalPlayerDetachedEvent>((object) this, __methodptr(OnPlayerDetached)), (Type[]) null, (Type[]) null);
  }

  private void OnPlayerAttached(
    Entity<ShowElectrocutionHUDComponent> ent,
    ref LocalPlayerAttachedEvent args)
  {
    this.ShowHUD();
  }

  private void OnPlayerDetached(
    Entity<ShowElectrocutionHUDComponent> ent,
    ref LocalPlayerDetachedEvent args)
  {
    this.RemoveHUD();
  }

  private void OnInit(Entity<ShowElectrocutionHUDComponent> ent, ref ComponentInit args)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerMan).LocalEntity;
    EntityUid entityUid = Entity<ShowElectrocutionHUDComponent>.op_Implicit(ent);
    if ((localEntity.HasValue ? (EntityUid.op_Equality(localEntity.GetValueOrDefault(), entityUid) ? 1 : 0) : 0) == 0)
      return;
    this.ShowHUD();
  }

  private void OnShutdown(Entity<ShowElectrocutionHUDComponent> ent, ref ComponentShutdown args)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerMan).LocalEntity;
    EntityUid entityUid = Entity<ShowElectrocutionHUDComponent>.op_Implicit(ent);
    if ((localEntity.HasValue ? (EntityUid.op_Equality(localEntity.GetValueOrDefault(), entityUid) ? 1 : 0) : 0) == 0)
      return;
    this.RemoveHUD();
  }

  private void ShowHUD()
  {
    AllEntityQueryEnumerator<ElectrocutionHUDVisualsComponent, AppearanceComponent, SpriteComponent> entityQueryEnumerator = ((EntitySystem) this).AllEntityQuery<ElectrocutionHUDVisualsComponent, AppearanceComponent, SpriteComponent>();
    EntityUid entityUid;
    ElectrocutionHUDVisualsComponent visualsComponent;
    AppearanceComponent appearanceComponent;
    SpriteComponent spriteComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref visualsComponent, ref appearanceComponent, ref spriteComponent))
    {
      bool flag;
      if (((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(entityUid, (Enum) ElectrifiedVisuals.IsElectrified, ref flag, appearanceComponent))
        this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent)), (Enum) ElectrifiedLayers.HUD, flag);
    }
  }

  private void RemoveHUD()
  {
    AllEntityQueryEnumerator<ElectrocutionHUDVisualsComponent, AppearanceComponent, SpriteComponent> entityQueryEnumerator = ((EntitySystem) this).AllEntityQuery<ElectrocutionHUDVisualsComponent, AppearanceComponent, SpriteComponent>();
    EntityUid entityUid;
    ElectrocutionHUDVisualsComponent visualsComponent;
    AppearanceComponent appearanceComponent;
    SpriteComponent spriteComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref visualsComponent, ref appearanceComponent, ref spriteComponent))
      this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent)), (Enum) ElectrifiedLayers.HUD, false);
  }

  protected virtual void OnAppearanceChange(
    EntityUid uid,
    ElectrocutionHUDVisualsComponent comp,
    ref AppearanceChangeEvent args)
  {
    bool flag;
    if (args.Sprite == null || !((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(uid, (Enum) ElectrifiedVisuals.IsElectrified, ref flag, args.Component))
      return;
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerMan).LocalEntity;
    this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) ElectrifiedLayers.HUD, flag && ((EntitySystem) this).HasComp<ShowElectrocutionHUDComponent>(localEntity));
  }
}
