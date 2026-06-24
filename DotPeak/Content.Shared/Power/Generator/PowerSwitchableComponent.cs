// Decompiled with JetBrains decompiler
// Type: Content.Shared.Power.Generator.PowerSwitchableComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
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
namespace Content.Shared.Power.Generator;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedPowerSwitchableSystem)})]
public sealed class PowerSwitchableComponent : 
  Component,
  ISerializationGenerated<PowerSwitchableComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int ActiveIndex;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? SwitchSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Machines/button.ogg");
  [DataField(null, false, 1, true, false, null)]
  public string ExamineText = string.Empty;
  [DataField(null, false, 1, true, false, null)]
  public string SwitchText = string.Empty;
  [DataField(null, false, 1, true, false, null)]
  public List<PowerSwitchableCable> Cables = new List<PowerSwitchableCable>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PowerSwitchableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PowerSwitchableComponent) target1;
    if (serialization.TryCustomCopy<PowerSwitchableComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.ActiveIndex, ref target2, hookCtx, false, context))
      target2 = this.ActiveIndex;
    target.ActiveIndex = target2;
    SoundSpecifier target3 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SwitchSound, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<SoundSpecifier>(this.SwitchSound, hookCtx, context);
    target.SwitchSound = target3;
    string target4 = (string) null;
    if (this.ExamineText == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ExamineText, ref target4, hookCtx, false, context))
      target4 = this.ExamineText;
    target.ExamineText = target4;
    string target5 = (string) null;
    if (this.SwitchText == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.SwitchText, ref target5, hookCtx, false, context))
      target5 = this.SwitchText;
    target.SwitchText = target5;
    List<PowerSwitchableCable> target6 = (List<PowerSwitchableCable>) null;
    if (this.Cables == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<PowerSwitchableCable>>(this.Cables, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<List<PowerSwitchableCable>>(this.Cables, hookCtx, context);
    target.Cables = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PowerSwitchableComponent target,
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
    PowerSwitchableComponent target1 = (PowerSwitchableComponent) target;
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
    PowerSwitchableComponent target1 = (PowerSwitchableComponent) target;
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
    PowerSwitchableComponent target1 = (PowerSwitchableComponent) target;
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
  virtual PowerSwitchableComponent Component.Instantiate() => new PowerSwitchableComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class PowerSwitchableComponent_AutoState : IComponentState
  {
    public int ActiveIndex;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class PowerSwitchableComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<PowerSwitchableComponent, ComponentGetState>(new ComponentEventRefHandler<PowerSwitchableComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<PowerSwitchableComponent, ComponentHandleState>(new ComponentEventRefHandler<PowerSwitchableComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      PowerSwitchableComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new PowerSwitchableComponent.PowerSwitchableComponent_AutoState()
      {
        ActiveIndex = component.ActiveIndex
      };
    }

    private void OnHandleState(
      EntityUid uid,
      PowerSwitchableComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is PowerSwitchableComponent.PowerSwitchableComponent_AutoState current))
        return;
      component.ActiveIndex = current.ActiveIndex;
    }
  }
}
