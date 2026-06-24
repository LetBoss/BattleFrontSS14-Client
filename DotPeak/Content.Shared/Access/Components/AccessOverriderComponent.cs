// Decompiled with JetBrains decompiler
// Type: Content.Shared.Access.Components.AccessOverriderComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Access.Systems;
using Content.Shared.Containers.ItemSlots;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Access.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Robust.Shared.Analyzers.Access(new Type[] {typeof (SharedAccessOverriderSystem)})]
public sealed class AccessOverriderComponent : 
  Component,
  ISerializationGenerated<AccessOverriderComponent>,
  ISerializationGenerated
{
  public static string PrivilegedIdCardSlotId = "AccessOverrider-privilegedId";
  [DataField(null, false, 1, false, false, null)]
  public ItemSlot PrivilegedIdSlot = new ItemSlot();
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? DenialSound;
  public EntityUid TargetAccessReaderId;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<ProtoId<AccessLevelPrototype>> AccessLevels = new List<ProtoId<AccessLevelPrototype>>();
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField(null, false, 1, false, false, null)]
  public float DoAfter;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AccessOverriderComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (AccessOverriderComponent) component;
    if (serialization.TryCustomCopy<AccessOverriderComponent>(this, ref target, hookCtx, false, context))
      return;
    ItemSlot itemSlot = (ItemSlot) null;
    if (this.PrivilegedIdSlot == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<ItemSlot>(this.PrivilegedIdSlot, ref itemSlot, hookCtx, false, context))
    {
      if (this.PrivilegedIdSlot == null)
        itemSlot = (ItemSlot) null;
      else
        serialization.CopyTo<ItemSlot>(this.PrivilegedIdSlot, ref itemSlot, hookCtx, context, true);
    }
    target.PrivilegedIdSlot = itemSlot;
    SoundSpecifier soundSpecifier = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.DenialSound, ref soundSpecifier, hookCtx, true, context))
      soundSpecifier = serialization.CreateCopy<SoundSpecifier>(this.DenialSound, hookCtx, context, false);
    target.DenialSound = soundSpecifier;
    List<ProtoId<AccessLevelPrototype>> protoIdList = (List<ProtoId<AccessLevelPrototype>>) null;
    if (this.AccessLevels == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<ProtoId<AccessLevelPrototype>>>(this.AccessLevels, ref protoIdList, hookCtx, true, context))
      protoIdList = serialization.CreateCopy<List<ProtoId<AccessLevelPrototype>>>(this.AccessLevels, hookCtx, context, false);
    target.AccessLevels = protoIdList;
    float num = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DoAfter, ref num, hookCtx, false, context))
      num = this.DoAfter;
    target.DoAfter = num;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AccessOverriderComponent target,
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
    AccessOverriderComponent target1 = (AccessOverriderComponent) target;
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
    AccessOverriderComponent target1 = (AccessOverriderComponent) target;
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
    AccessOverriderComponent target1 = (AccessOverriderComponent) target;
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
  virtual AccessOverriderComponent Component.Instantiate() => new AccessOverriderComponent();

  [NetSerializable]
  [Serializable]
  public sealed class WriteToTargetAccessReaderIdMessage : BoundUserInterfaceMessage
  {
    public readonly List<ProtoId<AccessLevelPrototype>> AccessList;

    public WriteToTargetAccessReaderIdMessage(List<ProtoId<AccessLevelPrototype>> accessList)
    {
      this.AccessList = accessList;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class AccessOverriderBoundUserInterfaceState : BoundUserInterfaceState
  {
    public readonly string TargetLabel;
    public readonly Color TargetLabelColor;
    public readonly string PrivilegedIdName;
    public readonly bool IsPrivilegedIdPresent;
    public readonly bool IsPrivilegedIdAuthorized;
    public readonly ProtoId<AccessLevelPrototype>[]? TargetAccessReaderIdAccessList;
    public readonly ProtoId<AccessLevelPrototype>[]? AllowedModifyAccessList;
    public readonly ProtoId<AccessLevelPrototype>[]? MissingPrivilegesList;

    public AccessOverriderBoundUserInterfaceState(
      bool isPrivilegedIdPresent,
      bool isPrivilegedIdAuthorized,
      ProtoId<AccessLevelPrototype>[]? targetAccessReaderIdAccessList,
      ProtoId<AccessLevelPrototype>[]? allowedModifyAccessList,
      ProtoId<AccessLevelPrototype>[]? missingPrivilegesList,
      string privilegedIdName,
      string targetLabel,
      Color targetLabelColor)
    {
      this.IsPrivilegedIdPresent = isPrivilegedIdPresent;
      this.IsPrivilegedIdAuthorized = isPrivilegedIdAuthorized;
      this.TargetAccessReaderIdAccessList = targetAccessReaderIdAccessList;
      this.AllowedModifyAccessList = allowedModifyAccessList;
      this.MissingPrivilegesList = missingPrivilegesList;
      this.PrivilegedIdName = privilegedIdName;
      this.TargetLabel = targetLabel;
      this.TargetLabelColor = targetLabelColor;
    }
  }

  [NetSerializable]
  [Serializable]
  public enum AccessOverriderUiKey : byte
  {
    Key,
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class AccessOverriderComponent_AutoState : IComponentState
  {
    public List<ProtoId<AccessLevelPrototype>> AccessLevels;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class AccessOverriderComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<AccessOverriderComponent, ComponentGetState>(new ComponentEventRefHandler<AccessOverriderComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<AccessOverriderComponent, ComponentHandleState>(new ComponentEventRefHandler<AccessOverriderComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(
      EntityUid uid,
      AccessOverriderComponent component,
      ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new AccessOverriderComponent.AccessOverriderComponent_AutoState()
      {
        AccessLevels = component.AccessLevels
      };
    }

    private void OnHandleState(
      EntityUid uid,
      AccessOverriderComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is AccessOverriderComponent.AccessOverriderComponent_AutoState current))
        return;
      component.AccessLevels = current.AccessLevels == null ? (List<ProtoId<AccessLevelPrototype>>) null : new List<ProtoId<AccessLevelPrototype>>((IEnumerable<ProtoId<AccessLevelPrototype>>) current.AccessLevels);
    }
  }
}
