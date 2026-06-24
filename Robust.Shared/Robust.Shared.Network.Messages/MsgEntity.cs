using System;
using System.IO;
using Lidgren.Network;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Robust.Shared.Network.Messages;

public sealed class MsgEntity : NetMessage
{
	public override MsgGroups MsgGroup => MsgGroups.EntityEvent;

	public EntityMessageType Type { get; set; }

	public EntityEventArgs SystemMessage { get; set; }

	public EntityUid EntityUid { get; set; }

	public uint NetId { get; set; }

	public uint Sequence { get; set; }

	public GameTick SourceTick { get; set; }

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		Type = (EntityMessageType)((NetBuffer)buffer).ReadByte();
		SourceTick = buffer.ReadGameTick();
		Sequence = ((NetBuffer)buffer).ReadUInt32();
		if (Type == EntityMessageType.SystemMessage)
		{
			int length = ((NetBuffer)buffer).ReadVariableInt32();
			using MemoryStream memoryStream = RobustMemoryManager.GetMemoryStream(length);
			buffer.ReadAlignedMemory(memoryStream, length);
			SystemMessage = serializer.Deserialize<EntityEventArgs>(memoryStream);
		}
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		((NetBuffer)buffer).Write((byte)Type);
		buffer.Write(SourceTick);
		((NetBuffer)buffer).Write(Sequence);
		if (Type == EntityMessageType.SystemMessage)
		{
			MemoryStream memoryStream = new MemoryStream();
			serializer.Serialize(memoryStream, SystemMessage);
			((NetBuffer)buffer).WriteVariableInt32((int)memoryStream.Length);
			((NetBuffer)buffer).Write((ReadOnlySpan<byte>)memoryStream.AsSpan());
		}
	}

	public override string ToString()
	{
		string value = $"T: {SourceTick} S: {Sequence}";
		return Type switch
		{
			EntityMessageType.Error => "MsgEntity Error", 
			EntityMessageType.SystemMessage => $"MsgEntity Comp, {value}, {SystemMessage}", 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}
}
