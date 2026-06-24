// Decompiled with JetBrains decompiler
// Type: Content.Shared.Foldable.FoldableComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Folded;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Foldable;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[Access(new Type[] {typeof (FoldableSystem), typeof (RMCFoldableSystem)})]
public sealed class FoldableComponent : 
  Component,
  ISerializationGenerated<FoldableComponent>,
  ISerializationGenerated
{
  [DataField("folded", false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool IsFolded;
  [DataField(null, false, 1, false, false, null)]
  public bool CanFoldInsideContainer;
  [DataField(null, false, 1, false, false, null)]
  public LocId UnfoldVerbText = (LocId) "unfold-verb";
  [DataField(null, false, 1, false, false, null)]
  public LocId FoldVerbText = (LocId) "fold-verb";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool FitIntoEntityStorage;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool AnchorOnUnfold;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool EnableStrapOnUnfold = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool IsLocked;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref FoldableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (FoldableComponent) target1;
    if (serialization.TryCustomCopy<FoldableComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsFolded, ref target2, hookCtx, false, context))
      target2 = this.IsFolded;
    target.IsFolded = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanFoldInsideContainer, ref target3, hookCtx, false, context))
      target3 = this.CanFoldInsideContainer;
    target.CanFoldInsideContainer = target3;
    LocId target4 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.UnfoldVerbText, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<LocId>(this.UnfoldVerbText, hookCtx, context);
    target.UnfoldVerbText = target4;
    LocId target5 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.FoldVerbText, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<LocId>(this.FoldVerbText, hookCtx, context);
    target.FoldVerbText = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.FitIntoEntityStorage, ref target6, hookCtx, false, context))
      target6 = this.FitIntoEntityStorage;
    target.FitIntoEntityStorage = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.AnchorOnUnfold, ref target7, hookCtx, false, context))
      target7 = this.AnchorOnUnfold;
    target.AnchorOnUnfold = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.EnableStrapOnUnfold, ref target8, hookCtx, false, context))
      target8 = this.EnableStrapOnUnfold;
    target.EnableStrapOnUnfold = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsLocked, ref target9, hookCtx, false, context))
      target9 = this.IsLocked;
    target.IsLocked = target9;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref FoldableComponent target,
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
    FoldableComponent target1 = (FoldableComponent) target;
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
    FoldableComponent target1 = (FoldableComponent) target;
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
    FoldableComponent target1 = (FoldableComponent) target;
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
  virtual FoldableComponent Component.Instantiate() => new FoldableComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class FoldableComponent_AutoState : IComponentState
  {
    public bool IsFolded;
    public bool FitIntoEntityStorage;
    public bool AnchorOnUnfold;
    public bool EnableStrapOnUnfold;
    public bool IsLocked;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class FoldableComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<FoldableComponent, ComponentGetState>(new ComponentEventRefHandler<FoldableComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<FoldableComponent, ComponentHandleState>(new ComponentEventRefHandler<FoldableComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, FoldableComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new FoldableComponent.FoldableComponent_AutoState()
      {
        IsFolded = component.IsFolded,
        FitIntoEntityStorage = component.FitIntoEntityStorage,
        AnchorOnUnfold = component.AnchorOnUnfold,
        EnableStrapOnUnfold = component.EnableStrapOnUnfold,
        IsLocked = component.IsLocked
      };
    }

    private void OnHandleState(
      EntityUid uid,
      FoldableComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is FoldableComponent.FoldableComponent_AutoState current))
        return;
      component.IsFolded = current.IsFolded;
      component.FitIntoEntityStorage = current.FitIntoEntityStorage;
      component.AnchorOnUnfold = current.AnchorOnUnfold;
      component.EnableStrapOnUnfold = current.EnableStrapOnUnfold;
      component.IsLocked = current.IsLocked;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, FoldableComponent>(uid, component, ref args1);
    }
  }
}
