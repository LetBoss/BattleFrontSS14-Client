// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.Components.SolutionManager.SolutionContainerManagerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.EntitySystems;
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
namespace Content.Shared.Chemistry.Components.SolutionManager;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedSolutionContainerSystem)})]
public sealed class SolutionContainerManagerComponent : 
  Component,
  ISerializationGenerated<SolutionContainerManagerComponent>,
  ISerializationGenerated
{
  public const int DefaultCapacity = 2;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public HashSet<string> Containers = new HashSet<string>(2);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<string, Solution>? Solutions;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SolutionContainerManagerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (SolutionContainerManagerComponent) component;
    if (serialization.TryCustomCopy<SolutionContainerManagerComponent>(this, ref target, hookCtx, false, context))
      return;
    HashSet<string> stringSet = (HashSet<string>) null;
    if (this.Containers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<string>>(this.Containers, ref stringSet, hookCtx, true, context))
      stringSet = serialization.CreateCopy<HashSet<string>>(this.Containers, hookCtx, context, false);
    target.Containers = stringSet;
    Dictionary<string, Solution> dictionary = (Dictionary<string, Solution>) null;
    if (!serialization.TryCustomCopy<Dictionary<string, Solution>>(this.Solutions, ref dictionary, hookCtx, true, context))
      dictionary = serialization.CreateCopy<Dictionary<string, Solution>>(this.Solutions, hookCtx, context, false);
    target.Solutions = dictionary;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SolutionContainerManagerComponent target,
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
    SolutionContainerManagerComponent target1 = (SolutionContainerManagerComponent) target;
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
    SolutionContainerManagerComponent target1 = (SolutionContainerManagerComponent) target;
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
    SolutionContainerManagerComponent target1 = (SolutionContainerManagerComponent) target;
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
  virtual SolutionContainerManagerComponent Component.Instantiate()
  {
    return new SolutionContainerManagerComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class SolutionContainerManagerComponent_AutoState : IComponentState
  {
    public HashSet<string> Containers;
    public Dictionary<string, Solution>? Solutions;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class SolutionContainerManagerComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<SolutionContainerManagerComponent, ComponentGetState>(new ComponentEventRefHandler<SolutionContainerManagerComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<SolutionContainerManagerComponent, ComponentHandleState>(new ComponentEventRefHandler<SolutionContainerManagerComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(
      EntityUid uid,
      SolutionContainerManagerComponent component,
      ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new SolutionContainerManagerComponent.SolutionContainerManagerComponent_AutoState()
      {
        Containers = component.Containers,
        Solutions = component.Solutions
      };
    }

    private void OnHandleState(
      EntityUid uid,
      SolutionContainerManagerComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is SolutionContainerManagerComponent.SolutionContainerManagerComponent_AutoState current))
        return;
      component.Containers = current.Containers == null ? (HashSet<string>) null : new HashSet<string>((IEnumerable<string>) current.Containers);
      component.Solutions = current.Solutions == null ? (Dictionary<string, Solution>) null : new Dictionary<string, Solution>((IDictionary<string, Solution>) current.Solutions);
    }
  }
}
