// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.Piping.Binary.Components.GasValveComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Atmos.Piping.Binary.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
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
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedGasValveSystem)})]
public sealed class GasValveComponent : 
  Component,
  ISerializationGenerated<GasValveComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  [Robust.Shared.ViewVariables.ViewVariables]
  public bool Open = true;
  [DataField("inlet", false, 1, false, false, null)]
  public string InletName = "inlet";
  [DataField("outlet", false, 1, false, false, null)]
  public string OutletName = "outlet";
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier ValveSound = (SoundSpecifier) new SoundCollectionSpecifier("valveSqueak", new AudioParams?());

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GasValveComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (GasValveComponent) component;
    if (serialization.TryCustomCopy<GasValveComponent>(this, ref target, hookCtx, false, context))
      return;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.Open, ref flag, hookCtx, false, context))
      flag = this.Open;
    target.Open = flag;
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
    SoundSpecifier soundSpecifier = (SoundSpecifier) null;
    if (this.ValveSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ValveSound, ref soundSpecifier, hookCtx, true, context))
      soundSpecifier = serialization.CreateCopy<SoundSpecifier>(this.ValveSound, hookCtx, context, false);
    target.ValveSound = soundSpecifier;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GasValveComponent target,
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
    GasValveComponent target1 = (GasValveComponent) target;
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
    GasValveComponent target1 = (GasValveComponent) target;
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
    GasValveComponent target1 = (GasValveComponent) target;
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
  virtual GasValveComponent Component.Instantiate() => new GasValveComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class GasValveComponent_AutoState : IComponentState
  {
    public bool Open;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class GasValveComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<GasValveComponent, ComponentGetState>(new ComponentEventRefHandler<GasValveComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<GasValveComponent, ComponentHandleState>(new ComponentEventRefHandler<GasValveComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(EntityUid uid, GasValveComponent component, ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new GasValveComponent.GasValveComponent_AutoState()
      {
        Open = component.Open
      };
    }

    private void OnHandleState(
      EntityUid uid,
      GasValveComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is GasValveComponent.GasValveComponent_AutoState current))
        return;
      component.Open = current.Open;
    }
  }
}
