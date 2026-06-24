// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.Chat.Controls.ChatInputBox
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Chat;
using Content.Shared.Input;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Analyzers;
using Robust.Shared.Localization;
using System;

#nullable enable
namespace Content.Client.UserInterface.Systems.Chat.Controls;

[Virtual]
public class ChatInputBox : PanelContainer
{
  public readonly ChannelSelectorButton ChannelSelector;
  public readonly HistoryLineEdit Input;
  public readonly ChannelFilterButton FilterButton;
  protected readonly BoxContainer Container;

  protected ChatChannel ActiveChannel { get; private set; } = ChatChannel.Local;

  public ChatInputBox()
  {
    this.Container = new BoxContainer()
    {
      Orientation = (BoxContainer.LayoutOrientation) 0,
      SeparationOverride = new int?(4)
    };
    ((Control) this).AddChild((Control) this.Container);
    ChannelSelectorButton channelSelectorButton = new ChannelSelectorButton();
    ((Control) channelSelectorButton).Name = nameof (ChannelSelector);
    ((BaseButton) channelSelectorButton).ToggleMode = true;
    ((Control) channelSelectorButton).StyleClasses.Add("chatSelectorOptionButton");
    ((Control) channelSelectorButton).MinWidth = 75f;
    this.ChannelSelector = channelSelectorButton;
    ((Control) this.Container).AddChild((Control) this.ChannelSelector);
    HistoryLineEdit historyLineEdit = new HistoryLineEdit();
    ((Control) historyLineEdit).Name = nameof (Input);
    ((LineEdit) historyLineEdit).PlaceHolder = ChatInputBox.GetChatboxInfoPlaceholder();
    ((Control) historyLineEdit).HorizontalExpand = true;
    ((Control) historyLineEdit).StyleClasses.Add("chatLineEdit");
    this.Input = historyLineEdit;
    ((Control) this.Container).AddChild((Control) this.Input);
    ChannelFilterButton channelFilterButton = new ChannelFilterButton();
    ((Control) channelFilterButton).Name = nameof (FilterButton);
    ((Control) channelFilterButton).StyleClasses.Add("chatFilterOptionButton");
    this.FilterButton = channelFilterButton;
    ((Control) this.Container).AddChild((Control) this.FilterButton);
    ((Control) this).AddStyleClass("ChatSubPanel");
    this.ChannelSelector.OnChannelSelect += new Action<ChatSelectChannel>(this.UpdateActiveChannel);
  }

  private void UpdateActiveChannel(ChatSelectChannel selectedChannel)
  {
    this.ActiveChannel = (ChatChannel) selectedChannel;
  }

  private static string GetChatboxInfoPlaceholder()
  {
    int num = BoundKeyHelper.IsBound(ContentKeyFunctions.FocusChat) ? 1 : 0;
    bool flag = BoundKeyHelper.IsBound(ContentKeyFunctions.CycleChatChannelForward);
    string chatboxInfoPlaceholder;
    if (num != 0)
    {
      if (flag)
        chatboxInfoPlaceholder = Loc.GetString("hud-chatbox-info", new (string, object)[2]
        {
          ("talk-key", (object) BoundKeyHelper.ShortKeyName(ContentKeyFunctions.FocusChat)),
          ("cycle-key", (object) BoundKeyHelper.ShortKeyName(ContentKeyFunctions.CycleChatChannelForward))
        });
      else
        chatboxInfoPlaceholder = Loc.GetString("hud-chatbox-info-talk", new (string, object)[1]
        {
          ("talk-key", (object) BoundKeyHelper.ShortKeyName(ContentKeyFunctions.FocusChat))
        });
    }
    else if (flag)
      chatboxInfoPlaceholder = Loc.GetString("hud-chatbox-info-cycle", new (string, object)[1]
      {
        ("cycle-key", (object) BoundKeyHelper.ShortKeyName(ContentKeyFunctions.CycleChatChannelForward))
      });
    else
      chatboxInfoPlaceholder = Loc.GetString("hud-chatbox-info-unbound");
    return chatboxInfoPlaceholder;
  }
}
