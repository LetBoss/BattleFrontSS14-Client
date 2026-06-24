// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.Reaction.ReactantPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;

#nullable enable
namespace Content.Shared.Chemistry.Reaction;

[DataDefinition]
public sealed class ReactantPrototype : 
  ISerializationGenerated<ReactantPrototype>,
  ISerializationGenerated
{
  [DataField("amount", false, 1, false, false, null)]
  private FixedPoint2 _amount = FixedPoint2.New(1);
  [DataField("catalyst", false, 1, false, false, null)]
  private bool _catalyst;

  public FixedPoint2 Amount => this._amount;

  public bool Catalyst => this._catalyst;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ReactantPrototype target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<ReactantPrototype>(this, ref target, hookCtx, false, context))
      return;
    FixedPoint2 fixedPoint2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this._amount, ref fixedPoint2, hookCtx, false, context))
      fixedPoint2 = serialization.CreateCopy<FixedPoint2>(this._amount, hookCtx, context, false);
    target._amount = fixedPoint2;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this._catalyst, ref flag, hookCtx, false, context))
      flag = this._catalyst;
    target._catalyst = flag;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ReactantPrototype target,
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
    ReactantPrototype target1 = (ReactantPrototype) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public ReactantPrototype Instantiate() => new ReactantPrototype();
}
