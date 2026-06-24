// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.Chat.Controls.ChannelSelectorPopup
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Chat;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.UserInterface.Systems.Chat.Controls;

public sealed class ChannelSelectorPopup : Popup
{
  public static readonly ChatSelectChannel[] ChannelSelectorOrder = new ChatSelectChannel[12]
  {
    ChatSelectChannel.Local,
    ChatSelectChannel.Whisper,
    ChatSelectChannel.Emotes,
    ChatSelectChannel.Radio,
    ChatSelectChannel.Party,
    ChatSelectChannel.MiniGame,
    ChatSelectChannel.Lobby,
    ChatSelectChannel.LOOC,
    ChatSelectChannel.OOC,
    ChatSelectChannel.Dead,
    ChatSelectChannel.Admin,
    ChatSelectChannel.Mentor
  };
  private readonly BoxContainer _channelSelectorHBox;
  private readonly Dictionary<ChatSelectChannel, ChannelSelectorItemButton> _selectorStates = new Dictionary<ChatSelectChannel, ChannelSelectorItemButton>();
  private readonly ChatUIController _chatUIController;

  public event Action<ChatSelectChannel>? Selected;

  public ChannelSelectorPopup()
  {
    this._channelSelectorHBox = new BoxContainer()
    {
      Orientation = (BoxContainer.LayoutOrientation) 0,
      SeparationOverride = new int?(1)
    };
    this._chatUIController = ((Control) this).UserInterfaceManager.GetUIController<ChatUIController>();
    this._chatUIController.SelectableChannelsChanged += new Action<ChatSelectChannel>(this.SetChannels);
    this.SetChannels(this._chatUIController.SelectableChannels);
    ((Control) this).AddChild((Control) this._channelSelectorHBox);
  }

  public ChatSelectChannel? FirstChannel
  {
    get
    {
      foreach (ChannelSelectorItemButton selectorItemButton in this._selectorStates.Values)
      {
        if (!selectorItemButton.IsHidden)
          return new ChatSelectChannel?(selectorItemButton.Channel);
      }
      return new ChatSelectChannel?();
    }
  }

  private bool IsPreferredAvailable()
  {
    ChannelSelectorItemButton selectorItemButton;
    return this._selectorStates.TryGetValue(this._chatUIController.MapLocalIfGhost(this._chatUIController.GetPreferredChannel()), out selectorItemButton) && !selectorItemButton.IsHidden;
  }

  public void SetChannels(ChatSelectChannel channels)
  {
    bool flag1 = this.IsPreferredAvailable();
    ((Control) this._channelSelectorHBox).RemoveAllChildren();
    foreach (ChatSelectChannel chatSelectChannel in ChannelSelectorPopup.ChannelSelectorOrder)
    {
      ChannelSelectorItemButton selectorItemButton;
      if (!this._selectorStates.TryGetValue(chatSelectChannel, out selectorItemButton))
      {
        selectorItemButton = new ChannelSelectorItemButton(chatSelectChannel);
        this._selectorStates.Add(chatSelectChannel, selectorItemButton);
        ((BaseButton) selectorItemButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.OnSelectorPressed);
      }
      if ((channels & chatSelectChannel) == ChatSelectChannel.None)
      {
        if (((Control) selectorItemButton).Parent == this._channelSelectorHBox)
          ((Control) this._channelSelectorHBox).RemoveChild((Control) selectorItemButton);
      }
      else if (selectorItemButton.IsHidden)
        ((Control) this._channelSelectorHBox).AddChild((Control) selectorItemButton);
    }
    bool flag2 = this.IsPreferredAvailable();
    if (!flag1 & flag2)
    {
      this.Select(this._chatUIController.GetPreferredChannel());
    }
    else
    {
      if (!flag1 || flag2)
        return;
      this.Select(ChatSelectChannel.OOC);
    }
  }

  private void OnSelectorPressed(BaseButton.ButtonEventArgs args)
  {
    this.Select(((ChannelSelectorItemButton) args.Button).Channel);
  }

  private void Select(ChatSelectChannel channel)
  {
    Action<ChatSelectChannel> selected = this.Selected;
    if (selected == null)
      return;
    selected(channel);
  }

  [Obsolete]
  protected virtual void Dispose(bool disposing)
  {
    ((Control) this).Dispose(disposing);
    if (!disposing)
      return;
    this._chatUIController.SelectableChannelsChanged -= new Action<ChatSelectChannel>(this.SetChannels);
  }
}
