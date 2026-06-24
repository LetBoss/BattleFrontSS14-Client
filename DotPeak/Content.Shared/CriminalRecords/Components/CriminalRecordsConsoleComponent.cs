// Decompiled with JetBrains decompiler
// Type: Content.Shared.CriminalRecords.Components.CriminalRecordsConsoleComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.CriminalRecords.Systems;
using Content.Shared.Radio;
using Content.Shared.Security;
using Content.Shared.StationRecords;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.CriminalRecords.Components;

[RegisterComponent]
[Access(new Type[] {typeof (SharedCriminalRecordsConsoleSystem)})]
public sealed class CriminalRecordsConsoleComponent : 
  Component,
  ISerializationGenerated<CriminalRecordsConsoleComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public uint? ActiveKey;
  [DataField(null, false, 1, false, false, null)]
  public StationRecordsFilter? Filter;
  [DataField(null, false, 1, false, false, null)]
  public SecurityStatus FilterStatus;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<RadioChannelPrototype> SecurityChannel = ProtoId<RadioChannelPrototype>.op_Implicit("Security");
  [DataField(null, false, 1, false, false, null)]
  public uint MaxStringLength = 256 /*0x0100*/;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CriminalRecordsConsoleComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (CriminalRecordsConsoleComponent) component;
    if (serialization.TryCustomCopy<CriminalRecordsConsoleComponent>(this, ref target, hookCtx, false, context))
      return;
    uint? nullable = new uint?();
    if (!serialization.TryCustomCopy<uint?>(this.ActiveKey, ref nullable, hookCtx, false, context))
      nullable = this.ActiveKey;
    target.ActiveKey = nullable;
    StationRecordsFilter stationRecordsFilter = (StationRecordsFilter) null;
    if (!serialization.TryCustomCopy<StationRecordsFilter>(this.Filter, ref stationRecordsFilter, hookCtx, false, context))
      stationRecordsFilter = serialization.CreateCopy<StationRecordsFilter>(this.Filter, hookCtx, context, false);
    target.Filter = stationRecordsFilter;
    SecurityStatus securityStatus = SecurityStatus.None;
    if (!serialization.TryCustomCopy<SecurityStatus>(this.FilterStatus, ref securityStatus, hookCtx, false, context))
      securityStatus = this.FilterStatus;
    target.FilterStatus = securityStatus;
    ProtoId<RadioChannelPrototype> protoId = new ProtoId<RadioChannelPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<RadioChannelPrototype>>(this.SecurityChannel, ref protoId, hookCtx, false, context))
      protoId = serialization.CreateCopy<ProtoId<RadioChannelPrototype>>(this.SecurityChannel, hookCtx, context, false);
    target.SecurityChannel = protoId;
    uint num = 0;
    if (!serialization.TryCustomCopy<uint>(this.MaxStringLength, ref num, hookCtx, false, context))
      num = this.MaxStringLength;
    target.MaxStringLength = num;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CriminalRecordsConsoleComponent target,
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
    CriminalRecordsConsoleComponent target1 = (CriminalRecordsConsoleComponent) target;
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
    CriminalRecordsConsoleComponent target1 = (CriminalRecordsConsoleComponent) target;
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
    CriminalRecordsConsoleComponent target1 = (CriminalRecordsConsoleComponent) target;
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
  virtual CriminalRecordsConsoleComponent Component.Instantiate()
  {
    return new CriminalRecordsConsoleComponent();
  }
}
