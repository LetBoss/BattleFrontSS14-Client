// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.AcidShroud.XenoAcidShroudComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.AcidShroud;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (XenoAcidShroudSystem)})]
public sealed class XenoAcidShroudComponent : 
  Component,
  ISerializationGenerated<XenoAcidShroudComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan DoAfter = TimeSpan.FromSeconds(0.75);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId Spawn = (EntProtoId) "RMCSmokeAcidShroud";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId[] Gases = new EntProtoId[2]
  {
    new EntProtoId("RMCSmokeAcidShroud"),
    new EntProtoId("RMCSmokeNeurotoxinShroud")
  };

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoAcidShroudComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoAcidShroudComponent) target1;
    if (serialization.TryCustomCopy<XenoAcidShroudComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DoAfter, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.DoAfter, hookCtx, context);
    target.DoAfter = target2;
    EntProtoId target3 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Spawn, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntProtoId>(this.Spawn, hookCtx, context);
    target.Spawn = target3;
    EntProtoId[] target4 = (EntProtoId[]) null;
    if (this.Gases == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<EntProtoId[]>(this.Gases, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<EntProtoId[]>(this.Gases, hookCtx, context);
    target.Gases = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoAcidShroudComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    XenoAcidShroudComponent target1 = (XenoAcidShroudComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    XenoAcidShroudComponent target1 = (XenoAcidShroudComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    XenoAcidShroudComponent target1 = (XenoAcidShroudComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual XenoAcidShroudComponent Component.Instantiate() => new XenoAcidShroudComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoAcidShroudComponent_AutoState : IComponentState
  {
    public TimeSpan DoAfter;
    public EntProtoId Spawn;
    public EntProtoId[] Gases;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoAcidShroudComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoAcidShroudComponent, ComponentGetState>(new ComponentEventRefHandler<XenoAcidShroudComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoAcidShroudComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoAcidShroudComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoAcidShroudComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoAcidShroudComponent.XenoAcidShroudComponent_AutoState()
      {
        DoAfter = component.DoAfter,
        Spawn = component.Spawn,
        Gases = component.Gases
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoAcidShroudComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoAcidShroudComponent.XenoAcidShroudComponent_AutoState current))
        return;
      component.DoAfter = current.DoAfter;
      component.Spawn = current.Spawn;
      component.Gases = current.Gases;
    }
  }
}
