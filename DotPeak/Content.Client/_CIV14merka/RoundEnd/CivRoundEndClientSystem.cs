// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.RoundEnd.CivRoundEndClientSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._CIV14merka;
using Robust.Client.Player;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using System;

#nullable enable
namespace Content.Client._CIV14merka.RoundEnd;

public sealed class CivRoundEndClientSystem : EntitySystem
{
  [Dependency]
  private readonly IPlayerManager _player;
  private CivRoundEndWindow? _window;

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<CivRoundEndMessageEvent>(new EntitySessionEventHandler<CivRoundEndMessageEvent>(this.OnRoundEndMessage), (Type[]) null, (Type[]) null);
  }

  private void OnRoundEndMessage(CivRoundEndMessageEvent ev, EntitySessionEventArgs args)
  {
    ((BaseWindow) this._window)?.Close();
    this._window = new CivRoundEndWindow();
    this._window.SetTitleText(ev.Title);
    this._window.SetSummary(ev.Summary);
    this._window.SetMyStats(((ISharedPlayerManager) this._player).LocalSession?.UserId, ev.TeamEntries);
    this._window.SetTeamEntries(ev.TeamEntries);
    ((BaseWindow) this._window).OpenCentered();
  }
}
