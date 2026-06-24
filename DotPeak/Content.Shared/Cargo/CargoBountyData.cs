// Decompiled with JetBrains decompiler
// Type: Content.Shared.Cargo.CargoBountyData
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
public readonly record struct CargoBountyData : 
  ISerializationGenerated<CargoBountyData>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public string Id { get; init; }

  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField(null, false, 1, true, false, null)]
  public ProtoId<CargoBountyPrototype> Bounty { get; init; }

  public CargoBountyData(CargoBountyPrototype bounty, int uniqueIdentifier)
  {
    // ISSUE: reference to a compiler-generated field
    this.\u003CId\u003Ek__BackingField = string.Empty;
    // ISSUE: reference to a compiler-generated field
    this.\u003CBounty\u003Ek__BackingField = ProtoId<CargoBountyPrototype>.op_Implicit(string.Empty);
    this.Bounty = ProtoId<CargoBountyPrototype>.op_Implicit(bounty.ID);
    this.Id = $"{bounty.IdPrefix}{uniqueIdentifier:D3}";
  }

  public CargoBountyData()
  {
    // ISSUE: reference to a compiler-generated field
    this.\u003CId\u003Ek__BackingField = string.Empty;
    // ISSUE: reference to a compiler-generated field
    this.\u003CBounty\u003Ek__BackingField = ProtoId<CargoBountyPrototype>.op_Implicit(string.Empty);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CargoBountyData target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<CargoBountyData>(this, ref target, hookCtx, false, context))
      return;
    string str = (string) null;
    if (this.Id == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Id, ref str, hookCtx, false, context))
      str = this.Id;
    ProtoId<CargoBountyPrototype> protoId = new ProtoId<CargoBountyPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<CargoBountyPrototype>>(this.Bounty, ref protoId, hookCtx, false, context))
      protoId = serialization.CreateCopy<ProtoId<CargoBountyPrototype>>(this.Bounty, hookCtx, context, false);
    target = target with { Id = str, Bounty = protoId };
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CargoBountyData target,
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
    CargoBountyData target1 = (CargoBountyData) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public CargoBountyData Instantiate() => new CargoBountyData();
}
