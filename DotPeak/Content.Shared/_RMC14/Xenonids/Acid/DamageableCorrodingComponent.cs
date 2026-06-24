// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Acid.DamageableCorrodingComponent
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
namespace Content.Shared._RMC14.Xenonids.Acid;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SharedXenoAcidSystem)})]
public sealed class DamageableCorrodingComponent : 
  Component,
  ISerializationGenerated<DamageableCorrodingComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid Acid;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public XenoAcidStrength Strength = XenoAcidStrength.Normal;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Dps;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier Damage;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan NextDamageAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan AcidExpiresAt;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DamageableCorrodingComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (DamageableCorrodingComponent) target1;
    if (serialization.TryCustomCopy<DamageableCorrodingComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityUid target2 = new EntityUid();
    if (!serialization.TryCustomCopy<EntityUid>(this.Acid, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityUid>(this.Acid, hookCtx, context);
    target.Acid = target2;
    XenoAcidStrength target3 = (XenoAcidStrength) 0;
    if (!serialization.TryCustomCopy<XenoAcidStrength>(this.Strength, ref target3, hookCtx, false, context))
      target3 = this.Strength;
    target.Strength = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Dps, ref target4, hookCtx, false, context))
      target4 = this.Dps;
    target.Dps = target4;
    DamageSpecifier target5 = (DamageSpecifier) null;
    if (this.Damage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.Damage, ref target5, hookCtx, false, context))
    {
      if (this.Damage == null)
        target5 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.Damage, ref target5, hookCtx, context, true);
    }
    target.Damage = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextDamageAt, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.NextDamageAt, hookCtx, context);
    target.NextDamageAt = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.AcidExpiresAt, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.AcidExpiresAt, hookCtx, context);
    target.AcidExpiresAt = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DamageableCorrodingComponent target,
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
    DamageableCorrodingComponent target1 = (DamageableCorrodingComponent) target;
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
    DamageableCorrodingComponent target1 = (DamageableCorrodingComponent) target;
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
    DamageableCorrodingComponent target1 = (DamageableCorrodingComponent) target;
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
  virtual DamageableCorrodingComponent Component.Instantiate()
  {
    return new DamageableCorrodingComponent();
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DamageableCorrodingComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<DamageableCorrodingComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<DamageableCorrodingComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      DamageableCorrodingComponent component,
      ref EntityUnpausedEvent args)
    {
      component.NextDamageAt += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class DamageableCorrodingComponent_AutoState : IComponentState
  {
    public NetEntity Acid;
    public XenoAcidStrength Strength;
    public float Dps;
    public 
    #nullable enable
    DamageSpecifier Damage;
    public TimeSpan NextDamageAt;
    public TimeSpan AcidExpiresAt;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DamageableCorrodingComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<DamageableCorrodingComponent, ComponentGetState>(new ComponentEventRefHandler<DamageableCorrodingComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<DamageableCorrodingComponent, ComponentHandleState>(new ComponentEventRefHandler<DamageableCorrodingComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      DamageableCorrodingComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new DamageableCorrodingComponent.DamageableCorrodingComponent_AutoState()
      {
        Acid = this.GetNetEntity(component.Acid),
        Strength = component.Strength,
        Dps = component.Dps,
        Damage = component.Damage,
        NextDamageAt = component.NextDamageAt,
        AcidExpiresAt = component.AcidExpiresAt
      };
    }

    private void OnHandleState(
      EntityUid uid,
      DamageableCorrodingComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is DamageableCorrodingComponent.DamageableCorrodingComponent_AutoState current))
        return;
      component.Acid = this.EnsureEntity<DamageableCorrodingComponent>(current.Acid, uid);
      component.Strength = current.Strength;
      component.Dps = current.Dps;
      component.Damage = current.Damage;
      component.NextDamageAt = current.NextDamageAt;
      component.AcidExpiresAt = current.AcidExpiresAt;
    }
  }
}
