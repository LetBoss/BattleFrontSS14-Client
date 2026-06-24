// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.MetaDataComponent
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.GameObjects;

[RegisterComponent]
[NetworkedComponent]
public sealed class MetaDataComponent : 
  Component,
  ISerializationGenerated<MetaDataComponent>,
  ISerializationGenerated
{
  [DataField("name", false, 1, false, false, null)]
  internal string? _entityName;
  [DataField("desc", false, 1, false, false, null)]
  internal string? _entityDescription;
  internal EntityPrototype? _entityPrototype;
  [Robust.Shared.ViewVariables.ViewVariables]
  internal readonly Dictionary<ushort, IComponent> NetComponents = new Dictionary<ushort, IComponent>();
  internal TimeSpan? PauseTime;
  internal MetaDataFlags _flags;
  [Robust.Shared.ViewVariables.ViewVariables]
  internal PvsChunkLocation? LastPvsLocation;
  internal PvsIndex PvsData = PvsIndex.Invalid;

  [Robust.Shared.ViewVariables.ViewVariables]
  [Access(new Type[] {typeof (EntityManager)}, Other = AccessPermissions.ReadExecute)]
  public NetEntity NetEntity { get; internal set; } = NetEntity.Invalid;

  [Robust.Shared.ViewVariables.ViewVariables]
  public GameTick EntityLastModifiedTick { get; internal set; } = GameTick.First;

  [Robust.Shared.ViewVariables.ViewVariables]
  public GameTick LastStateApplied { get; internal set; } = GameTick.Zero;

  [Robust.Shared.ViewVariables.ViewVariables]
  public GameTick LastComponentRemoved { get; internal set; } = GameTick.Zero;

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public string EntityName
  {
    get
    {
      if (this._entityName != null)
        return this._entityName;
      return this._entityPrototype == null ? string.Empty : this._entityPrototype.Name;
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public string EntityDescription
  {
    get
    {
      if (this._entityDescription != null)
        return this._entityDescription;
      return this._entityPrototype == null ? string.Empty : this._entityPrototype.Description;
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  public EntityPrototype? EntityPrototype
  {
    get => this._entityPrototype;
    [Obsolete("Use MetaDataSystem.SetEntityPrototype")] set
    {
      this._entityPrototype = value;
      this.Dirty((IEntityManager) null);
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  [Access(new Type[] {typeof (EntityManager)}, Other = AccessPermissions.ReadExecute)]
  public EntityLifeStage EntityLifeStage { get; internal set; }

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  public MetaDataFlags Flags
  {
    get => this._flags;
    internal set
    {
      if (this._flags == value)
        return;
      this._flags = value;
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  public ushort VisibilityMask { get; internal set; } = 1;

  [Robust.Shared.ViewVariables.ViewVariables]
  public bool EntityPaused => this.PauseTime.HasValue;

  public bool EntityInitialized => this.EntityLifeStage >= EntityLifeStage.Initialized;

  public bool EntityInitializing => this.EntityLifeStage == EntityLifeStage.Initializing;

  public bool EntityDeleted => this.EntityLifeStage >= EntityLifeStage.Deleted;

  private protected override void ClearTicks() => this.ClearCreationTick();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MetaDataComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (MetaDataComponent) target1;
    if (serialization.TryCustomCopy<MetaDataComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (!serialization.TryCustomCopy<string>(this._entityName, ref target2, hookCtx, false, context))
      target2 = this._entityName;
    target._entityName = target2;
    string target3 = (string) null;
    if (!serialization.TryCustomCopy<string>(this._entityDescription, ref target3, hookCtx, false, context))
      target3 = this._entityDescription;
    target._entityDescription = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MetaDataComponent target,
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
    MetaDataComponent target1 = (MetaDataComponent) target;
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
    MetaDataComponent target1 = (MetaDataComponent) target;
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
    MetaDataComponent target1 = (MetaDataComponent) target;
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
  virtual MetaDataComponent Component.Instantiate() => new MetaDataComponent();
}
