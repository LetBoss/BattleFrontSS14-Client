// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Configuration.NetConfigurationManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Collections;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Network;
using Robust.Shared.Network.Messages;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Configuration;

internal abstract class NetConfigurationManager : 
  ConfigurationManager,
  INetConfigurationManagerInternal,
  INetConfigurationManager,
  IConfigurationManager,
  IConfigurationManagerInternal
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
    this.FlushMessages();
  }

  public virtual void SetupNetworking()
  {
    this.Sawmill = Logger.GetSawmill("cfg");
    this.Sawmill.Level = new LogLevel?(LogLevel.Info);
    this.NetManager.RegisterNetMessage<MsgConVars>(new ProcessMessage<MsgConVars>(this.HandleNetVarMessage));
  }

  protected virtual void HandleNetVarMessage(MsgConVars message)
  {
    this._netVarsMessages.Add(message);
  }

  public void TickProcessMessages()
  {
    if (!this.Timing.InSimulation || this.Timing.InPrediction)
      return;
    ValueList<MsgConVars> valueList = new ValueList<MsgConVars>();
    for (int index = 0; index < this._netVarsMessages.Count; ++index)
    {
      MsgConVars netVarsMessage = this._netVarsMessages[index];
      if (!(netVarsMessage.Tick > this.Timing.CurTick))
      {
        valueList.Add(netVarsMessage);
        this._netVarsMessages.RemoveSwap<MsgConVars>(index);
        --index;
      }
    }
    if (valueList.Count == 0)
      return;
    valueList.Sort((Comparison<MsgConVars>) ((a, b) => a.Tick.CompareTo(b.Tick)));
    foreach (MsgConVars msgConVars in valueList)
    {
      this.ApplyNetVarChange(msgConVars.MsgChannel, msgConVars.NetworkedVars, msgConVars.Tick);
      if (msgConVars.Tick != new GameTick() && msgConVars.Tick < this.Timing.CurTick)
        this.Sawmill.Warning($"{msgConVars.MsgChannel}: Received late nwVar message ({msgConVars.Tick} < {this.Timing.CurTick} ).");
    }
  }

  public void FlushMessages()
  {
    this._netVarsMessages.Sort((Comparison<MsgConVars>) ((a, b) => a.Tick.Value.CompareTo(b.Tick.Value)));
    foreach (MsgConVars netVarsMessage in this._netVarsMessages)
      this.ApplyNetVarChange(netVarsMessage.MsgChannel, netVarsMessage.NetworkedVars, netVarsMessage.Tick);
    this._netVarsMessages.Clear();
  }

  protected abstract void ApplyNetVarChange(
    INetChannel msgChannel,
    List<(string name, object value)> networkedVars,
    GameTick tick);

  public void SyncConnectingClient(INetChannel client)
  {
    this.Sawmill.Info($"{client}: Sending server info...");
    this.NetManager.ServerSendMessage((NetMessage) new MsgConVars()
    {
      Tick = this.Timing.CurTick,
      NetworkedVars = this.GetReplicatedVars(false)
    }, client);
  }

  public List<(string name, object value)> GetReplicatedVars(bool all = false)
  {
    using (this.Lock.ReadGuard())
    {
      List<(string, object)> replicatedVars = new List<(string, object)>();
      foreach (ConfigurationManager.ConfigVar cVar in this._configVars.Values)
      {
        if (cVar.Registered && (cVar.Flags & CVar.REPLICATED) != CVar.NONE)
        {
          if (!all)
          {
            if (this.NetManager.IsClient)
            {
              if ((cVar.Flags & CVar.SERVER) != CVar.NONE)
                continue;
            }
            else if ((cVar.Flags & CVar.CLIENT) != CVar.NONE)
              continue;
          }
          replicatedVars.Add((cVar.Name, ConfigurationManager.GetConfigVarValue(cVar)));
          this.Sawmill.Debug($"name={cVar.Name}, val={cVar.Value ?? cVar.DefaultValue}");
        }
      }
      return replicatedVars;
    }
  }

  public abstract T GetClientCVar<T>(INetChannel channel, string name);
}
