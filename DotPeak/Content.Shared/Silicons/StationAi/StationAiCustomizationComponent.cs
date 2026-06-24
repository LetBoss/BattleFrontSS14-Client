// Decompiled with JetBrains decompiler
// Type: Content.Shared.Silicons.StationAi.StationAiCustomizationComponent
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
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Silicons.StationAi;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class StationAiCustomizationComponent : 
  Component,
  ISerializationGenerated<StationAiCustomizationComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<ProtoId<StationAiCustomizationGroupPrototype>, ProtoId<StationAiCustomizationPrototype>> ProtoIds = new Dictionary<ProtoId<StationAiCustomizationGroupPrototype>, ProtoId<StationAiCustomizationPrototype>>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref StationAiCustomizationComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (StationAiCustomizationComponent) target1;
    if (serialization.TryCustomCopy<StationAiCustomizationComponent>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<ProtoId<StationAiCustomizationGroupPrototype>, ProtoId<StationAiCustomizationPrototype>> target2 = (Dictionary<ProtoId<StationAiCustomizationGroupPrototype>, ProtoId<StationAiCustomizationPrototype>>) null;
    if (this.ProtoIds == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<ProtoId<StationAiCustomizationGroupPrototype>, ProtoId<StationAiCustomizationPrototype>>>(this.ProtoIds, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<Dictionary<ProtoId<StationAiCustomizationGroupPrototype>, ProtoId<StationAiCustomizationPrototype>>>(this.ProtoIds, hookCtx, context);
    target.ProtoIds = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref StationAiCustomizationComponent target,
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
    StationAiCustomizationComponent target1 = (StationAiCustomizationComponent) target;
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
    StationAiCustomizationComponent target1 = (StationAiCustomizationComponent) target;
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
    StationAiCustomizationComponent target1 = (StationAiCustomizationComponent) target;
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
  virtual StationAiCustomizationComponent Component.Instantiate()
  {
    return new StationAiCustomizationComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class StationAiCustomizationComponent_AutoState : IComponentState
  {
    public Dictionary<ProtoId<StationAiCustomizationGroupPrototype>, ProtoId<StationAiCustomizationPrototype>> ProtoIds;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class StationAiCustomizationComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<StationAiCustomizationComponent, ComponentGetState>(new ComponentEventRefHandler<StationAiCustomizationComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<StationAiCustomizationComponent, ComponentHandleState>(new ComponentEventRefHandler<StationAiCustomizationComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      StationAiCustomizationComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new StationAiCustomizationComponent.StationAiCustomizationComponent_AutoState()
      {
        ProtoIds = component.ProtoIds
      };
    }

    private void OnHandleState(
      EntityUid uid,
      StationAiCustomizationComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is StationAiCustomizationComponent.StationAiCustomizationComponent_AutoState current))
        return;
      component.ProtoIds = current.ProtoIds == null ? (Dictionary<ProtoId<StationAiCustomizationGroupPrototype>, ProtoId<StationAiCustomizationPrototype>>) null : new Dictionary<ProtoId<StationAiCustomizationGroupPrototype>, ProtoId<StationAiCustomizationPrototype>>((IDictionary<ProtoId<StationAiCustomizationGroupPrototype>, ProtoId<StationAiCustomizationPrototype>>) current.ProtoIds);
    }
  }
}
