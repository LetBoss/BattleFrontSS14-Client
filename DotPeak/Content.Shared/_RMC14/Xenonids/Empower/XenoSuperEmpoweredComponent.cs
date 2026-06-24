// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Empower.XenoSuperEmpoweredComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Empower;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class XenoSuperEmpoweredComponent : 
  Component,
  ISerializationGenerated<XenoSuperEmpoweredComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan PartialExpireAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan ExpireTime = TimeSpan.FromSeconds(1.5);
  [DataField(null, false, 1, false, false, null)]
  public Color FadingEmpowerColor = Color.FromHex((ReadOnlySpan<char>) "#FF000023", new Color?());
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan? ExpiresAt;
  [DataField(null, false, 1, false, false, null)]
  public int EmpoweredTargets;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier DamageIncreasePer = new DamageSpecifier();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier DamageTailIncreasePer = new DamageSpecifier();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier LeapDamage = new DamageSpecifier();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float FlingDistance = 3f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan StunDuration = TimeSpan.FromSeconds(3.2);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoSuperEmpoweredComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoSuperEmpoweredComponent) target1;
    if (serialization.TryCustomCopy<XenoSuperEmpoweredComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.PartialExpireAt, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.PartialExpireAt, hookCtx, context);
    target.PartialExpireAt = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ExpireTime, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.ExpireTime, hookCtx, context);
    target.ExpireTime = target3;
    Color target4 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.FadingEmpowerColor, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<Color>(this.FadingEmpowerColor, hookCtx, context);
    target.FadingEmpowerColor = target4;
    TimeSpan? target5 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.ExpiresAt, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan?>(this.ExpiresAt, hookCtx, context);
    target.ExpiresAt = target5;
    int target6 = 0;
    if (!serialization.TryCustomCopy<int>(this.EmpoweredTargets, ref target6, hookCtx, false, context))
      target6 = this.EmpoweredTargets;
    target.EmpoweredTargets = target6;
    DamageSpecifier target7 = (DamageSpecifier) null;
    if (this.DamageIncreasePer == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.DamageIncreasePer, ref target7, hookCtx, false, context))
    {
      if (this.DamageIncreasePer == null)
        target7 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.DamageIncreasePer, ref target7, hookCtx, context, true);
    }
    target.DamageIncreasePer = target7;
    DamageSpecifier target8 = (DamageSpecifier) null;
    if (this.DamageTailIncreasePer == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.DamageTailIncreasePer, ref target8, hookCtx, false, context))
    {
      if (this.DamageTailIncreasePer == null)
        target8 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.DamageTailIncreasePer, ref target8, hookCtx, context, true);
    }
    target.DamageTailIncreasePer = target8;
    DamageSpecifier target9 = (DamageSpecifier) null;
    if (this.LeapDamage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.LeapDamage, ref target9, hookCtx, false, context))
    {
      if (this.LeapDamage == null)
        target9 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.LeapDamage, ref target9, hookCtx, context, true);
    }
    target.LeapDamage = target9;
    float target10 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.FlingDistance, ref target10, hookCtx, false, context))
      target10 = this.FlingDistance;
    target.FlingDistance = target10;
    TimeSpan target11 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.StunDuration, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<TimeSpan>(this.StunDuration, hookCtx, context);
    target.StunDuration = target11;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoSuperEmpoweredComponent target,
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
    XenoSuperEmpoweredComponent target1 = (XenoSuperEmpoweredComponent) target;
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
    XenoSuperEmpoweredComponent target1 = (XenoSuperEmpoweredComponent) target;
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
    XenoSuperEmpoweredComponent target1 = (XenoSuperEmpoweredComponent) target;
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
  virtual XenoSuperEmpoweredComponent Component.Instantiate() => new XenoSuperEmpoweredComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoSuperEmpoweredComponent_AutoState : IComponentState
  {
    public TimeSpan PartialExpireAt;
    public TimeSpan ExpireTime;
    public TimeSpan? ExpiresAt;
    public DamageSpecifier DamageIncreasePer;
    public DamageSpecifier DamageTailIncreasePer;
    public DamageSpecifier LeapDamage;
    public float FlingDistance;
    public TimeSpan StunDuration;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoSuperEmpoweredComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoSuperEmpoweredComponent, ComponentGetState>(new ComponentEventRefHandler<XenoSuperEmpoweredComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoSuperEmpoweredComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoSuperEmpoweredComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoSuperEmpoweredComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoSuperEmpoweredComponent.XenoSuperEmpoweredComponent_AutoState()
      {
        PartialExpireAt = component.PartialExpireAt,
        ExpireTime = component.ExpireTime,
        ExpiresAt = component.ExpiresAt,
        DamageIncreasePer = component.DamageIncreasePer,
        DamageTailIncreasePer = component.DamageTailIncreasePer,
        LeapDamage = component.LeapDamage,
        FlingDistance = component.FlingDistance,
        StunDuration = component.StunDuration
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoSuperEmpoweredComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoSuperEmpoweredComponent.XenoSuperEmpoweredComponent_AutoState current))
        return;
      component.PartialExpireAt = current.PartialExpireAt;
      component.ExpireTime = current.ExpireTime;
      component.ExpiresAt = current.ExpiresAt;
      component.DamageIncreasePer = current.DamageIncreasePer;
      component.DamageTailIncreasePer = current.DamageTailIncreasePer;
      component.LeapDamage = current.LeapDamage;
      component.FlingDistance = current.FlingDistance;
      component.StunDuration = current.StunDuration;
    }
  }
}
