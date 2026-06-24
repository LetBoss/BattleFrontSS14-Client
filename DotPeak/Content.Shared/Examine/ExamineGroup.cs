// Decompiled with JetBrains decompiler
// Type: Content.Shared.Examine.ExamineGroup
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Examine;

[DataDefinition]
public sealed class ExamineGroup : ISerializationGenerated<ExamineGroup>, ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public string? Title;
  [DataField(null, false, 1, false, false, null)]
  public List<ExamineEntry> Entries = new List<ExamineEntry>();
  [DataField(null, false, 1, false, false, null)]
  public List<string> Components = new List<string>();
  [DataField(null, false, 1, false, false, null)]
  public SpriteSpecifier Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/examine-star.png"));
  [DataField(null, false, 1, false, false, null)]
  public LocId ContextText = (LocId) "verb-examine-group-other";
  [DataField(null, false, 1, false, false, null)]
  public string HoverMessage = string.Empty;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ExamineGroup target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<ExamineGroup>(this, ref target, hookCtx, false, context))
      return;
    string target1 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.Title, ref target1, hookCtx, false, context))
      target1 = this.Title;
    target.Title = target1;
    List<ExamineEntry> target2 = (List<ExamineEntry>) null;
    if (this.Entries == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<ExamineEntry>>(this.Entries, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<List<ExamineEntry>>(this.Entries, hookCtx, context);
    target.Entries = target2;
    List<string> target3 = (List<string>) null;
    if (this.Components == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<string>>(this.Components, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<List<string>>(this.Components, hookCtx, context);
    target.Components = target3;
    SpriteSpecifier target4 = (SpriteSpecifier) null;
    if (this.Icon == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SpriteSpecifier>(this.Icon, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<SpriteSpecifier>(this.Icon, hookCtx, context);
    target.Icon = target4;
    LocId target5 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.ContextText, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<LocId>(this.ContextText, hookCtx, context);
    target.ContextText = target5;
    string target6 = (string) null;
    if (this.HoverMessage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.HoverMessage, ref target6, hookCtx, false, context))
      target6 = this.HoverMessage;
    target.HoverMessage = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ExamineGroup target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ExamineGroup target1 = (ExamineGroup) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public ExamineGroup Instantiate() => new ExamineGroup();
}
