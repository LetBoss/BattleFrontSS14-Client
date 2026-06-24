// Decompiled with JetBrains decompiler
// Type: Content.Client.Voting.IVoteManager
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Voting;
using Robust.Client.UserInterface;
using System;

#nullable enable
namespace Content.Client.Voting;

public interface IVoteManager
{
  void Initialize();

  void SendCastVote(int voteId, int option);

  void ClearPopupContainer();

  void SetPopupContainer(Control container);

  bool CanCallVote { get; }

  bool CanCallStandardVote(StandardVoteType type, out TimeSpan whenCan);

  event Action<bool> CanCallVoteChanged;

  event Action CanCallStandardVotesChanged;
}
