// Decompiled with JetBrains decompiler
// Type: Content.Shared.Construction.Steps.MaterialConstructionGraphStep
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Examine;
using Content.Shared.Stacks;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Construction.Steps;

[DataDefinition]
public sealed class MaterialConstructionGraphStep : 
  EntityInsertConstructionGraphStep,
  ISerializationGenerated<MaterialConstructionGraphStep>,
  ISerializationGenerated
{
  [DataField("material", false, 1, true, false, null)]
  public ProtoId<StackPrototype> MaterialPrototypeId { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  public int Amount { get; private set; } = 1;

  public override void DoExamine(ExaminedEvent examinedEvent)
  {
    string str = Loc.GetString(IoCManager.Resolve<IPrototypeManager>().Index<StackPrototype>(this.MaterialPrototypeId).Name, new (string, object)[1]
    {
      ("amount", (object) this.Amount)
    });
    examinedEvent.PushMarkup(Loc.GetString("construction-insert-material-entity", new (string, object)[2]
    {
      ("amount", (object) this.Amount),
      ("materialName", (object) str)
    }));
  }

  public override bool EntityValid(
    EntityUid uid,
    IEntityManager entityManager,
    IComponentFactory compFactory)
  {
    StackComponent stackComponent;
    return entityManager.TryGetComponent<StackComponent>(uid, ref stackComponent) && ProtoId<StackPrototype>.op_Equality(ProtoId<StackPrototype>.op_Implicit(stackComponent.StackTypeId), this.MaterialPrototypeId) && stackComponent.Count >= this.Amount;
  }

  public bool EntityValid(EntityUid entity, [NotNullWhen(true)] out StackComponent? stack)
  {
    StackComponent stackComponent;
    stack = !IoCManager.Resolve<IEntityManager>().TryGetComponent<StackComponent>(entity, ref stackComponent) || !ProtoId<StackPrototype>.op_Equality(ProtoId<StackPrototype>.op_Implicit(stackComponent.StackTypeId), this.MaterialPrototypeId) || stackComponent.Count < this.Amount ? (StackComponent) null : stackComponent;
    return stack != null;
  }

  public override ConstructionGuideEntry GenerateGuideEntry()
  {
    StackPrototype stackPrototype = IoCManager.Resolve<IPrototypeManager>().Index<StackPrototype>(this.MaterialPrototypeId);
    string str = Loc.GetString(stackPrototype.Name, new (string, object)[1]
    {
      ("amount", (object) this.Amount)
    });
    return new ConstructionGuideEntry()
    {
      Localization = "construction-presenter-material-step",
      Arguments = new (string, object)[2]
      {
        ("amount", (object) this.Amount),
        ("material", (object) str)
      },
      Icon = stackPrototype.Icon
    };
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MaterialConstructionGraphStep target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityInsertConstructionGraphStep target1 = (EntityInsertConstructionGraphStep) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (MaterialConstructionGraphStep) target1;
    if (serialization.TryCustomCopy<MaterialConstructionGraphStep>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<StackPrototype> protoId = new ProtoId<StackPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<StackPrototype>>(this.MaterialPrototypeId, ref protoId, hookCtx, false, context))
      protoId = serialization.CreateCopy<ProtoId<StackPrototype>>(this.MaterialPrototypeId, hookCtx, context, false);
    target.MaterialPrototypeId = protoId;
    int num = 0;
    if (!serialization.TryCustomCopy<int>(this.Amount, ref num, hookCtx, false, context))
      num = this.Amount;
    target.Amount = num;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MaterialConstructionGraphStep target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref EntityInsertConstructionGraphStep target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    MaterialConstructionGraphStep target1 = (MaterialConstructionGraphStep) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (EntityInsertConstructionGraphStep) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    MaterialConstructionGraphStep target1 = (MaterialConstructionGraphStep) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual MaterialConstructionGraphStep EntityInsertConstructionGraphStep.Instantiate()
  {
    return new MaterialConstructionGraphStep();
  }
}
