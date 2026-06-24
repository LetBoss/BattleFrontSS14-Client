// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Projectile.Spit.Charge.XenoChargeSpitComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
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
namespace Content.Shared._RMC14.Xenonids.Projectile.Spit.Charge;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (XenoSpitSystem)})]
public sealed class XenoChargeSpitComponent : 
  Component,
  ISerializationGenerated<XenoChargeSpitComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 PlasmaCost = (FixedPoint2) 50;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Duration = TimeSpan.FromSeconds(4L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Armor = 5;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Speed = 1.4f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId Effect = (EntProtoId) "RMCEffectEmpowerGreen";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoChargeSpitComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoChargeSpitComponent) target1;
    if (serialization.TryCustomCopy<XenoChargeSpitComponent>(this, ref target, hookCtx, false, context))
      return;
    FixedPoint2 target2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.PlasmaCost, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<FixedPoint2>(this.PlasmaCost, hookCtx, context);
    target.PlasmaCost = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Duration, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.Duration, hookCtx, context);
    target.Duration = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.Armor, ref target4, hookCtx, false, context))
      target4 = this.Armor;
    target.Armor = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Speed, ref target5, hookCtx, false, context))
      target5 = this.Speed;
    target.Speed = target5;
    EntProtoId target6 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Effect, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<EntProtoId>(this.Effect, hookCtx, context);
    target.Effect = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoChargeSpitComponent target,
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
    XenoChargeSpitComponent target1 = (XenoChargeSpitComponent) target;
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
    XenoChargeSpitComponent target1 = (XenoChargeSpitComponent) target;
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
    XenoChargeSpitComponent target1 = (XenoChargeSpitComponent) target;
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
  virtual XenoChargeSpitComponent Component.Instantiate() => new XenoChargeSpitComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoChargeSpitComponent_AutoState : IComponentState
  {
    public FixedPoint2 PlasmaCost;
    public TimeSpan Duration;
    public int Armor;
    public float Speed;
    public EntProtoId Effect;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoChargeSpitComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoChargeSpitComponent, ComponentGetState>(new ComponentEventRefHandler<XenoChargeSpitComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoChargeSpitComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoChargeSpitComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoChargeSpitComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoChargeSpitComponent.XenoChargeSpitComponent_AutoState()
      {
        PlasmaCost = component.PlasmaCost,
        Duration = component.Duration,
        Armor = component.Armor,
        Speed = component.Speed,
        Effect = component.Effect
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoChargeSpitComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoChargeSpitComponent.XenoChargeSpitComponent_AutoState current))
        return;
      component.PlasmaCost = current.PlasmaCost;
      component.Duration = current.Duration;
      component.Armor = current.Armor;
      component.Speed = current.Speed;
      component.Effect = current.Effect;
    }
  }
}
