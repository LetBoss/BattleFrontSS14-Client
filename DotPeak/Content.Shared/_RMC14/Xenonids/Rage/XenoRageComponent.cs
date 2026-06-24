// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Rage.XenoRageComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Rage;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (XenoRageSystem)})]
public sealed class XenoRageComponent : 
  Component,
  ISerializationGenerated<XenoRageComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Rage;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MaxRage = 5;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool RageLocked;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int ArmorPerRage = 3;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float SpeedBuffPerRage = 0.028f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float AttackSpeedPerRage = 0.28f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan RageDecayTime = TimeSpan.FromSeconds(3L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan RageLockDuration = TimeSpan.FromSeconds(10L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan RageCooldownDuration = TimeSpan.FromSeconds(10L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan RageHealTime = TimeSpan.FromSeconds(0.05);
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan LastHit;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 HealAmount = (FixedPoint2) 45;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Color RageLockColor = Color.Black;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Color RageLockWeakenColor = Color.White;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan RageLockExpireAt;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan RageCooldownExpireAt;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoRageComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoRageComponent) target1;
    if (serialization.TryCustomCopy<XenoRageComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.Rage, ref target2, hookCtx, false, context))
      target2 = this.Rage;
    target.Rage = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxRage, ref target3, hookCtx, false, context))
      target3 = this.MaxRage;
    target.MaxRage = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.RageLocked, ref target4, hookCtx, false, context))
      target4 = this.RageLocked;
    target.RageLocked = target4;
    int target5 = 0;
    if (!serialization.TryCustomCopy<int>(this.ArmorPerRage, ref target5, hookCtx, false, context))
      target5 = this.ArmorPerRage;
    target.ArmorPerRage = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SpeedBuffPerRage, ref target6, hookCtx, false, context))
      target6 = this.SpeedBuffPerRage;
    target.SpeedBuffPerRage = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.AttackSpeedPerRage, ref target7, hookCtx, false, context))
      target7 = this.AttackSpeedPerRage;
    target.AttackSpeedPerRage = target7;
    TimeSpan target8 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.RageDecayTime, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<TimeSpan>(this.RageDecayTime, hookCtx, context);
    target.RageDecayTime = target8;
    TimeSpan target9 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.RageLockDuration, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<TimeSpan>(this.RageLockDuration, hookCtx, context);
    target.RageLockDuration = target9;
    TimeSpan target10 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.RageCooldownDuration, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<TimeSpan>(this.RageCooldownDuration, hookCtx, context);
    target.RageCooldownDuration = target10;
    TimeSpan target11 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.RageHealTime, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<TimeSpan>(this.RageHealTime, hookCtx, context);
    target.RageHealTime = target11;
    TimeSpan target12 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LastHit, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<TimeSpan>(this.LastHit, hookCtx, context);
    target.LastHit = target12;
    FixedPoint2 target13 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.HealAmount, ref target13, hookCtx, false, context))
      target13 = serialization.CreateCopy<FixedPoint2>(this.HealAmount, hookCtx, context);
    target.HealAmount = target13;
    Color target14 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.RageLockColor, ref target14, hookCtx, false, context))
      target14 = serialization.CreateCopy<Color>(this.RageLockColor, hookCtx, context);
    target.RageLockColor = target14;
    Color target15 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.RageLockWeakenColor, ref target15, hookCtx, false, context))
      target15 = serialization.CreateCopy<Color>(this.RageLockWeakenColor, hookCtx, context);
    target.RageLockWeakenColor = target15;
    TimeSpan target16 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.RageLockExpireAt, ref target16, hookCtx, false, context))
      target16 = serialization.CreateCopy<TimeSpan>(this.RageLockExpireAt, hookCtx, context);
    target.RageLockExpireAt = target16;
    TimeSpan target17 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.RageCooldownExpireAt, ref target17, hookCtx, false, context))
      target17 = serialization.CreateCopy<TimeSpan>(this.RageCooldownExpireAt, hookCtx, context);
    target.RageCooldownExpireAt = target17;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoRageComponent target,
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
    XenoRageComponent target1 = (XenoRageComponent) target;
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
    XenoRageComponent target1 = (XenoRageComponent) target;
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
    XenoRageComponent target1 = (XenoRageComponent) target;
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
  virtual XenoRageComponent Component.Instantiate() => new XenoRageComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoRageComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoRageComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<XenoRageComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      XenoRageComponent component,
      ref EntityUnpausedEvent args)
    {
      component.LastHit += args.PausedTime;
      component.RageLockExpireAt += args.PausedTime;
      component.RageCooldownExpireAt += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoRageComponent_AutoState : IComponentState
  {
    public int Rage;
    public int MaxRage;
    public bool RageLocked;
    public int ArmorPerRage;
    public float SpeedBuffPerRage;
    public float AttackSpeedPerRage;
    public TimeSpan RageDecayTime;
    public TimeSpan RageLockDuration;
    public TimeSpan RageCooldownDuration;
    public TimeSpan RageHealTime;
    public TimeSpan LastHit;
    public FixedPoint2 HealAmount;
    public Color RageLockColor;
    public Color RageLockWeakenColor;
    public TimeSpan RageLockExpireAt;
    public TimeSpan RageCooldownExpireAt;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoRageComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoRageComponent, ComponentGetState>(new ComponentEventRefHandler<XenoRageComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoRageComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoRageComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, 
    #nullable enable
    XenoRageComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoRageComponent.XenoRageComponent_AutoState()
      {
        Rage = component.Rage,
        MaxRage = component.MaxRage,
        RageLocked = component.RageLocked,
        ArmorPerRage = component.ArmorPerRage,
        SpeedBuffPerRage = component.SpeedBuffPerRage,
        AttackSpeedPerRage = component.AttackSpeedPerRage,
        RageDecayTime = component.RageDecayTime,
        RageLockDuration = component.RageLockDuration,
        RageCooldownDuration = component.RageCooldownDuration,
        RageHealTime = component.RageHealTime,
        LastHit = component.LastHit,
        HealAmount = component.HealAmount,
        RageLockColor = component.RageLockColor,
        RageLockWeakenColor = component.RageLockWeakenColor,
        RageLockExpireAt = component.RageLockExpireAt,
        RageCooldownExpireAt = component.RageCooldownExpireAt
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoRageComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoRageComponent.XenoRageComponent_AutoState current))
        return;
      component.Rage = current.Rage;
      component.MaxRage = current.MaxRage;
      component.RageLocked = current.RageLocked;
      component.ArmorPerRage = current.ArmorPerRage;
      component.SpeedBuffPerRage = current.SpeedBuffPerRage;
      component.AttackSpeedPerRage = current.AttackSpeedPerRage;
      component.RageDecayTime = current.RageDecayTime;
      component.RageLockDuration = current.RageLockDuration;
      component.RageCooldownDuration = current.RageCooldownDuration;
      component.RageHealTime = current.RageHealTime;
      component.LastHit = current.LastHit;
      component.HealAmount = current.HealAmount;
      component.RageLockColor = current.RageLockColor;
      component.RageLockWeakenColor = current.RageLockWeakenColor;
      component.RageLockExpireAt = current.RageLockExpireAt;
      component.RageCooldownExpireAt = current.RageCooldownExpireAt;
    }
  }
}
