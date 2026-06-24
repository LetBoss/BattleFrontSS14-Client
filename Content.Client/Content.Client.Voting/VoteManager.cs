using System;
using System.Collections.Generic;
using System.Linq;
using Content.Client.Voting.UI;
using Content.Shared.Voting;
using Robust.Client;
using Robust.Client.Audio;
using Robust.Client.Console;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Shared.Audio.Sources;
using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;

namespace Content.Client.Voting;

public sealed class VoteManager : IVoteManager
{
	public sealed class ActiveVote
	{
		public VoteEntry[] Entries;

		public TimeSpan StartTime;

		public TimeSpan EndTime;

		public string Title = "";

		public string Initiator = "";

		public int? OurVote;

		public int Id;

		public bool DisplayVotes;

		public int? TargetEntity;

		public ActiveVote(int voteId)
		{
			Id = voteId;
		}
	}

	public sealed class VoteEntry
	{
		public string Text { get; }

		public int Votes { get; set; }

		public VoteEntry(string text)
		{
			Text = text;
		}
	}

	[Dependency]
	private IAudioManager _audio;

	[Dependency]
	private IBaseClient _client;

	[Dependency]
	private IClientConsoleHost _console;

	[Dependency]
	private IClientNetManager _netManager;

	[Dependency]
	private IGameTiming _gameTiming;

	[Dependency]
	private IResourceCache _res;

	private readonly Dictionary<StandardVoteType, TimeSpan> _standardVoteTimeouts = new Dictionary<StandardVoteType, TimeSpan>();

	private readonly Dictionary<int, ActiveVote> _votes = new Dictionary<int, ActiveVote>();

	private readonly Dictionary<int, VotePopup> _votePopups = new Dictionary<int, VotePopup>();

	private Control? _popupContainer;

	private IAudioSource? _voteSource;

	public bool CanCallVote { get; private set; }

	public event Action<bool>? CanCallVoteChanged;

	public event Action? CanCallStandardVotesChanged;

	public void Initialize()
	{
		_voteSource = _audio.CreateAudioSource(AudioResource.op_Implicit(_res.GetResource<AudioResource>("/Audio/Effects/voteding.ogg", true)));
		if (_voteSource != null)
		{
			_voteSource.Global = true;
		}
		((INetManager)_netManager).RegisterNetMessage<MsgVoteData>((ProcessMessage<MsgVoteData>)ReceiveVoteData, (NetMessageAccept)3);
		((INetManager)_netManager).RegisterNetMessage<MsgVoteCanCall>((ProcessMessage<MsgVoteCanCall>)ReceiveVoteCanCall, (NetMessageAccept)3);
		_client.RunLevelChanged += ClientOnRunLevelChanged;
	}

	private void ClientOnRunLevelChanged(object? sender, RunLevelChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		if ((int)e.NewLevel == 1)
		{
			ClearPopupContainer();
			_votes.Clear();
		}
	}

	public bool CanCallStandardVote(StandardVoteType type, out TimeSpan whenCan)
	{
		return !_standardVoteTimeouts.TryGetValue(type, out whenCan);
	}

	public void ClearPopupContainer()
	{
		if (_popupContainer == null)
		{
			return;
		}
		if (!_popupContainer.Disposed)
		{
			foreach (VotePopup value in _votePopups.Values)
			{
				((Control)value).Orphan();
			}
		}
		_votePopups.Clear();
		_popupContainer = null;
	}

	public void SetPopupContainer(Control container)
	{
		if (_popupContainer != null)
		{
			ClearPopupContainer();
		}
		_popupContainer = container;
		SetVoteData();
	}

	private void SetVoteData()
	{
		if (_popupContainer == null)
		{
			return;
		}
		foreach (KeyValuePair<int, ActiveVote> vote in _votes)
		{
			vote.Deconstruct(out var key, out var value);
			int key2 = key;
			VotePopup votePopup = new VotePopup(value);
			_votePopups.Add(key2, votePopup);
			_popupContainer.AddChild((Control)(object)votePopup);
			votePopup.UpdateData();
		}
	}

	private void ReceiveVoteData(MsgVoteData message)
	{
		bool flag = false;
		int voteId = message.VoteId;
		if (!_votes.TryGetValue(voteId, out ActiveVote value))
		{
			if (!message.VoteActive)
			{
				return;
			}
			IAudioSource? voteSource = _voteSource;
			if (voteSource != null)
			{
				voteSource.Restart();
			}
			flag = true;
			Control popupContainer = _popupContainer;
			ClearPopupContainer();
			if (popupContainer != null)
			{
				SetPopupContainer(popupContainer);
			}
			ActiveVote activeVote = new ActiveVote(voteId)
			{
				Entries = message.Options.Select(((ushort votes, string name) c) => new VoteEntry(c.name)).ToArray()
			};
			value = activeVote;
			_votes.Add(voteId, activeVote);
		}
		else if (!message.VoteActive)
		{
			_votes.Remove(voteId);
			if (_votePopups.TryGetValue(voteId, out VotePopup value2))
			{
				((Control)value2).Orphan();
				_votePopups.Remove(voteId);
			}
			return;
		}
		if (message.IsYourVoteDirty)
		{
			value.OurVote = message.YourVote;
		}
		value.Initiator = message.VoteInitiator;
		value.Title = message.VoteTitle;
		value.StartTime = _gameTiming.RealServerToLocal(message.StartTime);
		value.EndTime = _gameTiming.RealServerToLocal(message.EndTime);
		value.DisplayVotes = message.DisplayVotes;
		value.TargetEntity = message.TargetEntity;
		for (int num = 0; num < message.Options.Length; num++)
		{
			value.Entries[num].Votes = message.Options[num].votes;
		}
		if (flag && _popupContainer != null)
		{
			VotePopup votePopup = new VotePopup(value);
			_votePopups.Add(voteId, votePopup);
			_popupContainer.AddChild((Control)(object)votePopup);
		}
		if (_votePopups.TryGetValue(voteId, out VotePopup value3))
		{
			value3.UpdateData();
		}
	}

	private void ReceiveVoteCanCall(MsgVoteCanCall message)
	{
		if (CanCallVote != message.CanCall)
		{
			CanCallVote = message.CanCall;
			this.CanCallVoteChanged?.Invoke(CanCallVote);
		}
		_standardVoteTimeouts.Clear();
		(StandardVoteType, TimeSpan)[] votesUnavailable = message.VotesUnavailable;
		for (int i = 0; i < votesUnavailable.Length; i++)
		{
			(StandardVoteType, TimeSpan) tuple = votesUnavailable[i];
			StandardVoteType item = tuple.Item1;
			TimeSpan item2 = tuple.Item2;
			TimeSpan value = ((item2 == TimeSpan.Zero) ? item2 : _gameTiming.RealServerToLocal(item2));
			_standardVoteTimeouts.Add(item, value);
		}
		this.CanCallStandardVotesChanged?.Invoke();
	}

	public void SendCastVote(int voteId, int option)
	{
		_votes[voteId].OurVote = option;
		((IConsoleHost)_console).LocalShell.RemoteExecuteCommand($"vote {voteId} {option}");
	}
}
