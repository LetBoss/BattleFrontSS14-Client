// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Armor.Magnetic.RMCReturnToInventoryComponent
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
namespace Content.Shared._RMC14.Armor.Magnetic;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (RMCMagneticSystem)})]
public sealed class RMCReturnToInventoryComponent : 
  Component,
  ISerializationGenerated<RMCReturnToInventoryComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid User;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid Magnetizer;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Returned;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? ReceivingItem;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string ReceivingContainer;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCReturnToInventoryComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCReturnToInventoryComponent) target1;
    if (serialization.TryCustomCopy<RMCReturnToInventoryComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityUid target2 = new EntityUid();
    if (!serialization.TryCustomCopy<EntityUid>(this.User, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityUid>(this.User, hookCtx, context);
    target.User = target2;
    EntityUid target3 = new EntityUid();
    if (!serialization.TryCustomCopy<EntityUid>(this.Magnetizer, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntityUid>(this.Magnetizer, hookCtx, context);
    target.Magnetizer = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.Returned, ref target4, hookCtx, false, context))
      target4 = this.Returned;
    target.Returned = target4;
    EntityUid? target5 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.ReceivingItem, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntityUid?>(this.ReceivingItem, hookCtx, context);
    target.ReceivingItem = target5;
    string target6 = (string) null;
    if (this.ReceivingContainer == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ReceivingContainer, ref target6, hookCtx, false, context))
      target6 = this.ReceivingContainer;
    target.ReceivingContainer = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCReturnToInventoryComponent target,
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
    RMCReturnToInventoryComponent target1 = (RMCReturnToInventoryComponent) target;
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
    RMCReturnToInventoryComponent target1 = (RMCReturnToInventoryComponent) target;
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
    RMCReturnToInventoryComponent target1 = (RMCReturnToInventoryComponent) target;
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
  virtual RMCReturnToInventoryComponent Component.Instantiate()
  {
    return new RMCReturnToInventoryComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCReturnToInventoryComponent_AutoState : IComponentState
  {
    public NetEntity User;
    public NetEntity Magnetizer;
    public bool Returned;
    public NetEntity? ReceivingItem;
    public string ReceivingContainer;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCReturnToInventoryComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCReturnToInventoryComponent, ComponentGetState>(new ComponentEventRefHandler<RMCReturnToInventoryComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCReturnToInventoryComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCReturnToInventoryComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCReturnToInventoryComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCReturnToInventoryComponent.RMCReturnToInventoryComponent_AutoState()
      {
        User = this.GetNetEntity(component.User),
        Magnetizer = this.GetNetEntity(component.Magnetizer),
        Returned = component.Returned,
        ReceivingItem = this.GetNetEntity(component.ReceivingItem),
        ReceivingContainer = component.ReceivingContainer
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCReturnToInventoryComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCReturnToInventoryComponent.RMCReturnToInventoryComponent_AutoState current))
        return;
      component.User = this.EnsureEntity<RMCReturnToInventoryComponent>(current.User, uid);
      component.Magnetizer = this.EnsureEntity<RMCReturnToInventoryComponent>(current.Magnetizer, uid);
      component.Returned = current.Returned;
      component.ReceivingItem = this.EnsureEntity<RMCReturnToInventoryComponent>(current.ReceivingItem, uid);
      component.ReceivingContainer = current.ReceivingContainer;
    }
  }
}
