// Decompiled with JetBrains decompiler
// Type: Content.Shared.Inventory.InventoryComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DisplacementMap;
using Robust.Shared.Analyzers;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Inventory;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (InventorySystem)})]
[AutoGenerateComponentState(true, false)]
public sealed class InventoryComponent : 
  Component,
  ISerializationGenerated<InventoryComponent>,
  ISerializationGenerated
{
  public SlotDefinition[] Slots = Array.Empty<SlotDefinition>();
  public ContainerSlot[] Containers = Array.Empty<ContainerSlot>();
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<string, DisplacementData> Displacements = new Dictionary<string, DisplacementData>();
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<string, DisplacementData> FemaleDisplacements = new Dictionary<string, DisplacementData>();
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<string, DisplacementData> MaleDisplacements = new Dictionary<string, DisplacementData>();

  [DataField("templateId", false, 1, false, false, typeof (PrototypeIdSerializer<InventoryTemplatePrototype>))]
  [AutoNetworkedField]
  public string TemplateId { get; set; } = "human";

  [DataField("speciesId", false, 1, false, false, null)]
  public string? SpeciesId { get; set; }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref InventoryComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (InventoryComponent) target1;
    if (serialization.TryCustomCopy<InventoryComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.TemplateId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.TemplateId, ref target2, hookCtx, false, context))
      target2 = this.TemplateId;
    target.TemplateId = target2;
    string target3 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.SpeciesId, ref target3, hookCtx, false, context))
      target3 = this.SpeciesId;
    target.SpeciesId = target3;
    Dictionary<string, DisplacementData> target4 = (Dictionary<string, DisplacementData>) null;
    if (this.Displacements == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, DisplacementData>>(this.Displacements, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<Dictionary<string, DisplacementData>>(this.Displacements, hookCtx, context);
    target.Displacements = target4;
    Dictionary<string, DisplacementData> target5 = (Dictionary<string, DisplacementData>) null;
    if (this.FemaleDisplacements == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, DisplacementData>>(this.FemaleDisplacements, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<Dictionary<string, DisplacementData>>(this.FemaleDisplacements, hookCtx, context);
    target.FemaleDisplacements = target5;
    Dictionary<string, DisplacementData> target6 = (Dictionary<string, DisplacementData>) null;
    if (this.MaleDisplacements == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, DisplacementData>>(this.MaleDisplacements, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<Dictionary<string, DisplacementData>>(this.MaleDisplacements, hookCtx, context);
    target.MaleDisplacements = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref InventoryComponent target,
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
    InventoryComponent target1 = (InventoryComponent) target;
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
    InventoryComponent target1 = (InventoryComponent) target;
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
    InventoryComponent target1 = (InventoryComponent) target;
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
  virtual InventoryComponent Component.Instantiate() => new InventoryComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class InventoryComponent_AutoState : IComponentState
  {
    public string TemplateId;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class InventoryComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<InventoryComponent, ComponentGetState>(new ComponentEventRefHandler<InventoryComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<InventoryComponent, ComponentHandleState>(new ComponentEventRefHandler<InventoryComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      InventoryComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new InventoryComponent.InventoryComponent_AutoState()
      {
        TemplateId = component.TemplateId
      };
    }

    private void OnHandleState(
      EntityUid uid,
      InventoryComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is InventoryComponent.InventoryComponent_AutoState current))
        return;
      component.TemplateId = current.TemplateId;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, InventoryComponent>(uid, component, ref args1);
    }
  }
}
