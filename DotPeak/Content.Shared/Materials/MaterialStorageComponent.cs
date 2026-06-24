// Decompiled with JetBrains decompiler
// Type: Content.Shared.Materials.MaterialStorageComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Materials;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedMaterialStorageSystem)})]
public sealed class MaterialStorageComponent : 
  Component,
  ISerializationGenerated<MaterialStorageComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public bool InsertOnInteract = true;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  public int? StorageLimit;
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? Whitelist;
  [DataField(null, false, 1, false, false, null)]
  public bool DropOnDeconstruct = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<ProtoId<MaterialPrototype>>? MaterialWhiteList;
  [DataField(null, false, 1, false, false, null)]
  public bool IgnoreColor;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? InsertingSound;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan InsertionTime = TimeSpan.FromSeconds(0.79000002145767212);
  [DataField(null, false, 1, false, false, null)]
  public bool CanEjectStoredMaterials = true;

  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<ProtoId<MaterialPrototype>, int> Storage { get; set; } = new Dictionary<ProtoId<MaterialPrototype>, int>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MaterialStorageComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (MaterialStorageComponent) target1;
    if (serialization.TryCustomCopy<MaterialStorageComponent>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<ProtoId<MaterialPrototype>, int> target2 = (Dictionary<ProtoId<MaterialPrototype>, int>) null;
    if (this.Storage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<ProtoId<MaterialPrototype>, int>>(this.Storage, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<Dictionary<ProtoId<MaterialPrototype>, int>>(this.Storage, hookCtx, context);
    target.Storage = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.InsertOnInteract, ref target3, hookCtx, false, context))
      target3 = this.InsertOnInteract;
    target.InsertOnInteract = target3;
    int? target4 = new int?();
    if (!serialization.TryCustomCopy<int?>(this.StorageLimit, ref target4, hookCtx, false, context))
      target4 = this.StorageLimit;
    target.StorageLimit = target4;
    EntityWhitelist target5 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref target5, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        target5 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref target5, hookCtx, context);
    }
    target.Whitelist = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.DropOnDeconstruct, ref target6, hookCtx, false, context))
      target6 = this.DropOnDeconstruct;
    target.DropOnDeconstruct = target6;
    List<ProtoId<MaterialPrototype>> target7 = (List<ProtoId<MaterialPrototype>>) null;
    if (!serialization.TryCustomCopy<List<ProtoId<MaterialPrototype>>>(this.MaterialWhiteList, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<List<ProtoId<MaterialPrototype>>>(this.MaterialWhiteList, hookCtx, context);
    target.MaterialWhiteList = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.IgnoreColor, ref target8, hookCtx, false, context))
      target8 = this.IgnoreColor;
    target.IgnoreColor = target8;
    SoundSpecifier target9 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.InsertingSound, ref target9, hookCtx, true, context))
      target9 = serialization.CreateCopy<SoundSpecifier>(this.InsertingSound, hookCtx, context);
    target.InsertingSound = target9;
    TimeSpan target10 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.InsertionTime, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<TimeSpan>(this.InsertionTime, hookCtx, context);
    target.InsertionTime = target10;
    bool target11 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanEjectStoredMaterials, ref target11, hookCtx, false, context))
      target11 = this.CanEjectStoredMaterials;
    target.CanEjectStoredMaterials = target11;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MaterialStorageComponent target,
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
    MaterialStorageComponent target1 = (MaterialStorageComponent) target;
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
    MaterialStorageComponent target1 = (MaterialStorageComponent) target;
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
    MaterialStorageComponent target1 = (MaterialStorageComponent) target;
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
  virtual MaterialStorageComponent Component.Instantiate() => new MaterialStorageComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class MaterialStorageComponent_AutoState : IComponentState
  {
    public Dictionary<ProtoId<MaterialPrototype>, int> Storage;
    public List<ProtoId<MaterialPrototype>>? MaterialWhiteList;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class MaterialStorageComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<MaterialStorageComponent, ComponentGetState>(new ComponentEventRefHandler<MaterialStorageComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<MaterialStorageComponent, ComponentHandleState>(new ComponentEventRefHandler<MaterialStorageComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      MaterialStorageComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new MaterialStorageComponent.MaterialStorageComponent_AutoState()
      {
        Storage = component.Storage,
        MaterialWhiteList = component.MaterialWhiteList
      };
    }

    private void OnHandleState(
      EntityUid uid,
      MaterialStorageComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is MaterialStorageComponent.MaterialStorageComponent_AutoState current))
        return;
      component.Storage = current.Storage == null ? (Dictionary<ProtoId<MaterialPrototype>, int>) null : new Dictionary<ProtoId<MaterialPrototype>, int>((IDictionary<ProtoId<MaterialPrototype>, int>) current.Storage);
      component.MaterialWhiteList = current.MaterialWhiteList == null ? (List<ProtoId<MaterialPrototype>>) null : new List<ProtoId<MaterialPrototype>>((IEnumerable<ProtoId<MaterialPrototype>>) current.MaterialWhiteList);
    }
  }
}
