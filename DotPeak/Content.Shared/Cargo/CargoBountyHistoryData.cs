// Decompiled with JetBrains decompiler
// Type: Content.Shared.Cargo.CargoBountyHistoryData
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Cargo.Prototypes;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;

#nullable enable
namespace Content.Shared.Cargo;

[DataDefinition]
[NetSerializable]
[Serializable]
public readonly record struct CargoBountyHistoryData : 
  ISerializationGenerated<CargoBountyHistoryData>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public string Id { get; init; }

  [DataField(null, false, 1, false, false, null)]
  public CargoBountyHistoryData.BountyResult Result { get; init; }

  [DataField(null, false, 1, false, false, null)]
  public string? ActorName { get; init; }

  [DataField(null, false, 1, false, false, null)]
  public TimeSpan Timestamp { get; init; }

  [DataField(null, false, 1, true, false, null)]
  public ProtoId<CargoBountyPrototype> Bounty { get; init; }

  public CargoBountyHistoryData(
    CargoBountyData bounty,
    CargoBountyHistoryData.BountyResult result,
    TimeSpan timestamp,
    string? actorName)
  {
    // ISSUE: reference to a compiler-generated field
    this.\u003CId\u003Ek__BackingField = string.Empty;
    // ISSUE: reference to a compiler-generated field
    this.\u003CResult\u003Ek__BackingField = CargoBountyHistoryData.BountyResult.Completed;
    // ISSUE: reference to a compiler-generated field
    this.\u003CActorName\u003Ek__BackingField = (string) null;
    // ISSUE: reference to a compiler-generated field
    this.\u003CTimestamp\u003Ek__BackingField = TimeSpan.MinValue;
    // ISSUE: reference to a compiler-generated field
    this.\u003CBounty\u003Ek__BackingField = ProtoId<CargoBountyPrototype>.op_Implicit(string.Empty);
    this.Bounty = bounty.Bounty;
    this.Result = result;
    this.Id = bounty.Id;
    this.ActorName = actorName;
    this.Timestamp = timestamp;
  }

  public CargoBountyHistoryData()
  {
    // ISSUE: reference to a compiler-generated field
    this.\u003CId\u003Ek__BackingField = string.Empty;
    // ISSUE: reference to a compiler-generated field
    this.\u003CResult\u003Ek__BackingField = CargoBountyHistoryData.BountyResult.Completed;
    // ISSUE: reference to a compiler-generated field
    this.\u003CActorName\u003Ek__BackingField = (string) null;
    // ISSUE: reference to a compiler-generated field
    this.\u003CTimestamp\u003Ek__BackingField = TimeSpan.MinValue;
    // ISSUE: reference to a compiler-generated field
    this.\u003CBounty\u003Ek__BackingField = ProtoId<CargoBountyPrototype>.op_Implicit(string.Empty);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CargoBountyHistoryData target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<CargoBountyHistoryData>(this, ref target, hookCtx, false, context))
      return;
    string str1 = (string) null;
    if (this.Id == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Id, ref str1, hookCtx, false, context))
      str1 = this.Id;
    CargoBountyHistoryData.BountyResult bountyResult = CargoBountyHistoryData.BountyResult.Completed;
    if (!serialization.TryCustomCopy<CargoBountyHistoryData.BountyResult>(this.Result, ref bountyResult, hookCtx, false, context))
      bountyResult = this.Result;
    string str2 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.ActorName, ref str2, hookCtx, false, context))
      str2 = this.ActorName;
    TimeSpan timeSpan = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Timestamp, ref timeSpan, hookCtx, false, context))
      timeSpan = serialization.CreateCopy<TimeSpan>(this.Timestamp, hookCtx, context, false);
    ProtoId<CargoBountyPrototype> protoId = new ProtoId<CargoBountyPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<CargoBountyPrototype>>(this.Bounty, ref protoId, hookCtx, false, context))
      protoId = serialization.CreateCopy<ProtoId<CargoBountyPrototype>>(this.Bounty, hookCtx, context, false);
    target = target with
    {
      Id = str1,
      Result = bountyResult,
      ActorName = str2,
      Timestamp = timeSpan,
      Bounty = protoId
    };
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CargoBountyHistoryData target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    CargoBountyHistoryData target1 = (CargoBountyHistoryData) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public CargoBountyHistoryData Instantiate() => new CargoBountyHistoryData();

  public enum BountyResult
  {
    Completed,
    Skipped,
  }
}
