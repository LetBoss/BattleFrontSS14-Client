// Decompiled with JetBrains decompiler
// Type: Content.Shared.Voting.MsgVoteCanCall
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Voting;

public sealed class MsgVoteCanCall : NetMessage
{
  public bool CanCall;
  public TimeSpan WhenCanCallVote;
  public (StandardVoteType type, TimeSpan whenAvailable)[] VotesUnavailable;

  public override MsgGroups MsgGroup => MsgGroups.Command;

  public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
    this.CanCall = ((NetBuffer) buffer).ReadBoolean();
    ((NetBuffer) buffer).ReadPadBits();
    this.WhenCanCallVote = TimeSpan.FromTicks(((NetBuffer) buffer).ReadInt64());
    byte length = ((NetBuffer) buffer).ReadByte();
    this.VotesUnavailable = new (StandardVoteType, TimeSpan)[(int) length];
    for (int index = 0; index < (int) length; ++index)
    {
      StandardVoteType standardVoteType = (StandardVoteType) ((NetBuffer) buffer).ReadByte();
      TimeSpan timeSpan = TimeSpan.FromTicks(((NetBuffer) buffer).ReadInt64());
      this.VotesUnavailable[index] = (standardVoteType, timeSpan);
    }
  }

  public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    ((NetBuffer) buffer).Write(this.CanCall);
    ((NetBuffer) buffer).WritePadBits();
    ((NetBuffer) buffer).Write(this.WhenCanCallVote.Ticks);
    ((NetBuffer) buffer).Write((byte) this.VotesUnavailable.Length);
    foreach ((StandardVoteType type, TimeSpan whenAvailable) in this.VotesUnavailable)
    {
      ((NetBuffer) buffer).Write((byte) type);
      ((NetBuffer) buffer).Write(whenAvailable.Ticks);
    }
  }
}
