// Decompiled with JetBrains decompiler
// Type: Content.Shared.Hands.Components.Hand
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Hands.Components;

[DataDefinition]
[NetSerializable]
[Serializable]
public record struct Hand : ISerializationGenerated<Hand>, ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public HandLocation Location;

  public Hand() => this.Location = HandLocation.Right;

  public Hand(HandLocation location)
  {
    this.Location = HandLocation.Right;
    this.Location = location;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref Hand target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<Hand>(this, ref target, hookCtx, false, context))
      return;
    HandLocation target1 = HandLocation.Left;
    if (!serialization.TryCustomCopy<HandLocation>(this.Location, ref target1, hookCtx, false, context))
      target1 = this.Location;
    target = target with { Location = target1 };
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref Hand target,
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
    Hand target1 = (Hand) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public Hand Instantiate() => new Hand();

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return EqualityComparer<HandLocation>.Default.GetHashCode(this.Location);
  }

  [CompilerGenerated]
  public readonly bool Equals(Hand other)
  {
    return EqualityComparer<HandLocation>.Default.Equals(this.Location, other.Location);
  }
}
