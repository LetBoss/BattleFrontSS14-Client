// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.RoundEnd.NoEorgPopupSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.GameTicking;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client._RMC14.RoundEnd;

public sealed class NoEorgPopupSystem : EntitySystem
{
  private NoEorgPopup? _window;

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<RoundEndMessageEvent>(new EntityEventHandler<RoundEndMessageEvent>(this.OnRoundEnd), (Type[]) null, (Type[]) null);
  }

  private void OnRoundEnd(RoundEndMessageEvent ev)
  {
  }

  private void OpenNoEorgPopup()
  {
    if (this._window != null)
      return;
    this._window = new NoEorgPopup();
    this._window.OpenCentered();
    this._window.OnClose += (Action) (() => this._window = (NoEorgPopup) null);
  }
}
