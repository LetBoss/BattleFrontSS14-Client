// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Visor.CycleableVisorComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Tools;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Visor;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (VisorSystem)})]
public sealed class CycleableVisorComponent : 
  Component,
  ISerializationGenerated<CycleableVisorComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId ActionId = (EntProtoId) "RMCActionCycleVisor";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Action;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<string> Containers = new List<string>()
  {
    "rmc_visor_one"
  };
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public HashSet<ProtoId<ToolQualityPrototype>> RemoveQuality = new HashSet<ProtoId<ToolQualityPrototype>>()
  {
    (ProtoId<ToolQualityPrototype>) "Screwing"
  };
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int? CurrentVisor;
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public SpriteSpecifier.Rsi OffIcon;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CycleableVisorComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (CycleableVisorComponent) target1;
    if (serialization.TryCustomCopy<CycleableVisorComponent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId target2 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.ActionId, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId>(this.ActionId, hookCtx, context);
    target.ActionId = target2;
    EntityUid? target3 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Action, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntityUid?>(this.Action, hookCtx, context);
    target.Action = target3;
    List<string> target4 = (List<string>) null;
    if (this.Containers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<string>>(this.Containers, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<List<string>>(this.Containers, hookCtx, context);
    target.Containers = target4;
    HashSet<ProtoId<ToolQualityPrototype>> target5 = (HashSet<ProtoId<ToolQualityPrototype>>) null;
    if (this.RemoveQuality == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<ProtoId<ToolQualityPrototype>>>(this.RemoveQuality, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<HashSet<ProtoId<ToolQualityPrototype>>>(this.RemoveQuality, hookCtx, context);
    target.RemoveQuality = target5;
    int? target6 = new int?();
    if (!serialization.TryCustomCopy<int?>(this.CurrentVisor, ref target6, hookCtx, false, context))
      target6 = this.CurrentVisor;
    target.CurrentVisor = target6;
    SpriteSpecifier.Rsi target7 = (SpriteSpecifier.Rsi) null;
    if (this.OffIcon == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SpriteSpecifier.Rsi>(this.OffIcon, ref target7, hookCtx, false, context))
    {
      if (this.OffIcon == null)
        target7 = (SpriteSpecifier.Rsi) null;
      else
        serialization.CopyTo<SpriteSpecifier.Rsi>(this.OffIcon, ref target7, hookCtx, context, true);
    }
    target.OffIcon = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CycleableVisorComponent target,
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
    CycleableVisorComponent target1 = (CycleableVisorComponent) target;
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
    CycleableVisorComponent target1 = (CycleableVisorComponent) target;
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
    CycleableVisorComponent target1 = (CycleableVisorComponent) target;
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
  virtual CycleableVisorComponent Component.Instantiate() => new CycleableVisorComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class CycleableVisorComponent_AutoState : IComponentState
  {
    public EntProtoId ActionId;
    public NetEntity? Action;
    public List<string> Containers;
    public HashSet<ProtoId<ToolQualityPrototype>> RemoveQuality;
    public int? CurrentVisor;
    public SpriteSpecifier.Rsi OffIcon;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class CycleableVisorComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<CycleableVisorComponent, ComponentGetState>(new ComponentEventRefHandler<CycleableVisorComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<CycleableVisorComponent, ComponentHandleState>(new ComponentEventRefHandler<CycleableVisorComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      CycleableVisorComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new CycleableVisorComponent.CycleableVisorComponent_AutoState()
      {
        ActionId = component.ActionId,
        Action = this.GetNetEntity(component.Action),
        Containers = component.Containers,
        RemoveQuality = component.RemoveQuality,
        CurrentVisor = component.CurrentVisor,
        OffIcon = component.OffIcon
      };
    }

    private void OnHandleState(
      EntityUid uid,
      CycleableVisorComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is CycleableVisorComponent.CycleableVisorComponent_AutoState current))
        return;
      component.ActionId = current.ActionId;
      component.Action = this.EnsureEntity<CycleableVisorComponent>(current.Action, uid);
      component.Containers = current.Containers == null ? (List<string>) null : new List<string>((IEnumerable<string>) current.Containers);
      component.RemoveQuality = current.RemoveQuality == null ? (HashSet<ProtoId<ToolQualityPrototype>>) null : new HashSet<ProtoId<ToolQualityPrototype>>((IEnumerable<ProtoId<ToolQualityPrototype>>) current.RemoveQuality);
      component.CurrentVisor = current.CurrentVisor;
      component.OffIcon = current.OffIcon;
    }
  }
}
