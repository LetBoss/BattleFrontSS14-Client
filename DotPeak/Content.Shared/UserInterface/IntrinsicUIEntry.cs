// Decompiled with JetBrains decompiler
// Type: Content.Shared.UserInterface.IntrinsicUIEntry
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;

#nullable enable
namespace Content.Shared.UserInterface;

[DataDefinition]
public sealed class IntrinsicUIEntry : 
  ISerializationGenerated<IntrinsicUIEntry>,
  ISerializationGenerated
{
  [DataField("toggleAction", false, 1, true, false, null)]
  public EntProtoId? ToggleAction;
  [DataField("toggleActionEntity", false, 1, false, false, null)]
  public EntityUid? ToggleActionEntity = new EntityUid?(new EntityUid());

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref IntrinsicUIEntry target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<IntrinsicUIEntry>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId? target1 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.ToggleAction, ref target1, hookCtx, false, context))
      target1 = serialization.CreateCopy<EntProtoId?>(this.ToggleAction, hookCtx, context);
    target.ToggleAction = target1;
    EntityUid? target2 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.ToggleActionEntity, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityUid?>(this.ToggleActionEntity, hookCtx, context);
    target.ToggleActionEntity = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref IntrinsicUIEntry target,
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
    IntrinsicUIEntry target1 = (IntrinsicUIEntry) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public IntrinsicUIEntry Instantiate() => new IntrinsicUIEntry();
}
