// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.Chat.Controls.ChannelFilterCheckbox
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Chat;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Localization;

#nullable disable
namespace Content.Client.UserInterface.Systems.Chat.Controls;

public sealed class ChannelFilterCheckbox : CheckBox
{
  public readonly ChatChannel Channel;

  public bool IsHidden => ((Control) this).Parent == null;

  public ChannelFilterCheckbox(ChatChannel channel)
  {
    this.Channel = channel;
    this.Text = Loc.GetString($"hud-chatbox-channel-{this.Channel}");
  }

  private void UpdateText(int? unread)
  {
    string str1 = Loc.GetString($"hud-chatbox-channel-{this.Channel}");
    int? nullable1 = unread;
    int num1 = 0;
    if (nullable1.GetValueOrDefault() > num1 & nullable1.HasValue)
    {
      string str2 = str1;
      int? nullable2 = unread;
      int num2 = 9;
      string str3 = (nullable2.GetValueOrDefault() > num2 & nullable2.HasValue ? (object) "9+" : (object) unread)?.ToString();
      str1 = $"{str2} ({str3})";
    }
    this.Text = str1;
  }

  public void UpdateUnreadCount(int? unread) => this.UpdateText(unread);
}
