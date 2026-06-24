// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Projectile.Spit.Charge.XenoActiveChargingSpitComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
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
public sealed class XenoActiveChargingSpitComponent : 
  Component,
  ISerializationGenerated<XenoActiveChargingSpitComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan ExpiresAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Armor = 5;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Speed = 1.4f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId Projectile = (EntProtoId) "XenoChargedSpitProjectile";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool FiredProjectile;
  [DataField(null, false, 1, false, false, null)]
  public bool DidPopup;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoActiveChargingSpitComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoActiveChargingSpitComponent) target1;
    if (serialization.TryCustomCopy<XenoActiveChargingSpitComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ExpiresAt, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.ExpiresAt, hookCtx, context);
    target.ExpiresAt = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.Armor, ref target3, hookCtx, false, context))
      target3 = this.Armor;
    target.Armor = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Speed, ref target4, hookCtx, false, context))
      target4 = this.Speed;
    target.Speed = target4;
    EntProtoId target5 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Projectile, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntProtoId>(this.Projectile, hookCtx, context);
    target.Projectile = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.FiredProjectile, ref target6, hookCtx, false, context))
      target6 = this.FiredProjectile;
    target.FiredProjectile = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.DidPopup, ref target7, hookCtx, false, context))
      target7 = this.DidPopup;
    target.DidPopup = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoActiveChargingSpitComponent target,
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
    XenoActiveChargingSpitComponent target1 = (XenoActiveChargingSpitComponent) target;
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
    XenoActiveChargingSpitComponent target1 = (XenoActiveChargingSpitComponent) target;
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
    XenoActiveChargingSpitComponent target1 = (XenoActiveChargingSpitComponent) target;
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
  virtual XenoActiveChargingSpitComponent Component.Instantiate()
  {
    return new XenoActiveChargingSpitComponent();
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoActiveChargingSpitComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoActiveChargingSpitComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<XenoActiveChargingSpitComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      XenoActiveChargingSpitComponent component,
      ref EntityUnpausedEvent args)
    {
      component.ExpiresAt += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoActiveChargingSpitComponent_AutoState : IComponentState
  {
    public TimeSpan ExpiresAt;
    public int Armor;
    public float Speed;
    public EntProtoId Projectile;
    public bool FiredProjectile;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoActiveChargingSpitComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoActiveChargingSpitComponent, ComponentGetState>(new ComponentEventRefHandler<XenoActiveChargingSpitComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoActiveChargingSpitComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoActiveChargingSpitComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      #nullable enable
      XenoActiveChargingSpitComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoActiveChargingSpitComponent.XenoActiveChargingSpitComponent_AutoState()
      {
        ExpiresAt = component.ExpiresAt,
        Armor = component.Armor,
        Speed = component.Speed,
        Projectile = component.Projectile,
        FiredProjectile = component.FiredProjectile
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoActiveChargingSpitComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoActiveChargingSpitComponent.XenoActiveChargingSpitComponent_AutoState current))
        return;
      component.ExpiresAt = current.ExpiresAt;
      component.Armor = current.Armor;
      component.Speed = current.Speed;
      component.Projectile = current.Projectile;
      component.FiredProjectile = current.FiredProjectile;
    }
  }
}
