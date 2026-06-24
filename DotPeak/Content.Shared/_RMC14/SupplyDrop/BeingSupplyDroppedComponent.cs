// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.SupplyDrop.BeingSupplyDroppedComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.SupplyDrop;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SharedSupplyDropSystem)})]
public sealed class BeingSupplyDroppedComponent : 
  Component,
  ISerializationGenerated<BeingSupplyDroppedComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityCoordinates Target;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan ArrivingSoundAt;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan DropAt;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan OpenAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? LandingEffect;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier? LandingDamage;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? ArrivingSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Machines/Techpod/techpod_drill.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool PlayedArrivingSound;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Landed;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? LandSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Machines/Techpod/techpod_hit.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? OpenSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Machines/Techpod/techpod_open.ogg");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref BeingSupplyDroppedComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (BeingSupplyDroppedComponent) target1;
    if (serialization.TryCustomCopy<BeingSupplyDroppedComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityCoordinates target2 = new EntityCoordinates();
    if (!serialization.TryCustomCopy<EntityCoordinates>(this.Target, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityCoordinates>(this.Target, hookCtx, context);
    target.Target = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ArrivingSoundAt, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.ArrivingSoundAt, hookCtx, context);
    target.ArrivingSoundAt = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DropAt, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.DropAt, hookCtx, context);
    target.DropAt = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.OpenAt, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.OpenAt, hookCtx, context);
    target.OpenAt = target5;
    EntityUid? target6 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.LandingEffect, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<EntityUid?>(this.LandingEffect, hookCtx, context);
    target.LandingEffect = target6;
    DamageSpecifier target7 = (DamageSpecifier) null;
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.LandingDamage, ref target7, hookCtx, false, context))
    {
      if (this.LandingDamage == null)
        target7 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.LandingDamage, ref target7, hookCtx, context);
    }
    target.LandingDamage = target7;
    SoundSpecifier target8 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ArrivingSound, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<SoundSpecifier>(this.ArrivingSound, hookCtx, context);
    target.ArrivingSound = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.PlayedArrivingSound, ref target9, hookCtx, false, context))
      target9 = this.PlayedArrivingSound;
    target.PlayedArrivingSound = target9;
    bool target10 = false;
    if (!serialization.TryCustomCopy<bool>(this.Landed, ref target10, hookCtx, false, context))
      target10 = this.Landed;
    target.Landed = target10;
    SoundSpecifier target11 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.LandSound, ref target11, hookCtx, true, context))
      target11 = serialization.CreateCopy<SoundSpecifier>(this.LandSound, hookCtx, context);
    target.LandSound = target11;
    SoundSpecifier target12 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.OpenSound, ref target12, hookCtx, true, context))
      target12 = serialization.CreateCopy<SoundSpecifier>(this.OpenSound, hookCtx, context);
    target.OpenSound = target12;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref BeingSupplyDroppedComponent target,
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
    BeingSupplyDroppedComponent target1 = (BeingSupplyDroppedComponent) target;
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
    BeingSupplyDroppedComponent target1 = (BeingSupplyDroppedComponent) target;
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
    BeingSupplyDroppedComponent target1 = (BeingSupplyDroppedComponent) target;
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
  virtual BeingSupplyDroppedComponent Component.Instantiate() => new BeingSupplyDroppedComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class BeingSupplyDroppedComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<BeingSupplyDroppedComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<BeingSupplyDroppedComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      BeingSupplyDroppedComponent component,
      ref EntityUnpausedEvent args)
    {
      component.ArrivingSoundAt += args.PausedTime;
      component.DropAt += args.PausedTime;
      component.OpenAt += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class BeingSupplyDroppedComponent_AutoState : IComponentState
  {
    public NetCoordinates Target;
    public TimeSpan ArrivingSoundAt;
    public TimeSpan DropAt;
    public TimeSpan OpenAt;
    public NetEntity? LandingEffect;
    public 
    #nullable enable
    DamageSpecifier? LandingDamage;
    public SoundSpecifier? ArrivingSound;
    public bool PlayedArrivingSound;
    public bool Landed;
    public SoundSpecifier? LandSound;
    public SoundSpecifier? OpenSound;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class BeingSupplyDroppedComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<BeingSupplyDroppedComponent, ComponentGetState>(new ComponentEventRefHandler<BeingSupplyDroppedComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<BeingSupplyDroppedComponent, ComponentHandleState>(new ComponentEventRefHandler<BeingSupplyDroppedComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      BeingSupplyDroppedComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new BeingSupplyDroppedComponent.BeingSupplyDroppedComponent_AutoState()
      {
        Target = this.GetNetCoordinates(component.Target),
        ArrivingSoundAt = component.ArrivingSoundAt,
        DropAt = component.DropAt,
        OpenAt = component.OpenAt,
        LandingEffect = this.GetNetEntity(component.LandingEffect),
        LandingDamage = component.LandingDamage,
        ArrivingSound = component.ArrivingSound,
        PlayedArrivingSound = component.PlayedArrivingSound,
        Landed = component.Landed,
        LandSound = component.LandSound,
        OpenSound = component.OpenSound
      };
    }

    private void OnHandleState(
      EntityUid uid,
      BeingSupplyDroppedComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is BeingSupplyDroppedComponent.BeingSupplyDroppedComponent_AutoState current))
        return;
      component.Target = this.EnsureCoordinates<BeingSupplyDroppedComponent>(current.Target, uid);
      component.ArrivingSoundAt = current.ArrivingSoundAt;
      component.DropAt = current.DropAt;
      component.OpenAt = current.OpenAt;
      component.LandingEffect = this.EnsureEntity<BeingSupplyDroppedComponent>(current.LandingEffect, uid);
      component.LandingDamage = current.LandingDamage;
      component.ArrivingSound = current.ArrivingSound;
      component.PlayedArrivingSound = current.PlayedArrivingSound;
      component.Landed = current.Landed;
      component.LandSound = current.LandSound;
      component.OpenSound = current.OpenSound;
    }
  }
}
