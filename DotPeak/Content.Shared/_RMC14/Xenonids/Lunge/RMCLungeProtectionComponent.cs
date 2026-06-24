// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Lunge.RMCLungeProtectionComponent
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
namespace Content.Shared._RMC14.Xenonids.Lunge;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class RMCLungeProtectionComponent : 
  Component,
  ISerializationGenerated<RMCLungeProtectionComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan StunDuration;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier BlockSound;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool FullProtection;

  public RMCLungeProtectionComponent()
  {
    SoundPathSpecifier soundPathSpecifier = new SoundPathSpecifier("/Audio/_RMC14/Machines/bonk.ogg");
    soundPathSpecifier.Params = AudioParams.Default.WithVariation(new float?(0.05f));
    this.BlockSound = (SoundSpecifier) soundPathSpecifier;
    this.FullProtection = true;
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCLungeProtectionComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCLungeProtectionComponent) target1;
    if (serialization.TryCustomCopy<RMCLungeProtectionComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.StunDuration, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.StunDuration, hookCtx, context);
    target.StunDuration = target2;
    SoundSpecifier target3 = (SoundSpecifier) null;
    if (this.BlockSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.BlockSound, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<SoundSpecifier>(this.BlockSound, hookCtx, context);
    target.BlockSound = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.FullProtection, ref target4, hookCtx, false, context))
      target4 = this.FullProtection;
    target.FullProtection = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCLungeProtectionComponent target,
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
    RMCLungeProtectionComponent target1 = (RMCLungeProtectionComponent) target;
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
    RMCLungeProtectionComponent target1 = (RMCLungeProtectionComponent) target;
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
    RMCLungeProtectionComponent target1 = (RMCLungeProtectionComponent) target;
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
  virtual RMCLungeProtectionComponent Component.Instantiate() => new RMCLungeProtectionComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCLungeProtectionComponent_AutoState : IComponentState
  {
    public TimeSpan StunDuration;
    public bool FullProtection;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCLungeProtectionComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCLungeProtectionComponent, ComponentGetState>(new ComponentEventRefHandler<RMCLungeProtectionComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCLungeProtectionComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCLungeProtectionComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCLungeProtectionComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCLungeProtectionComponent.RMCLungeProtectionComponent_AutoState()
      {
        StunDuration = component.StunDuration,
        FullProtection = component.FullProtection
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCLungeProtectionComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCLungeProtectionComponent.RMCLungeProtectionComponent_AutoState current))
        return;
      component.StunDuration = current.StunDuration;
      component.FullProtection = current.FullProtection;
    }
  }
}
