// Decompiled with JetBrains decompiler
// Type: Content.Shared.Construction.ConstructionInteractDoAfterEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Construction;

[NetSerializable]
[Serializable]
public sealed class ConstructionInteractDoAfterEvent : 
  DoAfterEvent,
  ISerializationGenerated<ConstructionInteractDoAfterEvent>,
  ISerializationGenerated
{
  [DataField("clickLocation", false, 1, false, false, null)]
  public NetCoordinates ClickLocation;

  private ConstructionInteractDoAfterEvent()
  {
  }

  public ConstructionInteractDoAfterEvent(IEntityManager entManager, InteractUsingEvent ev)
  {
    this.ClickLocation = entManager.GetNetCoordinates(ev.ClickLocation, (MetaDataComponent) null);
  }

  public override DoAfterEvent Clone() => (DoAfterEvent) this;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ConstructionInteractDoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DoAfterEvent target1 = (DoAfterEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ConstructionInteractDoAfterEvent) target1;
    if (serialization.TryCustomCopy<ConstructionInteractDoAfterEvent>(this, ref target, hookCtx, false, context))
      return;
    NetCoordinates netCoordinates = new NetCoordinates();
    if (!serialization.TryCustomCopy<NetCoordinates>(this.ClickLocation, ref netCoordinates, hookCtx, false, context))
      netCoordinates = serialization.CreateCopy<NetCoordinates>(this.ClickLocation, hookCtx, context, false);
    target.ClickLocation = netCoordinates;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ConstructionInteractDoAfterEvent target,
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
    ConstructionInteractDoAfterEvent target1 = (ConstructionInteractDoAfterEvent) target;
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
    ConstructionInteractDoAfterEvent target1 = (ConstructionInteractDoAfterEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual ConstructionInteractDoAfterEvent DoAfterEvent.Instantiate()
  {
    return new ConstructionInteractDoAfterEvent();
  }
}
