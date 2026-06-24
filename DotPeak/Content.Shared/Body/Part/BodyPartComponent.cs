// Decompiled with JetBrains decompiler
// Type: Content.Shared.Body.Part.BodyPartComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Body.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Body.Part;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedBodySystem)})]
public sealed class BodyPartComponent : 
  Component,
  ISerializationGenerated<BodyPartComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Body;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public BodyPartType PartType;
  [DataField("vital", false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool IsVital;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public BodyPartSymmetry Symmetry;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<string, BodyPartSlot> Children = new Dictionary<string, BodyPartSlot>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<string, OrganSlot> Organs = new Dictionary<string, OrganSlot>();

  [Robust.Shared.ViewVariables.ViewVariables]
  private List<ContainerSlot> BodyPartSlotsVV
  {
    get
    {
      List<ContainerSlot> bodyPartSlotsVv = new List<ContainerSlot>();
      SharedContainerSystem sharedContainerSystem = IoCManager.Resolve<IEntityManager>().System<SharedContainerSystem>();
      foreach (string key in this.Children.Keys)
        bodyPartSlotsVv.Add((ContainerSlot) sharedContainerSystem.GetContainer(this.Owner, "body_part_slot_" + key, (ContainerManagerComponent) null));
      return bodyPartSlotsVv;
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  private List<ContainerSlot> OrganSlotsVV
  {
    get
    {
      List<ContainerSlot> organSlotsVv = new List<ContainerSlot>();
      SharedContainerSystem sharedContainerSystem = IoCManager.Resolve<IEntityManager>().System<SharedContainerSystem>();
      foreach (string key in this.Organs.Keys)
        organSlotsVv.Add((ContainerSlot) sharedContainerSystem.GetContainer(this.Owner, "body_organ_slot_" + key, (ContainerManagerComponent) null));
      return organSlotsVv;
    }
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref BodyPartComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (BodyPartComponent) component;
    if (serialization.TryCustomCopy<BodyPartComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityUid? nullable = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Body, ref nullable, hookCtx, false, context))
      nullable = serialization.CreateCopy<EntityUid?>(this.Body, hookCtx, context, false);
    target.Body = nullable;
    BodyPartType bodyPartType = BodyPartType.Other;
    if (!serialization.TryCustomCopy<BodyPartType>(this.PartType, ref bodyPartType, hookCtx, false, context))
      bodyPartType = this.PartType;
    target.PartType = bodyPartType;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.IsVital, ref flag, hookCtx, false, context))
      flag = this.IsVital;
    target.IsVital = flag;
    BodyPartSymmetry bodyPartSymmetry = BodyPartSymmetry.None;
    if (!serialization.TryCustomCopy<BodyPartSymmetry>(this.Symmetry, ref bodyPartSymmetry, hookCtx, false, context))
      bodyPartSymmetry = this.Symmetry;
    target.Symmetry = bodyPartSymmetry;
    Dictionary<string, BodyPartSlot> dictionary1 = (Dictionary<string, BodyPartSlot>) null;
    if (this.Children == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, BodyPartSlot>>(this.Children, ref dictionary1, hookCtx, true, context))
      dictionary1 = serialization.CreateCopy<Dictionary<string, BodyPartSlot>>(this.Children, hookCtx, context, false);
    target.Children = dictionary1;
    Dictionary<string, OrganSlot> dictionary2 = (Dictionary<string, OrganSlot>) null;
    if (this.Organs == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, OrganSlot>>(this.Organs, ref dictionary2, hookCtx, true, context))
      dictionary2 = serialization.CreateCopy<Dictionary<string, OrganSlot>>(this.Organs, hookCtx, context, false);
    target.Organs = dictionary2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref BodyPartComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    BodyPartComponent target1 = (BodyPartComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    BodyPartComponent target1 = (BodyPartComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    BodyPartComponent target1 = (BodyPartComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    base.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual BodyPartComponent Component.Instantiate() => new BodyPartComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class BodyPartComponent_AutoState : IComponentState
  {
    public NetEntity? Body;
    public BodyPartType PartType;
    public bool IsVital;
    public BodyPartSymmetry Symmetry;
    public Dictionary<string, BodyPartSlot> Children;
    public Dictionary<string, OrganSlot> Organs;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class BodyPartComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<BodyPartComponent, ComponentGetState>(new ComponentEventRefHandler<BodyPartComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<BodyPartComponent, ComponentHandleState>(new ComponentEventRefHandler<BodyPartComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(EntityUid uid, BodyPartComponent component, ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new BodyPartComponent.BodyPartComponent_AutoState()
      {
        Body = this.GetNetEntity(component.Body, (MetaDataComponent) null),
        PartType = component.PartType,
        IsVital = component.IsVital,
        Symmetry = component.Symmetry,
        Children = component.Children,
        Organs = component.Organs
      };
    }

    private void OnHandleState(
      EntityUid uid,
      BodyPartComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is BodyPartComponent.BodyPartComponent_AutoState current))
        return;
      component.Body = this.EnsureEntity<BodyPartComponent>(current.Body, uid);
      component.PartType = current.PartType;
      component.IsVital = current.IsVital;
      component.Symmetry = current.Symmetry;
      component.Children = current.Children == null ? (Dictionary<string, BodyPartSlot>) null : new Dictionary<string, BodyPartSlot>((IDictionary<string, BodyPartSlot>) current.Children);
      component.Organs = current.Organs == null ? (Dictionary<string, OrganSlot>) null : new Dictionary<string, OrganSlot>((IDictionary<string, OrganSlot>) current.Organs);
    }
  }
}
