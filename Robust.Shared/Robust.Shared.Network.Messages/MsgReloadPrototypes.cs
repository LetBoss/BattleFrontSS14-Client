using Lidgren.Network;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Robust.Shared.Network.Messages;

public sealed class MsgReloadPrototypes : NetMessage
{
	public ResPath[] Paths;

	public override MsgGroups MsgGroup => MsgGroups.Command;

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		int num = ((NetBuffer)buffer).ReadInt32();
		Paths = new ResPath[num];
		for (int i = 0; i < num; i++)
		{
			Paths[i] = new ResPath(((NetBuffer)buffer).ReadString());
		}
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		((NetBuffer)buffer).Write(Paths.Length);
		ResPath[] paths = Paths;
		for (int i = 0; i < paths.Length; i++)
		{
			ResPath resPath = paths[i];
			((NetBuffer)buffer).Write(resPath.ToString());
		}
	}
}
