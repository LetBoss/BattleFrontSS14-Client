// Decompiled with JetBrains decompiler
// Type: Content.Client.ContextMenu.UI.EntityMenuElement
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Administration.Managers;
using Content.Client.Administration.Systems;
using Content.Client.UserInterface;
using Content.Shared.Administration;
using Content.Shared.IdentityManagement;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using System;
using System.Linq;

#nullable enable
namespace Content.Client.ContextMenu.UI;

public sealed class EntityMenuElement : ContextMenuElement, IEntityControl
{
  [Dependency]
  private IClientAdminManager _adminManager;
  [Dependency]
  private IEntityManager _entityManager;
  [Dependency]
  private IPlayerManager _playerManager;
  private AdminSystem _adminSystem;
  public EntityUid? Entity;

  public int Count { get; private set; }

  public EntityMenuElement(EntityUid? entity = null)
    : base()
  {
    IoCManager.InjectDependencies<EntityMenuElement>(this);
    this._adminSystem = this._entityManager.System<AdminSystem>();
    this.Entity = entity;
    if (!this.Entity.HasValue)
      return;
    this.Count = 1;
    this.UpdateEntity();
  }

  [Obsolete]
  protected override void Dispose(bool disposing)
  {
    base.Dispose(disposing);
    this.Entity = new EntityUid?();
    this.Count = 0;
  }

  private string? SearchPlayerName(EntityUid entity)
  {
    NetEntity netEntity = this._entityManager.GetNetEntity(entity, (MetaDataComponent) null);
    return this._adminSystem.PlayerList.FirstOrDefault<PlayerInfo>((Func<PlayerInfo, bool>) (player =>
    {
      NetEntity? netEntity1 = player.NetEntity;
      NetEntity netEntity2 = netEntity;
      return netEntity1.HasValue && NetEntity.op_Equality(netEntity1.GetValueOrDefault(), netEntity2);
    }))?.Username;
  }

  public void UpdateCount()
  {
    if (this.SubMenu == null)
      return;
    this.Count = 0;
    foreach (Control child in ((Control) this.SubMenu.MenuBody).Children)
    {
      if (child is EntityMenuElement entityMenuElement)
        this.Count += entityMenuElement.Count;
    }
    ((Control) this.IconLabel).Visible = this.Count > 1;
    if (!((Control) this.IconLabel).Visible)
      return;
    this.IconLabel.Text = this.Count.ToString();
  }

  private string GetEntityDescriptionAdmin(EntityUid entity)
  {
    EntityStringRepresentation prettyString = this._entityManager.ToPrettyString(Robust.Shared.GameObjects.Entity<MetaDataComponent>.op_Implicit(entity));
    string name = ((EntityStringRepresentation) ref prettyString).Name;
    string prototype = ((EntityStringRepresentation) ref prettyString).Prototype;
    string str = ((EntityStringRepresentation) ref prettyString).Session?.Name ?? this.SearchPlayerName(entity);
    bool deleted = ((EntityStringRepresentation) ref prettyString).Deleted;
    return $"{name} ({this._entityManager.GetNetEntity(entity, (MetaDataComponent) null).ToString()}{(prototype != null ? ", " + prototype : "")}{(str != null ? ", " + str : "")}){(deleted ? "D" : "")}";
  }

  private string GetEntityDescription(EntityUid entity)
  {
    return this._adminManager.HasFlag(AdminFlags.Admin | AdminFlags.Debug) ? this.GetEntityDescriptionAdmin(entity) : (string) Identity.Name(entity, this._entityManager, ((ISharedPlayerManager) this._playerManager).LocalEntity);
  }

  public void UpdateEntity(EntityUid? entity = null)
  {
    if (!entity.HasValue)
      entity = this.Entity;
    if (this._entityManager.Deleted(entity))
    {
      this.Icon.SetEntity(new EntityUid?());
      this.Text = string.Empty;
    }
    else
    {
      this.Icon.SetEntity(entity);
      this.Text = this.GetEntityDescription(entity.Value);
    }
  }

  EntityUid? IEntityControl.UiEntity => this.Entity;
}
