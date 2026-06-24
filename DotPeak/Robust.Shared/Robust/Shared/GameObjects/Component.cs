// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.Component
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.IoC;
using Robust.Shared.Reflection;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;
using System;

#nullable enable
namespace Robust.Shared.GameObjects;

[Reflect(false)]
[ImplicitDataDefinitionForInheritors]
public abstract class Component : 
  IComponent,
  ISerializationGenerated<IComponent>,
  ISerializationGenerated,
  ISerializationGenerated<Component>
{
  [DataField("netsync", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  private bool _netSync { get; set; } = true;

  internal bool Networked { get; set; } = true;

  bool IComponent.Networked
  {
    get => this.Networked;
    set => this.Networked = value;
  }

  public bool NetSyncEnabled
  {
    get => this.Networked && this._netSync;
    set => this._netSync = value;
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  [Obsolete("Update your API to allow accessing Owner through other means")]
  public EntityUid Owner { get; set; } = EntityUid.Invalid;

  [Robust.Shared.ViewVariables.ViewVariables]
  public ComponentLifeStage LifeStage { get; internal set; }

  ComponentLifeStage IComponent.LifeStage
  {
    get => this.LifeStage;
    set => this.LifeStage = value;
  }

  public virtual bool SendOnlyToOwner => false;

  public virtual bool SessionSpecific => false;

  [Robust.Shared.ViewVariables.ViewVariables]
  public bool Initialized => this.LifeStage >= ComponentLifeStage.Initializing;

  [Robust.Shared.ViewVariables.ViewVariables]
  public bool Running
  {
    get
    {
      return ComponentLifeStage.Starting <= this.LifeStage && this.LifeStage <= ComponentLifeStage.Stopping;
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  public bool Deleted => this.LifeStage >= ComponentLifeStage.Removing;

  [Robust.Shared.ViewVariables.ViewVariables]
  public GameTick CreationTick { get; internal set; }

  GameTick IComponent.CreationTick
  {
    get => this.CreationTick;
    set => this.CreationTick = value;
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  public GameTick LastModifiedTick { get; internal set; }

  GameTick IComponent.LastModifiedTick
  {
    get => this.LastModifiedTick;
    set => this.LastModifiedTick = value;
  }

  [Obsolete]
  public void Dirty(IEntityManager? entManager = null)
  {
    IoCManager.Resolve<IEntityManager>(ref entManager);
    entManager.Dirty(this.Owner, (IComponent) this);
  }

  void IComponent.ClearTicks() => this.ClearTicks();

  private protected virtual void ClearTicks()
  {
    this.LastModifiedTick = GameTick.Zero;
    this.ClearCreationTick();
  }

  void IComponent.ClearCreationTick() => this.ClearCreationTick();

  private protected void ClearCreationTick() => this.CreationTick = GameTick.Zero;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<Component>(this, ref target, hookCtx, false, context))
      return;
    bool target1 = false;
    if (!serialization.TryCustomCopy<bool>(this._netSync, ref target1, hookCtx, false, context))
      target1 = this._netSync;
    target._netSync = target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
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
    Component target1 = (Component) target;
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
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public virtual Component Instantiate() => throw new NotImplementedException();

  IComponent IComponent.Instantiate() => (IComponent) this.Instantiate();

  IComponent ISerializationGenerated<IComponent>.Instantiate() => (IComponent) this.Instantiate();
}
