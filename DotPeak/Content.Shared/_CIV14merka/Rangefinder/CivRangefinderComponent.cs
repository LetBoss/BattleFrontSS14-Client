// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Rangefinder.CivRangefinderComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._CIV14merka.Rangefinder;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[Access(new Type[] {typeof (SharedCivRangefinderSystem)})]
public sealed class CivRangefinderComponent : 
  Component,
  ISerializationGenerated<CivRangefinderComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2i? LastTarget;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public MapCoordinates? LastCoords;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string TargetUseDelay = "civ_rangefinder_target";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan TargetDelay = TimeSpan.FromSeconds(0.5);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Content.Shared.DoAfter.DoAfter? DoAfter;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? TargetSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Binoculars/nightvision.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? AcquireSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Binoculars/binoctarget.ogg");
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId MarkerSpawn = (EntProtoId) "RMCRangefinderTarget";
  public EntityUid? MarkerEntity;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CivRangefinderComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (CivRangefinderComponent) target1;
    if (serialization.TryCustomCopy<CivRangefinderComponent>(this, ref target, hookCtx, false, context))
      return;
    Vector2i? target2 = new Vector2i?();
    if (!serialization.TryCustomCopy<Vector2i?>(this.LastTarget, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<Vector2i?>(this.LastTarget, hookCtx, context);
    target.LastTarget = target2;
    MapCoordinates? target3 = new MapCoordinates?();
    if (!serialization.TryCustomCopy<MapCoordinates?>(this.LastCoords, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<MapCoordinates?>(this.LastCoords, hookCtx, context);
    target.LastCoords = target3;
    string target4 = (string) null;
    if (this.TargetUseDelay == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.TargetUseDelay, ref target4, hookCtx, false, context))
      target4 = this.TargetUseDelay;
    target.TargetUseDelay = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.TargetDelay, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.TargetDelay, hookCtx, context);
    target.TargetDelay = target5;
    Content.Shared.DoAfter.DoAfter target6 = (Content.Shared.DoAfter.DoAfter) null;
    if (!serialization.TryCustomCopy<Content.Shared.DoAfter.DoAfter>(this.DoAfter, ref target6, hookCtx, false, context))
    {
      if (this.DoAfter == null)
        target6 = (Content.Shared.DoAfter.DoAfter) null;
      else
        serialization.CopyTo<Content.Shared.DoAfter.DoAfter>(this.DoAfter, ref target6, hookCtx, context);
    }
    target.DoAfter = target6;
    SoundSpecifier target7 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.TargetSound, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<SoundSpecifier>(this.TargetSound, hookCtx, context);
    target.TargetSound = target7;
    SoundSpecifier target8 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.AcquireSound, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<SoundSpecifier>(this.AcquireSound, hookCtx, context);
    target.AcquireSound = target8;
    EntProtoId target9 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.MarkerSpawn, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<EntProtoId>(this.MarkerSpawn, hookCtx, context);
    target.MarkerSpawn = target9;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CivRangefinderComponent target,
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
    CivRangefinderComponent target1 = (CivRangefinderComponent) target;
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
    CivRangefinderComponent target1 = (CivRangefinderComponent) target;
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
    CivRangefinderComponent target1 = (CivRangefinderComponent) target;
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
  virtual CivRangefinderComponent Component.Instantiate() => new CivRangefinderComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class CivRangefinderComponent_AutoState : IComponentState
  {
    public Vector2i? LastTarget;
    public MapCoordinates? LastCoords;
    public string TargetUseDelay;
    public TimeSpan TargetDelay;
    public Content.Shared.DoAfter.DoAfter? DoAfter;
    public SoundSpecifier? TargetSound;
    public SoundSpecifier? AcquireSound;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class CivRangefinderComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<CivRangefinderComponent, ComponentGetState>(new ComponentEventRefHandler<CivRangefinderComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<CivRangefinderComponent, ComponentHandleState>(new ComponentEventRefHandler<CivRangefinderComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      CivRangefinderComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new CivRangefinderComponent.CivRangefinderComponent_AutoState()
      {
        LastTarget = component.LastTarget,
        LastCoords = component.LastCoords,
        TargetUseDelay = component.TargetUseDelay,
        TargetDelay = component.TargetDelay,
        DoAfter = component.DoAfter,
        TargetSound = component.TargetSound,
        AcquireSound = component.AcquireSound
      };
    }

    private void OnHandleState(
      EntityUid uid,
      CivRangefinderComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is CivRangefinderComponent.CivRangefinderComponent_AutoState current))
        return;
      component.LastTarget = current.LastTarget;
      component.LastCoords = current.LastCoords;
      component.TargetUseDelay = current.TargetUseDelay;
      component.TargetDelay = current.TargetDelay;
      component.DoAfter = current.DoAfter;
      component.TargetSound = current.TargetSound;
      component.AcquireSound = current.AcquireSound;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, CivRangefinderComponent>(uid, component, ref args1);
    }
  }
}
