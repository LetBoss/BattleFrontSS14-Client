// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Mortar.MortarLaserTargetUpdateDoAfterEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DoAfter;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Mortar;

[NetSerializable]
[Serializable]
public sealed class MortarLaserTargetUpdateDoAfterEvent : 
  DoAfterEvent,
  ISerializationGenerated<MortarLaserTargetUpdateDoAfterEvent>,
  ISerializationGenerated
{
  public NetCoordinates TargetCoordinates;

  public MortarLaserTargetUpdateDoAfterEvent(NetCoordinates targetCoordinates)
  {
    this.TargetCoordinates = targetCoordinates;
  }

  public override DoAfterEvent Clone()
  {
    return (DoAfterEvent) new MortarLaserTargetUpdateDoAfterEvent(this.TargetCoordinates);
  }

  public MortarLaserTargetUpdateDoAfterEvent()
  {
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MortarLaserTargetUpdateDoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DoAfterEvent target1 = (DoAfterEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (MortarLaserTargetUpdateDoAfterEvent) target1;
    serialization.TryCustomCopy<MortarLaserTargetUpdateDoAfterEvent>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MortarLaserTargetUpdateDoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref DoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    MortarLaserTargetUpdateDoAfterEvent target1 = (MortarLaserTargetUpdateDoAfterEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (DoAfterEvent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    MortarLaserTargetUpdateDoAfterEvent target1 = (MortarLaserTargetUpdateDoAfterEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual MortarLaserTargetUpdateDoAfterEvent DoAfterEvent.Instantiate()
  {
    return new MortarLaserTargetUpdateDoAfterEvent();
  }
}
