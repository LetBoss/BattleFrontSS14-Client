using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Chat;

[Serializable]
[NetSerializable]
public sealed class ChatMessage
{
	public ChatChannel Channel;

	public string Message;

	public string WrappedMessage;

	public NetEntity SenderEntity;

	public int? SenderKey;

	public bool HideChat;

	public Color? MessageColorOverride;

	public string? AudioPath;

	public float AudioVolume;

	public bool HidePopup;

	public string? SpeechStyleClass;

	public bool RepeatCheckSender;

	[NonSerialized]
	public bool Read;

	public ChatMessage(ChatChannel channel, string message, string wrappedMessage, NetEntity source, int? senderKey, bool hideChat = false, Color? colorOverride = null, string? audioPath = null, float audioVolume = 0f, bool hidePopup = false, string? speechStyleClass = null, bool repeatCheckSender = true)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		Channel = channel;
		Message = message;
		WrappedMessage = wrappedMessage;
		SenderEntity = source;
		SenderKey = senderKey;
		HideChat = hideChat;
		MessageColorOverride = colorOverride;
		AudioPath = audioPath;
		AudioVolume = audioVolume;
		HidePopup = hidePopup;
		SpeechStyleClass = speechStyleClass;
		RepeatCheckSender = repeatCheckSender;
	}
}
