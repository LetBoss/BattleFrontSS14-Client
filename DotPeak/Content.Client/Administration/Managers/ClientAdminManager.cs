// Decompiled with JetBrains decompiler
// Type: Content.Client.Administration.Managers.ClientAdminManager
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Administration;
using Content.Shared.Administration.Managers;
using Robust.Client.Console;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Shared.Console;
using Robust.Shared.ContentPack;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

#nullable enable
namespace Content.Client.Administration.Managers;

public sealed class ClientAdminManager : 
  IClientAdminManager,
  IClientConGroupImplementation,
  IPostInjectInit,
  ISharedAdminManager
{
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private IClientNetManager _netMgr;
  [Dependency]
  private IClientConGroupController _conGroup;
  [Dependency]
  private IResourceManager _res;
  [Dependency]
  private ILogManager _logManager;
  [Dependency]
  private IUserInterfaceManager _userInterface;
  private AdminData? _adminData;
  private readonly HashSet<string> _availableCommands = new HashSet<string>();
  private readonly AdminCommandPermissions _localCommandPermissions = new AdminCommandPermissions();
  private ISawmill _sawmill;

  public event Action? AdminStatusUpdated;

  public bool IsActive()
  {
    AdminData adminData = this._adminData;
    return adminData != null && adminData.Active;
  }

  public bool HasFlag(AdminFlags flag)
  {
    AdminData adminData = this._adminData;
    return adminData != null && adminData.HasFlag(flag);
  }

  public bool CanCommand(string cmdName)
  {
    return this._adminData != null && this._adminData.HasFlag(AdminFlags.Host) || this._localCommandPermissions.CanCommand(cmdName, this._adminData) || this._availableCommands.Contains(cmdName);
  }

  public bool CanViewVar() => this.CanCommand("vv");

  public bool CanAdminPlace()
  {
    AdminData adminData = this._adminData;
    return adminData != null && adminData.CanAdminPlace();
  }

  public bool CanScript()
  {
    AdminData adminData = this._adminData;
    return adminData != null && adminData.CanScript();
  }

  public bool CanAdminMenu()
  {
    AdminData adminData = this._adminData;
    return adminData != null && adminData.CanAdminMenu();
  }

  public void Initialize()
  {
    // ISSUE: method pointer
    ((INetManager) this._netMgr).RegisterNetMessage<MsgUpdateAdminStatus>(new ProcessMessage<MsgUpdateAdminStatus>((object) this, __methodptr(UpdateMessageRx)), (NetMessageAccept) 3);
    Stream fs;
    if (!this._res.TryContentFileRead(new ResPath?(new ResPath("/clientCommandPerms.yml")), ref fs))
      return;
    this._localCommandPermissions.LoadPermissionsFromStream(fs);
  }

  private void UpdateMessageRx(MsgUpdateAdminStatus message)
  {
    this._availableCommands.Clear();
    foreach ((string key, IConsoleCommand iconsoleCommand) in (IEnumerable<KeyValuePair<string, IConsoleCommand>>) ((IConsoleHost) IoCManager.Resolve<IClientConsoleHost>()).AvailableCommands)
    {
      if (Attribute.GetCustomAttribute((MemberInfo) iconsoleCommand.GetType(), typeof (AnyCommandAttribute)) != null)
        this._availableCommands.Add(key);
    }
    this._availableCommands.UnionWith((IEnumerable<string>) message.AvailableCommands);
    this._sawmill.Debug($"Have {message.AvailableCommands.Length} commands available");
    this._adminData = message.Admin;
    if (this._adminData != null)
    {
      this._sawmill.Info($"Updated admin status: {this._adminData.Active}/{this._adminData.Title}/{string.Join("|", AdminFlagsHelper.FlagsToNames(this._adminData.Flags))}");
      if (this._adminData.Active)
        this._userInterface.DebugMonitors.SetMonitor((DebugMonitor) 1, true);
    }
    else
      this._sawmill.Info("Updated admin status: Not admin");
    Action adminStatusUpdated = this.AdminStatusUpdated;
    if (adminStatusUpdated != null)
      adminStatusUpdated();
    Action conGroupUpdated = this.ConGroupUpdated;
    if (conGroupUpdated == null)
      return;
    conGroupUpdated();
  }

  public event Action? ConGroupUpdated;

  void IPostInjectInit.PostInject()
  {
    this._conGroup.Implementation = (IClientConGroupImplementation) this;
    this._sawmill = this._logManager.GetSawmill("admin");
  }

  public AdminData? GetAdminData(EntityUid uid, bool includeDeAdmin = false)
  {
    EntityUid entityUid = uid;
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    if ((localEntity.HasValue ? (EntityUid.op_Equality(entityUid, localEntity.GetValueOrDefault()) ? 1 : 0) : 0) != 0)
    {
      AdminData adminData = this._adminData;
      if ((adminData != null ? (adminData.Active ? 1 : 0) : (includeDeAdmin ? 1 : 0)) != 0)
        return this._adminData;
    }
    return (AdminData) null;
  }

  public AdminData? GetAdminData(ICommonSession session, bool includeDeAdmin = false)
  {
    NetUserId? localUser = ((ISharedPlayerManager) this._player).LocalUser;
    NetUserId userId = session.UserId;
    if ((localUser.HasValue ? (NetUserId.op_Equality(localUser.GetValueOrDefault(), userId) ? 1 : 0) : 0) != 0)
    {
      AdminData adminData = this._adminData;
      if ((adminData != null ? (adminData.Active ? 1 : 0) : (includeDeAdmin ? 1 : 0)) != 0)
        return this._adminData;
    }
    return (AdminData) null;
  }

  public AdminData? GetAdminData(bool includeDeAdmin = false)
  {
    ICommonSession localSession = ((ISharedPlayerManager) this._player).LocalSession;
    return localSession != null ? this.GetAdminData(localSession, includeDeAdmin) : (AdminData) null;
  }
}
