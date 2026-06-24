// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.Piping.Unary.Components.GasCanisterComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Containers.ItemSlots;
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
namespace Content.Shared.Atmos.Piping.Unary.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
public sealed class GasCanisterComponent : 
  Component,
  IGasMixtureHolder,
  ISerializationGenerated<GasCanisterComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public ItemSlot GasTankSlot = new ItemSlot();
  public float LastPressure;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float MinReleasePressure = 10.1325f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float MaxReleasePressure = 1013.25f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float ReleasePressure = 101.325f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ReleaseValve;

  [DataField("port", false, 1, false, false, null)]
  public string PortName { get; set; } = "port";

  [DataField("container", false, 1, false, false, null)]
  public string ContainerName { get; set; } = "tank_slot";

  [DataField("gasMixture", false, 1, false, false, null)]
  public GasMixture Air { get; set; } = new GasMixture();

  [GuidebookData]
  public float Volume => this.Air.Volume;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GasCanisterComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (GasCanisterComponent) component;
    if (serialization.TryCustomCopy<GasCanisterComponent>(this, ref target, hookCtx, false, context))
      return;
    string str1 = (string) null;
    if (this.PortName == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.PortName, ref str1, hookCtx, false, context))
      str1 = this.PortName;
    target.PortName = str1;
    string str2 = (string) null;
    if (this.ContainerName == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ContainerName, ref str2, hookCtx, false, context))
      str2 = this.ContainerName;
    target.ContainerName = str2;
    ItemSlot itemSlot = (ItemSlot) null;
    if (this.GasTankSlot == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<ItemSlot>(this.GasTankSlot, ref itemSlot, hookCtx, false, context))
    {
      if (this.GasTankSlot == null)
        itemSlot = (ItemSlot) null;
      else
        serialization.CopyTo<ItemSlot>(this.GasTankSlot, ref itemSlot, hookCtx, context, true);
    }
    target.GasTankSlot = itemSlot;
    GasMixture gasMixture = (GasMixture) null;
    if (this.Air == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<GasMixture>(this.Air, ref gasMixture, hookCtx, true, context))
    {
      if (this.Air == null)
        gasMixture = (GasMixture) null;
      else
        serialization.CopyTo<GasMixture>(this.Air, ref gasMixture, hookCtx, context, true);
    }
    target.Air = gasMixture;
    float num1 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MinReleasePressure, ref num1, hookCtx, false, context))
      num1 = this.MinReleasePressure;
    target.MinReleasePressure = num1;
    float num2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxReleasePressure, ref num2, hookCtx, false, context))
      num2 = this.MaxReleasePressure;
    target.MaxReleasePressure = num2;
    float num3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ReleasePressure, ref num3, hookCtx, false, context))
      num3 = this.ReleasePressure;
    target.ReleasePressure = num3;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.ReleaseValve, ref flag, hookCtx, false, context))
      flag = this.ReleaseValve;
    target.ReleaseValve = flag;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GasCanisterComponent target,
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
    GasCanisterComponent target1 = (GasCanisterComponent) target;
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
    GasCanisterComponent target1 = (GasCanisterComponent) target;
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
    GasCanisterComponent target1 = (GasCanisterComponent) target;
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
  virtual GasCanisterComponent Component.Instantiate() => new GasCanisterComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class GasCanisterComponent_AutoState : IComponentState
  {
    public float MinReleasePressure;
    public float MaxReleasePressure;
    public float ReleasePressure;
    public bool ReleaseValve;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class GasCanisterComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<GasCanisterComponent, ComponentGetState>(new ComponentEventRefHandler<GasCanisterComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<GasCanisterComponent, ComponentHandleState>(new ComponentEventRefHandler<GasCanisterComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(
      EntityUid uid,
      GasCanisterComponent component,
      ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new GasCanisterComponent.GasCanisterComponent_AutoState()
      {
        MinReleasePressure = component.MinReleasePressure,
        MaxReleasePressure = component.MaxReleasePressure,
        ReleasePressure = component.ReleasePressure,
        ReleaseValve = component.ReleaseValve
      };
    }

    private void OnHandleState(
      EntityUid uid,
      GasCanisterComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is GasCanisterComponent.GasCanisterComponent_AutoState current))
        return;
      component.MinReleasePressure = current.MinReleasePressure;
      component.MaxReleasePressure = current.MaxReleasePressure;
      component.ReleasePressure = current.ReleasePressure;
      component.ReleaseValve = current.ReleaseValve;
      AfterAutoHandleStateEvent handleStateEvent;
      // ISSUE: explicit constructor call
      ((AfterAutoHandleStateEvent) ref handleStateEvent).\u002Ector(((ComponentHandleState) ref args).Current);
      ((IDirectedEventBus) this.EntityManager.EventBus).RaiseComponentEvent<AfterAutoHandleStateEvent, GasCanisterComponent>(uid, component, ref handleStateEvent);
    }
  }
}
