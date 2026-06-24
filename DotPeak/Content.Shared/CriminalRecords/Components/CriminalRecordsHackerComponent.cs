// Decompiled with JetBrains decompiler
// Type: Content.Shared.CriminalRecords.Components.CriminalRecordsHackerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.CriminalRecords.Systems;
using Content.Shared.Dataset;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.CriminalRecords.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedCriminalRecordsHackerSystem)})]
public sealed class CriminalRecordsHackerComponent : 
  Component,
  ISerializationGenerated<CriminalRecordsHackerComponent>,
  ISerializationGenerated
{
  public TimeSpan Delay = TimeSpan.FromSeconds(20L);
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<LocalizedDatasetPrototype> Reasons = ProtoId<LocalizedDatasetPrototype>.op_Implicit("CriminalRecordsWantedReasonPlaceholders");
  [DataField(null, false, 1, false, false, null)]
  public LocId Announcement = LocId.op_Implicit("ninja-criminal-records-hack-announcement");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CriminalRecordsHackerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (CriminalRecordsHackerComponent) component;
    if (serialization.TryCustomCopy<CriminalRecordsHackerComponent>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<LocalizedDatasetPrototype> protoId = new ProtoId<LocalizedDatasetPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<LocalizedDatasetPrototype>>(this.Reasons, ref protoId, hookCtx, false, context))
      protoId = serialization.CreateCopy<ProtoId<LocalizedDatasetPrototype>>(this.Reasons, hookCtx, context, false);
    target.Reasons = protoId;
    LocId locId = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.Announcement, ref locId, hookCtx, false, context))
      locId = serialization.CreateCopy<LocId>(this.Announcement, hookCtx, context, false);
    target.Announcement = locId;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CriminalRecordsHackerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    CriminalRecordsHackerComponent target1 = (CriminalRecordsHackerComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    CriminalRecordsHackerComponent target1 = (CriminalRecordsHackerComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    CriminalRecordsHackerComponent target1 = (CriminalRecordsHackerComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    base.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual CriminalRecordsHackerComponent Component.Instantiate()
  {
    return new CriminalRecordsHackerComponent();
  }
}
