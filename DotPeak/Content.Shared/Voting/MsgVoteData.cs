// Decompiled with JetBrains decompiler
// Type: Content.Shared.Voting.MsgVoteData
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using System;

#nullable enable
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

  public override MsgGroups MsgGroup => MsgGroups.Command;

  public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
    this.VoteId = ((NetBuffer) buffer).ReadVariableInt32();
    this.VoteActive = ((NetBuffer) buffer).ReadBoolean();
    ((NetBuffer) buffer).ReadPadBits();
    if (!this.VoteActive)
      return;
    this.VoteTitle = ((NetBuffer) buffer).ReadString();
    this.VoteInitiator = ((NetBuffer) buffer).ReadString();
    this.StartTime = TimeSpan.FromTicks(((NetBuffer) buffer).ReadInt64());
    this.EndTime = TimeSpan.FromTicks(((NetBuffer) buffer).ReadInt64());
    this.DisplayVotes = ((NetBuffer) buffer).ReadBoolean();
    this.TargetEntity = ((NetBuffer) buffer).ReadVariableInt32();
    this.Options = new (ushort, string)[(int) ((NetBuffer) buffer).ReadByte()];
    for (int index = 0; index < this.Options.Length; ++index)
      this.Options[index] = (((NetBuffer) buffer).ReadUInt16(), ((NetBuffer) buffer).ReadString());
    this.IsYourVoteDirty = ((NetBuffer) buffer).ReadBoolean();
    if (!this.IsYourVoteDirty)
      return;
    this.YourVote = ((NetBuffer) buffer).ReadBoolean() ? new byte?(((NetBuffer) buffer).ReadByte()) : new byte?();
  }

  public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    ((NetBuffer) buffer).WriteVariableInt32(this.VoteId);
    ((NetBuffer) buffer).Write(this.VoteActive);
    ((NetBuffer) buffer).WritePadBits();
    if (!this.VoteActive)
      return;
    ((NetBuffer) buffer).Write(this.VoteTitle);
    ((NetBuffer) buffer).Write(this.VoteInitiator);
    ((NetBuffer) buffer).Write(this.StartTime.Ticks);
    ((NetBuffer) buffer).Write(this.EndTime.Ticks);
    ((NetBuffer) buffer).Write(this.DisplayVotes);
    ((NetBuffer) buffer).WriteVariableInt32(this.TargetEntity);
    ((NetBuffer) buffer).Write((byte) this.Options.Length);
    foreach ((ushort votes, string name) in this.Options)
    {
      ((NetBuffer) buffer).Write(votes);
      ((NetBuffer) buffer).Write(name);
    }
    ((NetBuffer) buffer).Write(this.IsYourVoteDirty);
    if (!this.IsYourVoteDirty)
      return;
    ((NetBuffer) buffer).Write(this.YourVote.HasValue);
    if (!this.YourVote.HasValue)
      return;
    ((NetBuffer) buffer).Write(this.YourVote.Value);
  }

  public override NetDeliveryMethod DeliveryMethod => (NetDeliveryMethod) 67;
}
