// Decompiled with JetBrains decompiler
// Type: Content.Client.Voting.VoteManager
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

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
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Client.Voting;

public sealed class VoteManager : IVoteManager
{
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
  private readonly Dictionary<int, VoteManager.ActiveVote> _votes = new Dictionary<int, VoteManager.ActiveVote>();
  private readonly Dictionary<int, VotePopup> _votePopups = new Dictionary<int, VotePopup>();
  private Control? _popupContainer;
  private IAudioSource? _voteSource;

  public bool CanCallVote { get; private set; }

  public event Action<bool>? CanCallVoteChanged;

  public event Action? CanCallStandardVotesChanged;

  public void Initialize()
  {
    this._voteSource = this._audio.CreateAudioSource(AudioResource.op_Implicit(this._res.GetResource<AudioResource>("/Audio/Effects/voteding.ogg", true)));
    if (this._voteSource != null)
      this._voteSource.Global = true;
    // ISSUE: method pointer
    ((INetManager) this._netManager).RegisterNetMessage<MsgVoteData>(new ProcessMessage<MsgVoteData>((object) this, __methodptr(ReceiveVoteData)), (NetMessageAccept) 3);
    // ISSUE: method pointer
    ((INetManager) this._netManager).RegisterNetMessage<MsgVoteCanCall>(new ProcessMessage<MsgVoteCanCall>((object) this, __methodptr(ReceiveVoteCanCall)), (NetMessageAccept) 3);
    this._client.RunLevelChanged += new EventHandler<RunLevelChangedEventArgs>(this.ClientOnRunLevelChanged);
  }

  private void ClientOnRunLevelChanged(object? sender, RunLevelChangedEventArgs e)
  {
    if (e.NewLevel != 1)
      return;
    this.ClearPopupContainer();
    this._votes.Clear();
  }

  public bool CanCallStandardVote(StandardVoteType type, out TimeSpan whenCan)
  {
    return !this._standardVoteTimeouts.TryGetValue(type, out whenCan);
  }

  public void ClearPopupContainer()
  {
    if (this._popupContainer == null)
      return;
    if (!this._popupContainer.Disposed)
    {
      foreach (Control control in this._votePopups.Values)
        control.Orphan();
    }
    this._votePopups.Clear();
    this._popupContainer = (Control) null;
  }

  public void SetPopupContainer(Control container)
  {
    if (this._popupContainer != null)
      this.ClearPopupContainer();
    this._popupContainer = container;
    this.SetVoteData();
  }

  private void SetVoteData()
  {
    if (this._popupContainer == null)
      return;
    foreach ((int key, VoteManager.ActiveVote vote) in this._votes)
    {
      VotePopup votePopup = new VotePopup(vote);
      this._votePopups.Add(key, votePopup);
      this._popupContainer.AddChild((Control) votePopup);
      votePopup.UpdateData();
    }
  }

  private void ReceiveVoteData(MsgVoteData message)
  {
    bool flag = false;
    int voteId = message.VoteId;
    VoteManager.ActiveVote vote;
    if (!this._votes.TryGetValue(voteId, out vote))
    {
      if (!message.VoteActive)
        return;
      this._voteSource?.Restart();
      flag = true;
      Control popupContainer = this._popupContainer;
      this.ClearPopupContainer();
      if (popupContainer != null)
        this.SetPopupContainer(popupContainer);
      VoteManager.ActiveVote activeVote = new VoteManager.ActiveVote(voteId)
      {
        Entries = ((IEnumerable<(ushort, string)>) message.Options).Select<(ushort, string), VoteManager.VoteEntry>((Func<(ushort, string), VoteManager.VoteEntry>) (c => new VoteManager.VoteEntry(c.name))).ToArray<VoteManager.VoteEntry>()
      };
      vote = activeVote;
      this._votes.Add(voteId, activeVote);
    }
    else if (!message.VoteActive)
    {
      this._votes.Remove(voteId);
      VotePopup votePopup;
      if (!this._votePopups.TryGetValue(voteId, out votePopup))
        return;
      votePopup.Orphan();
      this._votePopups.Remove(voteId);
      return;
    }
    if (message.IsYourVoteDirty)
    {
      VoteManager.ActiveVote activeVote = vote;
      byte? yourVote = message.YourVote;
      int? nullable = yourVote.HasValue ? new int?((int) yourVote.GetValueOrDefault()) : new int?();
      activeVote.OurVote = nullable;
    }
    vote.Initiator = message.VoteInitiator;
    vote.Title = message.VoteTitle;
    vote.StartTime = this._gameTiming.RealServerToLocal(message.StartTime);
    vote.EndTime = this._gameTiming.RealServerToLocal(message.EndTime);
    vote.DisplayVotes = message.DisplayVotes;
    vote.TargetEntity = new int?(message.TargetEntity);
    for (int index = 0; index < message.Options.Length; ++index)
      vote.Entries[index].Votes = (int) message.Options[index].votes;
    if (flag && this._popupContainer != null)
    {
      VotePopup votePopup = new VotePopup(vote);
      this._votePopups.Add(voteId, votePopup);
      this._popupContainer.AddChild((Control) votePopup);
    }
    VotePopup votePopup1;
    if (!this._votePopups.TryGetValue(voteId, out votePopup1))
      return;
    votePopup1.UpdateData();
  }

  private void ReceiveVoteCanCall(MsgVoteCanCall message)
  {
    if (this.CanCallVote != message.CanCall)
    {
      this.CanCallVote = message.CanCall;
      Action<bool> canCallVoteChanged = this.CanCallVoteChanged;
      if (canCallVoteChanged != null)
        canCallVoteChanged(this.CanCallVote);
    }
    this._standardVoteTimeouts.Clear();
    foreach ((StandardVoteType standardVoteType, TimeSpan whenAvailable) in message.VotesUnavailable)
    {
      TimeSpan timeSpan = whenAvailable == TimeSpan.Zero ? whenAvailable : this._gameTiming.RealServerToLocal(whenAvailable);
      this._standardVoteTimeouts.Add(standardVoteType, timeSpan);
    }
    Action standardVotesChanged = this.CanCallStandardVotesChanged;
    if (standardVotesChanged == null)
      return;
    standardVotesChanged();
  }

  public void SendCastVote(int voteId, int option)
  {
    this._votes[voteId].OurVote = new int?(option);
    ((IConsoleHost) this._console).LocalShell.RemoteExecuteCommand($"vote {voteId} {option}");
  }

  public sealed class ActiveVote
  {
    public VoteManager.VoteEntry[] Entries;
    public TimeSpan StartTime;
    public TimeSpan EndTime;
    public string Title = "";
    public string Initiator = "";
    public int? OurVote;
    public int Id;
    public bool DisplayVotes;
    public int? TargetEntity;

    public ActiveVote(int voteId) => this.Id = voteId;
  }

  public sealed class VoteEntry
  {
    public string Text { get; }

    public int Votes { get; set; }

    public VoteEntry(string text) => this.Text = text;
  }
}
