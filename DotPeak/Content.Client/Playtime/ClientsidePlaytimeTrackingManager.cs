// Decompiled with JetBrains decompiler
// Type: Content.Client.Playtime.ClientsidePlaytimeTrackingManager
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.CCVar;
using Robust.Client.Player;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Network;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Client.Playtime;

public sealed class ClientsidePlaytimeTrackingManager
{
  [Dependency]
  private IClientNetManager _clientNetManager;
  [Dependency]
  private IConfigurationManager _configurationManager;
  [Dependency]
  private ILogManager _logManager;
  [Dependency]
  private IPlayerManager _playerManager;
  [Dependency]
  private IGameTiming _gameTiming;
  private ISawmill _sawmill;
  private const string InternalDateFormat = "yyyy-MM-dd";
  [Robust.Shared.ViewVariables.ViewVariables]
  private TimeSpan? _mobAttachmentTime;

  [Robust.Shared.ViewVariables.ViewVariables]
  public float PlaytimeMinutesToday
  {
    get
    {
      float cvar = this._configurationManager.GetCVar<float>(CCVars.PlaytimeMinutesToday);
      return !this._mobAttachmentTime.HasValue ? cvar : cvar + (float) (this._gameTiming.RealTime - this._mobAttachmentTime.Value).TotalMinutes;
    }
  }

  public void Initialize()
  {
    this._sawmill = this._logManager.GetSawmill("clientplaytime");
    ((INetManager) this._clientNetManager).Connected += new EventHandler<NetChannelArgs>(this.OnConnected);
    this._playerManager.LocalPlayerAttached += new Action<EntityUid>(this.OnPlayerAttached);
    this._playerManager.LocalPlayerDetached += new Action<EntityUid>(this.OnPlayerDetached);
  }

  private void OnConnected(object? sender, NetChannelArgs args)
  {
    DateTime now = DateTime.Now;
    this._sawmill.Info($"Current day: {now.Day} Current Date: {now.Date.ToString("yyyy-MM-dd")}");
    string cvar = this._configurationManager.GetCVar<string>(CCVars.PlaytimeLastConnectDate);
    string str = now.Date.ToString("yyyy-MM-dd");
    if (str == cvar)
      return;
    this._configurationManager.SetCVar<float>(CCVars.PlaytimeMinutesToday, 0.0f, false);
    this._configurationManager.SetCVar<string>(CCVars.PlaytimeLastConnectDate, str, false);
  }

  private void OnPlayerAttached(EntityUid entity)
  {
    this._mobAttachmentTime = new TimeSpan?(this._gameTiming.RealTime);
  }

  private void OnPlayerDetached(EntityUid entity)
  {
    if (!this._mobAttachmentTime.HasValue)
      return;
    float playtimeMinutesToday = this.PlaytimeMinutesToday;
    this._mobAttachmentTime = new TimeSpan?();
    float num = playtimeMinutesToday - this._configurationManager.GetCVar<float>(CCVars.PlaytimeMinutesToday);
    if ((double) num < 0.0)
    {
      this._sawmill.Error("Time differential on player detachment somehow less than zero!");
    }
    else
    {
      if ((double) num < 1.0)
        return;
      this._configurationManager.SetCVar<float>(CCVars.PlaytimeMinutesToday, playtimeMinutesToday, false);
      this._sawmill.Info($"Recorded {num} minutes of living playtime!");
      try
      {
        this._configurationManager.SaveToFile();
      }
      catch
      {
      }
    }
  }
}
