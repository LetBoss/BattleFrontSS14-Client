// Decompiled with JetBrains decompiler
// Type: Content.Shared.Construction.Steps.ToolConstructionGraphStep
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Tools;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Construction.Steps;

[DataDefinition]
public sealed class ToolConstructionGraphStep : 
  ConstructionGraphStep,
  ISerializationGenerated<ToolConstructionGraphStep>,
  ISerializationGenerated
{
  [DataField("tool", false, 1, true, false, typeof (PrototypeIdSerializer<ToolQualityPrototype>))]
  public string Tool { get; private set; } = string.Empty;

  [DataField("fuel", false, 1, false, false, null)]
  public float Fuel { get; private set; } = 10f;

  [DataField("examine", false, 1, false, false, null)]
  public string ExamineOverride { get; private set; } = string.Empty;

  [DataField(null, false, 1, false, false, null)]
  public DuplicateConditions DuplicateConditions { get; private set; }

  public override void DoExamine(ExaminedEvent examinedEvent)
  {
    if (!string.IsNullOrEmpty(this.ExamineOverride))
    {
      examinedEvent.PushMarkup(Loc.GetString(this.ExamineOverride));
    }
    else
    {
      ToolQualityPrototype qualityPrototype;
      if (string.IsNullOrEmpty(this.Tool) || !IoCManager.Resolve<IPrototypeManager>().TryIndex<ToolQualityPrototype>(this.Tool, ref qualityPrototype))
        return;
      examinedEvent.PushMarkup(Loc.GetString("construction-use-tool-entity", new (string, object)[1]
      {
        ("toolName", (object) Loc.GetString(qualityPrototype.ToolName))
      }));
    }
  }

  public override ConstructionGuideEntry GenerateGuideEntry()
  {
    ToolQualityPrototype qualityPrototype = IoCManager.Resolve<IPrototypeManager>().Index<ToolQualityPrototype>(this.Tool);
    return new ConstructionGuideEntry()
    {
      Localization = "construction-presenter-tool-step",
      Arguments = new (string, object)[1]
      {
        ("tool", (object) qualityPrototype.ToolName)
      },
      Icon = qualityPrototype.Icon
    };
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ToolConstructionGraphStep target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ConstructionGraphStep target1 = (ConstructionGraphStep) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ToolConstructionGraphStep) target1;
    if (serialization.TryCustomCopy<ToolConstructionGraphStep>(this, ref target, hookCtx, false, context))
      return;
    string str1 = (string) null;
    if (this.Tool == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Tool, ref str1, hookCtx, false, context))
      str1 = this.Tool;
    target.Tool = str1;
    float num = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Fuel, ref num, hookCtx, false, context))
      num = this.Fuel;
    target.Fuel = num;
    string str2 = (string) null;
    if (this.ExamineOverride == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ExamineOverride, ref str2, hookCtx, false, context))
      str2 = this.ExamineOverride;
    target.ExamineOverride = str2;
    DuplicateConditions duplicateConditions = DuplicateConditions.None;
    if (!serialization.TryCustomCopy<DuplicateConditions>(this.DuplicateConditions, ref duplicateConditions, hookCtx, false, context))
      duplicateConditions = this.DuplicateConditions;
    target.DuplicateConditions = duplicateConditions;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ToolConstructionGraphStep target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref ConstructionGraphStep target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ToolConstructionGraphStep target1 = (ToolConstructionGraphStep) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (ConstructionGraphStep) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ToolConstructionGraphStep target1 = (ToolConstructionGraphStep) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual ToolConstructionGraphStep ConstructionGraphStep.Instantiate()
  {
    return new ToolConstructionGraphStep();
  }
}
