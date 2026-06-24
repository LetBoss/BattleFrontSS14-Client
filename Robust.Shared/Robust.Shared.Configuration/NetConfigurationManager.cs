using System.Collections.Generic;
using Robust.Shared.Collections;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Network;
using Robust.Shared.Network.Messages;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Robust.Shared.Configuration;

internal abstract class NetConfigurationManager : ConfigurationManager, INetConfigurationManagerInternal, INetConfigurationManager, IConfigurationManager, IConfigurationManagerInternal
{
	[Dependency]
	protected readonly INetManager NetManager;

	[Dependency]
	protected readonly IGameTiming Timing;

	private readonly List<MsgConVars> _netVarsMessages = new List<MsgConVars>();

	protected ISawmill Sawmill;

	public override void Shutdown()
	{
		base.Shutdown();
		FlushMessages();
	}

	public virtual void SetupNetworking()
	{
		Sawmill = Logger.GetSawmill("cfg");
		Sawmill.Level = LogLevel.Info;
		NetManager.RegisterNetMessage<MsgConVars>(HandleNetVarMessage);
	}

	protected virtual void HandleNetVarMessage(MsgConVars message)
	{
		_netVarsMessages.Add(message);
	}

	public void TickProcessMessages()
	{
		if (!Timing.InSimulation || Timing.InPrediction)
		{
			return;
		}
		ValueList<MsgConVars> valueList = default(ValueList<MsgConVars>);
		for (int i = 0; i < _netVarsMessages.Count; i++)
		{
			MsgConVars msgConVars = _netVarsMessages[i];
			if (!(msgConVars.Tick > Timing.CurTick))
			{
				valueList.Add(msgConVars);
				_netVarsMessages.RemoveSwap(i);
				i--;
			}
		}
		if (valueList.Count == 0)
		{
			return;
		}
		valueList.Sort((MsgConVars a, MsgConVars b) => a.Tick.CompareTo(b.Tick));
		foreach (MsgConVars item in valueList)
		{
			ApplyNetVarChange(item.MsgChannel, item.NetworkedVars, item.Tick);
			if (item.Tick != default(GameTick) && item.Tick < Timing.CurTick)
			{
				Sawmill.Warning($"{item.MsgChannel}: Received late nwVar message ({item.Tick} < {Timing.CurTick} ).");
			}
		}
	}

	public void FlushMessages()
	{
		_netVarsMessages.Sort((MsgConVars a, MsgConVars b) => a.Tick.Value.CompareTo(b.Tick.Value));
		foreach (MsgConVars netVarsMessage in _netVarsMessages)
		{
			ApplyNetVarChange(netVarsMessage.MsgChannel, netVarsMessage.NetworkedVars, netVarsMessage.Tick);
		}
		_netVarsMessages.Clear();
	}

	protected abstract void ApplyNetVarChange(INetChannel msgChannel, List<(string name, object value)> networkedVars, GameTick tick);

	public void SyncConnectingClient(INetChannel client)
	{
		Sawmill.Info($"{client}: Sending server info...");
		MsgConVars msgConVars = new MsgConVars();
		msgConVars.Tick = Timing.CurTick;
		msgConVars.NetworkedVars = GetReplicatedVars();
		NetManager.ServerSendMessage(msgConVars, client);
	}

	public List<(string name, object value)> GetReplicatedVars(bool all = false)
	{
		using (Lock.ReadGuard())
		{
			List<(string, object)> list = new List<(string, object)>();
			foreach (ConfigVar value in _configVars.Values)
			{
				if (!value.Registered || (value.Flags & CVar.REPLICATED) == 0)
				{
					continue;
				}
				if (!all)
				{
					if (NetManager.IsClient)
					{
						if ((value.Flags & CVar.SERVER) != CVar.NONE)
						{
							continue;
						}
					}
					else if ((value.Flags & CVar.CLIENT) != CVar.NONE)
					{
						continue;
					}
				}
				list.Add((value.Name, ConfigurationManager.GetConfigVarValue(value)));
				Sawmill.Debug($"name={value.Name}, val={value.Value ?? value.DefaultValue}");
			}
			return list;
		}
	}

	public abstract T GetClientCVar<T>(INetChannel channel, string name);
}
