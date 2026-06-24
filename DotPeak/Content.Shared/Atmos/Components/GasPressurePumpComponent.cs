// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.Components.GasPressurePumpComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Guidebook;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Atmos.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
public sealed class GasPressurePumpComponent : 
  Component,
  ISerializationGenerated<GasPressurePumpComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Enabled = true;
  [DataField("inlet", false, 1, false, false, null)]
  public string InletName = "inlet";
  [DataField("outlet", false, 1, false, false, null)]
  public string OutletName = "outlet";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float TargetPressure = 101.325f;
  [DataField(null, false, 1, false, false, null)]
  [GuidebookData]
  public float MaxTargetPressure = 4500f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GasPressurePumpComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (GasPressurePumpComponent) component;
    if (serialization.TryCustomCopy<GasPressurePumpComponent>(this, ref target, hookCtx, false, context))
      return;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.Enabled, ref flag, hookCtx, false, context))
      flag = this.Enabled;
    target.Enabled = flag;
    string str1 = (string) null;
    if (this.InletName == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.InletName, ref str1, hookCtx, false, context))
      str1 = this.InletName;
    target.InletName = str1;
    string str2 = (string) null;
    if (this.OutletName == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.OutletName, ref str2, hookCtx, false, context))
      str2 = this.OutletName;
    target.OutletName = str2;
    float num1 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.TargetPressure, ref num1, hookCtx, false, context))
      num1 = this.TargetPressure;
    target.TargetPressure = num1;
    float num2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxTargetPressure, ref num2, hookCtx, false, context))
      num2 = this.MaxTargetPressure;
    target.MaxTargetPressure = num2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GasPressurePumpComponent target,
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
    GasPressurePumpComponent target1 = (GasPressurePumpComponent) target;
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
    GasPressurePumpComponent target1 = (GasPressurePumpComponent) target;
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
    GasPressurePumpComponent target1 = (GasPressurePumpComponent) target;
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
  virtual GasPressurePumpComponent Component.Instantiate() => new GasPressurePumpComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class GasPressurePumpComponent_AutoState : IComponentState
  {
    public bool Enabled;
    public float TargetPressure;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class GasPressurePumpComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<GasPressurePumpComponent, ComponentGetState>(new ComponentEventRefHandler<GasPressurePumpComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<GasPressurePumpComponent, ComponentHandleState>(new ComponentEventRefHandler<GasPressurePumpComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(
      EntityUid uid,
      GasPressurePumpComponent component,
      ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new GasPressurePumpComponent.GasPressurePumpComponent_AutoState()
      {
        Enabled = component.Enabled,
        TargetPressure = component.TargetPressure
      };
    }

    private void OnHandleState(
      EntityUid uid,
      GasPressurePumpComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is GasPressurePumpComponent.GasPressurePumpComponent_AutoState current))
        return;
      component.Enabled = current.Enabled;
      component.TargetPressure = current.TargetPressure;
      AfterAutoHandleStateEvent handleStateEvent;
      // ISSUE: explicit constructor call
      ((AfterAutoHandleStateEvent) ref handleStateEvent).\u002Ector(((ComponentHandleState) ref args).Current);
      ((IDirectedEventBus) this.EntityManager.EventBus).RaiseComponentEvent<AfterAutoHandleStateEvent, GasPressurePumpComponent>(uid, component, ref handleStateEvent);
    }
  }
}
