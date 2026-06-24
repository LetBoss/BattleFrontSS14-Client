// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.SupplyDrop.CanBeSupplyDroppedComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.SupplyDrop;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedSupplyDropSystem)})]
public sealed class CanBeSupplyDroppedComponent : 
  Component,
  ISerializationGenerated<CanBeSupplyDroppedComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? LaunchSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Effects/bamf.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan ArrivingSoundDelay = TimeSpan.FromSeconds(9L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan DropDelay = TimeSpan.FromSeconds(12L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan OpenDelay = TimeSpan.FromSeconds(14L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId LandingEffectId = (EntProtoId) "RMCEffectAlert";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier? LandingDamage;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CanBeSupplyDroppedComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (CanBeSupplyDroppedComponent) target1;
    if (serialization.TryCustomCopy<CanBeSupplyDroppedComponent>(this, ref target, hookCtx, false, context))
      return;
    SoundSpecifier target2 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.LaunchSound, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<SoundSpecifier>(this.LaunchSound, hookCtx, context);
    target.LaunchSound = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ArrivingSoundDelay, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.ArrivingSoundDelay, hookCtx, context);
    target.ArrivingSoundDelay = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DropDelay, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.DropDelay, hookCtx, context);
    target.DropDelay = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.OpenDelay, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.OpenDelay, hookCtx, context);
    target.OpenDelay = target5;
    EntProtoId target6 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.LandingEffectId, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<EntProtoId>(this.LandingEffectId, hookCtx, context);
    target.LandingEffectId = target6;
    DamageSpecifier target7 = (DamageSpecifier) null;
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.LandingDamage, ref target7, hookCtx, false, context))
    {
      if (this.LandingDamage == null)
        target7 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.LandingDamage, ref target7, hookCtx, context);
    }
    target.LandingDamage = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CanBeSupplyDroppedComponent target,
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
    CanBeSupplyDroppedComponent target1 = (CanBeSupplyDroppedComponent) target;
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
    CanBeSupplyDroppedComponent target1 = (CanBeSupplyDroppedComponent) target;
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
    CanBeSupplyDroppedComponent target1 = (CanBeSupplyDroppedComponent) target;
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
  virtual CanBeSupplyDroppedComponent Component.Instantiate() => new CanBeSupplyDroppedComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class CanBeSupplyDroppedComponent_AutoState : IComponentState
  {
    public SoundSpecifier? LaunchSound;
    public TimeSpan ArrivingSoundDelay;
    public TimeSpan DropDelay;
    public TimeSpan OpenDelay;
    public EntProtoId LandingEffectId;
    public DamageSpecifier? LandingDamage;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class CanBeSupplyDroppedComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<CanBeSupplyDroppedComponent, ComponentGetState>(new ComponentEventRefHandler<CanBeSupplyDroppedComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<CanBeSupplyDroppedComponent, ComponentHandleState>(new ComponentEventRefHandler<CanBeSupplyDroppedComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      CanBeSupplyDroppedComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new CanBeSupplyDroppedComponent.CanBeSupplyDroppedComponent_AutoState()
      {
        LaunchSound = component.LaunchSound,
        ArrivingSoundDelay = component.ArrivingSoundDelay,
        DropDelay = component.DropDelay,
        OpenDelay = component.OpenDelay,
        LandingEffectId = component.LandingEffectId,
        LandingDamage = component.LandingDamage
      };
    }

    private void OnHandleState(
      EntityUid uid,
      CanBeSupplyDroppedComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is CanBeSupplyDroppedComponent.CanBeSupplyDroppedComponent_AutoState current))
        return;
      component.LaunchSound = current.LaunchSound;
      component.ArrivingSoundDelay = current.ArrivingSoundDelay;
      component.DropDelay = current.DropDelay;
      component.OpenDelay = current.OpenDelay;
      component.LandingEffectId = current.LandingEffectId;
      component.LandingDamage = current.LandingDamage;
    }
  }
}
