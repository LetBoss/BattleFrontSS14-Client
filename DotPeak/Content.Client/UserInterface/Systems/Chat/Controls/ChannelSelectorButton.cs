// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.Chat.Controls.ChannelSelectorButton
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Chat;
using Content.Shared.Radio;
using Robust.Client.UserInterface;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.UserInterface.Systems.Chat.Controls;

public sealed class ChannelSelectorButton : ChatPopupButton<ChannelSelectorPopup>
{
  private const int SelectorDropdownOffset = 38;

  public event Action<ChatSelectChannel>? OnChannelSelect;

  public ChatSelectChannel SelectedChannel { get; private set; }

  public ChannelSelectorButton()
  {
    ((Control) this).Name = "ChannelSelector";
    this.Popup.Selected += new Action<ChatSelectChannel>(this.OnChannelSelected);
    ChatSelectChannel? firstChannel = this.Popup.FirstChannel;
    if (!firstChannel.HasValue)
      return;
    this.Select(firstChannel.GetValueOrDefault());
  }

  protected override UIBox2 GetPopupPosition()
  {
    Vector2 vector2_1 = new Vector2(((Control) this).GlobalPosition.X, ((Control) this).GlobalPosition.Y + ((Control) this).Height);
    UIBox2 sizeBox = ((Control) this).SizeBox;
    Vector2 vector2_2 = new Vector2(((UIBox2) ref sizeBox).Width, 38f);
    return UIBox2.FromDimensions(vector2_1, vector2_2);
  }

  private void OnChannelSelected(ChatSelectChannel channel) => this.Select(channel);

  public void Select(ChatSelectChannel channel)
  {
    if (((Control) this.Popup).Visible)
      this.Popup.Close();
    if (this.SelectedChannel == channel)
      return;
    this.SelectedChannel = channel;
    Action<ChatSelectChannel> onChannelSelect = this.OnChannelSelect;
    if (onChannelSelect == null)
      return;
    onChannelSelect(channel);
  }

  public static string ChannelSelectorName(ChatSelectChannel channel)
  {
    return Loc.GetString($"hud-chatbox-select-channel-{channel}");
  }

  public Color ChannelSelectColor(ChatSelectChannel channel)
  {
    Color color;
    switch (channel)
    {
      case ChatSelectChannel.Radio:
        color = Color.LimeGreen;
        break;
      case ChatSelectChannel.LOOC:
        color = Color.MediumTurquoise;
        break;
      case ChatSelectChannel.OOC:
        color = Color.LightSkyBlue;
        break;
      case ChatSelectChannel.Dead:
        color = Color.MediumPurple;
        break;
      case ChatSelectChannel.Admin:
        color = Color.HotPink;
        break;
      case ChatSelectChannel.Mentor:
        color = Color.Orange;
        break;
      case ChatSelectChannel.Party:
        color = Color.Gold;
        break;
      case ChatSelectChannel.MiniGame:
        color = Color.DeepSkyBlue;
        break;
      case ChatSelectChannel.Lobby:
        color = Color.LightGreen;
        break;
      default:
        color = Color.DarkGray;
        break;
    }
    return color;
  }

  public void UpdateChannelSelectButton(
    ChatSelectChannel channel,
    RadioChannelPrototype? radio,
    string? textOverride = null,
    Color? colorOverride = null)
  {
    this.Text = textOverride ?? (radio != null ? Loc.GetString(LocId.op_Implicit(radio.Name)) : ChannelSelectorButton.ChannelSelectorName(channel));
    ((Control) this).Modulate = colorOverride ?? (radio != null ? radio.Color : this.ChannelSelectColor(channel));
  }
}
