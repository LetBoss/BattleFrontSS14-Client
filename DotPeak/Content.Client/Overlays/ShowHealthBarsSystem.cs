// Decompiled with JetBrains decompiler
// Type: Content.Client.Overlays.ShowHealthBarsSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Damage.Prototypes;
using Content.Shared.Inventory.Events;
using Content.Shared.Overlays;
using Robust.Client.Graphics;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Client.Overlays;

public sealed class ShowHealthBarsSystem : EquipmentHudSystem<ShowHealthBarsComponent>
{
  [Dependency]
  private IOverlayManager _overlayMan;
  [Dependency]
  private IPrototypeManager _prototype;
  private EntityHealthBarOverlay _overlay;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ShowHealthBarsComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<ShowHealthBarsComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    this._overlay = new EntityHealthBarOverlay((IEntityManager) this.EntityManager, this._prototype);
  }

  private void OnHandleState(
    Entity<ShowHealthBarsComponent> ent,
    ref AfterAutoHandleStateEvent args)
  {
    this.RefreshOverlay();
  }

  protected override void UpdateInternal(
    RefreshEquipmentHudEvent<ShowHealthBarsComponent> component)
  {
    base.UpdateInternal(component);
    foreach (ShowHealthBarsComponent component1 in component.Components)
    {
      foreach (ProtoId<DamageContainerPrototype> damageContainer in component1.DamageContainers)
        this._overlay.DamageContainers.Add(ProtoId<DamageContainerPrototype>.op_Implicit(damageContainer));
      this._overlay.StatusIcon = component1.HealthStatusIcon;
    }
    if (this._overlayMan.HasOverlay<EntityHealthBarOverlay>())
      return;
    this._overlayMan.AddOverlay((Overlay) this._overlay);
  }

  protected override void DeactivateInternal()
  {
    base.DeactivateInternal();
    this._overlay.DamageContainers.Clear();
    this._overlayMan.RemoveOverlay((Overlay) this._overlay);
  }
}
