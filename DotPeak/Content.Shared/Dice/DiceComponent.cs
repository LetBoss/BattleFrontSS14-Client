// Decompiled with JetBrains decompiler
// Type: Content.Shared.Dice.DiceComponent
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
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Dice;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedDiceSystem)})]
[AutoGenerateComponentState(true, false)]
public sealed class DiceComponent : 
  Component,
  ISerializationGenerated<DiceComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier Sound { get; private set; } = (SoundSpecifier) new SoundCollectionSpecifier("Dice", new AudioParams?());

  [DataField(null, false, 1, false, false, null)]
  public int Multiplier { get; private set; } = 1;

  [DataField(null, false, 1, false, false, null)]
  public int Offset { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  public int Sides { get; private set; } = 20;

  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int CurrentValue { get; set; } = 20;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DiceComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (DiceComponent) component;
    if (serialization.TryCustomCopy<DiceComponent>(this, ref target, hookCtx, false, context))
      return;
    SoundSpecifier soundSpecifier = (SoundSpecifier) null;
    if (this.Sound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref soundSpecifier, hookCtx, true, context))
      soundSpecifier = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context, false);
    target.Sound = soundSpecifier;
    int num1 = 0;
    if (!serialization.TryCustomCopy<int>(this.Multiplier, ref num1, hookCtx, false, context))
      num1 = this.Multiplier;
    target.Multiplier = num1;
    int num2 = 0;
    if (!serialization.TryCustomCopy<int>(this.Offset, ref num2, hookCtx, false, context))
      num2 = this.Offset;
    target.Offset = num2;
    int num3 = 0;
    if (!serialization.TryCustomCopy<int>(this.Sides, ref num3, hookCtx, false, context))
      num3 = this.Sides;
    target.Sides = num3;
    int num4 = 0;
    if (!serialization.TryCustomCopy<int>(this.CurrentValue, ref num4, hookCtx, false, context))
      num4 = this.CurrentValue;
    target.CurrentValue = num4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DiceComponent target,
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
    DiceComponent target1 = (DiceComponent) target;
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
    DiceComponent target1 = (DiceComponent) target;
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
    DiceComponent target1 = (DiceComponent) target;
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
  virtual DiceComponent Component.Instantiate() => new DiceComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class DiceComponent_AutoState : IComponentState
  {
    public int CurrentValue;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DiceComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<DiceComponent, ComponentGetState>(new ComponentEventRefHandler<DiceComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<DiceComponent, ComponentHandleState>(new ComponentEventRefHandler<DiceComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(EntityUid uid, DiceComponent component, ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new DiceComponent.DiceComponent_AutoState()
      {
        CurrentValue = component.CurrentValue
      };
    }

    private void OnHandleState(
      EntityUid uid,
      DiceComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is DiceComponent.DiceComponent_AutoState current))
        return;
      component.CurrentValue = current.CurrentValue;
      AfterAutoHandleStateEvent handleStateEvent;
      // ISSUE: explicit constructor call
      ((AfterAutoHandleStateEvent) ref handleStateEvent).\u002Ector(((ComponentHandleState) ref args).Current);
      ((IDirectedEventBus) this.EntityManager.EventBus).RaiseComponentEvent<AfterAutoHandleStateEvent, DiceComponent>(uid, component, ref handleStateEvent);
    }
  }
}
