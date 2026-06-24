// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.Components.HyposprayComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
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
namespace Content.Shared.Chemistry.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class HyposprayComponent : 
  Component,
  ISerializationGenerated<HyposprayComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public string SolutionName = "hypospray";
  [AutoNetworkedField]
  [DataField(null, false, 1, false, false, null)]
  public FixedPoint2 TransferAmount = FixedPoint2.New(5);
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier InjectSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Items/hypospray.ogg", new AudioParams?());
  [AutoNetworkedField]
  [DataField(null, false, 1, true, false, null)]
  public bool OnlyAffectsMobs;
  [AutoNetworkedField]
  [DataField(null, false, 1, false, false, null)]
  public bool CanContainerDraw = true;
  [DataField(null, false, 1, false, false, null)]
  public bool InjectOnly;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref HyposprayComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (HyposprayComponent) component;
    if (serialization.TryCustomCopy<HyposprayComponent>(this, ref target, hookCtx, false, context))
      return;
    string str = (string) null;
    if (this.SolutionName == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.SolutionName, ref str, hookCtx, false, context))
      str = this.SolutionName;
    target.SolutionName = str;
    FixedPoint2 fixedPoint2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.TransferAmount, ref fixedPoint2, hookCtx, false, context))
      fixedPoint2 = serialization.CreateCopy<FixedPoint2>(this.TransferAmount, hookCtx, context, false);
    target.TransferAmount = fixedPoint2;
    SoundSpecifier soundSpecifier = (SoundSpecifier) null;
    if (this.InjectSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.InjectSound, ref soundSpecifier, hookCtx, true, context))
      soundSpecifier = serialization.CreateCopy<SoundSpecifier>(this.InjectSound, hookCtx, context, false);
    target.InjectSound = soundSpecifier;
    bool flag1 = false;
    if (!serialization.TryCustomCopy<bool>(this.OnlyAffectsMobs, ref flag1, hookCtx, false, context))
      flag1 = this.OnlyAffectsMobs;
    target.OnlyAffectsMobs = flag1;
    bool flag2 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanContainerDraw, ref flag2, hookCtx, false, context))
      flag2 = this.CanContainerDraw;
    target.CanContainerDraw = flag2;
    bool flag3 = false;
    if (!serialization.TryCustomCopy<bool>(this.InjectOnly, ref flag3, hookCtx, false, context))
      flag3 = this.InjectOnly;
    target.InjectOnly = flag3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref HyposprayComponent target,
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
    HyposprayComponent target1 = (HyposprayComponent) target;
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
    HyposprayComponent target1 = (HyposprayComponent) target;
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
    HyposprayComponent target1 = (HyposprayComponent) target;
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
  virtual HyposprayComponent Component.Instantiate() => new HyposprayComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class HyposprayComponent_AutoState : IComponentState
  {
    public FixedPoint2 TransferAmount;
    public bool OnlyAffectsMobs;
    public bool CanContainerDraw;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class HyposprayComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<HyposprayComponent, ComponentGetState>(new ComponentEventRefHandler<HyposprayComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<HyposprayComponent, ComponentHandleState>(new ComponentEventRefHandler<HyposprayComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(
      EntityUid uid,
      HyposprayComponent component,
      ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new HyposprayComponent.HyposprayComponent_AutoState()
      {
        TransferAmount = component.TransferAmount,
        OnlyAffectsMobs = component.OnlyAffectsMobs,
        CanContainerDraw = component.CanContainerDraw
      };
    }

    private void OnHandleState(
      EntityUid uid,
      HyposprayComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is HyposprayComponent.HyposprayComponent_AutoState current))
        return;
      component.TransferAmount = current.TransferAmount;
      component.OnlyAffectsMobs = current.OnlyAffectsMobs;
      component.CanContainerDraw = current.CanContainerDraw;
    }
  }
}
