// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Aircraft.AircraftComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Alert;
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
namespace Content.Shared._CIV14merka.Aircraft;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
public sealed class AircraftComponent : 
  Component,
  ISerializationGenerated<AircraftComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Altitude;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MaxAltitude = 3;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float TakeoffSpeed = 5f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float StallSpeed = 4f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float BrakeDeceleration = 4f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float MinAirTurnFactor = 0.5f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float AirborneScale = 1.3f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float ScalePerLevel = 0.35f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float OffsetPerLevel = 3f;
  [DataField(null, false, 1, false, false, null)]
  public float ShadowAlpha = 0.55f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float ZoomPerLevel = 0.6f;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId BombProto = (EntProtoId) "CivAircraftBomb";
  [DataField(null, false, 1, false, false, null)]
  public float BombFallTimePerLevel = 0.5f;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<AlertPrototype> AltitudeAlert = (ProtoId<AlertPrototype>) "AircraftAltitude";
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? CrashSound;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId AscendActionId = (EntProtoId) "ActionAircraftAscend";
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId DescendActionId = (EntProtoId) "ActionAircraftDescend";
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId BombActionId = (EntProtoId) "ActionAircraftBomb";

  public bool Airborne => this.Altitude > 0;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AircraftComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (AircraftComponent) target1;
    if (serialization.TryCustomCopy<AircraftComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.Altitude, ref target2, hookCtx, false, context))
      target2 = this.Altitude;
    target.Altitude = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxAltitude, ref target3, hookCtx, false, context))
      target3 = this.MaxAltitude;
    target.MaxAltitude = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.TakeoffSpeed, ref target4, hookCtx, false, context))
      target4 = this.TakeoffSpeed;
    target.TakeoffSpeed = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.StallSpeed, ref target5, hookCtx, false, context))
      target5 = this.StallSpeed;
    target.StallSpeed = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BrakeDeceleration, ref target6, hookCtx, false, context))
      target6 = this.BrakeDeceleration;
    target.BrakeDeceleration = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MinAirTurnFactor, ref target7, hookCtx, false, context))
      target7 = this.MinAirTurnFactor;
    target.MinAirTurnFactor = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.AirborneScale, ref target8, hookCtx, false, context))
      target8 = this.AirborneScale;
    target.AirborneScale = target8;
    float target9 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ScalePerLevel, ref target9, hookCtx, false, context))
      target9 = this.ScalePerLevel;
    target.ScalePerLevel = target9;
    float target10 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.OffsetPerLevel, ref target10, hookCtx, false, context))
      target10 = this.OffsetPerLevel;
    target.OffsetPerLevel = target10;
    float target11 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ShadowAlpha, ref target11, hookCtx, false, context))
      target11 = this.ShadowAlpha;
    target.ShadowAlpha = target11;
    float target12 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ZoomPerLevel, ref target12, hookCtx, false, context))
      target12 = this.ZoomPerLevel;
    target.ZoomPerLevel = target12;
    EntProtoId target13 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.BombProto, ref target13, hookCtx, false, context))
      target13 = serialization.CreateCopy<EntProtoId>(this.BombProto, hookCtx, context);
    target.BombProto = target13;
    float target14 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BombFallTimePerLevel, ref target14, hookCtx, false, context))
      target14 = this.BombFallTimePerLevel;
    target.BombFallTimePerLevel = target14;
    ProtoId<AlertPrototype> target15 = new ProtoId<AlertPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<AlertPrototype>>(this.AltitudeAlert, ref target15, hookCtx, false, context))
      target15 = serialization.CreateCopy<ProtoId<AlertPrototype>>(this.AltitudeAlert, hookCtx, context);
    target.AltitudeAlert = target15;
    SoundSpecifier target16 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.CrashSound, ref target16, hookCtx, true, context))
      target16 = serialization.CreateCopy<SoundSpecifier>(this.CrashSound, hookCtx, context);
    target.CrashSound = target16;
    EntProtoId target17 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.AscendActionId, ref target17, hookCtx, false, context))
      target17 = serialization.CreateCopy<EntProtoId>(this.AscendActionId, hookCtx, context);
    target.AscendActionId = target17;
    EntProtoId target18 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.DescendActionId, ref target18, hookCtx, false, context))
      target18 = serialization.CreateCopy<EntProtoId>(this.DescendActionId, hookCtx, context);
    target.DescendActionId = target18;
    EntProtoId target19 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.BombActionId, ref target19, hookCtx, false, context))
      target19 = serialization.CreateCopy<EntProtoId>(this.BombActionId, hookCtx, context);
    target.BombActionId = target19;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AircraftComponent target,
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
    AircraftComponent target1 = (AircraftComponent) target;
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
    AircraftComponent target1 = (AircraftComponent) target;
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
    AircraftComponent target1 = (AircraftComponent) target;
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
  virtual AircraftComponent Component.Instantiate() => new AircraftComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class AircraftComponent_AutoState : IComponentState
  {
    public int Altitude;
    public int MaxAltitude;
    public float TakeoffSpeed;
    public float StallSpeed;
    public float BrakeDeceleration;
    public float MinAirTurnFactor;
    public float AirborneScale;
    public float ScalePerLevel;
    public float OffsetPerLevel;
    public float ZoomPerLevel;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class AircraftComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<AircraftComponent, ComponentGetState>(new ComponentEventRefHandler<AircraftComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<AircraftComponent, ComponentHandleState>(new ComponentEventRefHandler<AircraftComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, AircraftComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new AircraftComponent.AircraftComponent_AutoState()
      {
        Altitude = component.Altitude,
        MaxAltitude = component.MaxAltitude,
        TakeoffSpeed = component.TakeoffSpeed,
        StallSpeed = component.StallSpeed,
        BrakeDeceleration = component.BrakeDeceleration,
        MinAirTurnFactor = component.MinAirTurnFactor,
        AirborneScale = component.AirborneScale,
        ScalePerLevel = component.ScalePerLevel,
        OffsetPerLevel = component.OffsetPerLevel,
        ZoomPerLevel = component.ZoomPerLevel
      };
    }

    private void OnHandleState(
      EntityUid uid,
      AircraftComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is AircraftComponent.AircraftComponent_AutoState current))
        return;
      component.Altitude = current.Altitude;
      component.MaxAltitude = current.MaxAltitude;
      component.TakeoffSpeed = current.TakeoffSpeed;
      component.StallSpeed = current.StallSpeed;
      component.BrakeDeceleration = current.BrakeDeceleration;
      component.MinAirTurnFactor = current.MinAirTurnFactor;
      component.AirborneScale = current.AirborneScale;
      component.ScalePerLevel = current.ScalePerLevel;
      component.OffsetPerLevel = current.OffsetPerLevel;
      component.ZoomPerLevel = current.ZoomPerLevel;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, AircraftComponent>(uid, component, ref args1);
    }
  }
}
