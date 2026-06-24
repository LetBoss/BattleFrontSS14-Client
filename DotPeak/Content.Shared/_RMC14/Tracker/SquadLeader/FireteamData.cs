// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Tracker.SquadLeader.FireteamData
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

#nullable enable
namespace Content.Shared._RMC14.Tracker.SquadLeader;

[DataDefinition]
[NetSerializable]
[Serializable]
public record FireteamData() : ISerializationGenerated<FireteamData>, ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public SquadLeaderTrackerFireteam?[] Fireteams = new SquadLeaderTrackerFireteam[3];
  [DataField(null, false, 1, false, false, null)]
  public string? SquadLeader;
  [DataField(null, false, 1, false, false, null)]
  public NetEntity? SquadLeaderId;
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<NetEntity, SquadLeaderTrackerMarine> Unassigned = new Dictionary<NetEntity, SquadLeaderTrackerMarine>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref FireteamData target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<FireteamData>(this, ref target, hookCtx, false, context))
      return;
    SquadLeaderTrackerFireteam[] target1 = (SquadLeaderTrackerFireteam[]) null;
    if (this.Fireteams == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SquadLeaderTrackerFireteam[]>(this.Fireteams, ref target1, hookCtx, true, context))
      target1 = serialization.CreateCopy<SquadLeaderTrackerFireteam[]>(this.Fireteams, hookCtx, context);
    target.Fireteams = target1;
    string target2 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.SquadLeader, ref target2, hookCtx, false, context))
      target2 = this.SquadLeader;
    target.SquadLeader = target2;
    NetEntity? target3 = new NetEntity?();
    if (!serialization.TryCustomCopy<NetEntity?>(this.SquadLeaderId, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<NetEntity?>(this.SquadLeaderId, hookCtx, context);
    target.SquadLeaderId = target3;
    Dictionary<NetEntity, SquadLeaderTrackerMarine> target4 = (Dictionary<NetEntity, SquadLeaderTrackerMarine>) null;
    if (this.Unassigned == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<NetEntity, SquadLeaderTrackerMarine>>(this.Unassigned, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<Dictionary<NetEntity, SquadLeaderTrackerMarine>>(this.Unassigned, hookCtx, context);
    target.Unassigned = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref FireteamData target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    FireteamData target1 = (FireteamData) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public virtual FireteamData Instantiate() => new FireteamData();

  [CompilerGenerated]
  protected virtual bool PrintMembers(StringBuilder builder)
  {
    RuntimeHelpers.EnsureSufficientExecutionStack();
    builder.Append("Fireteams = ");
    builder.Append((object) this.Fireteams);
    builder.Append(", SquadLeader = ");
    builder.Append((object) this.SquadLeader);
    builder.Append(", SquadLeaderId = ");
    builder.Append(this.SquadLeaderId.ToString());
    builder.Append(", Unassigned = ");
    builder.Append((object) this.Unassigned);
    return true;
  }

  [CompilerGenerated]
  public override int GetHashCode()
  {
    return (((EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<SquadLeaderTrackerFireteam[]>.Default.GetHashCode(this.Fireteams)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.SquadLeader)) * -1521134295 + EqualityComparer<NetEntity?>.Default.GetHashCode(this.SquadLeaderId)) * -1521134295 + EqualityComparer<Dictionary<NetEntity, SquadLeaderTrackerMarine>>.Default.GetHashCode(this.Unassigned);
  }

  [CompilerGenerated]
  public virtual bool Equals(FireteamData? other)
  {
    if ((object) this == (object) other)
      return true;
    return (object) other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<SquadLeaderTrackerFireteam[]>.Default.Equals(this.Fireteams, other.Fireteams) && EqualityComparer<string>.Default.Equals(this.SquadLeader, other.SquadLeader) && EqualityComparer<NetEntity?>.Default.Equals(this.SquadLeaderId, other.SquadLeaderId) && EqualityComparer<Dictionary<NetEntity, SquadLeaderTrackerMarine>>.Default.Equals(this.Unassigned, other.Unassigned);
  }

  [CompilerGenerated]
  protected FireteamData(FireteamData original)
  {
    this.Fireteams = original.Fireteams;
    this.SquadLeader = original.SquadLeader;
    this.SquadLeaderId = original.SquadLeaderId;
    this.Unassigned = original.Unassigned;
  }
}
