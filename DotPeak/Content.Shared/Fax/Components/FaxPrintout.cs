// Decompiled with JetBrains decompiler
// Type: Content.Shared.Fax.Components.FaxPrintout
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Paper;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Fax.Components;

[DataDefinition]
public sealed class FaxPrintout : ISerializationGenerated<FaxPrintout>, ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public string Name { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  public string? Label { get; private set; }

  [DataField(null, false, 1, true, false, null)]
  public string Content { get; private set; }

  [DataField(null, false, 1, true, false, typeof (PrototypeIdSerializer<EntityPrototype>))]
  public string PrototypeId { get; private set; }

  [DataField("stampState", false, 1, false, false, null)]
  public string? StampState { get; private set; }

  [DataField("stampedBy", false, 1, false, false, null)]
  public List<StampDisplayInfo> StampedBy { get; private set; } = new List<StampDisplayInfo>();

  [DataField(null, false, 1, false, false, null)]
  public bool Locked { get; private set; }

  private FaxPrintout()
  {
  }

  public FaxPrintout(
    string content,
    string name,
    string? label = null,
    string? prototypeId = null,
    string? stampState = null,
    List<StampDisplayInfo>? stampedBy = null,
    bool locked = false)
  {
    this.Content = content;
    this.Name = name;
    this.Label = label;
    this.PrototypeId = prototypeId ?? "";
    this.StampState = stampState;
    this.StampedBy = stampedBy ?? new List<StampDisplayInfo>();
    this.Locked = locked;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref FaxPrintout target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<FaxPrintout>(this, ref target, hookCtx, false, context))
      return;
    string target1 = (string) null;
    if (this.Name == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Name, ref target1, hookCtx, false, context))
      target1 = this.Name;
    target.Name = target1;
    string target2 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.Label, ref target2, hookCtx, false, context))
      target2 = this.Label;
    target.Label = target2;
    string target3 = (string) null;
    if (this.Content == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Content, ref target3, hookCtx, false, context))
      target3 = this.Content;
    target.Content = target3;
    string target4 = (string) null;
    if (this.PrototypeId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.PrototypeId, ref target4, hookCtx, false, context))
      target4 = this.PrototypeId;
    target.PrototypeId = target4;
    string target5 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.StampState, ref target5, hookCtx, false, context))
      target5 = this.StampState;
    target.StampState = target5;
    List<StampDisplayInfo> target6 = (List<StampDisplayInfo>) null;
    if (this.StampedBy == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<StampDisplayInfo>>(this.StampedBy, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<List<StampDisplayInfo>>(this.StampedBy, hookCtx, context);
    target.StampedBy = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.Locked, ref target7, hookCtx, false, context))
      target7 = this.Locked;
    target.Locked = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref FaxPrintout target,
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
    FaxPrintout target1 = (FaxPrintout) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public FaxPrintout Instantiate() => new FaxPrintout();
}
