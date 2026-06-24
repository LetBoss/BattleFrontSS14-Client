// Decompiled with JetBrains decompiler
// Type: Content.Shared.Materials.CollideMaterialReclaimerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Materials;

[RegisterComponent]
public sealed class CollideMaterialReclaimerComponent : 
  Component,
  ISerializationGenerated<CollideMaterialReclaimerComponent>,
  ISerializationGenerated
{
  [DataField("fixtureId", false, 1, false, false, null)]
  public string FixtureId = "brrt";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CollideMaterialReclaimerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (CollideMaterialReclaimerComponent) target1;
    if (serialization.TryCustomCopy<CollideMaterialReclaimerComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.FixtureId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.FixtureId, ref target2, hookCtx, false, context))
      target2 = this.FixtureId;
    target.FixtureId = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CollideMaterialReclaimerComponent target,
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
    CollideMaterialReclaimerComponent target1 = (CollideMaterialReclaimerComponent) target;
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
    CollideMaterialReclaimerComponent target1 = (CollideMaterialReclaimerComponent) target;
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
    CollideMaterialReclaimerComponent target1 = (CollideMaterialReclaimerComponent) target;
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
  virtual CollideMaterialReclaimerComponent Component.Instantiate()
  {
    return new CollideMaterialReclaimerComponent();
  }
}
