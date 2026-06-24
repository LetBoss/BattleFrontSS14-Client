// Decompiled with JetBrains decompiler
// Type: Content.Client.Ghost.GhostSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._RMC14.NightVision;
using Content.Client.Movement.Systems;
using Content.Shared._PUBG.Ghost;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Ghost;
using Robust.Client.Console;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Analyzers;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using System;

#nullable enable
namespace Content.Client.Ghost;

public sealed class GhostSystem : SharedGhostSystem
{
  [Dependency]
  private IClientConsoleHost _console;
  [Dependency]
  private IPlayerManager _playerManager;
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private PointLightSystem _pointLightSystem;
  [Dependency]
  private ContentEyeSystem _contentEye;
  [Dependency]
  private SpriteSystem _sprite;
  [Dependency]
  private IOverlayManager _overlay;
  private bool _ghostVisibility = true;

  public int AvailableGhostRoleCount { get; private set; }

  private bool GhostVisibility
  {
    get => this._ghostVisibility;
    set
    {
      if (this._ghostVisibility == value)
        return;
      this._ghostVisibility = value;
      AllEntityQueryEnumerator<GhostComponent, SpriteComponent> entityQueryEnumerator = this.AllEntityQuery<GhostComponent, SpriteComponent>();
      EntityUid entityUid1;
      GhostComponent ghostComponent;
      SpriteComponent spriteComponent;
      while (entityQueryEnumerator.MoveNext(ref entityUid1, ref ghostComponent, ref spriteComponent))
      {
        SpriteSystem sprite = this._sprite;
        Entity<SpriteComponent> entity = Entity<SpriteComponent>.op_Implicit((entityUid1, spriteComponent));
        int num;
        if (!value)
        {
          EntityUid entityUid2 = entityUid1;
          EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
          num = localEntity.HasValue ? (EntityUid.op_Equality(entityUid2, localEntity.GetValueOrDefault()) ? 1 : 0) : 0;
        }
        else
          num = 1;
        sprite.SetVisible(entity, num != 0);
      }
    }
  }

  public GhostComponent? Player
  {
    get
    {
      return this.CompOrNull<GhostComponent>(((ISharedPlayerManager) this._playerManager).LocalEntity);
    }
  }

  public bool IsGhost => this.Player != null;

  public event Action<GhostComponent>? PlayerRemoved;

  public event Action<GhostComponent>? PlayerUpdated;

  public event Action<GhostComponent>? PlayerAttached;

  public event Action? PlayerDetached;

  public event Action<GhostWarpsResponseEvent>? GhostWarpsResponse;

  public event Action<GhostUpdateGhostRoleCountEvent>? GhostRoleCountUpdated;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GhostComponent, ComponentStartup>(new ComponentEventHandler<GhostComponent, ComponentStartup>((object) this, __methodptr(OnStartup)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GhostComponent, ComponentRemove>(new ComponentEventHandler<GhostComponent, ComponentRemove>((object) this, __methodptr(OnGhostRemove)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GhostComponent, AfterAutoHandleStateEvent>(new ComponentEventRefHandler<GhostComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnGhostState)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GhostComponent, LocalPlayerAttachedEvent>(new ComponentEventHandler<GhostComponent, LocalPlayerAttachedEvent>((object) this, __methodptr(OnGhostPlayerAttach)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GhostComponent, LocalPlayerDetachedEvent>(new ComponentEventHandler<GhostComponent, LocalPlayerDetachedEvent>((object) this, __methodptr(OnGhostPlayerDetach)), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<GhostWarpsResponseEvent>(new EntityEventHandler<GhostWarpsResponseEvent>(this.OnGhostWarpsResponse), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<GhostUpdateGhostRoleCountEvent>(new EntityEventHandler<GhostUpdateGhostRoleCountEvent>(this.OnUpdateGhostRoleCount), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<EyeComponent, ToggleLightingActionEvent>(new ComponentEventHandler<EyeComponent, ToggleLightingActionEvent>((object) this, __methodptr(OnToggleLighting)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<EyeComponent, ToggleFoVActionEvent>(new ComponentEventHandler<EyeComponent, ToggleFoVActionEvent>((object) this, __methodptr(OnToggleFoV)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GhostComponent, ToggleGhostsActionEvent>(new ComponentEventHandler<GhostComponent, ToggleGhostsActionEvent>((object) this, __methodptr(OnToggleGhosts)), (Type[]) null, (Type[]) null);
  }

  private void OnStartup(EntityUid uid, GhostComponent component, ComponentStartup args)
  {
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(uid, ref spriteComponent))
      return;
    SpriteSystem sprite = this._sprite;
    Entity<SpriteComponent> entity = Entity<SpriteComponent>.op_Implicit((uid, spriteComponent));
    int num;
    if (!this.GhostVisibility)
    {
      EntityUid entityUid = uid;
      EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
      num = localEntity.HasValue ? (EntityUid.op_Equality(entityUid, localEntity.GetValueOrDefault()) ? 1 : 0) : 0;
    }
    else
      num = 1;
    sprite.SetVisible(entity, num != 0);
  }

  private void OnToggleLighting(
    EntityUid uid,
    EyeComponent component,
    ToggleLightingActionEvent args)
  {
    if (args.Handled)
      return;
    PointLightComponent pointLightComponent;
    this.TryComp<PointLightComponent>(uid, ref pointLightComponent);
    if (!component.DrawLight)
    {
      this.Popup.PopupEntity(this.Loc.GetString("ghost-gui-toggle-lighting-manager-popup-normal"), args.Performer);
      this._contentEye.RequestEye(component.DrawFov, true);
    }
    else if (pointLightComponent != null && !((SharedPointLightComponent) pointLightComponent).Enabled && !this._overlay.HasOverlay<HalfNightVisionBrightnessOverlay>())
    {
      this.Popup.PopupEntity(this.Loc.GetString("ghost-gui-toggle-lighting-manager-popup-personal-light"), args.Performer);
      ((SharedPointLightSystem) this._pointLightSystem).SetEnabled(uid, true, (SharedPointLightComponent) pointLightComponent, (MetaDataComponent) null);
    }
    else if (pointLightComponent != null && ((SharedPointLightComponent) pointLightComponent).Enabled && !this._overlay.HasOverlay<HalfNightVisionBrightnessOverlay>())
    {
      this.Popup.PopupEntity(this.Loc.GetString("rmc-ghost-gui-toggle-lighting-manager-popup-halfbright"), args.Performer);
      ((SharedPointLightSystem) this._pointLightSystem).SetEnabled(uid, false, (SharedPointLightComponent) pointLightComponent, (MetaDataComponent) null);
      this._overlay.AddOverlay((Overlay) new HalfNightVisionBrightnessOverlay());
    }
    else
    {
      this.Popup.PopupEntity(this.Loc.GetString("ghost-gui-toggle-lighting-manager-popup-fullbright"), args.Performer);
      this._contentEye.RequestEye(component.DrawFov, false);
      ((SharedPointLightSystem) this._pointLightSystem).SetEnabled(uid, false, (SharedPointLightComponent) pointLightComponent, (MetaDataComponent) null);
      this._overlay.RemoveOverlay<HalfNightVisionBrightnessOverlay>();
    }
    args.Handled = true;
  }

  private void OnToggleFoV(EntityUid uid, EyeComponent component, ToggleFoVActionEvent args)
  {
    if (args.Handled)
      return;
    this.Popup.PopupEntity(this.Loc.GetString("ghost-gui-toggle-fov-popup"), args.Performer);
    this._contentEye.RequestToggleFov(uid, component);
    args.Handled = true;
  }

  private void OnToggleGhosts(
    EntityUid uid,
    GhostComponent component,
    ToggleGhostsActionEvent args)
  {
    if (args.Handled)
      return;
    this.Popup.PopupEntity(this.Loc.GetString(this.GhostVisibility ? "ghost-gui-toggle-ghost-visibility-popup-off" : "ghost-gui-toggle-ghost-visibility-popup-on"), args.Performer);
    EntityUid entityUid = uid;
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    if ((localEntity.HasValue ? (EntityUid.op_Equality(entityUid, localEntity.GetValueOrDefault()) ? 1 : 0) : 0) != 0)
      this.ToggleGhostVisibility();
    args.Handled = true;
  }

  private void OnGhostRemove(EntityUid uid, GhostComponent component, ComponentRemove args)
  {
    SharedActionsSystem actions1 = this._actions;
    Entity<ActionsComponent> performer1 = Entity<ActionsComponent>.op_Implicit(uid);
    EntityUid? nullable = component.ToggleLightingActionEntity;
    Entity<ActionComponent>? action1 = nullable.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(nullable.GetValueOrDefault())) : new Entity<ActionComponent>?();
    actions1.RemoveAction(performer1, action1);
    SharedActionsSystem actions2 = this._actions;
    Entity<ActionsComponent> performer2 = Entity<ActionsComponent>.op_Implicit(uid);
    nullable = component.ToggleFoVActionEntity;
    Entity<ActionComponent>? action2 = nullable.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(nullable.GetValueOrDefault())) : new Entity<ActionComponent>?();
    actions2.RemoveAction(performer2, action2);
    SharedActionsSystem actions3 = this._actions;
    Entity<ActionsComponent> performer3 = Entity<ActionsComponent>.op_Implicit(uid);
    nullable = component.ToggleGhostsActionEntity;
    Entity<ActionComponent>? action3 = nullable.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(nullable.GetValueOrDefault())) : new Entity<ActionComponent>?();
    actions3.RemoveAction(performer3, action3);
    SharedActionsSystem actions4 = this._actions;
    Entity<ActionsComponent> performer4 = Entity<ActionsComponent>.op_Implicit(uid);
    nullable = component.ToggleGhostHearingActionEntity;
    Entity<ActionComponent>? action4 = nullable.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(nullable.GetValueOrDefault())) : new Entity<ActionComponent>?();
    actions4.RemoveAction(performer4, action4);
    EntityUid entityUid = uid;
    nullable = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    if ((nullable.HasValue ? (EntityUid.op_Inequality(entityUid, nullable.GetValueOrDefault()) ? 1 : 0) : 1) != 0)
      return;
    this._overlay.RemoveOverlay<HalfNightVisionBrightnessOverlay>();
    this.GhostVisibility = false;
    Action<GhostComponent> playerRemoved = this.PlayerRemoved;
    if (playerRemoved == null)
      return;
    playerRemoved(component);
  }

  private void OnGhostPlayerAttach(
    EntityUid uid,
    GhostComponent component,
    LocalPlayerAttachedEvent localPlayerAttachedEvent)
  {
    this.GhostVisibility = true;
    Action<GhostComponent> playerAttached = this.PlayerAttached;
    if (playerAttached == null)
      return;
    playerAttached(component);
  }

  private void OnGhostState(
    EntityUid uid,
    GhostComponent component,
    ref AfterAutoHandleStateEvent args)
  {
    SpriteComponent spriteComponent;
    if (this.TryComp<SpriteComponent>(uid, ref spriteComponent))
      this._sprite.LayerSetColor(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), 0, component.Color);
    EntityUid entityUid = uid;
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    if ((localEntity.HasValue ? (EntityUid.op_Inequality(entityUid, localEntity.GetValueOrDefault()) ? 1 : 0) : 1) != 0)
      return;
    Action<GhostComponent> playerUpdated = this.PlayerUpdated;
    if (playerUpdated == null)
      return;
    playerUpdated(component);
  }

  private void OnGhostPlayerDetach(
    EntityUid uid,
    GhostComponent component,
    LocalPlayerDetachedEvent args)
  {
    this.GhostVisibility = false;
    Action playerDetached = this.PlayerDetached;
    if (playerDetached != null)
      playerDetached();
    this._overlay.RemoveOverlay<HalfNightVisionBrightnessOverlay>();
  }

  private void OnGhostWarpsResponse(GhostWarpsResponseEvent msg)
  {
    if (!this.IsGhost)
      return;
    Action<GhostWarpsResponseEvent> ghostWarpsResponse = this.GhostWarpsResponse;
    if (ghostWarpsResponse == null)
      return;
    ghostWarpsResponse(msg);
  }

  private void OnUpdateGhostRoleCount(GhostUpdateGhostRoleCountEvent msg)
  {
    this.AvailableGhostRoleCount = msg.AvailableGhostRoles;
    Action<GhostUpdateGhostRoleCountEvent> roleCountUpdated = this.GhostRoleCountUpdated;
    if (roleCountUpdated == null)
      return;
    roleCountUpdated(msg);
  }

  public void RequestWarps()
  {
    this.RaiseNetworkEvent((EntityEventArgs) new GhostWarpsRequestEvent());
  }

  public void ReturnToBody()
  {
    this.RaiseNetworkEvent((EntityEventArgs) new GhostReturnToBodyRequest());
  }

  public void OpenGhostRoles()
  {
    ((IConsoleHost) this._console).RemoteExecuteCommand((ICommonSession) null, "ghostroles");
  }

  public void RequestGhostBar()
  {
    this.RaiseNetworkEvent((EntityEventArgs) new PubgGhostBarTeleportRequestEvent());
  }

  public void RequestLobbyRespawn()
  {
    this.RaiseNetworkEvent((EntityEventArgs) new PubgGhostLobbyRespawnRequestEvent());
  }

  public void ToggleGhostVisibility(bool? visibility = null)
  {
    this.GhostVisibility = ((int) visibility ?? (!this.GhostVisibility ? 1 : 0)) != 0;
  }
}
