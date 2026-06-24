// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Dropship.Fabricator.DropshipFabricatorPrintableComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Skills;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Dropship.Fabricator;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (DropshipFabricatorSystem)})]
public sealed class DropshipFabricatorPrintableComponent : 
  Component,
  ISerializationGenerated<DropshipFabricatorPrintableComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Cost = 50;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float RecycleMultiplier = 0.8f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId<SkillDefinitionComponent> RecycleSkill = (EntProtoId<SkillDefinitionComponent>) "RMCSkillEngineer";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Delay = TimeSpan.FromSeconds(3L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DropshipFabricatorPrintableComponent.CategoryType Category;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DropshipFabricatorPrintableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (DropshipFabricatorPrintableComponent) target1;
    if (serialization.TryCustomCopy<DropshipFabricatorPrintableComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.Cost, ref target2, hookCtx, false, context))
      target2 = this.Cost;
    target.Cost = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RecycleMultiplier, ref target3, hookCtx, false, context))
      target3 = this.RecycleMultiplier;
    target.RecycleMultiplier = target3;
    EntProtoId<SkillDefinitionComponent> target4 = new EntProtoId<SkillDefinitionComponent>();
    if (!serialization.TryCustomCopy<EntProtoId<SkillDefinitionComponent>>(this.RecycleSkill, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntProtoId<SkillDefinitionComponent>>(this.RecycleSkill, hookCtx, context);
    target.RecycleSkill = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Delay, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.Delay, hookCtx, context);
    target.Delay = target5;
    DropshipFabricatorPrintableComponent.CategoryType target6 = DropshipFabricatorPrintableComponent.CategoryType.Equipment;
    if (!serialization.TryCustomCopy<DropshipFabricatorPrintableComponent.CategoryType>(this.Category, ref target6, hookCtx, false, context))
      target6 = this.Category;
    target.Category = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DropshipFabricatorPrintableComponent target,
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
    DropshipFabricatorPrintableComponent target1 = (DropshipFabricatorPrintableComponent) target;
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
    DropshipFabricatorPrintableComponent target1 = (DropshipFabricatorPrintableComponent) target;
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
    DropshipFabricatorPrintableComponent target1 = (DropshipFabricatorPrintableComponent) target;
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
  virtual DropshipFabricatorPrintableComponent Component.Instantiate()
  {
    return new DropshipFabricatorPrintableComponent();
  }

  public enum CategoryType
  {
    Equipment,
    Ammo,
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class DropshipFabricatorPrintableComponent_AutoState : IComponentState
  {
    public int Cost;
    public float RecycleMultiplier;
    public EntProtoId<SkillDefinitionComponent> RecycleSkill;
    public TimeSpan Delay;
    public DropshipFabricatorPrintableComponent.CategoryType Category;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DropshipFabricatorPrintableComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<DropshipFabricatorPrintableComponent, ComponentGetState>(new ComponentEventRefHandler<DropshipFabricatorPrintableComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<DropshipFabricatorPrintableComponent, ComponentHandleState>(new ComponentEventRefHandler<DropshipFabricatorPrintableComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      DropshipFabricatorPrintableComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new DropshipFabricatorPrintableComponent.DropshipFabricatorPrintableComponent_AutoState()
      {
        Cost = component.Cost,
        RecycleMultiplier = component.RecycleMultiplier,
        RecycleSkill = component.RecycleSkill,
        Delay = component.Delay,
        Category = component.Category
      };
    }

    private void OnHandleState(
      EntityUid uid,
      DropshipFabricatorPrintableComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is DropshipFabricatorPrintableComponent.DropshipFabricatorPrintableComponent_AutoState current))
        return;
      component.Cost = current.Cost;
      component.RecycleMultiplier = current.RecycleMultiplier;
      component.RecycleSkill = current.RecycleSkill;
      component.Delay = current.Delay;
      component.Category = current.Category;
    }
  }
}
