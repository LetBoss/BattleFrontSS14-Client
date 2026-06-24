// Decompiled with JetBrains decompiler
// Type: Content.Shared.Access.Components.IdCardConsoleComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Access.Systems;
using Content.Shared.Containers.ItemSlots;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
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
[Robust.Shared.Analyzers.Access(new Type[] {typeof (SharedIdCardConsoleSystem)})]
public sealed class IdCardConsoleComponent : 
  Component,
  ISerializationGenerated<IdCardConsoleComponent>,
  ISerializationGenerated
{
  public static string PrivilegedIdCardSlotId = "IdCardConsole-privilegedId";
  public static string TargetIdCardSlotId = "IdCardConsole-targetId";
  [DataField(null, false, 1, false, false, null)]
  public ItemSlot PrivilegedIdSlot = new ItemSlot();
  [DataField(null, false, 1, false, false, null)]
  public ItemSlot TargetIdSlot = new ItemSlot();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<ProtoId<AccessLevelPrototype>> AccessLevels = new List<ProtoId<AccessLevelPrototype>>()
  {
    ProtoId<AccessLevelPrototype>.op_Implicit("Armory"),
    ProtoId<AccessLevelPrototype>.op_Implicit("Atmospherics"),
    ProtoId<AccessLevelPrototype>.op_Implicit("Bar"),
    ProtoId<AccessLevelPrototype>.op_Implicit("Brig"),
    ProtoId<AccessLevelPrototype>.op_Implicit("Detective"),
    ProtoId<AccessLevelPrototype>.op_Implicit("Captain"),
    ProtoId<AccessLevelPrototype>.op_Implicit("Cargo"),
    ProtoId<AccessLevelPrototype>.op_Implicit("Chapel"),
    ProtoId<AccessLevelPrototype>.op_Implicit("Chemistry"),
    ProtoId<AccessLevelPrototype>.op_Implicit("ChiefEngineer"),
    ProtoId<AccessLevelPrototype>.op_Implicit("ChiefMedicalOfficer"),
    ProtoId<AccessLevelPrototype>.op_Implicit("Command"),
    ProtoId<AccessLevelPrototype>.op_Implicit("Cryogenics"),
    ProtoId<AccessLevelPrototype>.op_Implicit("Engineering"),
    ProtoId<AccessLevelPrototype>.op_Implicit("External"),
    ProtoId<AccessLevelPrototype>.op_Implicit("HeadOfPersonnel"),
    ProtoId<AccessLevelPrototype>.op_Implicit("HeadOfSecurity"),
    ProtoId<AccessLevelPrototype>.op_Implicit("Hydroponics"),
    ProtoId<AccessLevelPrototype>.op_Implicit("Janitor"),
    ProtoId<AccessLevelPrototype>.op_Implicit("Kitchen"),
    ProtoId<AccessLevelPrototype>.op_Implicit("Lawyer"),
    ProtoId<AccessLevelPrototype>.op_Implicit("Maintenance"),
    ProtoId<AccessLevelPrototype>.op_Implicit("Medical"),
    ProtoId<AccessLevelPrototype>.op_Implicit("Quartermaster"),
    ProtoId<AccessLevelPrototype>.op_Implicit("Research"),
    ProtoId<AccessLevelPrototype>.op_Implicit("ResearchDirector"),
    ProtoId<AccessLevelPrototype>.op_Implicit("Salvage"),
    ProtoId<AccessLevelPrototype>.op_Implicit("Security"),
    ProtoId<AccessLevelPrototype>.op_Implicit("Service"),
    ProtoId<AccessLevelPrototype>.op_Implicit("Theatre")
  };

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref IdCardConsoleComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (IdCardConsoleComponent) component;
    if (serialization.TryCustomCopy<IdCardConsoleComponent>(this, ref target, hookCtx, false, context))
      return;
    ItemSlot itemSlot1 = (ItemSlot) null;
    if (this.PrivilegedIdSlot == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<ItemSlot>(this.PrivilegedIdSlot, ref itemSlot1, hookCtx, false, context))
    {
      if (this.PrivilegedIdSlot == null)
        itemSlot1 = (ItemSlot) null;
      else
        serialization.CopyTo<ItemSlot>(this.PrivilegedIdSlot, ref itemSlot1, hookCtx, context, true);
    }
    target.PrivilegedIdSlot = itemSlot1;
    ItemSlot itemSlot2 = (ItemSlot) null;
    if (this.TargetIdSlot == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<ItemSlot>(this.TargetIdSlot, ref itemSlot2, hookCtx, false, context))
    {
      if (this.TargetIdSlot == null)
        itemSlot2 = (ItemSlot) null;
      else
        serialization.CopyTo<ItemSlot>(this.TargetIdSlot, ref itemSlot2, hookCtx, context, true);
    }
    target.TargetIdSlot = itemSlot2;
    List<ProtoId<AccessLevelPrototype>> protoIdList = (List<ProtoId<AccessLevelPrototype>>) null;
    if (this.AccessLevels == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<ProtoId<AccessLevelPrototype>>>(this.AccessLevels, ref protoIdList, hookCtx, true, context))
      protoIdList = serialization.CreateCopy<List<ProtoId<AccessLevelPrototype>>>(this.AccessLevels, hookCtx, context, false);
    target.AccessLevels = protoIdList;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref IdCardConsoleComponent target,
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
    IdCardConsoleComponent target1 = (IdCardConsoleComponent) target;
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
    IdCardConsoleComponent target1 = (IdCardConsoleComponent) target;
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
    IdCardConsoleComponent target1 = (IdCardConsoleComponent) target;
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
  virtual IdCardConsoleComponent Component.Instantiate() => new IdCardConsoleComponent();

  [NetSerializable]
  [Serializable]
  public sealed class WriteToTargetIdMessage : BoundUserInterfaceMessage
  {
    public readonly string FullName;
    public readonly string JobTitle;
    public readonly List<ProtoId<AccessLevelPrototype>> AccessList;
    public readonly ProtoId<AccessLevelPrototype> JobPrototype;

    public WriteToTargetIdMessage(
      string fullName,
      string jobTitle,
      List<ProtoId<AccessLevelPrototype>> accessList,
      ProtoId<AccessLevelPrototype> jobPrototype)
    {
      this.FullName = fullName;
      this.JobTitle = jobTitle;
      this.AccessList = accessList;
      this.JobPrototype = jobPrototype;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class IdCardConsoleBoundUserInterfaceState : BoundUserInterfaceState
  {
    public readonly string PrivilegedIdName;
    public readonly bool IsPrivilegedIdPresent;
    public readonly bool IsPrivilegedIdAuthorized;
    public readonly bool IsTargetIdPresent;
    public readonly string TargetIdName;
    public readonly string? TargetIdFullName;
    public readonly string? TargetIdJobTitle;
    public readonly List<ProtoId<AccessLevelPrototype>>? TargetIdAccessList;
    public readonly List<ProtoId<AccessLevelPrototype>>? AllowedModifyAccessList;
    public readonly ProtoId<AccessLevelPrototype> TargetIdJobPrototype;

    public IdCardConsoleBoundUserInterfaceState(
      bool isPrivilegedIdPresent,
      bool isPrivilegedIdAuthorized,
      bool isTargetIdPresent,
      string? targetIdFullName,
      string? targetIdJobTitle,
      List<ProtoId<AccessLevelPrototype>>? targetIdAccessList,
      List<ProtoId<AccessLevelPrototype>>? allowedModifyAccessList,
      ProtoId<AccessLevelPrototype> targetIdJobPrototype,
      string privilegedIdName,
      string targetIdName)
    {
      this.IsPrivilegedIdPresent = isPrivilegedIdPresent;
      this.IsPrivilegedIdAuthorized = isPrivilegedIdAuthorized;
      this.IsTargetIdPresent = isTargetIdPresent;
      this.TargetIdFullName = targetIdFullName;
      this.TargetIdJobTitle = targetIdJobTitle;
      this.TargetIdAccessList = targetIdAccessList;
      this.AllowedModifyAccessList = allowedModifyAccessList;
      this.TargetIdJobPrototype = targetIdJobPrototype;
      this.PrivilegedIdName = privilegedIdName;
      this.TargetIdName = targetIdName;
    }
  }

  [NetSerializable]
  [Serializable]
  public enum IdCardConsoleUiKey : byte
  {
    Key,
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class IdCardConsoleComponent_AutoState : IComponentState
  {
    public List<ProtoId<AccessLevelPrototype>> AccessLevels;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class IdCardConsoleComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<IdCardConsoleComponent, ComponentGetState>(new ComponentEventRefHandler<IdCardConsoleComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<IdCardConsoleComponent, ComponentHandleState>(new ComponentEventRefHandler<IdCardConsoleComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(
      EntityUid uid,
      IdCardConsoleComponent component,
      ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new IdCardConsoleComponent.IdCardConsoleComponent_AutoState()
      {
        AccessLevels = component.AccessLevels
      };
    }

    private void OnHandleState(
      EntityUid uid,
      IdCardConsoleComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is IdCardConsoleComponent.IdCardConsoleComponent_AutoState current))
        return;
      component.AccessLevels = current.AccessLevels == null ? (List<ProtoId<AccessLevelPrototype>>) null : new List<ProtoId<AccessLevelPrototype>>((IEnumerable<ProtoId<AccessLevelPrototype>>) current.AccessLevels);
    }
  }
}
