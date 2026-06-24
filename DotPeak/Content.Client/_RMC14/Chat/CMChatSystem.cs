// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Chat.CMChatSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.UserInterface.Systems.Chat.Widgets;
using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Chat;
using Content.Shared.Chat;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Client._RMC14.Chat;

public sealed class CMChatSystem : SharedCMChatSystem
{
  [Dependency]
  private IConfigurationManager _config;
  private int _repeatHistory;

  public override void Initialize()
  {
    base.Initialize();
    EntitySystemSubscriptionExt.CVar<int>(this.Subs, this._config, RMCCVars.RMCChatRepeatHistory, (Action<int>) (v => this._repeatHistory = v), true);
  }

  public bool TryRepetition(
    ChatBox chat,
    OutputPanel contents,
    FormattedMessage message,
    NetEntity sender,
    string unwrapped,
    ChatChannel channel,
    bool repeatCheckSender)
  {
    bool flag = false;
    foreach (RepeatedMessage repeat in chat.RepeatQueue)
    {
      if (repeat.Message.Equals(unwrapped) && repeat.Channel == channel && (!repeatCheckSender || ((NetEntity) ref repeat.SenderEntity).Equals(sender)))
      {
        FormattedMessage formattedMessage = new FormattedMessage(repeat.FormattedMessage);
        ++repeat.Count;
        formattedMessage.AddMarkupPermissive($" [color=red]x{repeat.Count}[/color]");
        contents.SetMessage((Index) repeat.Index, formattedMessage, new Color?());
        flag = true;
        break;
      }
    }
    if (!flag)
    {
      chat.RepeatQueue.Enqueue(new RepeatedMessage(contents.EntryCount, message, sender, unwrapped, channel));
      if (this._repeatHistory > 0)
      {
        while (chat.RepeatQueue.Count > this._repeatHistory)
          chat.RepeatQueue.Dequeue();
      }
    }
    return flag;
  }
}
