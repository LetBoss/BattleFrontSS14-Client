// Decompiled with JetBrains decompiler
// Type: Content.Shared.StationAi.StationAiVisionComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Silicons.StationAi;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.StationAi;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedStationAiSystem)})]
public sealed class StationAiVisionComponent : 
  Component,
  ISerializationGenerated<StationAiVisionComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Enabled = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Occluded = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool NeedsPower;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool NeedsAnchoring;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Range = 7.5f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref StationAiVisionComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (StationAiVisionComponent) target1;
    if (serialization.TryCustomCopy<StationAiVisionComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Enabled, ref target2, hookCtx, false, context))
      target2 = this.Enabled;
    target.Enabled = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.Occluded, ref target3, hookCtx, false, context))
      target3 = this.Occluded;
    target.Occluded = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.NeedsPower, ref target4, hookCtx, false, context))
      target4 = this.NeedsPower;
    target.NeedsPower = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.NeedsAnchoring, ref target5, hookCtx, false, context))
      target5 = this.NeedsAnchoring;
    target.NeedsAnchoring = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Range, ref target6, hookCtx, false, context))
      target6 = this.Range;
    target.Range = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref StationAiVisionComponent target,
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
    StationAiVisionComponent target1 = (StationAiVisionComponent) target;
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
    StationAiVisionComponent target1 = (StationAiVisionComponent) target;
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
    StationAiVisionComponent target1 = (StationAiVisionComponent) target;
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
  virtual StationAiVisionComponent Component.Instantiate() => new StationAiVisionComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class StationAiVisionComponent_AutoState : IComponentState
  {
    public bool Enabled;
    public bool Occluded;
    public bool NeedsPower;
    public bool NeedsAnchoring;
    public float Range;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class StationAiVisionComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<StationAiVisionComponent, ComponentGetState>(new ComponentEventRefHandler<StationAiVisionComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<StationAiVisionComponent, ComponentHandleState>(new ComponentEventRefHandler<StationAiVisionComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      StationAiVisionComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new StationAiVisionComponent.StationAiVisionComponent_AutoState()
      {
        Enabled = component.Enabled,
        Occluded = component.Occluded,
        NeedsPower = component.NeedsPower,
        NeedsAnchoring = component.NeedsAnchoring,
        Range = component.Range
      };
    }

    private void OnHandleState(
      EntityUid uid,
      StationAiVisionComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is StationAiVisionComponent.StationAiVisionComponent_AutoState current))
        return;
      component.Enabled = current.Enabled;
      component.Occluded = current.Occluded;
      component.NeedsPower = current.NeedsPower;
      component.NeedsAnchoring = current.NeedsAnchoring;
      component.Range = current.Range;
    }
  }
}
