// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Tracker.SquadLeader.SquadLeaderTrackerFireteam
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

#nullable enable
namespace Content.Shared._RMC14.Tracker.SquadLeader;

[DataDefinition]
[NetSerializable]
[Serializable]
public record SquadLeaderTrackerFireteam() : 
  ISerializationGenerated<SquadLeaderTrackerFireteam>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public SquadLeaderTrackerMarine? Leader;
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<NetEntity, SquadLeaderTrackerMarine>? Members;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref SquadLeaderTrackerFireteam target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<SquadLeaderTrackerFireteam>(this, ref target, hookCtx, false, context))
      return;
    SquadLeaderTrackerMarine? target1 = new SquadLeaderTrackerMarine?();
    if (!serialization.TryCustomCopy<SquadLeaderTrackerMarine?>(this.Leader, ref target1, hookCtx, false, context))
      target1 = serialization.CreateCopy<SquadLeaderTrackerMarine?>(this.Leader, hookCtx, context);
    target.Leader = target1;
    Dictionary<NetEntity, SquadLeaderTrackerMarine> target2 = (Dictionary<NetEntity, SquadLeaderTrackerMarine>) null;
    if (!serialization.TryCustomCopy<Dictionary<NetEntity, SquadLeaderTrackerMarine>>(this.Members, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<Dictionary<NetEntity, SquadLeaderTrackerMarine>>(this.Members, hookCtx, context);
    target.Members = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref SquadLeaderTrackerFireteam target,
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
    SquadLeaderTrackerFireteam target1 = (SquadLeaderTrackerFireteam) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public virtual SquadLeaderTrackerFireteam Instantiate() => new SquadLeaderTrackerFireteam();

  [CompilerGenerated]
  protected virtual bool PrintMembers(StringBuilder builder)
  {
    RuntimeHelpers.EnsureSufficientExecutionStack();
    builder.Append("Leader = ");
    builder.Append(this.Leader.ToString());
    builder.Append(", Members = ");
    builder.Append((object) this.Members);
    return true;
  }

  [CompilerGenerated]
  public override int GetHashCode()
  {
    return (EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<SquadLeaderTrackerMarine?>.Default.GetHashCode(this.Leader)) * -1521134295 + EqualityComparer<Dictionary<NetEntity, SquadLeaderTrackerMarine>>.Default.GetHashCode(this.Members);
  }

  [CompilerGenerated]
  public virtual bool Equals(SquadLeaderTrackerFireteam? other)
  {
    if ((object) this == (object) other)
      return true;
    return (object) other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<SquadLeaderTrackerMarine?>.Default.Equals(this.Leader, other.Leader) && EqualityComparer<Dictionary<NetEntity, SquadLeaderTrackerMarine>>.Default.Equals(this.Members, other.Members);
  }

  [CompilerGenerated]
  protected SquadLeaderTrackerFireteam(SquadLeaderTrackerFireteam original)
  {
    this.Leader = original.Leader;
    this.Members = original.Members;
  }
}
