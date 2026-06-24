using System;
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

namespace Content.Client._RMC14.Chat;

public sealed class CMChatSystem : SharedCMChatSystem
{
	[Dependency]
	private IConfigurationManager _config;

	private int _repeatHistory;

	public override void Initialize()
	{
		base.Initialize();
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _config, RMCCVars.RMCChatRepeatHistory, (Action<int>)delegate(int v)
		{
			_repeatHistory = v;
		}, true);
	}

	public bool TryRepetition(ChatBox chat, OutputPanel contents, FormattedMessage message, NetEntity sender, string unwrapped, ChatChannel channel, bool repeatCheckSender)
	{
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		foreach (RepeatedMessage item in chat.RepeatQueue)
		{
			if (item.Message.Equals(unwrapped) && item.Channel == channel && (!repeatCheckSender || ((NetEntity)(ref item.SenderEntity)).Equals(sender)))
			{
				FormattedMessage val = new FormattedMessage(item.FormattedMessage);
				item.Count++;
				val.AddMarkupPermissive($" [color=red]x{item.Count}[/color]");
				contents.SetMessage((Index)item.Index, val, (Color?)null);
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			chat.RepeatQueue.Enqueue(new RepeatedMessage(contents.EntryCount, message, sender, unwrapped, channel));
			if (_repeatHistory > 0)
			{
				while (chat.RepeatQueue.Count > _repeatHistory)
				{
					chat.RepeatQueue.Dequeue();
				}
			}
		}
		return flag;
	}
}
