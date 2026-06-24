// Decompiled with JetBrains decompiler
// Type: Content.Shared.Species.Components.GibActionComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Mobs;
using Robust.Shared.Analyzers;
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
namespace Content.Shared.Species.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class GibActionComponent : 
  Component,
  ISerializationGenerated<GibActionComponent>,
  ISerializationGenerated
{
  [DataField("actionPrototype", false, 1, true, false, null)]
  public EntProtoId ActionPrototype;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? ActionEntity;
  [DataField("allowedStates", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public List<MobState> AllowedStates = new List<MobState>();
  [DataField("popupText", false, 1, false, false, null)]
  public string PopupText = "diona-gib-action-use";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GibActionComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (GibActionComponent) target1;
    if (serialization.TryCustomCopy<GibActionComponent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId target2 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.ActionPrototype, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId>(this.ActionPrototype, hookCtx, context);
    target.ActionPrototype = target2;
    EntityUid? target3 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.ActionEntity, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntityUid?>(this.ActionEntity, hookCtx, context);
    target.ActionEntity = target3;
    List<MobState> target4 = (List<MobState>) null;
    if (this.AllowedStates == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<MobState>>(this.AllowedStates, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<List<MobState>>(this.AllowedStates, hookCtx, context);
    target.AllowedStates = target4;
    string target5 = (string) null;
    if (this.PopupText == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.PopupText, ref target5, hookCtx, false, context))
      target5 = this.PopupText;
    target.PopupText = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GibActionComponent target,
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
    GibActionComponent target1 = (GibActionComponent) target;
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
    GibActionComponent target1 = (GibActionComponent) target;
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
    GibActionComponent target1 = (GibActionComponent) target;
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
  virtual GibActionComponent Component.Instantiate() => new GibActionComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class GibActionComponent_AutoState : IComponentState
  {
    public NetEntity? ActionEntity;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class GibActionComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<GibActionComponent, ComponentGetState>(new ComponentEventRefHandler<GibActionComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<GibActionComponent, ComponentHandleState>(new ComponentEventRefHandler<GibActionComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      GibActionComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new GibActionComponent.GibActionComponent_AutoState()
      {
        ActionEntity = this.GetNetEntity(component.ActionEntity)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      GibActionComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is GibActionComponent.GibActionComponent_AutoState current))
        return;
      component.ActionEntity = this.EnsureEntity<GibActionComponent>(current.ActionEntity, uid);
    }
  }
}
