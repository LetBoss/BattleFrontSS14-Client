// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Intel.ViewIntelObjectivesComponent
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
namespace Content.Shared._RMC14.Intel;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[Access(new Type[] {typeof (IntelSystem)})]
public sealed class ViewIntelObjectivesComponent : 
  Component,
  ISerializationGenerated<ViewIntelObjectivesComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string ActionId = "RMCActionViewIntelObjectives";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Action;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public IntelTechTree Tree = new IntelTechTree();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ViewIntelObjectivesComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ViewIntelObjectivesComponent) target1;
    if (serialization.TryCustomCopy<ViewIntelObjectivesComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.ActionId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ActionId, ref target2, hookCtx, false, context))
      target2 = this.ActionId;
    target.ActionId = target2;
    EntityUid? target3 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Action, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntityUid?>(this.Action, hookCtx, context);
    target.Action = target3;
    IntelTechTree target4 = (IntelTechTree) null;
    if (this.Tree == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<IntelTechTree>(this.Tree, ref target4, hookCtx, false, context))
    {
      if (this.Tree == null)
        target4 = (IntelTechTree) null;
      else
        serialization.CopyTo<IntelTechTree>(this.Tree, ref target4, hookCtx, context, true);
    }
    target.Tree = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ViewIntelObjectivesComponent target,
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
    ViewIntelObjectivesComponent target1 = (ViewIntelObjectivesComponent) target;
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
    ViewIntelObjectivesComponent target1 = (ViewIntelObjectivesComponent) target;
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
    ViewIntelObjectivesComponent target1 = (ViewIntelObjectivesComponent) target;
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
  virtual ViewIntelObjectivesComponent Component.Instantiate()
  {
    return new ViewIntelObjectivesComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ViewIntelObjectivesComponent_AutoState : IComponentState
  {
    public string ActionId;
    public NetEntity? Action;
    public IntelTechTree Tree;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ViewIntelObjectivesComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ViewIntelObjectivesComponent, ComponentGetState>(new ComponentEventRefHandler<ViewIntelObjectivesComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ViewIntelObjectivesComponent, ComponentHandleState>(new ComponentEventRefHandler<ViewIntelObjectivesComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      ViewIntelObjectivesComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new ViewIntelObjectivesComponent.ViewIntelObjectivesComponent_AutoState()
      {
        ActionId = component.ActionId,
        Action = this.GetNetEntity(component.Action),
        Tree = component.Tree
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ViewIntelObjectivesComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is ViewIntelObjectivesComponent.ViewIntelObjectivesComponent_AutoState current))
        return;
      component.ActionId = current.ActionId;
      component.Action = this.EnsureEntity<ViewIntelObjectivesComponent>(current.Action, uid);
      component.Tree = current.Tree;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, ViewIntelObjectivesComponent>(uid, component, ref args1);
    }
  }
}
