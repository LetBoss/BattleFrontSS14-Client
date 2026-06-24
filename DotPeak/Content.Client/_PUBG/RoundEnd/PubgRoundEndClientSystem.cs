// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.RoundEnd.PubgRoundEndClientSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._PUBG.RoundEnd;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client._PUBG.RoundEnd;

public sealed class PubgRoundEndClientSystem : EntitySystem
{
  private PubgInstanceRoundEndWindow? _window;

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<PubgInstanceRoundEndMessageEvent>(new EntitySessionEventHandler<PubgInstanceRoundEndMessageEvent>(this.OnRoundEndMessage), (Type[]) null, (Type[]) null);
  }

  private void OnRoundEndMessage(PubgInstanceRoundEndMessageEvent ev, EntitySessionEventArgs args)
  {
    ((BaseWindow) this._window)?.Close();
    this._window = new PubgInstanceRoundEndWindow();
    this._window.SetTitleText(ev.Title);
    this._window.SetRoundEndText(ev.RoundEndText);
    this._window.SetPartyEntries(ev.PartyEntries);
    ((BaseWindow) this._window).OpenCentered();
  }
}
