// Decompiled with JetBrains decompiler
// Type: Content.Shared.Storage.ItemStorageLocation
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Storage;

[DataDefinition]
[NetSerializable]
[Serializable]
public record struct ItemStorageLocation : 
  ISerializationGenerated<ItemStorageLocation>,
  ISerializationGenerated
{
  [DataField("_rotation", false, 1, false, false, null)]
  public Direction Direction;
  [DataField(null, false, 1, false, false, null)]
  public Vector2i Position;

  public Angle Rotation
  {
    get => DirectionExtensions.ToAngle(this.Direction);
    set => this.Direction = ((Angle) ref value).GetCardinalDir();
  }

  public ItemStorageLocation(Angle rotation, Vector2i position)
  {
    this.Direction = (Direction) 0;
    this.Position = new Vector2i();
    this.Rotation = rotation;
    this.Position = position;
  }

  public bool Equals(ItemStorageLocation? other)
  {
    Angle rotation1 = this.Rotation;
    Angle? rotation2 = other?.Rotation;
    return (rotation2.HasValue ? (Angle.op_Equality(rotation1, rotation2.GetValueOrDefault()) ? 1 : 0) : 0) != 0 && Vector2i.op_Equality(this.Position, other.Value.Position);
  }

  public ItemStorageLocation()
  {
    this.Direction = (Direction) 0;
    this.Position = new Vector2i();
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ItemStorageLocation target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<ItemStorageLocation>(this, ref target, hookCtx, false, context))
      return;
    Direction target1 = (Direction) 0;
    if (!serialization.TryCustomCopy<Direction>(this.Direction, ref target1, hookCtx, false, context))
      target1 = this.Direction;
    Vector2i target2 = new Vector2i();
    if (!serialization.TryCustomCopy<Vector2i>(this.Position, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<Vector2i>(this.Position, hookCtx, context);
    target = target with
    {
      Direction = target1,
      Position = target2
    };
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ItemStorageLocation target,
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
    ItemStorageLocation target1 = (ItemStorageLocation) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public ItemStorageLocation Instantiate() => new ItemStorageLocation();

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return EqualityComparer<Direction>.Default.GetHashCode(this.Direction) * -1521134295 + EqualityComparer<Vector2i>.Default.GetHashCode(this.Position);
  }

  [CompilerGenerated]
  public readonly bool Equals(ItemStorageLocation other)
  {
    return EqualityComparer<Direction>.Default.Equals(this.Direction, other.Direction) && EqualityComparer<Vector2i>.Default.Equals(this.Position, other.Position);
  }
}
