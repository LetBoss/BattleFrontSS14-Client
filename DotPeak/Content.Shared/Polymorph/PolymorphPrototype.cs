// Decompiled with JetBrains decompiler
// Type: Content.Shared.Polymorph.PolymorphPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;
using System;

#nullable enable
namespace Content.Shared.Polymorph;

[Robust.Shared.Prototypes.Prototype(null, 1)]
[DataDefinition]
public sealed class PolymorphPrototype : 
  IPrototype,
  IInheritingPrototype,
  ISerializationGenerated<PolymorphPrototype>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, true, null)]
  public PolymorphConfiguration Configuration = new PolymorphConfiguration();

  [Robust.Shared.ViewVariables.ViewVariables]
  [IdDataField(1, null)]
  public string ID { get; private set; }

  [ParentDataField(typeof (AbstractPrototypeIdArraySerializer<PolymorphPrototype>), 1)]
  public string[]? Parents { get; private set; }

  [NeverPushInheritance]
  [AbstractDataField(1)]
  public bool Abstract { get; private set; }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PolymorphPrototype target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<PolymorphPrototype>(this, ref target, hookCtx, false, context))
      return;
    string target1 = (string) null;
    if (this.ID == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ID, ref target1, hookCtx, false, context))
      target1 = this.ID;
    target.ID = target1;
    string[] target2 = (string[]) null;
    if (!serialization.TryCustomCopy<string[]>(this.Parents, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<string[]>(this.Parents, hookCtx, context);
    target.Parents = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.Abstract, ref target3, hookCtx, false, context))
      target3 = this.Abstract;
    target.Abstract = target3;
    PolymorphConfiguration target4 = (PolymorphConfiguration) null;
    if (this.Configuration == (PolymorphConfiguration) null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<PolymorphConfiguration>(this.Configuration, ref target4, hookCtx, false, context))
    {
      if (this.Configuration == (PolymorphConfiguration) null)
        target4 = (PolymorphConfiguration) null;
      else
        serialization.CopyTo<PolymorphConfiguration>(this.Configuration, ref target4, hookCtx, context, true);
    }
    target.Configuration = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PolymorphPrototype target,
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
    PolymorphPrototype target1 = (PolymorphPrototype) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public PolymorphPrototype Instantiate() => new PolymorphPrototype();
}
