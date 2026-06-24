// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.Piping.Binary.Components.GasVolumePumpComponent
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
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Atmos.Piping.Binary.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
public sealed class GasVolumePumpComponent : 
  Component,
  ISerializationGenerated<GasVolumePumpComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Enabled = true;
  [DataField(null, false, 1, false, false, null)]
  public bool Blocked;
  [Robust.Shared.ViewVariables.ViewVariables]
  public bool Overclocked;
  [DataField("inlet", false, 1, false, false, null)]
  public string InletName = "inlet";
  [DataField("outlet", false, 1, false, false, null)]
  public string OutletName = "outlet";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float TransferRate = 200f;
  [DataField(null, false, 1, false, false, null)]
  public float MaxTransferRate = 200f;
  [DataField(null, false, 1, false, false, null)]
  public float LeakRatio = 0.1f;
  [DataField(null, false, 1, false, false, null)]
  public float LowerThreshold = 0.01f;
  [DataField(null, false, 1, false, false, null)]
  public float HigherThreshold = GasVolumePumpComponent.DefaultHigherThreshold;
  public static readonly float DefaultHigherThreshold = 9000f;
  [DataField(null, false, 1, false, false, null)]
  public float OverclockThreshold = 1000f;
  [DataField(null, false, 1, false, false, null)]
  public float LastMolesTransferred;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GasVolumePumpComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (GasVolumePumpComponent) component;
    if (serialization.TryCustomCopy<GasVolumePumpComponent>(this, ref target, hookCtx, false, context))
      return;
    bool flag1 = false;
    if (!serialization.TryCustomCopy<bool>(this.Enabled, ref flag1, hookCtx, false, context))
      flag1 = this.Enabled;
    target.Enabled = flag1;
    bool flag2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Blocked, ref flag2, hookCtx, false, context))
      flag2 = this.Blocked;
    target.Blocked = flag2;
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
    if (!serialization.TryCustomCopy<float>(this.TransferRate, ref num1, hookCtx, false, context))
      num1 = this.TransferRate;
    target.TransferRate = num1;
    float num2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxTransferRate, ref num2, hookCtx, false, context))
      num2 = this.MaxTransferRate;
    target.MaxTransferRate = num2;
    float num3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.LeakRatio, ref num3, hookCtx, false, context))
      num3 = this.LeakRatio;
    target.LeakRatio = num3;
    float num4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.LowerThreshold, ref num4, hookCtx, false, context))
      num4 = this.LowerThreshold;
    target.LowerThreshold = num4;
    float num5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.HigherThreshold, ref num5, hookCtx, false, context))
      num5 = this.HigherThreshold;
    target.HigherThreshold = num5;
    float num6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.OverclockThreshold, ref num6, hookCtx, false, context))
      num6 = this.OverclockThreshold;
    target.OverclockThreshold = num6;
    float num7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.LastMolesTransferred, ref num7, hookCtx, false, context))
      num7 = this.LastMolesTransferred;
    target.LastMolesTransferred = num7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GasVolumePumpComponent target,
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
    GasVolumePumpComponent target1 = (GasVolumePumpComponent) target;
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
    GasVolumePumpComponent target1 = (GasVolumePumpComponent) target;
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
    GasVolumePumpComponent target1 = (GasVolumePumpComponent) target;
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
  virtual GasVolumePumpComponent Component.Instantiate() => new GasVolumePumpComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class GasVolumePumpComponent_AutoState : IComponentState
  {
    public bool Enabled;
    public float TransferRate;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class GasVolumePumpComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<GasVolumePumpComponent, ComponentGetState>(new ComponentEventRefHandler<GasVolumePumpComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<GasVolumePumpComponent, ComponentHandleState>(new ComponentEventRefHandler<GasVolumePumpComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(
      EntityUid uid,
      GasVolumePumpComponent component,
      ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new GasVolumePumpComponent.GasVolumePumpComponent_AutoState()
      {
        Enabled = component.Enabled,
        TransferRate = component.TransferRate
      };
    }

    private void OnHandleState(
      EntityUid uid,
      GasVolumePumpComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is GasVolumePumpComponent.GasVolumePumpComponent_AutoState current))
        return;
      component.Enabled = current.Enabled;
      component.TransferRate = current.TransferRate;
      AfterAutoHandleStateEvent handleStateEvent;
      // ISSUE: explicit constructor call
      ((AfterAutoHandleStateEvent) ref handleStateEvent).\u002Ector(((ComponentHandleState) ref args).Current);
      ((IDirectedEventBus) this.EntityManager.EventBus).RaiseComponentEvent<AfterAutoHandleStateEvent, GasVolumePumpComponent>(uid, component, ref handleStateEvent);
    }
  }
}
