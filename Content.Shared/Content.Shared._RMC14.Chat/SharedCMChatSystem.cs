using System;
using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Marines;
using Content.Shared._RMC14.Marines.Squads;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Chat;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;

namespace Content.Shared._RMC14.Chat;

public abstract class SharedCMChatSystem : EntitySystem
{
	[Dependency]
	private IConfigurationManager _config;

	[Dependency]
	private SquadSystem _squadSystem;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<MarineComponent, ChatGetPrefixEvent>((EntityEventRefHandler<MarineComponent, ChatGetPrefixEvent>)OnMarineGetPrefix, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoComponent, ChatGetPrefixEvent>((EntityEventRefHandler<XenoComponent, ChatGetPrefixEvent>)OnXenoGetPrefix, (Type[])null, (Type[])null);
	}

	private void OnMarineGetPrefix(Entity<MarineComponent> ent, ref ChatGetPrefixEvent args)
	{
		if (args.Channel?.ID == SharedChatSystem.HivemindChannel.Id)
		{
			args.Channel = null;
		}
	}

	private void OnXenoGetPrefix(Entity<XenoComponent> ent, ref ChatGetPrefixEvent args)
	{
		if (args.Channel?.ID != SharedChatSystem.HivemindChannel.Id)
		{
			args.Channel = null;
		}
	}

	public virtual string SanitizeMessageReplaceWords(EntityUid source, string msg)
	{
		return msg;
	}

	public virtual void ChatMessageToOne(ChatChannel channel, string message, string wrappedMessage, EntityUid source, bool hideChat, INetChannel client, Color? colorOverride = null, bool recordReplay = false, string? audioPath = null, float audioVolume = 0f, NetUserId? author = null)
	{
	}

	public void ChatMessageToOne(string message, EntityUid target, ChatChannel channel = ChatChannel.Local, bool hideChat = false, Color? colorOverride = null, bool recordReplay = false, string? audioPath = null, float audioVolume = 0f, NetUserId? author = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		ActorComponent actor = default(ActorComponent);
		if (((EntitySystem)this).TryComp<ActorComponent>(target, ref actor))
		{
			ChatMessageToOne(channel, message, message, default(EntityUid), hideChat, actor.PlayerSession.Channel, colorOverride, recordReplay, audioPath, audioVolume, author);
		}
	}

	public virtual void ChatMessageToMany(string message, string wrappedMessage, Filter filter, ChatChannel channel, EntityUid source = default(EntityUid), bool hideChat = false, Color? colorOverride = null, bool recordReplay = false, string? audioPath = null, float audioVolume = 0f, NetUserId? author = null)
	{
	}

	public virtual void Emote(EntityUid source, string message, string? nameOverride = null, bool checkRadioPrefix = true, bool ignoreActionBlocker = false)
	{
	}

	public string? ColorizeSpeakerNameBySquadOrNull(ChatMessage msg)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		bool cVar = _config.GetCVar<bool>(RMCCVars.RMCChatSquadColorMode);
		Color? squadColor = null;
		if (cVar && _squadSystem.TryGetSquadMemberColor(((EntitySystem)this).GetEntity(msg.SenderEntity), out var color, accessible: true))
		{
			squadColor = color;
		}
		if (squadColor.HasValue)
		{
			Color value = squadColor.Value;
			msg.WrappedMessage = SharedChatSystem.InjectTagInsideTag(msg, "Name", "color", ((Color)(ref value)).ToHex());
			return msg.WrappedMessage;
		}
		return null;
	}
}
