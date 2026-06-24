// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Localization.CivClientLocSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.UserInterface.Systems.Chat;
using Content.Shared._CIV14merka.Chat;
using Content.Shared._CIV14merka.Localization;
using Content.Shared.Chat;
using Content.Shared.Popups;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Client._CIV14merka.Localization;

public sealed class CivClientLocSystem : EntitySystem
{
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private IUserInterfaceManager _ui;

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<CivLocPopupEvent>(new EntityEventHandler<CivLocPopupEvent>(this.OnLocPopup), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<CivLocChatEvent>(new EntityEventHandler<CivLocChatEvent>(this.OnLocChat), (Type[]) null, (Type[]) null);
  }

  private void OnLocPopup(CivLocPopupEvent ev)
  {
    string message = ev.Message.Resolve();
    NetEntity? at = ev.At;
    EntityUid? nullable;
    if (at.HasValue && this.TryGetEntity(at.GetValueOrDefault(), ref nullable))
      this._popup.PopupEntity(message, nullable.Value, ev.Type);
    else
      this._popup.PopupCursor(message, ev.Type);
  }

  private void OnLocChat(CivLocChatEvent ev)
  {
    string str = ev.Body.Resolve();
    string wrappedMessage = CivAnnouncementFormat.BuildWrapped(ev.Kind, ev.Color, ev.SideColor, ev.SideTag, str, ev.Speaker, ev.ChannelTag);
    ChatMessage msg = new ChatMessage(ChatChannel.Radio, str, wrappedMessage, NetEntity.Invalid, new int?(), colorOverride: new Color?(ev.Color));
    this._ui.GetUIController<ChatUIController>().ProcessChatMessage(msg, false);
  }
}
