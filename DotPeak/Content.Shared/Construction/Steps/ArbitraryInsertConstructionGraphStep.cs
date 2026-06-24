// Decompiled with JetBrains decompiler
// Type: Content.Shared.Construction.Steps.ArbitraryInsertConstructionGraphStep
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Examine;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Construction.Steps;

public abstract class ArbitraryInsertConstructionGraphStep : 
  EntityInsertConstructionGraphStep,
  ISerializationGenerated<ArbitraryInsertConstructionGraphStep>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public string Name { get; private set; } = string.Empty;

  [DataField(null, false, 1, false, false, null)]
  public SpriteSpecifier? Icon { get; private set; }

  public override void DoExamine(ExaminedEvent examinedEvent)
  {
    if (string.IsNullOrEmpty(this.Name))
      return;
    string str = Loc.GetString(this.Name);
    examinedEvent.PushMarkup(Loc.GetString("construction-insert-arbitrary-entity", new (string, object)[1]
    {
      ("stepName", (object) str)
    }));
  }

  public override ConstructionGuideEntry GenerateGuideEntry()
  {
    string str = Loc.GetString(this.Name);
    return new ConstructionGuideEntry()
    {
      Localization = "construction-presenter-arbitrary-step",
      Arguments = new (string, object)[1]
      {
        ("name", (object) str)
      },
      Icon = this.Icon
    };
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref ArbitraryInsertConstructionGraphStep target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityInsertConstructionGraphStep target1 = (EntityInsertConstructionGraphStep) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ArbitraryInsertConstructionGraphStep) target1;
    if (serialization.TryCustomCopy<ArbitraryInsertConstructionGraphStep>(this, ref target, hookCtx, false, context))
      return;
    string str = (string) null;
    if (this.Name == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Name, ref str, hookCtx, false, context))
      str = this.Name;
    target.Name = str;
    SpriteSpecifier spriteSpecifier = (SpriteSpecifier) null;
    if (!serialization.TryCustomCopy<SpriteSpecifier>(this.Icon, ref spriteSpecifier, hookCtx, true, context))
      spriteSpecifier = serialization.CreateCopy<SpriteSpecifier>(this.Icon, hookCtx, context, false);
    target.Icon = spriteSpecifier;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref ArbitraryInsertConstructionGraphStep target,
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
    ArbitraryInsertConstructionGraphStep target1 = (ArbitraryInsertConstructionGraphStep) target;
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
    ArbitraryInsertConstructionGraphStep target1 = (ArbitraryInsertConstructionGraphStep) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual ArbitraryInsertConstructionGraphStep EntityInsertConstructionGraphStep.Instantiate()
  {
    throw new NotImplementedException();
  }
}
