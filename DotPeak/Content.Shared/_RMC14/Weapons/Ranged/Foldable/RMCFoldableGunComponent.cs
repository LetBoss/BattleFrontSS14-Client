// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.Foldable.RMCFoldableGunComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
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
namespace Content.Shared._RMC14.Weapons.Ranged.Foldable;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class RMCFoldableGunComponent : 
  Component,
  ISerializationGenerated<RMCFoldableGunComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Fired;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool OnActivate;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan FoldDelay = TimeSpan.FromSeconds(2L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId FoldText;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId FoldTextOthers;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId FinishText;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId FinishTextOthers;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId ExamineText = (LocId) "rmc-gun-foldable-launcher-examine";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? ToggleFoldSound;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId FoldedEntity;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCFoldableGunComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCFoldableGunComponent) target1;
    if (serialization.TryCustomCopy<RMCFoldableGunComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Fired, ref target2, hookCtx, false, context))
      target2 = this.Fired;
    target.Fired = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.OnActivate, ref target3, hookCtx, false, context))
      target3 = this.OnActivate;
    target.OnActivate = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.FoldDelay, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.FoldDelay, hookCtx, context);
    target.FoldDelay = target4;
    LocId target5 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.FoldText, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<LocId>(this.FoldText, hookCtx, context);
    target.FoldText = target5;
    LocId target6 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.FoldTextOthers, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<LocId>(this.FoldTextOthers, hookCtx, context);
    target.FoldTextOthers = target6;
    LocId target7 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.FinishText, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<LocId>(this.FinishText, hookCtx, context);
    target.FinishText = target7;
    LocId target8 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.FinishTextOthers, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<LocId>(this.FinishTextOthers, hookCtx, context);
    target.FinishTextOthers = target8;
    LocId target9 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.ExamineText, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<LocId>(this.ExamineText, hookCtx, context);
    target.ExamineText = target9;
    SoundSpecifier target10 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ToggleFoldSound, ref target10, hookCtx, true, context))
      target10 = serialization.CreateCopy<SoundSpecifier>(this.ToggleFoldSound, hookCtx, context);
    target.ToggleFoldSound = target10;
    EntProtoId target11 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.FoldedEntity, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<EntProtoId>(this.FoldedEntity, hookCtx, context);
    target.FoldedEntity = target11;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCFoldableGunComponent target,
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
    RMCFoldableGunComponent target1 = (RMCFoldableGunComponent) target;
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
    RMCFoldableGunComponent target1 = (RMCFoldableGunComponent) target;
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
    RMCFoldableGunComponent target1 = (RMCFoldableGunComponent) target;
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
  virtual RMCFoldableGunComponent Component.Instantiate() => new RMCFoldableGunComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCFoldableGunComponent_AutoState : IComponentState
  {
    public bool Fired;
    public bool OnActivate;
    public TimeSpan FoldDelay;
    public LocId FoldText;
    public LocId FoldTextOthers;
    public LocId FinishText;
    public LocId FinishTextOthers;
    public LocId ExamineText;
    public SoundSpecifier? ToggleFoldSound;
    public EntProtoId FoldedEntity;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCFoldableGunComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCFoldableGunComponent, ComponentGetState>(new ComponentEventRefHandler<RMCFoldableGunComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCFoldableGunComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCFoldableGunComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCFoldableGunComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCFoldableGunComponent.RMCFoldableGunComponent_AutoState()
      {
        Fired = component.Fired,
        OnActivate = component.OnActivate,
        FoldDelay = component.FoldDelay,
        FoldText = component.FoldText,
        FoldTextOthers = component.FoldTextOthers,
        FinishText = component.FinishText,
        FinishTextOthers = component.FinishTextOthers,
        ExamineText = component.ExamineText,
        ToggleFoldSound = component.ToggleFoldSound,
        FoldedEntity = component.FoldedEntity
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCFoldableGunComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCFoldableGunComponent.RMCFoldableGunComponent_AutoState current))
        return;
      component.Fired = current.Fired;
      component.OnActivate = current.OnActivate;
      component.FoldDelay = current.FoldDelay;
      component.FoldText = current.FoldText;
      component.FoldTextOthers = current.FoldTextOthers;
      component.FinishText = current.FinishText;
      component.FinishTextOthers = current.FinishTextOthers;
      component.ExamineText = current.ExamineText;
      component.ToggleFoldSound = current.ToggleFoldSound;
      component.FoldedEntity = current.FoldedEntity;
    }
  }
}
