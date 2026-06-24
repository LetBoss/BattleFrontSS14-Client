using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Info;

public sealed class SendRulesInformationMessage : NetMessage
{
	public override MsgGroups MsgGroup => (MsgGroups)4;

	public float PopupTime { get; set; }

	public string CoreRules { get; set; } = string.Empty;

	public bool ShouldShowRules { get; set; }

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		PopupTime = ((NetBuffer)buffer).ReadFloat();
		CoreRules = ((NetBuffer)buffer).ReadString();
		ShouldShowRules = ((NetBuffer)buffer).ReadBoolean();
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		((NetBuffer)buffer).Write(PopupTime);
		((NetBuffer)buffer).Write(CoreRules);
		((NetBuffer)buffer).Write(ShouldShowRules);
	}
}
