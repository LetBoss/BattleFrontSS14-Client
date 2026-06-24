// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Construction.XenoAnnounceStructureDestructionComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Construction;

[RegisterComponent]
public sealed class XenoAnnounceStructureDestructionComponent : 
  Component,
  ISerializationGenerated<XenoAnnounceStructureDestructionComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public LocId MessageID = (LocId) "rmc-xeno-construction-structure-destroyed";
  [DataField(null, false, 1, false, false, null)]
  public string? StructureName;
  [DataField(null, false, 1, false, false, null)]
  public string DestructionVerb = "destroyed";
  [DataField(null, false, 1, false, false, null)]
  public Color MessageColor = Color.FromHex((ReadOnlySpan<char>) "#2A623D", new Color?());

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoAnnounceStructureDestructionComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoAnnounceStructureDestructionComponent) target1;
    if (serialization.TryCustomCopy<XenoAnnounceStructureDestructionComponent>(this, ref target, hookCtx, false, context))
      return;
    LocId target2 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.MessageID, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<LocId>(this.MessageID, hookCtx, context);
    target.MessageID = target2;
    string target3 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.StructureName, ref target3, hookCtx, false, context))
      target3 = this.StructureName;
    target.StructureName = target3;
    string target4 = (string) null;
    if (this.DestructionVerb == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.DestructionVerb, ref target4, hookCtx, false, context))
      target4 = this.DestructionVerb;
    target.DestructionVerb = target4;
    Color target5 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.MessageColor, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<Color>(this.MessageColor, hookCtx, context);
    target.MessageColor = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoAnnounceStructureDestructionComponent target,
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
    XenoAnnounceStructureDestructionComponent target1 = (XenoAnnounceStructureDestructionComponent) target;
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
    XenoAnnounceStructureDestructionComponent target1 = (XenoAnnounceStructureDestructionComponent) target;
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
    XenoAnnounceStructureDestructionComponent target1 = (XenoAnnounceStructureDestructionComponent) target;
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
  virtual XenoAnnounceStructureDestructionComponent Component.Instantiate()
  {
    return new XenoAnnounceStructureDestructionComponent();
  }
}
