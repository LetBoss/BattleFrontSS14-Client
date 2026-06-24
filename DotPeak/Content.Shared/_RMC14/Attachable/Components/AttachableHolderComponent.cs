// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Attachable.Components.AttachableHolderComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Attachable.Systems;
using Robust.Shared.Analyzers;
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
namespace Content.Shared._RMC14.Attachable.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (AttachableHolderSystem)})]
public sealed class AttachableHolderComponent : 
  Component,
  ISerializationGenerated<AttachableHolderComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? SupercedingAttachable;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<string, AttachableSlot> Slots = new Dictionary<string, AttachableSlot>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float RandomAttachmentChance = 0.5f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AttachableHolderComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (AttachableHolderComponent) target1;
    if (serialization.TryCustomCopy<AttachableHolderComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityUid? target2 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.SupercedingAttachable, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityUid?>(this.SupercedingAttachable, hookCtx, context);
    target.SupercedingAttachable = target2;
    Dictionary<string, AttachableSlot> target3 = (Dictionary<string, AttachableSlot>) null;
    if (this.Slots == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, AttachableSlot>>(this.Slots, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<Dictionary<string, AttachableSlot>>(this.Slots, hookCtx, context);
    target.Slots = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RandomAttachmentChance, ref target4, hookCtx, false, context))
      target4 = this.RandomAttachmentChance;
    target.RandomAttachmentChance = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AttachableHolderComponent target,
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
    AttachableHolderComponent target1 = (AttachableHolderComponent) target;
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
    AttachableHolderComponent target1 = (AttachableHolderComponent) target;
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
    AttachableHolderComponent target1 = (AttachableHolderComponent) target;
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
  virtual AttachableHolderComponent Component.Instantiate() => new AttachableHolderComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class AttachableHolderComponent_AutoState : IComponentState
  {
    public NetEntity? SupercedingAttachable;
    public Dictionary<string, AttachableSlot> Slots;
    public float RandomAttachmentChance;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class AttachableHolderComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<AttachableHolderComponent, ComponentGetState>(new ComponentEventRefHandler<AttachableHolderComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<AttachableHolderComponent, ComponentHandleState>(new ComponentEventRefHandler<AttachableHolderComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      AttachableHolderComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new AttachableHolderComponent.AttachableHolderComponent_AutoState()
      {
        SupercedingAttachable = this.GetNetEntity(component.SupercedingAttachable),
        Slots = component.Slots,
        RandomAttachmentChance = component.RandomAttachmentChance
      };
    }

    private void OnHandleState(
      EntityUid uid,
      AttachableHolderComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is AttachableHolderComponent.AttachableHolderComponent_AutoState current))
        return;
      component.SupercedingAttachable = this.EnsureEntity<AttachableHolderComponent>(current.SupercedingAttachable, uid);
      component.Slots = current.Slots == null ? (Dictionary<string, AttachableSlot>) null : new Dictionary<string, AttachableSlot>((IDictionary<string, AttachableSlot>) current.Slots);
      component.RandomAttachmentChance = current.RandomAttachmentChance;
    }
  }
}
