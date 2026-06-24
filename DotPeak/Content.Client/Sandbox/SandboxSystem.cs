// Decompiled with JetBrains decompiler
// Type: Content.Client.Sandbox.SandboxSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Administration.Managers;
using Content.Client.Movement.Systems;
using Content.Shared.Sandbox;
using Robust.Client.Console;
using Robust.Client.Placement;
using Robust.Shared.Console;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Player;
using System;

#nullable enable
namespace Content.Client.Sandbox;

public sealed class SandboxSystem : SharedSandboxSystem
{
  [Dependency]
  private IClientAdminManager _adminManager;
  [Dependency]
  private IClientConsoleHost _consoleHost;
  [Dependency]
  private IMapManager _map;
  [Dependency]
  private IPlacementManager _placement;
  [Dependency]
  private ContentEyeSystem _contentEye;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedMapSystem _mapSystem;
  private bool _sandboxEnabled;

  public bool SandboxAllowed { get; private set; }

  public event Action? SandboxEnabled;

  public event Action? SandboxDisabled;

  public virtual void Initialize()
  {
    this._adminManager.AdminStatusUpdated += new Action(this.CheckStatus);
    this.SubscribeNetworkEvent<SharedSandboxSystem.MsgSandboxStatus>(new EntityEventHandler<SharedSandboxSystem.MsgSandboxStatus>(this.OnSandboxStatus), (Type[]) null, (Type[]) null);
  }

  private void CheckStatus()
  {
    bool flag = this._sandboxEnabled || this._adminManager.IsActive();
    if (flag == this.SandboxAllowed)
      return;
    this.SandboxAllowed = flag;
    if (this.SandboxAllowed)
    {
      Action sandboxEnabled = this.SandboxEnabled;
      if (sandboxEnabled == null)
        return;
      sandboxEnabled();
    }
    else
    {
      Action sandboxDisabled = this.SandboxDisabled;
      if (sandboxDisabled == null)
        return;
      sandboxDisabled();
    }
  }

  public virtual void Shutdown()
  {
    this._adminManager.AdminStatusUpdated -= new Action(this.CheckStatus);
    base.Shutdown();
  }

  private void OnSandboxStatus(SharedSandboxSystem.MsgSandboxStatus ev)
  {
    this.SetAllowed(ev.SandboxAllowed);
  }

  private void SetAllowed(bool sandboxEnabled)
  {
    this._sandboxEnabled = sandboxEnabled;
    this.CheckStatus();
  }

  public void Respawn()
  {
    this.RaiseNetworkEvent((EntityEventArgs) new SharedSandboxSystem.MsgSandboxRespawn());
  }

  public void GiveAdminAccess()
  {
    this.RaiseNetworkEvent((EntityEventArgs) new SharedSandboxSystem.MsgSandboxGiveAccess());
  }

  public void GiveAGhost()
  {
    this.RaiseNetworkEvent((EntityEventArgs) new SharedSandboxSystem.MsgSandboxGiveAghost());
  }

  public void Suicide()
  {
    this.RaiseNetworkEvent((EntityEventArgs) new SharedSandboxSystem.MsgSandboxSuicide());
  }

  public bool Copy(ICommonSession? session, EntityCoordinates coords, EntityUid uid)
  {
    if (!this.SandboxAllowed)
      return false;
    MetaDataComponent metaDataComponent;
    if (((EntityUid) ref uid).IsValid() && this.TryComp(uid, ref metaDataComponent) && !metaDataComponent.EntityDeleted)
    {
      if (metaDataComponent.EntityPrototype == null || metaDataComponent.EntityPrototype.HideSpawnMenu || metaDataComponent.EntityPrototype.Abstract)
        return false;
      if (this._placement.Eraser)
        this._placement.ToggleEraser();
      this._placement.BeginPlacing(new PlacementInformation()
      {
        EntityType = metaDataComponent.EntityPrototype.ID,
        IsTile = false,
        TileType = 0,
        PlacementOption = metaDataComponent.EntityPrototype.PlacementMode
      }, (PlacementHijack) null);
      return true;
    }
    EntityUid entityUid;
    MapGridComponent mapGridComponent;
    TileRef tileRef;
    if (!this._map.TryFindGridAt(this._transform.ToMapCoordinates(coords, true), ref entityUid, ref mapGridComponent) || !this._mapSystem.TryGetTileRef(entityUid, mapGridComponent, coords, ref tileRef))
      return false;
    if (this._placement.Eraser)
      this._placement.ToggleEraser();
    this._placement.BeginPlacing(new PlacementInformation()
    {
      EntityType = (string) null,
      IsTile = true,
      TileType = tileRef.Tile.TypeId,
      PlacementOption = "AlignTileAny"
    }, (PlacementHijack) null);
    return true;
  }

  public void ToggleLight() => ((IConsoleHost) this._consoleHost).ExecuteCommand("togglelight");

  public void ToggleFov() => this._contentEye.RequestToggleFov();

  public void ToggleShadows() => ((IConsoleHost) this._consoleHost).ExecuteCommand("toggleshadows");

  public void ToggleSubFloor() => ((IConsoleHost) this._consoleHost).ExecuteCommand("showsubfloor");

  public void ShowMarkers() => ((IConsoleHost) this._consoleHost).ExecuteCommand("showmarkers");

  public void ShowBb() => ((IConsoleHost) this._consoleHost).ExecuteCommand("physics shapes");
}
