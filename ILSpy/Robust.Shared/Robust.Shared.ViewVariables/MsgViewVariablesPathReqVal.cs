using Lidgren.Network;
using Robust.Shared.Serialization;

namespace Robust.Shared.ViewVariables;

internal abstract class MsgViewVariablesPathReqVal : MsgViewVariablesPathReq
{
	public string? Value { get; set; }

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		base.ReadFromBuffer(buffer, serializer);
		Value = ((NetBuffer)buffer).ReadString();
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		base.WriteToBuffer(buffer, serializer);
		((NetBuffer)buffer).Write(Value);
	}
}
