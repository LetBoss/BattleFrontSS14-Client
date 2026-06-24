using System;
using Lidgren.Network;
using Robust.Shared.Serialization;

namespace Robust.Shared.Network;

public sealed class MsgStringTableEntries : NetMessage
{
	public struct Entry
	{
		public string String { get; set; }

		public int Id { get; set; }
	}

	public override MsgGroups MsgGroup => MsgGroups.String;

	public Entry[] Entries { get; set; }

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		uint num = ((NetBuffer)buffer).ReadUInt32();
		Entries = new Entry[num];
		for (int i = 0; i < num; i++)
		{
			Entries[i].Id = ((NetBuffer)buffer).ReadVariableInt32();
			Entries[i].String = ((NetBuffer)buffer).ReadString();
		}
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		if (Entries == null)
		{
			throw new InvalidOperationException("Entries is null!");
		}
		((NetBuffer)buffer).Write(Entries.Length);
		Entry[] entries = Entries;
		for (int i = 0; i < entries.Length; i++)
		{
			Entry entry = entries[i];
			((NetBuffer)buffer).WriteVariableInt32(entry.Id);
			((NetBuffer)buffer).Write(entry.String);
		}
	}
}
