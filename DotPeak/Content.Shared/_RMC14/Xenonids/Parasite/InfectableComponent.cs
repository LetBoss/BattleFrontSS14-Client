// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Parasite.InfectableComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Humanoid;
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
namespace Content.Shared._RMC14.Xenonids.Parasite;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class InfectableComponent : 
  Component,
  ISerializationGenerated<InfectableComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool BeingInfected;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<Sex, SoundSpecifier> Sound = new Dictionary<Sex, SoundSpecifier>()
  {
    [Sex.Male] = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Voice/Human/infected_male.ogg"),
    [Sex.Female] = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Voice/Human/infected_female.ogg"),
    [Sex.Unsexed] = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Voice/Human/infected_male.ogg")
  };

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref InfectableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (InfectableComponent) target1;
    if (serialization.TryCustomCopy<InfectableComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.BeingInfected, ref target2, hookCtx, false, context))
      target2 = this.BeingInfected;
    target.BeingInfected = target2;
    Dictionary<Sex, SoundSpecifier> target3 = (Dictionary<Sex, SoundSpecifier>) null;
    if (this.Sound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<Sex, SoundSpecifier>>(this.Sound, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<Dictionary<Sex, SoundSpecifier>>(this.Sound, hookCtx, context);
    target.Sound = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref InfectableComponent target,
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
    InfectableComponent target1 = (InfectableComponent) target;
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
    InfectableComponent target1 = (InfectableComponent) target;
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
    InfectableComponent target1 = (InfectableComponent) target;
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
  virtual InfectableComponent Component.Instantiate() => new InfectableComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class InfectableComponent_AutoState : IComponentState
  {
    public bool BeingInfected;
    public Dictionary<Sex, SoundSpecifier> Sound;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class InfectableComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<InfectableComponent, ComponentGetState>(new ComponentEventRefHandler<InfectableComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<InfectableComponent, ComponentHandleState>(new ComponentEventRefHandler<InfectableComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      InfectableComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new InfectableComponent.InfectableComponent_AutoState()
      {
        BeingInfected = component.BeingInfected,
        Sound = component.Sound
      };
    }

    private void OnHandleState(
      EntityUid uid,
      InfectableComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is InfectableComponent.InfectableComponent_AutoState current))
        return;
      component.BeingInfected = current.BeingInfected;
      component.Sound = current.Sound == null ? (Dictionary<Sex, SoundSpecifier>) null : new Dictionary<Sex, SoundSpecifier>((IDictionary<Sex, SoundSpecifier>) current.Sound);
    }
  }
}
