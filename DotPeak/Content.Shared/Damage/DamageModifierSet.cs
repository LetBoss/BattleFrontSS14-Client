// Decompiled with JetBrains decompiler
// Type: Content.Shared.Damage.DamageModifierSet
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage.Prototypes;
using Robust.Shared.Analyzers;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;
using System;

#nullable enable
namespace Content.Shared.Damage;

[DataDefinition]
[NetSerializable]
[Virtual]
[Serializable]
public class DamageModifierSet : ISerializationGenerated<DamageModifierSet>, ISerializationGenerated
{
  [DataField("coefficients", false, 1, false, false, typeof (PrototypeIdDictionarySerializer<float, DamageTypePrototype>))]
  public System.Collections.Generic.Dictionary<string, float> Coefficients = new System.Collections.Generic.Dictionary<string, float>();
  [DataField("flatReductions", false, 1, false, false, typeof (PrototypeIdDictionarySerializer<float, DamageTypePrototype>))]
  public System.Collections.Generic.Dictionary<string, float> FlatReduction = new System.Collections.Generic.Dictionary<string, float>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref DamageModifierSet target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<DamageModifierSet>(this, ref target, hookCtx, false, context))
      return;
    System.Collections.Generic.Dictionary<string, float> dictionary1 = (System.Collections.Generic.Dictionary<string, float>) null;
    if (this.Coefficients == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<System.Collections.Generic.Dictionary<string, float>>(this.Coefficients, ref dictionary1, hookCtx, true, context))
      dictionary1 = serialization.CreateCopy<System.Collections.Generic.Dictionary<string, float>>(this.Coefficients, hookCtx, context, false);
    target.Coefficients = dictionary1;
    System.Collections.Generic.Dictionary<string, float> dictionary2 = (System.Collections.Generic.Dictionary<string, float>) null;
    if (this.FlatReduction == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<System.Collections.Generic.Dictionary<string, float>>(this.FlatReduction, ref dictionary2, hookCtx, true, context))
      dictionary2 = serialization.CreateCopy<System.Collections.Generic.Dictionary<string, float>>(this.FlatReduction, hookCtx, context, false);
    target.FlatReduction = dictionary2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref DamageModifierSet target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DamageModifierSet target1 = (DamageModifierSet) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public virtual DamageModifierSet Instantiate() => new DamageModifierSet();
}
