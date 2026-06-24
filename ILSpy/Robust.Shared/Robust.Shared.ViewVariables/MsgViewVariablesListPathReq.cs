using System;
using System.IO;
using Lidgren.Network;
using Robust.Shared.GameObjects;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Robust.Shared.ViewVariables;

internal sealed class MsgViewVariablesListPathReq : MsgViewVariablesPathReq
{
	public VVListPathOptions Options { get; set; }

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		base.ReadFromBuffer(buffer, serializer);
		int length = ((NetBuffer)buffer).ReadInt32();
		using MemoryStream memoryStream = RobustMemoryManager.GetMemoryStream(length);
		buffer.ReadAlignedMemory(memoryStream, length);
		Options = serializer.Deserialize<VVListPathOptions>(memoryStream);
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		base.WriteToBuffer(buffer, serializer);
		MemoryStream memoryStream = new MemoryStream();
		serializer.Serialize(memoryStream, Options);
		((NetBuffer)buffer).Write((int)memoryStream.Length);
		((NetBuffer)buffer).Write((ReadOnlySpan<byte>)memoryStream.AsSpan());
	}
}
