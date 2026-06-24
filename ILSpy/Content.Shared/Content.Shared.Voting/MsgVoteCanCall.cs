using System;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Voting;

public sealed class MsgVoteCanCall : NetMessage
{
	public bool CanCall;

	public TimeSpan WhenCanCallVote;

	public (StandardVoteType type, TimeSpan whenAvailable)[] VotesUnavailable;

	public override MsgGroups MsgGroup => (MsgGroups)4;

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		CanCall = ((NetBuffer)buffer).ReadBoolean();
		((NetBuffer)buffer).ReadPadBits();
		WhenCanCallVote = TimeSpan.FromTicks(((NetBuffer)buffer).ReadInt64());
		byte lenVotes = ((NetBuffer)buffer).ReadByte();
		VotesUnavailable = new(StandardVoteType, TimeSpan)[lenVotes];
		for (int i = 0; i < lenVotes; i++)
		{
			StandardVoteType type = (StandardVoteType)((NetBuffer)buffer).ReadByte();
			TimeSpan timeOut = TimeSpan.FromTicks(((NetBuffer)buffer).ReadInt64());
			VotesUnavailable[i] = (type: type, whenAvailable: timeOut);
		}
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		((NetBuffer)buffer).Write(CanCall);
		((NetBuffer)buffer).WritePadBits();
		((NetBuffer)buffer).Write(WhenCanCallVote.Ticks);
		((NetBuffer)buffer).Write((byte)VotesUnavailable.Length);
		(StandardVoteType, TimeSpan)[] votesUnavailable = VotesUnavailable;
		for (int i = 0; i < votesUnavailable.Length; i++)
		{
			var (type, timeout) = votesUnavailable[i];
			((NetBuffer)buffer).Write((byte)type);
			((NetBuffer)buffer).Write(timeout.Ticks);
		}
	}
}
