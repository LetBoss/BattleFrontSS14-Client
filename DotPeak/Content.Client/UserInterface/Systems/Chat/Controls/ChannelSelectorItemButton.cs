// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.Chat.Controls.ChannelSelectorItemButton
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Chat;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Localization;

#nullable disable
namespace Content.Client.UserInterface.Systems.Chat.Controls;

public sealed class ChannelSelectorItemButton : Button
{
  public readonly ChatSelectChannel Channel;

  public bool IsHidden => ((Control) this).Parent == null;

  public ChannelSelectorItemButton(ChatSelectChannel selector)
  {
    this.Channel = selector;
    ((Control) this).AddStyleClass("chatSelectorOptionButton");
    this.Text = ChannelSelectorButton.ChannelSelectorName(selector);
    char channelPrefix = ChatUIController.ChannelPrefixes[selector];
    if (channelPrefix == char.MinValue)
      return;
    this.Text = Loc.GetString("hud-chatbox-select-name-prefixed", new (string, object)[2]
    {
      ("name", (object) this.Text),
      ("prefix", (object) channelPrefix)
    });
  }
}
