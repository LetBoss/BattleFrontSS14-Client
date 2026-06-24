// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Requisitions.Components.RequisitionsComputerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Requisitions.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedRequisitionsSystem)})]
public sealed class RequisitionsComputerComponent : 
  Component,
  ISerializationGenerated<RequisitionsComputerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? Account;
  [DataField("soundIncomingSurplus", false, 1, false, false, null)]
  public SoundSpecifier IncomingSurplus = (SoundSpecifier) new SoundPathSpecifier("/Audio/Effects/Cargo/ping.ogg");
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? Platform;
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  [AlwaysPushInheritance]
  public List<RequisitionsCategory> Categories = new List<RequisitionsCategory>();
  [DataField(null, false, 1, false, false, null)]
  public bool IsLastInteracted;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RequisitionsComputerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RequisitionsComputerComponent) target1;
    if (serialization.TryCustomCopy<RequisitionsComputerComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityUid? target2 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Account, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityUid?>(this.Account, hookCtx, context);
    target.Account = target2;
    SoundSpecifier target3 = (SoundSpecifier) null;
    if (this.IncomingSurplus == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.IncomingSurplus, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<SoundSpecifier>(this.IncomingSurplus, hookCtx, context);
    target.IncomingSurplus = target3;
    EntityUid? target4 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Platform, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntityUid?>(this.Platform, hookCtx, context);
    target.Platform = target4;
    List<RequisitionsCategory> target5 = (List<RequisitionsCategory>) null;
    if (this.Categories == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<RequisitionsCategory>>(this.Categories, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<List<RequisitionsCategory>>(this.Categories, hookCtx, context);
    target.Categories = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsLastInteracted, ref target6, hookCtx, false, context))
      target6 = this.IsLastInteracted;
    target.IsLastInteracted = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RequisitionsComputerComponent target,
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
    RequisitionsComputerComponent target1 = (RequisitionsComputerComponent) target;
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
    RequisitionsComputerComponent target1 = (RequisitionsComputerComponent) target;
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
    RequisitionsComputerComponent target1 = (RequisitionsComputerComponent) target;
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
  virtual RequisitionsComputerComponent Component.Instantiate()
  {
    return new RequisitionsComputerComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RequisitionsComputerComponent_AutoState : IComponentState
  {
    public List<RequisitionsCategory> Categories;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RequisitionsComputerComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RequisitionsComputerComponent, ComponentGetState>(new ComponentEventRefHandler<RequisitionsComputerComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RequisitionsComputerComponent, ComponentHandleState>(new ComponentEventRefHandler<RequisitionsComputerComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RequisitionsComputerComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RequisitionsComputerComponent.RequisitionsComputerComponent_AutoState()
      {
        Categories = component.Categories
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RequisitionsComputerComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RequisitionsComputerComponent.RequisitionsComputerComponent_AutoState current))
        return;
      component.Categories = current.Categories == null ? (List<RequisitionsCategory>) null : new List<RequisitionsCategory>((IEnumerable<RequisitionsCategory>) current.Categories);
    }
  }
}
