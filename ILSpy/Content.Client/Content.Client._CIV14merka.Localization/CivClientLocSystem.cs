using System;
using Content.Client.UserInterface.Systems.Chat;
using Content.Shared._CIV14merka.Chat;
using Content.Shared._CIV14merka.Localization;
using Content.Shared.Chat;
using Content.Shared.Popups;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._CIV14merka.Localization;

public sealed class CivClientLocSystem : EntitySystem
{
	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private IUserInterfaceManager _ui;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<CivLocPopupEvent>((EntityEventHandler<CivLocPopupEvent>)OnLocPopup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<CivLocChatEvent>((EntityEventHandler<CivLocChatEvent>)OnLocChat, (Type[])null, (Type[])null);
	}

	private void OnLocPopup(CivLocPopupEvent ev)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		string message = ev.Message.Resolve();
		NetEntity? at = ev.At;
		if (at.HasValue)
		{
			NetEntity valueOrDefault = at.GetValueOrDefault();
			EntityUid? val = default(EntityUid?);
			if (((EntitySystem)this).TryGetEntity(valueOrDefault, ref val))
			{
				_popup.PopupEntity(message, val.Value, ev.Type);
				return;
			}
		}
		_popup.PopupCursor(message, ev.Type);
	}

	private void OnLocChat(CivLocChatEvent ev)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		string text = ev.Body.Resolve();
		string wrappedMessage = CivAnnouncementFormat.BuildWrapped(ev.Kind, ev.Color, ev.SideColor, ev.SideTag, text, ev.Speaker, ev.ChannelTag);
		ChatMessage msg = new ChatMessage(ChatChannel.Radio, text, wrappedMessage, NetEntity.Invalid, null, hideChat: false, ev.Color);
		_ui.GetUIController<ChatUIController>().ProcessChatMessage(msg, speechBubble: false);
	}
}
