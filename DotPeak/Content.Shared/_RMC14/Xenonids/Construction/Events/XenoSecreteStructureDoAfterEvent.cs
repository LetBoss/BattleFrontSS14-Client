// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Construction.Events.XenoSecreteStructureDoAfterEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DoAfter;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Construction.Events;

[NetSerializable]
[Serializable]
public sealed class XenoSecreteStructureDoAfterEvent : 
  SimpleDoAfterEvent,
  ISerializationGenerated<XenoSecreteStructureDoAfterEvent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public NetCoordinates Coordinates;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId StructureId = (EntProtoId) "WallXenoResin";
  [DataField(null, false, 1, false, false, null)]
  public NetEntity? Effect;

  public XenoSecreteStructureDoAfterEvent(
    NetCoordinates coordinates,
    EntProtoId structureId,
    NetEntity? effect = null)
  {
    this.Coordinates = coordinates;
    this.StructureId = structureId;
    this.Effect = effect;
  }

  public XenoSecreteStructureDoAfterEvent()
  {
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoSecreteStructureDoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    SimpleDoAfterEvent target1 = (SimpleDoAfterEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoSecreteStructureDoAfterEvent) target1;
    if (serialization.TryCustomCopy<XenoSecreteStructureDoAfterEvent>(this, ref target, hookCtx, false, context))
      return;
    NetCoordinates target2 = new NetCoordinates();
    if (!serialization.TryCustomCopy<NetCoordinates>(this.Coordinates, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<NetCoordinates>(this.Coordinates, hookCtx, context);
    target.Coordinates = target2;
    EntProtoId target3 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.StructureId, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntProtoId>(this.StructureId, hookCtx, context);
    target.StructureId = target3;
    NetEntity? target4 = new NetEntity?();
    if (!serialization.TryCustomCopy<NetEntity?>(this.Effect, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<NetEntity?>(this.Effect, hookCtx, context);
    target.Effect = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoSecreteStructureDoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref SimpleDoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    XenoSecreteStructureDoAfterEvent target1 = (XenoSecreteStructureDoAfterEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (SimpleDoAfterEvent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    XenoSecreteStructureDoAfterEvent target1 = (XenoSecreteStructureDoAfterEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual XenoSecreteStructureDoAfterEvent SimpleDoAfterEvent.Instantiate()
  {
    return new XenoSecreteStructureDoAfterEvent();
  }
}
