// Decompiled with JetBrains decompiler
// Type: Content.Shared.Preferences.Loadouts.Loadout
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;

#nullable enable
namespace Content.Shared.Preferences.Loadouts;

[NetSerializable]
[DataDefinition]
[Serializable]
public sealed class Loadout : 
  IEquatable<Loadout>,
  ISerializationGenerated<Loadout>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<LoadoutPrototype> Prototype;

  public bool Equals(Loadout? other)
  {
    if (other == null)
      return false;
    return this == other || this.Prototype.Equals(other.Prototype);
  }

  public override bool Equals(object? obj)
  {
    if (this == obj)
      return true;
    return obj is Loadout other && this.Equals(other);
  }

  public override int GetHashCode() => this.Prototype.GetHashCode();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref Loadout target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<Loadout>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<LoadoutPrototype> target1 = new ProtoId<LoadoutPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<LoadoutPrototype>>(this.Prototype, ref target1, hookCtx, false, context))
      target1 = serialization.CreateCopy<ProtoId<LoadoutPrototype>>(this.Prototype, hookCtx, context);
    target.Prototype = target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref Loadout target,
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
    Loadout target1 = (Loadout) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public Loadout Instantiate() => new Loadout();
}
