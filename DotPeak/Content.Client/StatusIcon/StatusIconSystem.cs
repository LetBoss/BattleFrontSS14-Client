// Decompiled with JetBrains decompiler
// Type: Content.Client.StatusIcon.StatusIconSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Stealth;
using Content.Shared.CCVar;
using Content.Shared.Ghost;
using Content.Shared.StatusIcon;
using Content.Shared.StatusIcon.Components;
using Content.Shared.Stealth.Components;
using Content.Shared.Whitelist;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.StatusIcon;

public sealed class StatusIconSystem : SharedStatusIconSystem
{
  [Dependency]
  private IConfigurationManager _configuration;
  [Dependency]
  private IOverlayManager _overlay;
  [Dependency]
  private IPlayerManager _playerManager;
  [Dependency]
  private EntityWhitelistSystem _entityWhitelist;
  private bool _globalEnabled;
  private bool _localEnabled;

  public virtual void Initialize()
  {
    EntitySystemSubscriptionExt.CVar<bool>(this.Subs, this._configuration, CCVars.LocalStatusIconsEnabled, new Action<bool>(this.OnLocalStatusIconChanged), true);
    EntitySystemSubscriptionExt.CVar<bool>(this.Subs, this._configuration, CCVars.GlobalStatusIconsEnabled, new Action<bool>(this.OnGlobalStatusIconChanged), true);
  }

  private void OnLocalStatusIconChanged(bool obj)
  {
    this._localEnabled = obj;
    this.UpdateOverlayVisible();
  }

  private void OnGlobalStatusIconChanged(bool obj)
  {
    this._globalEnabled = obj;
    this.UpdateOverlayVisible();
  }

  private void UpdateOverlayVisible()
  {
    if (this._overlay.RemoveOverlay<StatusIconOverlay>() || !this._globalEnabled || !this._localEnabled)
      return;
    this._overlay.AddOverlay((Overlay) new StatusIconOverlay());
  }

  public List<StatusIconData> GetStatusIcons(EntityUid uid, MetaDataComponent? meta = null)
  {
    List<StatusIconData> StatusIcons = new List<StatusIconData>();
    if (!this.Resolve(uid, ref meta, true) || meta.EntityLifeStage >= 4)
      return StatusIcons;
    GetStatusIconsEvent statusIconsEvent = new GetStatusIconsEvent(StatusIcons);
    this.RaiseLocalEvent<GetStatusIconsEvent>(uid, ref statusIconsEvent, false);
    return statusIconsEvent.StatusIcons;
  }

  public bool IsVisible(Entity<MetaDataComponent> ent, StatusIconData data)
  {
    EntityUid? attachedEntity = (EntityUid?) ((ISharedPlayerManager) this._playerManager).LocalSession?.AttachedEntity;
    EntityUid? nullable = attachedEntity;
    EntityUid owner = ent.Owner;
    StealthComponent stealthComponent;
    SpriteComponent spriteComponent;
    return (nullable.HasValue ? (EntityUid.op_Equality(nullable.GetValueOrDefault(), owner) ? 1 : 0) : 0) != 0 || data.VisibleToGhosts && this.HasComp<GhostComponent>(attachedEntity) || (!data.HideInContainer || (ent.Comp.Flags & 2) == null) && (!data.HideOnStealth || !this.TryComp<StealthComponent>(Entity<MetaDataComponent>.op_Implicit(ent), ref stealthComponent) || !stealthComponent.Enabled) && (!this.TryComp<SpriteComponent>(Entity<MetaDataComponent>.op_Implicit(ent), ref spriteComponent) || spriteComponent.Visible) && (!data.HideOnStealth || !this.HasComp<EntityActiveInvisibleComponent>(Entity<MetaDataComponent>.op_Implicit(ent))) && (data.ShowTo == null || this._entityWhitelist.IsValid(data.ShowTo, attachedEntity));
  }
}
