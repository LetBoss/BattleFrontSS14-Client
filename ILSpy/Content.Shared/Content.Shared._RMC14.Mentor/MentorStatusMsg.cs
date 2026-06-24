using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Mentor;

public sealed class MentorStatusMsg : NetMessage
{
	public bool IsMentor;

	public bool CanReMentor;

	public override MsgGroups MsgGroup => (MsgGroups)1;

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		IsMentor = ((NetBuffer)buffer).ReadBoolean();
		CanReMentor = ((NetBuffer)buffer).ReadBoolean();
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		((NetBuffer)buffer).Write(IsMentor);
		((NetBuffer)buffer).Write(CanReMentor);
	}
}
