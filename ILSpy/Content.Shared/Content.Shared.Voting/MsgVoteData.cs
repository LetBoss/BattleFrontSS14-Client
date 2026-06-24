using System;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Voting;

public sealed class MsgVoteData : NetMessage
{
	public int VoteId;

	public bool VoteActive;

	public string VoteTitle = string.Empty;

	public string VoteInitiator = string.Empty;

	public TimeSpan StartTime;

	public TimeSpan EndTime;

	public (ushort votes, string name)[] Options;

	public bool IsYourVoteDirty;

	public byte? YourVote;

	public bool DisplayVotes;

	public int TargetEntity;

	public override MsgGroups MsgGroup => (MsgGroups)4;

	public override NetDeliveryMethod DeliveryMethod => (NetDeliveryMethod)67;

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		VoteId = ((NetBuffer)buffer).ReadVariableInt32();
		VoteActive = ((NetBuffer)buffer).ReadBoolean();
		((NetBuffer)buffer).ReadPadBits();
		if (VoteActive)
		{
			VoteTitle = ((NetBuffer)buffer).ReadString();
			VoteInitiator = ((NetBuffer)buffer).ReadString();
			StartTime = TimeSpan.FromTicks(((NetBuffer)buffer).ReadInt64());
			EndTime = TimeSpan.FromTicks(((NetBuffer)buffer).ReadInt64());
			DisplayVotes = ((NetBuffer)buffer).ReadBoolean();
			TargetEntity = ((NetBuffer)buffer).ReadVariableInt32();
			Options = new(ushort, string)[((NetBuffer)buffer).ReadByte()];
			for (int i = 0; i < Options.Length; i++)
			{
				Options[i] = (votes: ((NetBuffer)buffer).ReadUInt16(), name: ((NetBuffer)buffer).ReadString());
			}
			IsYourVoteDirty = ((NetBuffer)buffer).ReadBoolean();
			if (IsYourVoteDirty)
			{
				YourVote = (((NetBuffer)buffer).ReadBoolean() ? new byte?(((NetBuffer)buffer).ReadByte()) : ((byte?)null));
			}
		}
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		((NetBuffer)buffer).WriteVariableInt32(VoteId);
		((NetBuffer)buffer).Write(VoteActive);
		((NetBuffer)buffer).WritePadBits();
		if (!VoteActive)
		{
			return;
		}
		((NetBuffer)buffer).Write(VoteTitle);
		((NetBuffer)buffer).Write(VoteInitiator);
		((NetBuffer)buffer).Write(StartTime.Ticks);
		((NetBuffer)buffer).Write(EndTime.Ticks);
		((NetBuffer)buffer).Write(DisplayVotes);
		((NetBuffer)buffer).WriteVariableInt32(TargetEntity);
		((NetBuffer)buffer).Write((byte)Options.Length);
		(ushort, string)[] options = Options;
		for (int i = 0; i < options.Length; i++)
		{
			var (votes, name) = options[i];
			((NetBuffer)buffer).Write(votes);
			((NetBuffer)buffer).Write(name);
		}
		((NetBuffer)buffer).Write(IsYourVoteDirty);
		if (IsYourVoteDirty)
		{
			((NetBuffer)buffer).Write(YourVote.HasValue);
			if (YourVote.HasValue)
			{
				((NetBuffer)buffer).Write(YourVote.Value);
			}
		}
	}
}
