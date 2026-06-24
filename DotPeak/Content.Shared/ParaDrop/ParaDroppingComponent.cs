// Decompiled with JetBrains decompiler
// Type: Content.Shared.ParaDrop.ParaDroppingComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.ParaDrop;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class ParaDroppingComponent : 
  Component,
  ISerializationGenerated<ParaDroppingComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float RemainingTime;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<string, int> OriginalLayers = new Dictionary<string, int>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<string, int> OriginalMasks = new Dictionary<string, int>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ParaDroppingComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ParaDroppingComponent) target1;
    if (serialization.TryCustomCopy<ParaDroppingComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RemainingTime, ref target2, hookCtx, false, context))
      target2 = this.RemainingTime;
    target.RemainingTime = target2;
    Dictionary<string, int> target3 = (Dictionary<string, int>) null;
    if (this.OriginalLayers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, int>>(this.OriginalLayers, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<Dictionary<string, int>>(this.OriginalLayers, hookCtx, context);
    target.OriginalLayers = target3;
    Dictionary<string, int> target4 = (Dictionary<string, int>) null;
    if (this.OriginalMasks == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, int>>(this.OriginalMasks, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<Dictionary<string, int>>(this.OriginalMasks, hookCtx, context);
    target.OriginalMasks = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ParaDroppingComponent target,
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
    ParaDroppingComponent target1 = (ParaDroppingComponent) target;
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
    ParaDroppingComponent target1 = (ParaDroppingComponent) target;
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
    ParaDroppingComponent target1 = (ParaDroppingComponent) target;
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
  virtual ParaDroppingComponent Component.Instantiate() => new ParaDroppingComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ParaDroppingComponent_AutoState : IComponentState
  {
    public float RemainingTime;
    public Dictionary<string, int> OriginalLayers;
    public Dictionary<string, int> OriginalMasks;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ParaDroppingComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ParaDroppingComponent, ComponentGetState>(new ComponentEventRefHandler<ParaDroppingComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ParaDroppingComponent, ComponentHandleState>(new ComponentEventRefHandler<ParaDroppingComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      ParaDroppingComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new ParaDroppingComponent.ParaDroppingComponent_AutoState()
      {
        RemainingTime = component.RemainingTime,
        OriginalLayers = component.OriginalLayers,
        OriginalMasks = component.OriginalMasks
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ParaDroppingComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is ParaDroppingComponent.ParaDroppingComponent_AutoState current))
        return;
      component.RemainingTime = current.RemainingTime;
      component.OriginalLayers = current.OriginalLayers == null ? (Dictionary<string, int>) null : new Dictionary<string, int>((IDictionary<string, int>) current.OriginalLayers);
      component.OriginalMasks = current.OriginalMasks == null ? (Dictionary<string, int>) null : new Dictionary<string, int>((IDictionary<string, int>) current.OriginalMasks);
    }
  }
}
