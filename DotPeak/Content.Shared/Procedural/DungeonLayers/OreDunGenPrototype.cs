// Decompiled with JetBrains decompiler
// Type: Content.Shared.Procedural.DungeonLayers.OreDunGenPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Procedural.DungeonLayers;

[Prototype(null, 1)]
public sealed class OreDunGenPrototype : 
  OreDunGen,
  IPrototype,
  ISerializationGenerated<OreDunGenPrototype>,
  ISerializationGenerated
{
  [IdDataField(1, null)]
  public string ID { set; get; }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref OreDunGenPrototype target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    OreDunGen target1 = (OreDunGen) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (OreDunGenPrototype) target1;
    if (serialization.TryCustomCopy<OreDunGenPrototype>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.ID == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ID, ref target2, hookCtx, false, context))
      target2 = this.ID;
    target.ID = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref OreDunGenPrototype target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref OreDunGen target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    OreDunGenPrototype target1 = (OreDunGenPrototype) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (OreDunGen) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    OreDunGenPrototype target1 = (OreDunGenPrototype) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void InternalCopy(
    ref IDunGenLayer target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    OreDunGenPrototype target1 = (OreDunGenPrototype) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IDunGenLayer) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref IDunGenLayer target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual OreDunGenPrototype OreDunGen.Instantiate() => new OreDunGenPrototype();
}
