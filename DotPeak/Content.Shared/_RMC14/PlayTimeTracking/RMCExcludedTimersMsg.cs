// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.PlayTimeTracking.RMCExcludedTimersMsg
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.PlayTimeTracking;

public sealed class RMCExcludedTimersMsg : NetMessage
{
  public HashSet<string> Trackers;

  public override MsgGroups MsgGroup => MsgGroups.Core;

  public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
    int capacity = ((NetBuffer) buffer).ReadVariableInt32();
    this.Trackers = new HashSet<string>(capacity);
    for (int index = 0; index < capacity; ++index)
      this.Trackers.Add(((NetBuffer) buffer).ReadString());
  }

  public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    ((NetBuffer) buffer).WriteVariableInt32(this.Trackers.Count);
    foreach (string tracker in this.Trackers)
      ((NetBuffer) buffer).Write(tracker);
  }
}
