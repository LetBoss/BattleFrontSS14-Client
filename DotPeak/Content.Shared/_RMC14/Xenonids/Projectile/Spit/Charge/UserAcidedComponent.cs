// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Projectile.Spit.Charge.UserAcidedComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Projectile.Spit.Charge;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (XenoSpitSystem)})]
public sealed class UserAcidedComponent : 
  Component,
  ISerializationGenerated<UserAcidedComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier Damage = new DamageSpecifier();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int ArmorPiercing;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Duration = TimeSpan.FromSeconds(20L);
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan ExpiresAt;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan NextDamageAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan DamageEvery = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Combo;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan ResistDuration = TimeSpan.FromSeconds(3L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int ResistsNeeded = 1;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan AllowVaporHitAfter;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan ExtinguishGracePeriod = TimeSpan.FromSeconds(1L);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref UserAcidedComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (UserAcidedComponent) target1;
    if (serialization.TryCustomCopy<UserAcidedComponent>(this, ref target, hookCtx, false, context))
      return;
    DamageSpecifier target2 = (DamageSpecifier) null;
    if (this.Damage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.Damage, ref target2, hookCtx, false, context))
    {
      if (this.Damage == null)
        target2 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.Damage, ref target2, hookCtx, context, true);
    }
    target.Damage = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.ArmorPiercing, ref target3, hookCtx, false, context))
      target3 = this.ArmorPiercing;
    target.ArmorPiercing = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Duration, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.Duration, hookCtx, context);
    target.Duration = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ExpiresAt, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.ExpiresAt, hookCtx, context);
    target.ExpiresAt = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextDamageAt, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.NextDamageAt, hookCtx, context);
    target.NextDamageAt = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DamageEvery, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.DamageEvery, hookCtx, context);
    target.DamageEvery = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.Combo, ref target8, hookCtx, false, context))
      target8 = this.Combo;
    target.Combo = target8;
    TimeSpan target9 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ResistDuration, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<TimeSpan>(this.ResistDuration, hookCtx, context);
    target.ResistDuration = target9;
    int target10 = 0;
    if (!serialization.TryCustomCopy<int>(this.ResistsNeeded, ref target10, hookCtx, false, context))
      target10 = this.ResistsNeeded;
    target.ResistsNeeded = target10;
    TimeSpan target11 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.AllowVaporHitAfter, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<TimeSpan>(this.AllowVaporHitAfter, hookCtx, context);
    target.AllowVaporHitAfter = target11;
    TimeSpan target12 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ExtinguishGracePeriod, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<TimeSpan>(this.ExtinguishGracePeriod, hookCtx, context);
    target.ExtinguishGracePeriod = target12;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref UserAcidedComponent target,
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
    UserAcidedComponent target1 = (UserAcidedComponent) target;
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
    UserAcidedComponent target1 = (UserAcidedComponent) target;
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
    UserAcidedComponent target1 = (UserAcidedComponent) target;
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
  virtual UserAcidedComponent Component.Instantiate() => new UserAcidedComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class UserAcidedComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<UserAcidedComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<UserAcidedComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      UserAcidedComponent component,
      ref EntityUnpausedEvent args)
    {
      component.ExpiresAt += args.PausedTime;
      component.NextDamageAt += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class UserAcidedComponent_AutoState : IComponentState
  {
    public 
    #nullable enable
    DamageSpecifier Damage;
    public int ArmorPiercing;
    public TimeSpan Duration;
    public TimeSpan ExpiresAt;
    public TimeSpan NextDamageAt;
    public TimeSpan DamageEvery;
    public bool Combo;
    public TimeSpan ResistDuration;
    public int ResistsNeeded;
    public TimeSpan AllowVaporHitAfter;
    public TimeSpan ExtinguishGracePeriod;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class UserAcidedComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<UserAcidedComponent, ComponentGetState>(new ComponentEventRefHandler<UserAcidedComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<UserAcidedComponent, ComponentHandleState>(new ComponentEventRefHandler<UserAcidedComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      UserAcidedComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new UserAcidedComponent.UserAcidedComponent_AutoState()
      {
        Damage = component.Damage,
        ArmorPiercing = component.ArmorPiercing,
        Duration = component.Duration,
        ExpiresAt = component.ExpiresAt,
        NextDamageAt = component.NextDamageAt,
        DamageEvery = component.DamageEvery,
        Combo = component.Combo,
        ResistDuration = component.ResistDuration,
        ResistsNeeded = component.ResistsNeeded,
        AllowVaporHitAfter = component.AllowVaporHitAfter,
        ExtinguishGracePeriod = component.ExtinguishGracePeriod
      };
    }

    private void OnHandleState(
      EntityUid uid,
      UserAcidedComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is UserAcidedComponent.UserAcidedComponent_AutoState current))
        return;
      component.Damage = current.Damage;
      component.ArmorPiercing = current.ArmorPiercing;
      component.Duration = current.Duration;
      component.ExpiresAt = current.ExpiresAt;
      component.NextDamageAt = current.NextDamageAt;
      component.DamageEvery = current.DamageEvery;
      component.Combo = current.Combo;
      component.ResistDuration = current.ResistDuration;
      component.ResistsNeeded = current.ResistsNeeded;
      component.AllowVaporHitAfter = current.AllowVaporHitAfter;
      component.ExtinguishGracePeriod = current.ExtinguishGracePeriod;
    }
  }
}
