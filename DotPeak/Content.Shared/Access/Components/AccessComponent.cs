// Decompiled with JetBrains decompiler
// Type: Content.Shared.Access.Components.AccessComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Access.Systems;
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
[Robust.Shared.Analyzers.Access(new Type[] {typeof (SharedAccessSystem)})]
[AutoGenerateComponentState(false, false)]
public sealed class AccessComponent : 
  Component,
  ISerializationGenerated<AccessComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  [AutoNetworkedField]
  public bool Enabled = true;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.Analyzers.Access(new Type[] {typeof (SharedAccessSystem)})]
  [AutoNetworkedField]
  public HashSet<ProtoId<AccessLevelPrototype>> Tags = new HashSet<ProtoId<AccessLevelPrototype>>();
  [DataField(null, true, 1, false, false, null)]
  [AutoNetworkedField]
  public HashSet<ProtoId<AccessGroupPrototype>> Groups = new HashSet<ProtoId<AccessGroupPrototype>>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AccessComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (AccessComponent) component;
    if (serialization.TryCustomCopy<AccessComponent>(this, ref target, hookCtx, false, context))
      return;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.Enabled, ref flag, hookCtx, false, context))
      flag = this.Enabled;
    target.Enabled = flag;
    HashSet<ProtoId<AccessLevelPrototype>> protoIdSet1 = (HashSet<ProtoId<AccessLevelPrototype>>) null;
    if (this.Tags == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<ProtoId<AccessLevelPrototype>>>(this.Tags, ref protoIdSet1, hookCtx, true, context))
      protoIdSet1 = serialization.CreateCopy<HashSet<ProtoId<AccessLevelPrototype>>>(this.Tags, hookCtx, context, false);
    target.Tags = protoIdSet1;
    HashSet<ProtoId<AccessGroupPrototype>> protoIdSet2 = (HashSet<ProtoId<AccessGroupPrototype>>) null;
    if (this.Groups == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<ProtoId<AccessGroupPrototype>>>(this.Groups, ref protoIdSet2, hookCtx, true, context))
      protoIdSet2 = serialization.CreateCopy<HashSet<ProtoId<AccessGroupPrototype>>>(this.Groups, hookCtx, context, false);
    target.Groups = protoIdSet2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AccessComponent target,
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
    AccessComponent target1 = (AccessComponent) target;
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
    AccessComponent target1 = (AccessComponent) target;
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
    AccessComponent target1 = (AccessComponent) target;
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
  virtual AccessComponent Component.Instantiate() => new AccessComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class AccessComponent_AutoState : IComponentState
  {
    public bool Enabled;
    public HashSet<ProtoId<AccessLevelPrototype>> Tags;
    public HashSet<ProtoId<AccessGroupPrototype>> Groups;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class AccessComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<AccessComponent, ComponentGetState>(new ComponentEventRefHandler<AccessComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<AccessComponent, ComponentHandleState>(new ComponentEventRefHandler<AccessComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(EntityUid uid, AccessComponent component, ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new AccessComponent.AccessComponent_AutoState()
      {
        Enabled = component.Enabled,
        Tags = component.Tags,
        Groups = component.Groups
      };
    }

    private void OnHandleState(
      EntityUid uid,
      AccessComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is AccessComponent.AccessComponent_AutoState current))
        return;
      component.Enabled = current.Enabled;
      component.Tags = current.Tags == null ? (HashSet<ProtoId<AccessLevelPrototype>>) null : new HashSet<ProtoId<AccessLevelPrototype>>((IEnumerable<ProtoId<AccessLevelPrototype>>) current.Tags);
      component.Groups = current.Groups == null ? (HashSet<ProtoId<AccessGroupPrototype>>) null : new HashSet<ProtoId<AccessGroupPrototype>>((IEnumerable<ProtoId<AccessGroupPrototype>>) current.Groups);
    }
  }
}
