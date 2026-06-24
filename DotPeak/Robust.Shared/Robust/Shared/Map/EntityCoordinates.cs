// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Map.EntityCoordinates
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.Map;

[DataRecord]
public readonly record struct EntityCoordinates : ISpanFormattable, IFormattable
{
  public static readonly EntityCoordinates Invalid = new EntityCoordinates(EntityUid.Invalid, Vector2.Zero);
  public readonly EntityUid EntityId;
  public readonly Vector2 Position;

  public float X => this.Position.X;

  public float Y => this.Position.Y;

  public EntityCoordinates()
  {
    this.EntityId = EntityUid.Invalid;
    this.Position = Vector2.Zero;
  }

  public EntityCoordinates(EntityUid entityId, Vector2 position)
  {
    this.EntityId = entityId;
    this.Position = position;
  }

  public EntityCoordinates(EntityUid entityId, float x, float y)
  {
    this.EntityId = entityId;
    this.Position = new Vector2(x, y);
  }

  public bool IsValid(IEntityManager entityManager)
  {
    return this.EntityId.IsValid() && entityManager.EntityExists(this.EntityId) && float.IsFinite(this.Position.X) && float.IsFinite(this.Position.Y);
  }

  [Obsolete("Use SharedTransformSystem.ToMapCoordinates()")]
  public MapCoordinates ToMap(IEntityManager entityManager, SharedTransformSystem transformSystem)
  {
    return transformSystem.ToMapCoordinates(this);
  }

  [Obsolete("Use SharedTransformSystem.ToMapCoordinates()")]
  public Vector2 ToMapPos(IEntityManager entityManager, SharedTransformSystem transformSystem)
  {
    return this.ToMap(entityManager, transformSystem).Position;
  }

  [Obsolete("Use SharedTransformSystem.ToCoordinates()")]
  public static EntityCoordinates FromMap(
    EntityUid entity,
    MapCoordinates coordinates,
    SharedTransformSystem transformSystem,
    IEntityManager? entMan = null)
  {
    return transformSystem.ToCoordinates((Entity<TransformComponent>) entity, coordinates);
  }

  [Obsolete("Use SharedTransformSystem.ToCoordinates()")]
  public static EntityCoordinates FromMap(IMapManager mapManager, MapCoordinates coordinates)
  {
    return IoCManager.Resolve<IEntityManager>().System<SharedTransformSystem>().ToCoordinates(coordinates);
  }

  public Vector2i ToVector2i(
    IEntityManager entityManager,
    IMapManager mapManager,
    SharedTransformSystem transformSystem)
  {
    if (!this.IsValid(entityManager))
      return new Vector2i();
    SharedMapSystem sharedMapSystem = entityManager.System<SharedMapSystem>();
    EntityUid? grid = transformSystem.GetGrid(this);
    if (grid.HasValue)
    {
      EntityUid valueOrDefault = grid.GetValueOrDefault();
      if (valueOrDefault.IsValid())
      {
        MapGridComponent component = entityManager.GetComponent<MapGridComponent>(valueOrDefault);
        return sharedMapSystem.GetTileRef(valueOrDefault, component, this).GridIndices;
      }
    }
    MapCoordinates mapCoordinates = transformSystem.ToMapCoordinates(this);
    return new Vector2i((int) MathF.Floor(mapCoordinates.X), (int) MathF.Floor(mapCoordinates.Y));
  }

  public EntityCoordinates WithPosition(Vector2 newPosition)
  {
    return new EntityCoordinates(this.EntityId, newPosition);
  }

  [Obsolete("Use SharedTransformSystem.WithEntityId()")]
  public EntityCoordinates WithEntityId(IEntityManager entityManager, EntityUid entityId)
  {
    return !entityManager.EntityExists(entityId) ? new EntityCoordinates(entityId, Vector2.Zero) : this.WithEntityId(entityId);
  }

  [Obsolete("Use SharedTransformSystem.WithEntityId()")]
  public EntityCoordinates WithEntityId(EntityUid entity, IEntityManager? entMan = null)
  {
    IoCManager.Resolve<IEntityManager>(ref entMan);
    return this.WithEntityId(entity, entMan.System<SharedTransformSystem>(), entMan);
  }

  [Obsolete("Use SharedTransformSystem.WithEntityId()")]
  public EntityCoordinates WithEntityId(
    EntityUid entity,
    SharedTransformSystem transformSystem,
    IEntityManager? entMan = null)
  {
    return transformSystem.WithEntityId(this, entity);
  }

  [Obsolete("Use SharedTransformSystem.GetGrid()")]
  public EntityUid? GetGridUid(IEntityManager entityManager)
  {
    return this.IsValid(entityManager) ? entityManager.GetComponent<TransformComponent>(this.EntityId).GridUid : new EntityUid?();
  }

  [Obsolete("Use SharedTransformSystem.GetMapId()")]
  public MapId GetMapId(IEntityManager entityManager)
  {
    return this.IsValid(entityManager) ? entityManager.GetComponent<TransformComponent>(this.EntityId).MapID : MapId.Nullspace;
  }

  [Obsolete("Use SharedTransformSystem.GetMap()")]
  public EntityUid? GetMapUid(IEntityManager entityManager)
  {
    return this.IsValid(entityManager) ? entityManager.GetComponent<TransformComponent>(this.EntityId).MapUid : new EntityUid?();
  }

  public EntityCoordinates Offset(Vector2 position)
  {
    return new EntityCoordinates(this.EntityId, this.Position + position);
  }

  [Obsolete("Use TransformSystem.InRange()")]
  public bool InRange(
    IEntityManager entityManager,
    EntityCoordinates otherCoordinates,
    float range)
  {
    return this.InRange(entityManager, entityManager.System<SharedTransformSystem>(), otherCoordinates, range);
  }

  [Obsolete("Use TransformSystem.InRange()")]
  public bool InRange(
    IEntityManager entityManager,
    SharedTransformSystem transformSystem,
    EntityCoordinates otherCoordinates,
    float range)
  {
    return transformSystem.InRange(this, otherCoordinates, range);
  }

  public bool TryDistance(
    IEntityManager entityManager,
    EntityCoordinates otherCoordinates,
    out float distance)
  {
    return this.TryDistance(entityManager, entityManager.System<SharedTransformSystem>(), otherCoordinates, out distance);
  }

  public bool TryDistance(
    IEntityManager entityManager,
    SharedTransformSystem transformSystem,
    EntityCoordinates otherCoordinates,
    out float distance)
  {
    Vector2 delta;
    if (this.TryDelta(entityManager, transformSystem, otherCoordinates, out delta))
    {
      distance = delta.Length();
      return true;
    }
    distance = 0.0f;
    return false;
  }

  public bool TryDelta(
    IEntityManager entityManager,
    SharedTransformSystem transformSystem,
    EntityCoordinates otherCoordinates,
    out Vector2 delta)
  {
    delta = Vector2.Zero;
    if (!this.IsValid(entityManager) || !otherCoordinates.IsValid(entityManager))
      return false;
    if (this.EntityId == otherCoordinates.EntityId)
    {
      delta = this.Position - otherCoordinates.Position;
      return true;
    }
    MapCoordinates mapCoordinates1 = transformSystem.ToMapCoordinates(this);
    MapCoordinates mapCoordinates2 = transformSystem.ToMapCoordinates(otherCoordinates);
    if (mapCoordinates1.MapId != mapCoordinates2.MapId)
      return false;
    delta = mapCoordinates1.Position - mapCoordinates2.Position;
    return true;
  }

  public static EntityCoordinates operator +(EntityCoordinates left, EntityCoordinates right)
  {
    if (left.EntityId != right.EntityId)
      throw new ArgumentException("Can't sum EntityCoordinates with different relative entities.");
    return new EntityCoordinates(left.EntityId, left.Position + right.Position);
  }

  public static EntityCoordinates operator -(EntityCoordinates left, EntityCoordinates right)
  {
    if (left.EntityId != right.EntityId)
      throw new ArgumentException("Can't subtract EntityCoordinates with different relative entities.");
    return new EntityCoordinates(left.EntityId, left.Position - right.Position);
  }

  public static EntityCoordinates operator *(EntityCoordinates left, EntityCoordinates right)
  {
    if (left.EntityId != right.EntityId)
      throw new ArgumentException("Can't multiply EntityCoordinates with different relative entities.");
    return new EntityCoordinates(left.EntityId, left.Position * right.Position);
  }

  public static EntityCoordinates operator *(EntityCoordinates left, float right)
  {
    return new EntityCoordinates(left.EntityId, left.Position * right);
  }

  public static EntityCoordinates operator *(EntityCoordinates left, int right)
  {
    return new EntityCoordinates(left.EntityId, left.Position * (float) right);
  }

  public void Deconstruct(out EntityUid entId, out Vector2 localPos)
  {
    entId = this.EntityId;
    localPos = this.Position;
  }

  public override string ToString()
  {
    return $"EntId={this.EntityId}, X={this.Position.X:N2}, Y={this.Position.Y:N2}";
  }

  public string ToString(string? format, IFormatProvider? formatProvider) => this.ToString();

  public bool TryFormat(
    Span<char> destination,
    out int charsWritten,
    ReadOnlySpan<char> format,
    IFormatProvider? provider)
  {
    Span<char> span1 = destination;
    Span<char> span2 = span1;
    ref int local1 = ref charsWritten;
    BufferInterpolatedStringHandler interpolatedStringHandler;
    // ISSUE: explicit constructor call
    ((BufferInterpolatedStringHandler) ref interpolatedStringHandler).\u002Ector(14, 3, span1);
    ((BufferInterpolatedStringHandler) ref interpolatedStringHandler).AppendLiteral("EntId=");
    ((BufferInterpolatedStringHandler) ref interpolatedStringHandler).AppendFormatted<EntityUid>(this.EntityId);
    ((BufferInterpolatedStringHandler) ref interpolatedStringHandler).AppendLiteral(", X=");
    ((BufferInterpolatedStringHandler) ref interpolatedStringHandler).AppendFormatted<float>(this.Position.X, "N2");
    ((BufferInterpolatedStringHandler) ref interpolatedStringHandler).AppendLiteral(", Y=");
    ((BufferInterpolatedStringHandler) ref interpolatedStringHandler).AppendFormatted<float>(this.Position.Y, "N2");
    ref BufferInterpolatedStringHandler local2 = ref interpolatedStringHandler;
    return FormatHelpers.TryFormatInto(span2, ref local1, ref local2);
  }

  [CompilerGenerated]
  public override int GetHashCode()
  {
    return EqualityComparer<EntityUid>.Default.GetHashCode(this.EntityId) * -1521134295 + EqualityComparer<Vector2>.Default.GetHashCode(this.Position);
  }

  [CompilerGenerated]
  public bool Equals(EntityCoordinates other)
  {
    return EqualityComparer<EntityUid>.Default.Equals(this.EntityId, other.EntityId) && EqualityComparer<Vector2>.Default.Equals(this.Position, other.Position);
  }
}
