// Decompiled with JetBrains decompiler
// Type: Content.Shared.Construction.Steps.ComponentConstructionGraphStep
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Examine;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Construction.Steps;

[DataDefinition]
public sealed class ComponentConstructionGraphStep : 
  ArbitraryInsertConstructionGraphStep,
  ISerializationGenerated<ComponentConstructionGraphStep>,
  ISerializationGenerated
{
  [DataField("component", false, 1, false, false, null)]
  public string Component { get; private set; } = string.Empty;

  public override bool EntityValid(
    EntityUid uid,
    IEntityManager entityManager,
    IComponentFactory compFactory)
  {
    foreach (IComponent component in entityManager.GetComponents(uid))
    {
      if (compFactory.GetComponentName(component.GetType()) == this.Component)
        return true;
    }
    return false;
  }

  public override void DoExamine(ExaminedEvent examinedEvent)
  {
    ExaminedEvent examinedEvent1 = examinedEvent;
    string markup;
    if (!string.IsNullOrEmpty(this.Name))
      markup = Loc.GetString("construction-insert-exact-entity", new (string, object)[1]
      {
        ("entityName", (object) Loc.GetString(this.Name))
      });
    else
      markup = Loc.GetString("construction-insert-entity-with-component", new (string, object)[1]
      {
        ("componentName", (object) this.Component)
      });
    examinedEvent1.PushMarkup(markup);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ComponentConstructionGraphStep target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ArbitraryInsertConstructionGraphStep target1 = (ArbitraryInsertConstructionGraphStep) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ComponentConstructionGraphStep) target1;
    if (serialization.TryCustomCopy<ComponentConstructionGraphStep>(this, ref target, hookCtx, false, context))
      return;
    string str = (string) null;
    if (this.Component == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Component, ref str, hookCtx, false, context))
      str = this.Component;
    target.Component = str;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ComponentConstructionGraphStep target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref ArbitraryInsertConstructionGraphStep target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ComponentConstructionGraphStep target1 = (ComponentConstructionGraphStep) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (ArbitraryInsertConstructionGraphStep) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ComponentConstructionGraphStep target1 = (ComponentConstructionGraphStep) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual ComponentConstructionGraphStep ArbitraryInsertConstructionGraphStep.Instantiate()
  {
    return new ComponentConstructionGraphStep();
  }
}
