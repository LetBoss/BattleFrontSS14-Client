// Decompiled with JetBrains decompiler
// Type: Content.Shared.RetractableItemAction.RetractableItemActionComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.RetractableItemAction;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (RetractableItemActionSystem)})]
public sealed class RetractableItemActionComponent : 
  Component,
  ISerializationGenerated<RetractableItemActionComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public EntProtoId SpawnedPrototype;
  [DataField(null, false, 1, false, false, null)]
  public SoundCollectionSpecifier? SummonSounds;
  [DataField(null, false, 1, false, false, null)]
  public SoundCollectionSpecifier? RetractSounds;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? ActionItemUid;
  public const string ContainerId = "item-action-item-container";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RetractableItemActionComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RetractableItemActionComponent) target1;
    if (serialization.TryCustomCopy<RetractableItemActionComponent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId target2 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.SpawnedPrototype, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId>(this.SpawnedPrototype, hookCtx, context);
    target.SpawnedPrototype = target2;
    SoundCollectionSpecifier target3 = (SoundCollectionSpecifier) null;
    if (!serialization.TryCustomCopy<SoundCollectionSpecifier>(this.SummonSounds, ref target3, hookCtx, false, context))
    {
      if (this.SummonSounds == null)
        target3 = (SoundCollectionSpecifier) null;
      else
        serialization.CopyTo<SoundCollectionSpecifier>(this.SummonSounds, ref target3, hookCtx, context);
    }
    target.SummonSounds = target3;
    SoundCollectionSpecifier target4 = (SoundCollectionSpecifier) null;
    if (!serialization.TryCustomCopy<SoundCollectionSpecifier>(this.RetractSounds, ref target4, hookCtx, false, context))
    {
      if (this.RetractSounds == null)
        target4 = (SoundCollectionSpecifier) null;
      else
        serialization.CopyTo<SoundCollectionSpecifier>(this.RetractSounds, ref target4, hookCtx, context);
    }
    target.RetractSounds = target4;
    EntityUid? target5 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.ActionItemUid, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntityUid?>(this.ActionItemUid, hookCtx, context);
    target.ActionItemUid = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RetractableItemActionComponent target,
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
    RetractableItemActionComponent target1 = (RetractableItemActionComponent) target;
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
    RetractableItemActionComponent target1 = (RetractableItemActionComponent) target;
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
    RetractableItemActionComponent target1 = (RetractableItemActionComponent) target;
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
  virtual RetractableItemActionComponent Component.Instantiate()
  {
    return new RetractableItemActionComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RetractableItemActionComponent_AutoState : IComponentState
  {
    public NetEntity? ActionItemUid;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RetractableItemActionComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RetractableItemActionComponent, ComponentGetState>(new ComponentEventRefHandler<RetractableItemActionComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RetractableItemActionComponent, ComponentHandleState>(new ComponentEventRefHandler<RetractableItemActionComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RetractableItemActionComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RetractableItemActionComponent.RetractableItemActionComponent_AutoState()
      {
        ActionItemUid = this.GetNetEntity(component.ActionItemUid)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RetractableItemActionComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RetractableItemActionComponent.RetractableItemActionComponent_AutoState current))
        return;
      component.ActionItemUid = this.EnsureEntity<RetractableItemActionComponent>(current.ActionItemUid, uid);
    }
  }
}
