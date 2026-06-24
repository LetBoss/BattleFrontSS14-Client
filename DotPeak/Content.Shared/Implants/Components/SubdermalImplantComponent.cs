// Decompiled with JetBrains decompiler
// Type: Content.Shared.Implants.Components.SubdermalImplantComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Implants.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class SubdermalImplantComponent : 
  Component,
  ISerializationGenerated<SubdermalImplantComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("implantAction", false, 1, false, false, null)]
  public EntProtoId? ImplantAction;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Action;
  [Robust.Shared.ViewVariables.ViewVariables]
  [AutoNetworkedField]
  public EntityUid? ImplantedEntity;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("permanent", false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Permanent;
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? Whitelist;
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? Blacklist;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId? DrawableProtoIdOverride;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SubdermalImplantComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SubdermalImplantComponent) target1;
    if (serialization.TryCustomCopy<SubdermalImplantComponent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId? target2 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.ImplantAction, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId?>(this.ImplantAction, hookCtx, context);
    target.ImplantAction = target2;
    EntityUid? target3 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Action, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntityUid?>(this.Action, hookCtx, context);
    target.Action = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.Permanent, ref target4, hookCtx, false, context))
      target4 = this.Permanent;
    target.Permanent = target4;
    EntityWhitelist target5 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref target5, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        target5 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref target5, hookCtx, context);
    }
    target.Whitelist = target5;
    EntityWhitelist target6 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Blacklist, ref target6, hookCtx, false, context))
    {
      if (this.Blacklist == null)
        target6 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Blacklist, ref target6, hookCtx, context);
    }
    target.Blacklist = target6;
    EntProtoId? target7 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.DrawableProtoIdOverride, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<EntProtoId?>(this.DrawableProtoIdOverride, hookCtx, context);
    target.DrawableProtoIdOverride = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SubdermalImplantComponent target,
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
    SubdermalImplantComponent target1 = (SubdermalImplantComponent) target;
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
    SubdermalImplantComponent target1 = (SubdermalImplantComponent) target;
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
    SubdermalImplantComponent target1 = (SubdermalImplantComponent) target;
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
  virtual SubdermalImplantComponent Component.Instantiate() => new SubdermalImplantComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class SubdermalImplantComponent_AutoState : IComponentState
  {
    public NetEntity? Action;
    public NetEntity? ImplantedEntity;
    public bool Permanent;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class SubdermalImplantComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<SubdermalImplantComponent, ComponentGetState>(new ComponentEventRefHandler<SubdermalImplantComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<SubdermalImplantComponent, ComponentHandleState>(new ComponentEventRefHandler<SubdermalImplantComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      SubdermalImplantComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new SubdermalImplantComponent.SubdermalImplantComponent_AutoState()
      {
        Action = this.GetNetEntity(component.Action),
        ImplantedEntity = this.GetNetEntity(component.ImplantedEntity),
        Permanent = component.Permanent
      };
    }

    private void OnHandleState(
      EntityUid uid,
      SubdermalImplantComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is SubdermalImplantComponent.SubdermalImplantComponent_AutoState current))
        return;
      component.Action = this.EnsureEntity<SubdermalImplantComponent>(current.Action, uid);
      component.ImplantedEntity = this.EnsureEntity<SubdermalImplantComponent>(current.ImplantedEntity, uid);
      component.Permanent = current.Permanent;
    }
  }
}
