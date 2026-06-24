// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Marker.DamageMarkerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Weapons.Marker;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedDamageMarkerSystem)})]
[AutoGenerateComponentPause]
public sealed class DamageMarkerComponent : 
  Component,
  ISerializationGenerated<DamageMarkerComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("effect", false, 1, false, false, null)]
  public SpriteSpecifier.Rsi? Effect = new SpriteSpecifier.Rsi(new ResPath("/Textures/Objects/Weapons/Effects"), "shield2");
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("sound", false, 1, false, false, null)]
  public SoundSpecifier? Sound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Weapons/Guns/Gunshots/kinetic_accel.ogg");
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("damage", false, 1, false, false, null)]
  public DamageSpecifier Damage = new DamageSpecifier();
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("marker", false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid Marker;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("endTime", false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan EndTime;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DamageMarkerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (DamageMarkerComponent) target1;
    if (serialization.TryCustomCopy<DamageMarkerComponent>(this, ref target, hookCtx, false, context))
      return;
    SpriteSpecifier.Rsi target2 = (SpriteSpecifier.Rsi) null;
    if (!serialization.TryCustomCopy<SpriteSpecifier.Rsi>(this.Effect, ref target2, hookCtx, false, context))
    {
      if (this.Effect == null)
        target2 = (SpriteSpecifier.Rsi) null;
      else
        serialization.CopyTo<SpriteSpecifier.Rsi>(this.Effect, ref target2, hookCtx, context);
    }
    target.Effect = target2;
    SoundSpecifier target3 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target3;
    DamageSpecifier target4 = (DamageSpecifier) null;
    if (this.Damage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.Damage, ref target4, hookCtx, false, context))
    {
      if (this.Damage == null)
        target4 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.Damage, ref target4, hookCtx, context, true);
    }
    target.Damage = target4;
    EntityUid target5 = new EntityUid();
    if (!serialization.TryCustomCopy<EntityUid>(this.Marker, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntityUid>(this.Marker, hookCtx, context);
    target.Marker = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.EndTime, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.EndTime, hookCtx, context);
    target.EndTime = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DamageMarkerComponent target,
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
    DamageMarkerComponent target1 = (DamageMarkerComponent) target;
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
    DamageMarkerComponent target1 = (DamageMarkerComponent) target;
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
    DamageMarkerComponent target1 = (DamageMarkerComponent) target;
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
  virtual DamageMarkerComponent Component.Instantiate() => new DamageMarkerComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DamageMarkerComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<DamageMarkerComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<DamageMarkerComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      DamageMarkerComponent component,
      ref EntityUnpausedEvent args)
    {
      component.EndTime += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class DamageMarkerComponent_AutoState : IComponentState
  {
    public NetEntity Marker;
    public TimeSpan EndTime;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DamageMarkerComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<DamageMarkerComponent, ComponentGetState>(new ComponentEventRefHandler<DamageMarkerComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<DamageMarkerComponent, ComponentHandleState>(new ComponentEventRefHandler<DamageMarkerComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      #nullable enable
      DamageMarkerComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new DamageMarkerComponent.DamageMarkerComponent_AutoState()
      {
        Marker = this.GetNetEntity(component.Marker),
        EndTime = component.EndTime
      };
    }

    private void OnHandleState(
      EntityUid uid,
      DamageMarkerComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is DamageMarkerComponent.DamageMarkerComponent_AutoState current))
        return;
      component.Marker = this.EnsureEntity<DamageMarkerComponent>(current.Marker, uid);
      component.EndTime = current.EndTime;
    }
  }
}
