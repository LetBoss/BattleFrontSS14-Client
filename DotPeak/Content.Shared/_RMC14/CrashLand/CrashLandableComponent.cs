// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.CrashLand.CrashLandableComponent
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
namespace Content.Shared._RMC14.CrashLand;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedCrashLandSystem)})]
public sealed class CrashLandableComponent : 
  Component,
  ISerializationGenerated<CrashLandableComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float CrashDuration = 0.75f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float FallHeight = 20f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier CrashSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Weapons/punch1.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan? LastCrash;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CrashLandableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (CrashLandableComponent) target1;
    if (serialization.TryCustomCopy<CrashLandableComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.CrashDuration, ref target2, hookCtx, false, context))
      target2 = this.CrashDuration;
    target.CrashDuration = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.FallHeight, ref target3, hookCtx, false, context))
      target3 = this.FallHeight;
    target.FallHeight = target3;
    SoundSpecifier target4 = (SoundSpecifier) null;
    if (this.CrashSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.CrashSound, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<SoundSpecifier>(this.CrashSound, hookCtx, context);
    target.CrashSound = target4;
    TimeSpan? target5 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.LastCrash, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan?>(this.LastCrash, hookCtx, context);
    target.LastCrash = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CrashLandableComponent target,
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
    CrashLandableComponent target1 = (CrashLandableComponent) target;
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
    CrashLandableComponent target1 = (CrashLandableComponent) target;
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
    CrashLandableComponent target1 = (CrashLandableComponent) target;
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
  virtual CrashLandableComponent Component.Instantiate() => new CrashLandableComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class CrashLandableComponent_AutoState : IComponentState
  {
    public float CrashDuration;
    public float FallHeight;
    public SoundSpecifier CrashSound;
    public TimeSpan? LastCrash;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class CrashLandableComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<CrashLandableComponent, ComponentGetState>(new ComponentEventRefHandler<CrashLandableComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<CrashLandableComponent, ComponentHandleState>(new ComponentEventRefHandler<CrashLandableComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      CrashLandableComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new CrashLandableComponent.CrashLandableComponent_AutoState()
      {
        CrashDuration = component.CrashDuration,
        FallHeight = component.FallHeight,
        CrashSound = component.CrashSound,
        LastCrash = component.LastCrash
      };
    }

    private void OnHandleState(
      EntityUid uid,
      CrashLandableComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is CrashLandableComponent.CrashLandableComponent_AutoState current))
        return;
      component.CrashDuration = current.CrashDuration;
      component.FallHeight = current.FallHeight;
      component.CrashSound = current.CrashSound;
      component.LastCrash = current.LastCrash;
    }
  }
}
