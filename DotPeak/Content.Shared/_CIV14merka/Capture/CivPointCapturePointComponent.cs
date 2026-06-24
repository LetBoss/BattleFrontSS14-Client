// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Capture.CivPointCapturePointComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._CIV14merka.Capture;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
public sealed class CivPointCapturePointComponent : 
  Component,
  ISerializationGenerated<CivPointCapturePointComponent>,
  ISerializationGenerated
{
  [DataField("captureRadius", false, 1, false, false, null)]
  [AutoNetworkedField]
  public float CaptureRadius = 5f;
  [DataField("captureProgressPerSecond", false, 1, false, false, null)]
  public float CaptureProgressPerSecond = 0.1f;
  [DataField("captureBasePeople", false, 1, false, false, null)]
  public float CaptureBasePeople = 3f;
  [DataField("label", false, 1, false, false, null)]
  [AutoNetworkedField]
  public string Label = string.Empty;
  [DataField("initialOwnerTeamId", false, 1, false, false, null)]
  public int InitialOwnerTeamId;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int OwnerTeamId;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int CapturingTeamId;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float CaptureProgress;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int EchelonTier;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CivPointCapturePointComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (CivPointCapturePointComponent) target1;
    if (serialization.TryCustomCopy<CivPointCapturePointComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.CaptureRadius, ref target2, hookCtx, false, context))
      target2 = this.CaptureRadius;
    target.CaptureRadius = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.CaptureProgressPerSecond, ref target3, hookCtx, false, context))
      target3 = this.CaptureProgressPerSecond;
    target.CaptureProgressPerSecond = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.CaptureBasePeople, ref target4, hookCtx, false, context))
      target4 = this.CaptureBasePeople;
    target.CaptureBasePeople = target4;
    string target5 = (string) null;
    if (this.Label == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Label, ref target5, hookCtx, false, context))
      target5 = this.Label;
    target.Label = target5;
    int target6 = 0;
    if (!serialization.TryCustomCopy<int>(this.InitialOwnerTeamId, ref target6, hookCtx, false, context))
      target6 = this.InitialOwnerTeamId;
    target.InitialOwnerTeamId = target6;
    int target7 = 0;
    if (!serialization.TryCustomCopy<int>(this.OwnerTeamId, ref target7, hookCtx, false, context))
      target7 = this.OwnerTeamId;
    target.OwnerTeamId = target7;
    int target8 = 0;
    if (!serialization.TryCustomCopy<int>(this.CapturingTeamId, ref target8, hookCtx, false, context))
      target8 = this.CapturingTeamId;
    target.CapturingTeamId = target8;
    float target9 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.CaptureProgress, ref target9, hookCtx, false, context))
      target9 = this.CaptureProgress;
    target.CaptureProgress = target9;
    int target10 = 0;
    if (!serialization.TryCustomCopy<int>(this.EchelonTier, ref target10, hookCtx, false, context))
      target10 = this.EchelonTier;
    target.EchelonTier = target10;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CivPointCapturePointComponent target,
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
    CivPointCapturePointComponent target1 = (CivPointCapturePointComponent) target;
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
    CivPointCapturePointComponent target1 = (CivPointCapturePointComponent) target;
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
    CivPointCapturePointComponent target1 = (CivPointCapturePointComponent) target;
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
  virtual CivPointCapturePointComponent Component.Instantiate()
  {
    return new CivPointCapturePointComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class CivPointCapturePointComponent_AutoState : IComponentState
  {
    public float CaptureRadius;
    public string Label;
    public int OwnerTeamId;
    public int CapturingTeamId;
    public float CaptureProgress;
    public int EchelonTier;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class CivPointCapturePointComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<CivPointCapturePointComponent, ComponentGetState>(new ComponentEventRefHandler<CivPointCapturePointComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<CivPointCapturePointComponent, ComponentHandleState>(new ComponentEventRefHandler<CivPointCapturePointComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      CivPointCapturePointComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new CivPointCapturePointComponent.CivPointCapturePointComponent_AutoState()
      {
        CaptureRadius = component.CaptureRadius,
        Label = component.Label,
        OwnerTeamId = component.OwnerTeamId,
        CapturingTeamId = component.CapturingTeamId,
        CaptureProgress = component.CaptureProgress,
        EchelonTier = component.EchelonTier
      };
    }

    private void OnHandleState(
      EntityUid uid,
      CivPointCapturePointComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is CivPointCapturePointComponent.CivPointCapturePointComponent_AutoState current))
        return;
      component.CaptureRadius = current.CaptureRadius;
      component.Label = current.Label;
      component.OwnerTeamId = current.OwnerTeamId;
      component.CapturingTeamId = current.CapturingTeamId;
      component.CaptureProgress = current.CaptureProgress;
      component.EchelonTier = current.EchelonTier;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, CivPointCapturePointComponent>(uid, component, ref args1);
    }
  }
}
