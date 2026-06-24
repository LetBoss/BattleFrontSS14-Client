using System;
using System.Collections.Generic;
using System.IO;
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

namespace Content.Client.Administration.Managers;

public sealed class ClientAdminManager : IClientAdminManager, IClientConGroupImplementation, IPostInjectInit, ISharedAdminManager
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

	public event Action? ConGroupUpdated;

	public bool IsActive()
	{
		return _adminData?.Active ?? false;
	}

	public bool HasFlag(AdminFlags flag)
	{
		return _adminData?.HasFlag(flag) ?? false;
	}

	public bool CanCommand(string cmdName)
	{
		if (_adminData != null && _adminData.HasFlag(AdminFlags.Host))
		{
			return true;
		}
		if (_localCommandPermissions.CanCommand(cmdName, _adminData))
		{
			return true;
		}
		return _availableCommands.Contains(cmdName);
	}

	public bool CanViewVar()
	{
		return CanCommand("vv");
	}

	public bool CanAdminPlace()
	{
		return _adminData?.CanAdminPlace() ?? false;
	}

	public bool CanScript()
	{
		return _adminData?.CanScript() ?? false;
	}

	public bool CanAdminMenu()
	{
		return _adminData?.CanAdminMenu() ?? false;
	}

	public void Initialize()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		((INetManager)_netMgr).RegisterNetMessage<MsgUpdateAdminStatus>((ProcessMessage<MsgUpdateAdminStatus>)UpdateMessageRx, (NetMessageAccept)3);
		Stream fs = default(Stream);
		if (_res.TryContentFileRead((ResPath?)new ResPath("/clientCommandPerms.yml"), ref fs))
		{
			_localCommandPermissions.LoadPermissionsFromStream(fs);
		}
	}

	private void UpdateMessageRx(MsgUpdateAdminStatus message)
	{
		_availableCommands.Clear();
		foreach (var (item, val2) in ((IConsoleHost)IoCManager.Resolve<IClientConsoleHost>()).AvailableCommands)
		{
			if (Attribute.GetCustomAttribute(((object)val2).GetType(), typeof(AnyCommandAttribute)) != null)
			{
				_availableCommands.Add(item);
			}
		}
		_availableCommands.UnionWith(message.AvailableCommands);
		_sawmill.Debug($"Have {message.AvailableCommands.Length} commands available");
		_adminData = message.Admin;
		if (_adminData != null)
		{
			string value = string.Join("|", AdminFlagsHelper.FlagsToNames(_adminData.Flags));
			_sawmill.Info($"Updated admin status: {_adminData.Active}/{_adminData.Title}/{value}");
			if (_adminData.Active)
			{
				_userInterface.DebugMonitors.SetMonitor((DebugMonitor)1, true);
			}
		}
		else
		{
			_sawmill.Info("Updated admin status: Not admin");
		}
		this.AdminStatusUpdated?.Invoke();
		this.ConGroupUpdated?.Invoke();
	}

	void IPostInjectInit.PostInject()
	{
		_conGroup.Implementation = (IClientConGroupImplementation)(object)this;
		_sawmill = _logManager.GetSawmill("admin");
	}

	public AdminData? GetAdminData(EntityUid uid, bool includeDeAdmin = false)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (localEntity.HasValue && uid == localEntity.GetValueOrDefault() && (_adminData?.Active ?? includeDeAdmin))
		{
			return _adminData;
		}
		return null;
	}

	public AdminData? GetAdminData(ICommonSession session, bool includeDeAdmin = false)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		NetUserId? localUser = ((ISharedPlayerManager)_player).LocalUser;
		NetUserId userId = session.UserId;
		if (localUser.HasValue && localUser.GetValueOrDefault() == userId && (_adminData?.Active ?? includeDeAdmin))
		{
			return _adminData;
		}
		return null;
	}

	public AdminData? GetAdminData(bool includeDeAdmin = false)
	{
		ICommonSession localSession = ((ISharedPlayerManager)_player).LocalSession;
		if (localSession != null)
		{
			return GetAdminData(localSession, includeDeAdmin);
		}
		return null;
	}
}
