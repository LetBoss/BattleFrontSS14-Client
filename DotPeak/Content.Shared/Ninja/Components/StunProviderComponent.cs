// Decompiled with JetBrains decompiler
// Type: Content.Shared.Ninja.Components.StunProviderComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Content.Shared.Ninja.Systems;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Ninja.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedStunProviderSystem)})]
public sealed class StunProviderComponent : 
  Component,
  ISerializationGenerated<StunProviderComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? BatteryUid;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier Sound = (SoundSpecifier) new SoundCollectionSpecifier("sparks");
  [DataField(null, false, 1, false, false, null)]
  public float StunCharge = 36f;
  [DataField(null, false, 1, false, false, null)]
  public DamageSpecifier StunDamage = new DamageSpecifier()
  {
    DamageDict = new Dictionary<string, FixedPoint2>()
    {
      {
        "Shock",
        (FixedPoint2) 5
      }
    }
  };
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan StunTime = TimeSpan.FromSeconds(5L);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan Cooldown = TimeSpan.FromSeconds(2L);
  [DataField(null, false, 1, false, false, null)]
  public string DelayId = "stun_cooldown";
  [DataField(null, false, 1, true, false, null)]
  public LocId NoPowerPopup = (LocId) string.Empty;
  [DataField(null, false, 1, true, false, null)]
  public EntityWhitelist Whitelist = new EntityWhitelist();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref StunProviderComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (StunProviderComponent) target1;
    if (serialization.TryCustomCopy<StunProviderComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityUid? target2 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.BatteryUid, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityUid?>(this.BatteryUid, hookCtx, context);
    target.BatteryUid = target2;
    SoundSpecifier target3 = (SoundSpecifier) null;
    if (this.Sound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.StunCharge, ref target4, hookCtx, false, context))
      target4 = this.StunCharge;
    target.StunCharge = target4;
    DamageSpecifier target5 = (DamageSpecifier) null;
    if (this.StunDamage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.StunDamage, ref target5, hookCtx, false, context))
    {
      if (this.StunDamage == null)
        target5 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.StunDamage, ref target5, hookCtx, context, true);
    }
    target.StunDamage = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.StunTime, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.StunTime, hookCtx, context);
    target.StunTime = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Cooldown, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.Cooldown, hookCtx, context);
    target.Cooldown = target7;
    string target8 = (string) null;
    if (this.DelayId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.DelayId, ref target8, hookCtx, false, context))
      target8 = this.DelayId;
    target.DelayId = target8;
    LocId target9 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.NoPowerPopup, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<LocId>(this.NoPowerPopup, hookCtx, context);
    target.NoPowerPopup = target9;
    EntityWhitelist target10 = (EntityWhitelist) null;
    if (this.Whitelist == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref target10, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        target10 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref target10, hookCtx, context, true);
    }
    target.Whitelist = target10;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref StunProviderComponent target,
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
    StunProviderComponent target1 = (StunProviderComponent) target;
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
    StunProviderComponent target1 = (StunProviderComponent) target;
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
    StunProviderComponent target1 = (StunProviderComponent) target;
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
  virtual StunProviderComponent Component.Instantiate() => new StunProviderComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class StunProviderComponent_AutoState : IComponentState
  {
    public NetEntity? BatteryUid;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class StunProviderComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<StunProviderComponent, ComponentGetState>(new ComponentEventRefHandler<StunProviderComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<StunProviderComponent, ComponentHandleState>(new ComponentEventRefHandler<StunProviderComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      StunProviderComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new StunProviderComponent.StunProviderComponent_AutoState()
      {
        BatteryUid = this.GetNetEntity(component.BatteryUid)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      StunProviderComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is StunProviderComponent.StunProviderComponent_AutoState current))
        return;
      component.BatteryUid = this.EnsureEntity<StunProviderComponent>(current.BatteryUid, uid);
    }
  }
}
