// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.ShakeStun.StunShakeableComponent
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
namespace Content.Shared._RMC14.ShakeStun;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (StunShakeableSystem)})]
public sealed class StunShakeableComponent : 
  Component,
  ISerializationGenerated<StunShakeableComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan DurationRemoved;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier ShakeSound;

  public StunShakeableComponent()
  {
    SoundPathSpecifier soundPathSpecifier = new SoundPathSpecifier("/Audio/Effects/thudswoosh.ogg");
    soundPathSpecifier.Params = AudioParams.Default.WithVariation(new float?(0.05f));
    this.ShakeSound = (SoundSpecifier) soundPathSpecifier;
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref StunShakeableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (StunShakeableComponent) target1;
    if (serialization.TryCustomCopy<StunShakeableComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DurationRemoved, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.DurationRemoved, hookCtx, context);
    target.DurationRemoved = target2;
    SoundSpecifier target3 = (SoundSpecifier) null;
    if (this.ShakeSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ShakeSound, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<SoundSpecifier>(this.ShakeSound, hookCtx, context);
    target.ShakeSound = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref StunShakeableComponent target,
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
    StunShakeableComponent target1 = (StunShakeableComponent) target;
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
    StunShakeableComponent target1 = (StunShakeableComponent) target;
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
    StunShakeableComponent target1 = (StunShakeableComponent) target;
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
  virtual StunShakeableComponent Component.Instantiate() => new StunShakeableComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class StunShakeableComponent_AutoState : IComponentState
  {
    public TimeSpan DurationRemoved;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class StunShakeableComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<StunShakeableComponent, ComponentGetState>(new ComponentEventRefHandler<StunShakeableComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<StunShakeableComponent, ComponentHandleState>(new ComponentEventRefHandler<StunShakeableComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      StunShakeableComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new StunShakeableComponent.StunShakeableComponent_AutoState()
      {
        DurationRemoved = component.DurationRemoved
      };
    }

    private void OnHandleState(
      EntityUid uid,
      StunShakeableComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is StunShakeableComponent.StunShakeableComponent_AutoState current))
        return;
      component.DurationRemoved = current.DurationRemoved;
    }
  }
}
