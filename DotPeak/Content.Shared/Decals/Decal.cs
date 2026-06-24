// Decompiled with JetBrains decompiler
// Type: Content.Shared.Decals.Decal
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Numerics;

#nullable enable
namespace Content.Shared.Decals;

[NetSerializable]
[DataDefinition]
[Serializable]
public sealed class Decal : ISerializationGenerated<Decal>, ISerializationGenerated
{
  [DataField("coordinates", false, 1, false, false, null)]
  public Vector2 Coordinates = Vector2.Zero;
  [DataField("id", false, 1, false, false, null)]
  public string Id = string.Empty;
  [DataField("color", false, 1, false, false, null)]
  public Robust.Shared.Maths.Color? Color;
  [DataField("angle", false, 1, false, false, null)]
  public Angle Angle = Angle.Zero;
  [DataField("zIndex", false, 1, false, false, null)]
  public int ZIndex;
  [DataField("cleanable", false, 1, false, false, null)]
  public bool Cleanable;

  public Decal()
  {
  }

  public Decal(
    Vector2 coordinates,
    string id,
    Robust.Shared.Maths.Color? color,
    Angle angle,
    int zIndex,
    bool cleanable)
  {
    this.Coordinates = coordinates;
    this.Id = id;
    this.Color = color;
    this.Angle = angle;
    this.ZIndex = zIndex;
    this.Cleanable = cleanable;
  }

  public Decal WithCoordinates(Vector2 coordinates)
  {
    return new Decal(coordinates, this.Id, this.Color, this.Angle, this.ZIndex, this.Cleanable);
  }

  public Decal WithId(string id)
  {
    return new Decal(this.Coordinates, id, this.Color, this.Angle, this.ZIndex, this.Cleanable);
  }

  public Decal WithColor(Robust.Shared.Maths.Color? color)
  {
    return new Decal(this.Coordinates, this.Id, color, this.Angle, this.ZIndex, this.Cleanable);
  }

  public Decal WithRotation(Angle angle)
  {
    return new Decal(this.Coordinates, this.Id, this.Color, angle, this.ZIndex, this.Cleanable);
  }

  public Decal WithZIndex(int zIndex)
  {
    return new Decal(this.Coordinates, this.Id, this.Color, this.Angle, zIndex, this.Cleanable);
  }

  public Decal WithCleanable(bool cleanable)
  {
    return new Decal(this.Coordinates, this.Id, this.Color, this.Angle, this.ZIndex, cleanable);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref Decal target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<Decal>(this, ref target, hookCtx, false, context))
      return;
    Vector2 vector2 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.Coordinates, ref vector2, hookCtx, false, context))
      vector2 = serialization.CreateCopy<Vector2>(this.Coordinates, hookCtx, context, false);
    target.Coordinates = vector2;
    string str = (string) null;
    if (this.Id == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Id, ref str, hookCtx, false, context))
      str = this.Id;
    target.Id = str;
    Robust.Shared.Maths.Color? nullable = new Robust.Shared.Maths.Color?();
    if (!serialization.TryCustomCopy<Robust.Shared.Maths.Color?>(this.Color, ref nullable, hookCtx, false, context))
      nullable = serialization.CreateCopy<Robust.Shared.Maths.Color?>(this.Color, hookCtx, context, false);
    target.Color = nullable;
    Angle angle = new Angle();
    if (!serialization.TryCustomCopy<Angle>(this.Angle, ref angle, hookCtx, false, context))
      angle = serialization.CreateCopy<Angle>(this.Angle, hookCtx, context, false);
    target.Angle = angle;
    int num = 0;
    if (!serialization.TryCustomCopy<int>(this.ZIndex, ref num, hookCtx, false, context))
      num = this.ZIndex;
    target.ZIndex = num;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.Cleanable, ref flag, hookCtx, false, context))
      flag = this.Cleanable;
    target.Cleanable = flag;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref Decal target,
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
    Decal target1 = (Decal) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public Decal Instantiate() => new Decal();
}
