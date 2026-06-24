// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.HardpointSlot
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Whitelist;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;

#nullable enable
namespace Content.Shared._RMC14.Vehicle;

[NetSerializable]
[DataDefinition]
[Serializable]
public sealed class HardpointSlot : ISerializationGenerated<HardpointSlot>, ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public string Id { get; set; } = string.Empty;

  [DataField(null, false, 1, true, false, null)]
  public string HardpointType { get; set; } = string.Empty;

  [DataField(null, false, 1, false, false, null)]
  public ProtoId<HardpointSlotTypePrototype>? SlotType { get; set; }

  [DataField(null, false, 1, false, false, null)]
  public string? CompatibilityId { get; set; }

  [DataField(null, false, 1, false, false, null)]
  public string VisualLayer { get; set; } = string.Empty;

  [DataField(null, false, 1, false, false, null)]
  public bool Required { get; set; } = true;

  [DataField(null, false, 1, false, false, null)]
  public float InsertDelay { get; set; } = 1f;

  [DataField(null, false, 1, false, false, null)]
  public float RemoveDelay { get; set; } = -1f;

  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? Whitelist { get; set; }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref HardpointSlot target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<HardpointSlot>(this, ref target, hookCtx, false, context))
      return;
    string target1 = (string) null;
    if (this.Id == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Id, ref target1, hookCtx, false, context))
      target1 = this.Id;
    target.Id = target1;
    string target2 = (string) null;
    if (this.HardpointType == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.HardpointType, ref target2, hookCtx, false, context))
      target2 = this.HardpointType;
    target.HardpointType = target2;
    ProtoId<HardpointSlotTypePrototype>? target3 = new ProtoId<HardpointSlotTypePrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<HardpointSlotTypePrototype>?>(this.SlotType, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<ProtoId<HardpointSlotTypePrototype>?>(this.SlotType, hookCtx, context);
    target.SlotType = target3;
    string target4 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.CompatibilityId, ref target4, hookCtx, false, context))
      target4 = this.CompatibilityId;
    target.CompatibilityId = target4;
    string target5 = (string) null;
    if (this.VisualLayer == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.VisualLayer, ref target5, hookCtx, false, context))
      target5 = this.VisualLayer;
    target.VisualLayer = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.Required, ref target6, hookCtx, false, context))
      target6 = this.Required;
    target.Required = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.InsertDelay, ref target7, hookCtx, false, context))
      target7 = this.InsertDelay;
    target.InsertDelay = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RemoveDelay, ref target8, hookCtx, false, context))
      target8 = this.RemoveDelay;
    target.RemoveDelay = target8;
    EntityWhitelist target9 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref target9, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        target9 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref target9, hookCtx, context);
    }
    target.Whitelist = target9;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref HardpointSlot target,
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
    HardpointSlot target1 = (HardpointSlot) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public HardpointSlot Instantiate() => new HardpointSlot();
}
