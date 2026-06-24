using System;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Robust.Shared.Upload;

[Obsolete("The engine no longer uses this message")]
public sealed class NetworkResourceUploadMessage : NetMessage
{
	public override MsgGroups MsgGroup => MsgGroups.String;

	public byte[] Data { get; set; } = Array.Empty<byte>();

	public ResPath RelativePath { get; set; } = ResPath.Self;

	public NetworkResourceUploadMessage()
	{
	}

	public NetworkResourceUploadMessage(byte[] data, ResPath relativePath)
	{
		Data = data;
		RelativePath = relativePath;
	}

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		int num = ((NetBuffer)buffer).ReadVariableInt32();
		Data = ((NetBuffer)buffer).ReadBytes(num);
		RelativePath = new ResPath(((NetBuffer)buffer).ReadString());
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		((NetBuffer)buffer).WriteVariableInt32(Data.Length);
		((NetBuffer)buffer).Write(Data);
		((NetBuffer)buffer).Write(RelativePath.ToString());
		((NetBuffer)buffer).Write((ushort)47);
	}
}
