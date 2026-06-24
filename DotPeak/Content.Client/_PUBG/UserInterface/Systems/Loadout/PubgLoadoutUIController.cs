// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.UserInterface.Systems.Loadout.PubgLoadoutUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Gameplay;
using Content.Shared._PUBG.Input;
using Content.Shared._PUBG.Loadout;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using System;

#nullable enable
namespace Content.Client._PUBG.UserInterface.Systems.Loadout;

public sealed class PubgLoadoutUIController : 
  UIController,
  IOnStateEntered<GameplayState>,
  IOnStateExited<GameplayState>
{
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private IEntityManager _entMan;
  private PubgWeaponModulesWindow? _window;
  private bool _systemSubscribed;

  public void OnStateEntered(GameplayState state)
  {
    this.EnsureSystemSubscribed();
    CommandBinds.Builder.Bind(PubgKeyFunctions.PubgInventoryMenu, (InputCmdHandler) new PubgLoadoutUIController.ToggleLoadoutInputHandler(new Func<bool>(this.TryToggleWindow))).Register<PubgLoadoutUIController>();
  }

  public void OnStateExited(GameplayState state)
  {
    CommandBinds.Unregister<PubgLoadoutUIController>();
    this.SetPolling(false);
    this.UnsubscribeSystem();
    this.CloseWindow();
  }

  private void EnsureSystemSubscribed()
  {
    if (this._systemSubscribed)
      return;
    this._entMan.System<PubgLoadoutSystem>().OnStateReceived += new Action<PubgLoadoutStateMessage>(this.OnStateReceived);
    this._systemSubscribed = true;
  }

  private void UnsubscribeSystem()
  {
    if (!this._systemSubscribed)
      return;
    PubgLoadoutSystem pubgLoadoutSystem = this._entMan.SystemOrNull<PubgLoadoutSystem>();
    if (pubgLoadoutSystem != null)
      pubgLoadoutSystem.OnStateReceived -= new Action<PubgLoadoutStateMessage>(this.OnStateReceived);
    this._systemSubscribed = false;
  }

  private void EnsureWindow()
  {
    PubgWeaponModulesWindow window = this._window;
    if (window != null && !((Control) window).Disposed)
      return;
    this._window = this.UIManager.CreateWindow<PubgWeaponModulesWindow>();
    this._window.ModuleRemoveRequested += new Action<EntityUid, string>(this.OnModuleRemoveRequested);
    this._window.OnClose += new Action(this.OnWindowClosed);
  }

  public void ToggleForTest() => this.TryToggleWindow();

  private bool TryToggleWindow()
  {
    PubgLoadoutSystem pubgLoadoutSystem = this._entMan.SystemOrNull<PubgLoadoutSystem>();
    if (pubgLoadoutSystem == null)
      return false;
    this.EnsureWindow();
    if (this._window == null)
      return false;
    if (this._window.IsOpen)
    {
      this.SetPolling(false);
      this.CloseWindow();
      return true;
    }
    this._window.OpenCentered();
    pubgLoadoutSystem.SetPolling(true);
    pubgLoadoutSystem.RequestState(true);
    return true;
  }

  private void OnWindowClosed()
  {
    this.SetPolling(false);
    this.ReleaseWindow();
  }

  private void OnModuleRemoveRequested(EntityUid weapon, string slotId)
  {
    this._entMan.SystemOrNull<PubgLoadoutSystem>()?.RequestAction(PubgLoadoutActionType.RemoveModule, new EntityUid(), weapon: weapon, targetSlotId: slotId);
  }

  private void OnStateReceived(PubgLoadoutStateMessage msg)
  {
    if (this._window == null || !this._window.IsOpen)
      return;
    (EntityUid Uid, PubgLoadoutWeaponState State)? nullable = this.ResolveActiveWeapon(msg);
    if (nullable.HasValue)
    {
      (EntityUid Uid, PubgLoadoutWeaponState State) valueOrDefault = nullable.GetValueOrDefault();
      this._window.UpdateData(valueOrDefault.Uid, valueOrDefault.State, this._entMan);
    }
    else
      this._window.ShowEmpty();
  }

  private (EntityUid Uid, PubgLoadoutWeaponState State)? ResolveActiveWeapon(
    PubgLoadoutStateMessage msg)
  {
    if (msg.Weapons.Count == 0)
      return new (EntityUid, PubgLoadoutWeaponState)?();
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    if (!localEntity.HasValue)
      return this.ResolveFirstValid(msg);
    EntityUid valueOrDefault = localEntity.GetValueOrDefault();
    HandsComponent handsComponent;
    if (!this._entMan.TryGetComponent<HandsComponent>(valueOrDefault, ref handsComponent))
      return this.ResolveFirstValid(msg);
    EntityUid? nullable;
    if (!this._entMan.System<SharedHandsSystem>().TryGetActiveItem(Entity<HandsComponent>.op_Implicit((valueOrDefault, handsComponent)), out nullable) || !nullable.HasValue)
      return new (EntityUid, PubgLoadoutWeaponState)?();
    foreach (PubgLoadoutWeaponState weapon in msg.Weapons)
    {
      EntityUid entity = this._entMan.GetEntity(weapon.Entity);
      if (EntityUid.op_Equality(entity, nullable.Value))
        return new (EntityUid, PubgLoadoutWeaponState)?((entity, weapon));
    }
    return new (EntityUid, PubgLoadoutWeaponState)?();
  }

  private (EntityUid Uid, PubgLoadoutWeaponState State)? ResolveFirstValid(
    PubgLoadoutStateMessage msg)
  {
    foreach (PubgLoadoutWeaponState weapon in msg.Weapons)
    {
      EntityUid entity = this._entMan.GetEntity(weapon.Entity);
      if (EntityUid.op_Inequality(entity, EntityUid.Invalid) && this._entMan.EntityExists(entity))
        return new (EntityUid, PubgLoadoutWeaponState)?((entity, weapon));
    }
    return new (EntityUid, PubgLoadoutWeaponState)?();
  }

  private void SetPolling(bool enabled)
  {
    this._entMan.SystemOrNull<PubgLoadoutSystem>()?.SetPolling(enabled);
  }

  private void CloseWindow()
  {
    if (this._window == null)
      return;
    PubgWeaponModulesWindow window = this._window;
    this.ReleaseWindow();
    window.Close();
  }

  private void ReleaseWindow()
  {
    if (this._window == null)
      return;
    PubgWeaponModulesWindow window = this._window;
    this._window = (PubgWeaponModulesWindow) null;
    window.ModuleRemoveRequested -= new Action<EntityUid, string>(this.OnModuleRemoveRequested);
    window.OnClose -= new Action(this.OnWindowClosed);
  }

  private sealed class ToggleLoadoutInputHandler : InputCmdHandler
  {
    private readonly Func<bool> _onPressed;

    public ToggleLoadoutInputHandler(Func<bool> onPressed) => this._onPressed = onPressed;

    public virtual bool HandleCmdMessage(
      IEntityManager entManager,
      ICommonSession? session,
      IFullInputCmdMessage message)
    {
      return message.State == 1 && this._onPressed();
    }
  }
}
