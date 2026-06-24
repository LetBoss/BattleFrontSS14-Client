// Decompiled with JetBrains decompiler
// Type: Content.Shared.Tools.Components.WeldableComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Tools.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class WeldableComponent : 
  Component,
  ISerializationGenerated<WeldableComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<ToolQualityPrototype> WeldingQuality = (ProtoId<ToolQualityPrototype>) "Welding";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Time = TimeSpan.FromSeconds(1.0);
  [DataField(null, false, 1, false, false, null)]
  public float Fuel = 3f;
  [DataField(null, false, 1, false, false, null)]
  public LocId? WeldedExamineMessage = (LocId?) "weldable-component-examine-is-welded";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool IsWelded;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref WeldableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (WeldableComponent) target1;
    if (serialization.TryCustomCopy<WeldableComponent>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<ToolQualityPrototype> target2 = new ProtoId<ToolQualityPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<ToolQualityPrototype>>(this.WeldingQuality, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<ProtoId<ToolQualityPrototype>>(this.WeldingQuality, hookCtx, context);
    target.WeldingQuality = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Time, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.Time, hookCtx, context);
    target.Time = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Fuel, ref target4, hookCtx, false, context))
      target4 = this.Fuel;
    target.Fuel = target4;
    LocId? target5 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.WeldedExamineMessage, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<LocId?>(this.WeldedExamineMessage, hookCtx, context);
    target.WeldedExamineMessage = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsWelded, ref target6, hookCtx, false, context))
      target6 = this.IsWelded;
    target.IsWelded = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref WeldableComponent target,
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
    WeldableComponent target1 = (WeldableComponent) target;
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
    WeldableComponent target1 = (WeldableComponent) target;
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
    WeldableComponent target1 = (WeldableComponent) target;
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
  virtual WeldableComponent Component.Instantiate() => new WeldableComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class WeldableComponent_AutoState : IComponentState
  {
    public TimeSpan Time;
    public bool IsWelded;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class WeldableComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<WeldableComponent, ComponentGetState>(new ComponentEventRefHandler<WeldableComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<WeldableComponent, ComponentHandleState>(new ComponentEventRefHandler<WeldableComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, WeldableComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new WeldableComponent.WeldableComponent_AutoState()
      {
        Time = component.Time,
        IsWelded = component.IsWelded
      };
    }

    private void OnHandleState(
      EntityUid uid,
      WeldableComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is WeldableComponent.WeldableComponent_AutoState current))
        return;
      component.Time = current.Time;
      component.IsWelded = current.IsWelded;
    }
  }
}
