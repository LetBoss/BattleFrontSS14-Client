// Decompiled with JetBrains decompiler
// Type: Content.Client.CombatMode.CombatModeSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Hands.Systems;
using Content.Client.NPC.HTN;
using Content.Shared.CCVar;
using Content.Shared.CombatMode;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Shared.Analyzers;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using System;

#nullable enable
namespace Content.Client.CombatMode;

public sealed class CombatModeSystem : SharedCombatModeSystem
{
  [Dependency]
  private IOverlayManager _overlayManager;
  [Dependency]
  private IPlayerManager _playerManager;
  [Dependency]
  private IConfigurationManager _cfg;
  [Dependency]
  private IInputManager _inputManager;
  [Dependency]
  private IEyeManager _eye;

  public event Action<bool>? LocalPlayerCombatModeUpdated;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CombatModeComponent, AfterAutoHandleStateEvent>(new ComponentEventRefHandler<CombatModeComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    EntitySystemSubscriptionExt.CVar<bool>(this.Subs, this._cfg, CCVars.CombatModeIndicatorsPointShow, new Action<bool>(this.OnShowCombatIndicatorsChanged), true);
  }

  private void OnHandleState(
    EntityUid uid,
    CombatModeComponent component,
    ref AfterAutoHandleStateEvent args)
  {
    this.UpdateHud(uid);
  }

  public virtual void Shutdown()
  {
    this._overlayManager.RemoveOverlay<CombatModeIndicatorsOverlay>();
    base.Shutdown();
  }

  public bool IsInCombatMode()
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    return localEntity.HasValue && this.IsInCombatMode(new EntityUid?(localEntity.Value));
  }

  public override void SetInCombatMode(EntityUid entity, bool value, CombatModeComponent? component = null)
  {
    base.SetInCombatMode(entity, value, component);
    this.UpdateHud(entity);
  }

  protected override bool IsNpc(EntityUid uid) => this.HasComp<HTNComponent>(uid);

  private void UpdateHud(EntityUid entity)
  {
    EntityUid entityUid = entity;
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    if ((localEntity.HasValue ? (EntityUid.op_Inequality(entityUid, localEntity.GetValueOrDefault()) ? 1 : 0) : 1) != 0 || !this.Timing.IsFirstTimePredicted)
      return;
    bool flag = this.IsInCombatMode();
    Action<bool> combatModeUpdated = this.LocalPlayerCombatModeUpdated;
    if (combatModeUpdated == null)
      return;
    combatModeUpdated(flag);
  }

  private void OnShowCombatIndicatorsChanged(bool isShow)
  {
    if (isShow)
      this._overlayManager.AddOverlay((Overlay) new CombatModeIndicatorsOverlay(this._inputManager, (IEntityManager) this.EntityManager, this._eye, this, this.EntityManager.System<HandsSystem>()));
    else
      this._overlayManager.RemoveOverlay<CombatModeIndicatorsOverlay>();
  }
}
