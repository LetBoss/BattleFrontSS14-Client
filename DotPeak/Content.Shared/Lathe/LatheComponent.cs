// Decompiled with JetBrains decompiler
// Type: Content.Shared.Lathe.LatheComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Lathe.Prototypes;
using Content.Shared.Research.Prototypes;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Lathe;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class LatheComponent : 
  Component,
  ISerializationGenerated<LatheComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public List<ProtoId<LatheRecipePackPrototype>> StaticPacks = new List<ProtoId<LatheRecipePackPrototype>>();
  [DataField(null, false, 1, false, false, null)]
  public List<ProtoId<LatheRecipePackPrototype>> DynamicPacks = new List<ProtoId<LatheRecipePackPrototype>>();
  [DataField(null, false, 1, false, false, null)]
  public System.Collections.Generic.Queue<ProtoId<LatheRecipePrototype>> Queue = new System.Collections.Generic.Queue<ProtoId<LatheRecipePrototype>>();
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? ProducingSound;
  [DataField(null, false, 1, false, false, null)]
  public string? ReagentOutputSlotId;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int DefaultProductionAmount = 1;
  [DataField(null, false, 1, false, false, null)]
  public string? IdleState;
  [DataField(null, false, 1, false, false, null)]
  public string? RunningState;
  [DataField(null, false, 1, false, false, null)]
  public string? UnlitIdleState;
  [DataField(null, false, 1, false, false, null)]
  public string? UnlitRunningState;
  [Robust.Shared.ViewVariables.ViewVariables]
  public ProtoId<LatheRecipePrototype>? CurrentRecipe;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float TimeMultiplier = 1f;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [AutoNetworkedField]
  public float MaterialUseMultiplier = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MaxQueue = 6;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref LatheComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (LatheComponent) target1;
    if (serialization.TryCustomCopy<LatheComponent>(this, ref target, hookCtx, false, context))
      return;
    List<ProtoId<LatheRecipePackPrototype>> target2 = (List<ProtoId<LatheRecipePackPrototype>>) null;
    if (this.StaticPacks == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<ProtoId<LatheRecipePackPrototype>>>(this.StaticPacks, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<List<ProtoId<LatheRecipePackPrototype>>>(this.StaticPacks, hookCtx, context);
    target.StaticPacks = target2;
    List<ProtoId<LatheRecipePackPrototype>> target3 = (List<ProtoId<LatheRecipePackPrototype>>) null;
    if (this.DynamicPacks == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<ProtoId<LatheRecipePackPrototype>>>(this.DynamicPacks, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<List<ProtoId<LatheRecipePackPrototype>>>(this.DynamicPacks, hookCtx, context);
    target.DynamicPacks = target3;
    System.Collections.Generic.Queue<ProtoId<LatheRecipePrototype>> target4 = (System.Collections.Generic.Queue<ProtoId<LatheRecipePrototype>>) null;
    if (this.Queue == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<System.Collections.Generic.Queue<ProtoId<LatheRecipePrototype>>>(this.Queue, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<System.Collections.Generic.Queue<ProtoId<LatheRecipePrototype>>>(this.Queue, hookCtx, context);
    target.Queue = target4;
    SoundSpecifier target5 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ProducingSound, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<SoundSpecifier>(this.ProducingSound, hookCtx, context);
    target.ProducingSound = target5;
    string target6 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.ReagentOutputSlotId, ref target6, hookCtx, false, context))
      target6 = this.ReagentOutputSlotId;
    target.ReagentOutputSlotId = target6;
    int target7 = 0;
    if (!serialization.TryCustomCopy<int>(this.DefaultProductionAmount, ref target7, hookCtx, false, context))
      target7 = this.DefaultProductionAmount;
    target.DefaultProductionAmount = target7;
    string target8 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.IdleState, ref target8, hookCtx, false, context))
      target8 = this.IdleState;
    target.IdleState = target8;
    string target9 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.RunningState, ref target9, hookCtx, false, context))
      target9 = this.RunningState;
    target.RunningState = target9;
    string target10 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.UnlitIdleState, ref target10, hookCtx, false, context))
      target10 = this.UnlitIdleState;
    target.UnlitIdleState = target10;
    string target11 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.UnlitRunningState, ref target11, hookCtx, false, context))
      target11 = this.UnlitRunningState;
    target.UnlitRunningState = target11;
    float target12 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.TimeMultiplier, ref target12, hookCtx, false, context))
      target12 = this.TimeMultiplier;
    target.TimeMultiplier = target12;
    float target13 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaterialUseMultiplier, ref target13, hookCtx, false, context))
      target13 = this.MaterialUseMultiplier;
    target.MaterialUseMultiplier = target13;
    int target14 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxQueue, ref target14, hookCtx, false, context))
      target14 = this.MaxQueue;
    target.MaxQueue = target14;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref LatheComponent target,
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
    LatheComponent target1 = (LatheComponent) target;
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
    LatheComponent target1 = (LatheComponent) target;
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
    LatheComponent target1 = (LatheComponent) target;
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
  virtual LatheComponent Component.Instantiate() => new LatheComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class LatheComponent_AutoState : IComponentState
  {
    public int DefaultProductionAmount;
    public float MaterialUseMultiplier;
    public int MaxQueue;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class LatheComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<LatheComponent, ComponentGetState>(new ComponentEventRefHandler<LatheComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<LatheComponent, ComponentHandleState>(new ComponentEventRefHandler<LatheComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, LatheComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new LatheComponent.LatheComponent_AutoState()
      {
        DefaultProductionAmount = component.DefaultProductionAmount,
        MaterialUseMultiplier = component.MaterialUseMultiplier,
        MaxQueue = component.MaxQueue
      };
    }

    private void OnHandleState(
      EntityUid uid,
      LatheComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is LatheComponent.LatheComponent_AutoState current))
        return;
      component.DefaultProductionAmount = current.DefaultProductionAmount;
      component.MaterialUseMultiplier = current.MaterialUseMultiplier;
      component.MaxQueue = current.MaxQueue;
    }
  }
}
