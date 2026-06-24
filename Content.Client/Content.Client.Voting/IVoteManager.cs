using System;
using Content.Shared.Voting;
using Robust.Client.UserInterface;

namespace Content.Client.Voting;

public interface IVoteManager
{
	bool CanCallVote { get; }

	event Action<bool> CanCallVoteChanged;

	event Action CanCallStandardVotesChanged;

	void Initialize();

	void SendCastVote(int voteId, int option);

	void ClearPopupContainer();

	void SetPopupContainer(Control container);

	bool CanCallStandardVote(StandardVoteType type, out TimeSpan whenCan);
}
