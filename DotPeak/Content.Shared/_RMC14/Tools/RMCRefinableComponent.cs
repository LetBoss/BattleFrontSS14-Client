// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Tools.RMCRefinableComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Storage;
using Content.Shared.Tools;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Tools;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (RMCToolSystem)})]
public sealed class RMCRefinableComponent : 
  Component,
  ISerializationGenerated<RMCRefinableComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Amount = 4;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<ToolQualityPrototype> Tool = (ProtoId<ToolQualityPrototype>) "Welding";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Delay;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Fuel;
  [DataField(null, false, 1, true, false, null)]
  public List<EntitySpawnEntry> Spawn = new List<EntitySpawnEntry>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCRefinableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCRefinableComponent) target1;
    if (serialization.TryCustomCopy<RMCRefinableComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.Amount, ref target2, hookCtx, false, context))
      target2 = this.Amount;
    target.Amount = target2;
    ProtoId<ToolQualityPrototype> target3 = new ProtoId<ToolQualityPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<ToolQualityPrototype>>(this.Tool, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<ProtoId<ToolQualityPrototype>>(this.Tool, hookCtx, context);
    target.Tool = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Delay, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.Delay, hookCtx, context);
    target.Delay = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Fuel, ref target5, hookCtx, false, context))
      target5 = this.Fuel;
    target.Fuel = target5;
    List<EntitySpawnEntry> target6 = (List<EntitySpawnEntry>) null;
    if (this.Spawn == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntitySpawnEntry>>(this.Spawn, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<List<EntitySpawnEntry>>(this.Spawn, hookCtx, context);
    target.Spawn = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCRefinableComponent target,
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
    RMCRefinableComponent target1 = (RMCRefinableComponent) target;
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
    RMCRefinableComponent target1 = (RMCRefinableComponent) target;
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
    RMCRefinableComponent target1 = (RMCRefinableComponent) target;
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
  virtual RMCRefinableComponent Component.Instantiate() => new RMCRefinableComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCRefinableComponent_AutoState : IComponentState
  {
    public int Amount;
    public ProtoId<ToolQualityPrototype> Tool;
    public TimeSpan Delay;
    public float Fuel;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCRefinableComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCRefinableComponent, ComponentGetState>(new ComponentEventRefHandler<RMCRefinableComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCRefinableComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCRefinableComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCRefinableComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCRefinableComponent.RMCRefinableComponent_AutoState()
      {
        Amount = component.Amount,
        Tool = component.Tool,
        Delay = component.Delay,
        Fuel = component.Fuel
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCRefinableComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCRefinableComponent.RMCRefinableComponent_AutoState current))
        return;
      component.Amount = current.Amount;
      component.Tool = current.Tool;
      component.Delay = current.Delay;
      component.Fuel = current.Fuel;
    }
  }
}
