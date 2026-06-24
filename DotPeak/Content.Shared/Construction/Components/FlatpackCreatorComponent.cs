// Decompiled with JetBrains decompiler
// Type: Content.Shared.Construction.Components.FlatpackCreatorComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Materials;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Construction.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedFlatpackSystem)})]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
public sealed class FlatpackCreatorComponent : 
  Component,
  ISerializationGenerated<FlatpackCreatorComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  [AutoNetworkedField]
  public bool Packing;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [Robust.Shared.ViewVariables.ViewVariables]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan PackEndTime;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public TimeSpan PackDuration = TimeSpan.FromSeconds(3L);
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public EntProtoId BaseFlatpackPrototype = EntProtoId.op_Implicit("BaseFlatpack");
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<ProtoId<MaterialPrototype>, int> BaseMachineCost = new Dictionary<ProtoId<MaterialPrototype>, int>();
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<ProtoId<MaterialPrototype>, int> BaseComputerCost = new Dictionary<ProtoId<MaterialPrototype>, int>();
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public string SlotId = "board_slot";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref FlatpackCreatorComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (FlatpackCreatorComponent) component;
    if (serialization.TryCustomCopy<FlatpackCreatorComponent>(this, ref target, hookCtx, false, context))
      return;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.Packing, ref flag, hookCtx, false, context))
      flag = this.Packing;
    target.Packing = flag;
    TimeSpan timeSpan1 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.PackEndTime, ref timeSpan1, hookCtx, false, context))
      timeSpan1 = serialization.CreateCopy<TimeSpan>(this.PackEndTime, hookCtx, context, false);
    target.PackEndTime = timeSpan1;
    TimeSpan timeSpan2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.PackDuration, ref timeSpan2, hookCtx, false, context))
      timeSpan2 = serialization.CreateCopy<TimeSpan>(this.PackDuration, hookCtx, context, false);
    target.PackDuration = timeSpan2;
    EntProtoId entProtoId = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.BaseFlatpackPrototype, ref entProtoId, hookCtx, false, context))
      entProtoId = serialization.CreateCopy<EntProtoId>(this.BaseFlatpackPrototype, hookCtx, context, false);
    target.BaseFlatpackPrototype = entProtoId;
    Dictionary<ProtoId<MaterialPrototype>, int> dictionary1 = (Dictionary<ProtoId<MaterialPrototype>, int>) null;
    if (this.BaseMachineCost == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<ProtoId<MaterialPrototype>, int>>(this.BaseMachineCost, ref dictionary1, hookCtx, true, context))
      dictionary1 = serialization.CreateCopy<Dictionary<ProtoId<MaterialPrototype>, int>>(this.BaseMachineCost, hookCtx, context, false);
    target.BaseMachineCost = dictionary1;
    Dictionary<ProtoId<MaterialPrototype>, int> dictionary2 = (Dictionary<ProtoId<MaterialPrototype>, int>) null;
    if (this.BaseComputerCost == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<ProtoId<MaterialPrototype>, int>>(this.BaseComputerCost, ref dictionary2, hookCtx, true, context))
      dictionary2 = serialization.CreateCopy<Dictionary<ProtoId<MaterialPrototype>, int>>(this.BaseComputerCost, hookCtx, context, false);
    target.BaseComputerCost = dictionary2;
    string str = (string) null;
    if (this.SlotId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.SlotId, ref str, hookCtx, false, context))
      str = this.SlotId;
    target.SlotId = str;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref FlatpackCreatorComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    FlatpackCreatorComponent target1 = (FlatpackCreatorComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    FlatpackCreatorComponent target1 = (FlatpackCreatorComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    FlatpackCreatorComponent target1 = (FlatpackCreatorComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    base.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual FlatpackCreatorComponent Component.Instantiate() => new FlatpackCreatorComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class FlatpackCreatorComponent_AutoPauseSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<FlatpackCreatorComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<FlatpackCreatorComponent, EntityUnpausedEvent>((object) this, __methodptr(OnEntityUnpaused)), (Type[]) null, (Type[]) null);
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      FlatpackCreatorComponent component,
      ref EntityUnpausedEvent args)
    {
      component.PackEndTime += args.PausedTime;
      this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class FlatpackCreatorComponent_AutoState : IComponentState
  {
    public bool Packing;
    public TimeSpan PackEndTime;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class FlatpackCreatorComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<FlatpackCreatorComponent, ComponentGetState>(new ComponentEventRefHandler<FlatpackCreatorComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<FlatpackCreatorComponent, ComponentHandleState>(new ComponentEventRefHandler<FlatpackCreatorComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(
      EntityUid uid,
      #nullable enable
      FlatpackCreatorComponent component,
      ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new FlatpackCreatorComponent.FlatpackCreatorComponent_AutoState()
      {
        Packing = component.Packing,
        PackEndTime = component.PackEndTime
      };
    }

    private void OnHandleState(
      EntityUid uid,
      FlatpackCreatorComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is FlatpackCreatorComponent.FlatpackCreatorComponent_AutoState current))
        return;
      component.Packing = current.Packing;
      component.PackEndTime = current.PackEndTime;
    }
  }
}
