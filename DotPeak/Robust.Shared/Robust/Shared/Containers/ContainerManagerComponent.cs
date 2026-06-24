// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Containers.ContainerManagerComponent
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.Containers;

[NetworkedComponent]
[RegisterComponent]
[ComponentProtoName("ContainerContainer")]
public sealed class ContainerManagerComponent : 
  Component,
  ISerializationHooks,
  ISerializationGenerated<ContainerManagerComponent>,
  ISerializationGenerated
{
  [Robust.Shared.IoC.Dependency]
  private readonly IEntityManager _entMan;
  [DataField("containers", false, 1, false, false, null)]
  public Dictionary<string, BaseContainer> Containers = new Dictionary<string, BaseContainer>();

  void ISerializationHooks.AfterDeserialization()
  {
    foreach ((string str, BaseContainer baseContainer) in this.Containers)
      baseContainer.Init((SharedContainerSystem) null, str, (Entity<ContainerManagerComponent>) (this.Owner, this));
  }

  [Obsolete]
  public bool TryGetContainer(string id, [NotNullWhen(true)] out BaseContainer? container)
  {
    return this._entMan.System<SharedContainerSystem>().TryGetContainer(this.Owner, id, out container, this);
  }

  [Obsolete]
  public bool TryGetContainer(EntityUid entity, [NotNullWhen(true)] out BaseContainer? container)
  {
    return this._entMan.System<SharedContainerSystem>().TryGetContainingContainer(this.Owner, entity, out container, this);
  }

  [Obsolete]
  public ContainerManagerComponent.AllContainersEnumerable GetAllContainers()
  {
    return this._entMan.System<SharedContainerSystem>().GetAllContainers(this.Owner, this);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ContainerManagerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ContainerManagerComponent) target1;
    if (serialization.TryCustomCopy<ContainerManagerComponent>(this, ref target, hookCtx, true, context))
      return;
    Dictionary<string, BaseContainer> target2 = (Dictionary<string, BaseContainer>) null;
    if (this.Containers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, BaseContainer>>(this.Containers, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<Dictionary<string, BaseContainer>>(this.Containers, hookCtx, context);
    target.Containers = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ContainerManagerComponent target,
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
    ContainerManagerComponent target1 = (ContainerManagerComponent) target;
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
    ContainerManagerComponent target1 = (ContainerManagerComponent) target;
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
    ContainerManagerComponent target1 = (ContainerManagerComponent) target;
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
  virtual ContainerManagerComponent Component.Instantiate() => new ContainerManagerComponent();

  [NetSerializable]
  [Serializable]
  internal sealed class ContainerManagerComponentState : ComponentState
  {
    public Dictionary<string, ContainerManagerComponent.ContainerManagerComponentState.ContainerData> Containers;

    public ContainerManagerComponentState(
      Dictionary<string, ContainerManagerComponent.ContainerManagerComponentState.ContainerData> containers)
    {
      this.Containers = containers;
    }

    [NetSerializable]
    [Serializable]
    public readonly struct ContainerData(
      string containerType,
      bool showContents,
      bool occludesLight,
      NetEntity[] containedEntities)
    {
      public readonly string ContainerType = containerType;
      public readonly bool ShowContents = showContents;
      public readonly bool OccludesLight = occludesLight;
      public readonly NetEntity[] ContainedEntities = containedEntities;

      public void Deconstruct(
        out string type,
        out bool showEnts,
        out bool occludesLight,
        out NetEntity[] ents)
      {
        type = this.ContainerType;
        showEnts = this.ShowContents;
        occludesLight = this.OccludesLight;
        ents = this.ContainedEntities;
      }
    }
  }

  public readonly struct AllContainersEnumerable(ContainerManagerComponent? manager) : 
    IEnumerable<BaseContainer>,
    IEnumerable
  {
    private readonly ContainerManagerComponent? _manager = manager;

    public ContainerManagerComponent.AllContainersEnumerator GetEnumerator()
    {
      return new ContainerManagerComponent.AllContainersEnumerator(this._manager);
    }

    IEnumerator<BaseContainer> IEnumerable<BaseContainer>.GetEnumerator()
    {
      return (IEnumerator<BaseContainer>) this.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
  }

  public struct AllContainersEnumerator : IEnumerator<BaseContainer>, IEnumerator, IDisposable
  {
    private Dictionary<string, BaseContainer>.ValueCollection.Enumerator _enumerator;

    public AllContainersEnumerator(ContainerManagerComponent? manager)
    {
      this._enumerator = manager != null ? manager.Containers.Values.GetEnumerator() : new Dictionary<string, BaseContainer>.ValueCollection.Enumerator();
      this.Current = (BaseContainer) null;
    }

    public bool MoveNext()
    {
      if (!this._enumerator.MoveNext())
        return false;
      this.Current = this._enumerator.Current;
      return true;
    }

    void IEnumerator.Reset() => ((IEnumerator) this._enumerator).Reset();

    public BaseContainer Current { get; [param: AllowNull] private set; }

    object IEnumerator.Current => (object) this.Current;

    public void Dispose()
    {
    }
  }
}
