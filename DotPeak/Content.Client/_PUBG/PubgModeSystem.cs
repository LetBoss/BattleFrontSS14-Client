// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.PubgModeSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._PUBG;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client._PUBG;

public sealed class PubgModeSystem : EntitySystem
{
  public bool IsPubgModeActive { get; private set; }

  public int TeamSize { get; private set; }

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<PubgModeStatusEvent>(new EntitySessionEventHandler<PubgModeStatusEvent>(this.OnPubgModeStatus), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<PubgTeamModeStatusEvent>(new EntitySessionEventHandler<PubgTeamModeStatusEvent>(this.OnPubgTeamStatus), (Type[]) null, (Type[]) null);
  }

  private void OnPubgModeStatus(PubgModeStatusEvent ev, EntitySessionEventArgs args)
  {
    this.IsPubgModeActive = ev.Enabled;
    if (ev.Enabled)
      return;
    this.TeamSize = 0;
  }

  private void OnPubgTeamStatus(PubgTeamModeStatusEvent ev, EntitySessionEventArgs args)
  {
    this.IsPubgModeActive = ev.Enabled;
    this.TeamSize = ev.TeamSize;
  }
}
