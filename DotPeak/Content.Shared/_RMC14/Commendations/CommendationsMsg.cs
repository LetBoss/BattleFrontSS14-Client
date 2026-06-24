// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Commendations.CommendationsMsg
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Database;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Commendations;

public sealed class CommendationsMsg : NetMessage
{
  public List<Commendation> CommendationsReceived = new List<Commendation>();
  public List<Commendation> CommendationsGiven = new List<Commendation>();

  public override MsgGroups MsgGroup => MsgGroups.Core;

  public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
    this.CommendationsReceived.Clear();
    this.CommendationsGiven.Clear();
    this.ReadCommendations(buffer, this.CommendationsReceived);
    this.ReadCommendations(buffer, this.CommendationsGiven);
  }

  public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    this.WriteCommendations(buffer, this.CommendationsReceived);
    this.WriteCommendations(buffer, this.CommendationsGiven);
  }

  private void ReadCommendations(NetIncomingMessage buffer, List<Commendation> commendations)
  {
    int num = ((NetBuffer) buffer).ReadInt32();
    for (int index = 0; index < num; ++index)
    {
      string Giver = ((NetBuffer) buffer).ReadString();
      string Receiver = ((NetBuffer) buffer).ReadString();
      string Name = ((NetBuffer) buffer).ReadString();
      string Text = ((NetBuffer) buffer).ReadString();
      CommendationType Type = (CommendationType) ((NetBuffer) buffer).ReadInt32();
      int Round = ((NetBuffer) buffer).ReadInt32();
      commendations.Add(new Commendation(Giver, Receiver, Name, Text, Type, Round));
    }
  }

  private void WriteCommendations(NetOutgoingMessage buffer, List<Commendation> commendations)
  {
    ((NetBuffer) buffer).Write(commendations.Count);
    foreach (Commendation commendation in commendations)
    {
      ((NetBuffer) buffer).Write(commendation.Giver);
      ((NetBuffer) buffer).Write(commendation.Receiver);
      ((NetBuffer) buffer).Write(commendation.Name);
      ((NetBuffer) buffer).Write(commendation.Text);
      ((NetBuffer) buffer).Write((int) commendation.Type);
      ((NetBuffer) buffer).Write(commendation.Round);
    }
  }
}
