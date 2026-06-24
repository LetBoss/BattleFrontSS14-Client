// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Construction.ResinHole.XenoPlaceResinHoleDestroyWeedSourceDoAfterEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DoAfter;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Construction.ResinHole;

[NetSerializable]
[Serializable]
public sealed class XenoPlaceResinHoleDestroyWeedSourceDoAfterEvent : 
  SimpleDoAfterEvent,
  ISerializationGenerated<XenoPlaceResinHoleDestroyWeedSourceDoAfterEvent>,
  ISerializationGenerated
{
  public EntProtoId ResinHolePrototype;
  public int PlasmaCost;

  public XenoPlaceResinHoleDestroyWeedSourceDoAfterEvent(
    EntProtoId resinHolePrototype,
    int plasmaCost)
  {
    this.ResinHolePrototype = resinHolePrototype;
    this.PlasmaCost = plasmaCost;
  }

  public XenoPlaceResinHoleDestroyWeedSourceDoAfterEvent()
  {
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoPlaceResinHoleDestroyWeedSourceDoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    SimpleDoAfterEvent target1 = (SimpleDoAfterEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoPlaceResinHoleDestroyWeedSourceDoAfterEvent) target1;
    serialization.TryCustomCopy<XenoPlaceResinHoleDestroyWeedSourceDoAfterEvent>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoPlaceResinHoleDestroyWeedSourceDoAfterEvent target,
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
    XenoPlaceResinHoleDestroyWeedSourceDoAfterEvent target1 = (XenoPlaceResinHoleDestroyWeedSourceDoAfterEvent) target;
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
    XenoPlaceResinHoleDestroyWeedSourceDoAfterEvent target1 = (XenoPlaceResinHoleDestroyWeedSourceDoAfterEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual XenoPlaceResinHoleDestroyWeedSourceDoAfterEvent SimpleDoAfterEvent.Instantiate()
  {
    return new XenoPlaceResinHoleDestroyWeedSourceDoAfterEvent();
  }
}
