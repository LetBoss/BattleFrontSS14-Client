// Decompiled with JetBrains decompiler
// Type: Content.Client.Botany.Components.SeedComponent
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Botany.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client.Botany.Components;

[RegisterComponent]
public sealed class SeedComponent : 
  SharedSeedComponent,
  ISerializationGenerated<SeedComponent>,
  ISerializationGenerated
{
  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SeedComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    SharedSeedComponent target1 = (SharedSeedComponent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SeedComponent) target1;
    serialization.TryCustomCopy<SeedComponent>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SeedComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref SharedSeedComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    SeedComponent target1 = (SeedComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (SharedSeedComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    SeedComponent target1 = (SeedComponent) target;
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
    SeedComponent target1 = (SeedComponent) target;
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
    base.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual SeedComponent SharedSeedComponent.Instantiate() => new SeedComponent();
}
