using Lidgren.Network;
using Robust.Shared.Serialization;

namespace Robust.Shared.Network.Messages;

public sealed class MsgConCompletion : NetMessage
{
	public override MsgGroups MsgGroup => MsgGroups.Command;

	public int Seq { get; set; }

	public string[] Args { get; set; }

	public string ArgString { get; set; }

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		Seq = ((NetBuffer)buffer).ReadInt32();
		int num = ((NetBuffer)buffer).ReadVariableInt32();
		Args = new string[num];
		for (int i = 0; i < num; i++)
		{
			Args[i] = ((NetBuffer)buffer).ReadString();
		}
		ArgString = ((NetBuffer)buffer).ReadString();
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		((NetBuffer)buffer).Write(Seq);
		((NetBuffer)buffer).WriteVariableInt32(Args.Length);
		string[] args = Args;
		foreach (string text in args)
		{
			((NetBuffer)buffer).Write(text);
		}
		((NetBuffer)buffer).Write(ArgString);
	}
}
