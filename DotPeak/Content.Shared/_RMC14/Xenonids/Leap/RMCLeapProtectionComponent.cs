// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Leap.RMCLeapProtectionComponent
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
namespace Content.Shared._RMC14.Xenonids.Leap;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class RMCLeapProtectionComponent : 
  Component,
  ISerializationGenerated<RMCLeapProtectionComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan StunDuration;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier BlockSound;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan? InherentStunDuration;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier InherentBlockSound;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public HashSet<EntityUid> ProtectionProviders;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool FullProtection;

  public RMCLeapProtectionComponent()
  {
    SoundPathSpecifier soundPathSpecifier1 = new SoundPathSpecifier("/Audio/_RMC14/Machines/bonk.ogg");
    soundPathSpecifier1.Params = AudioParams.Default.WithVariation(new float?(0.05f));
    this.BlockSound = (SoundSpecifier) soundPathSpecifier1;
    SoundPathSpecifier soundPathSpecifier2 = new SoundPathSpecifier("/Audio/_RMC14/Machines/bonk.ogg");
    soundPathSpecifier2.Params = AudioParams.Default.WithVariation(new float?(0.05f));
    this.InherentBlockSound = (SoundSpecifier) soundPathSpecifier2;
    this.ProtectionProviders = new HashSet<EntityUid>();
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCLeapProtectionComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCLeapProtectionComponent) target1;
    if (serialization.TryCustomCopy<RMCLeapProtectionComponent>(this, ref target, hookCtx, false, context))
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
    TimeSpan? target4 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.InherentStunDuration, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan?>(this.InherentStunDuration, hookCtx, context);
    target.InherentStunDuration = target4;
    SoundSpecifier target5 = (SoundSpecifier) null;
    if (this.InherentBlockSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.InherentBlockSound, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<SoundSpecifier>(this.InherentBlockSound, hookCtx, context);
    target.InherentBlockSound = target5;
    HashSet<EntityUid> target6 = (HashSet<EntityUid>) null;
    if (this.ProtectionProviders == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<EntityUid>>(this.ProtectionProviders, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<HashSet<EntityUid>>(this.ProtectionProviders, hookCtx, context);
    target.ProtectionProviders = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.FullProtection, ref target7, hookCtx, false, context))
      target7 = this.FullProtection;
    target.FullProtection = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCLeapProtectionComponent target,
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
    RMCLeapProtectionComponent target1 = (RMCLeapProtectionComponent) target;
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
    RMCLeapProtectionComponent target1 = (RMCLeapProtectionComponent) target;
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
    RMCLeapProtectionComponent target1 = (RMCLeapProtectionComponent) target;
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
  virtual RMCLeapProtectionComponent Component.Instantiate() => new RMCLeapProtectionComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCLeapProtectionComponent_AutoState : IComponentState
  {
    public TimeSpan StunDuration;
    public TimeSpan? InherentStunDuration;
    public HashSet<NetEntity> ProtectionProviders;
    public bool FullProtection;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCLeapProtectionComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCLeapProtectionComponent, ComponentGetState>(new ComponentEventRefHandler<RMCLeapProtectionComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCLeapProtectionComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCLeapProtectionComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCLeapProtectionComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCLeapProtectionComponent.RMCLeapProtectionComponent_AutoState()
      {
        StunDuration = component.StunDuration,
        InherentStunDuration = component.InherentStunDuration,
        ProtectionProviders = this.GetNetEntitySet(component.ProtectionProviders),
        FullProtection = component.FullProtection
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCLeapProtectionComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCLeapProtectionComponent.RMCLeapProtectionComponent_AutoState current))
        return;
      component.StunDuration = current.StunDuration;
      component.InherentStunDuration = current.InherentStunDuration;
      this.EnsureEntitySet<RMCLeapProtectionComponent>(current.ProtectionProviders, uid, component.ProtectionProviders);
      component.FullProtection = current.FullProtection;
    }
  }
}
