using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Robust.Shared.Map;

[DataRecord]
public readonly record struct EntityCoordinates : ISpanFormattable, IFormattable
{
	public float X => Position.X;

	public float Y => Position.Y;

	public static readonly EntityCoordinates Invalid = new EntityCoordinates(EntityUid.Invalid, Vector2.Zero);

	public readonly EntityUid EntityId;

	public readonly Vector2 Position;

	public EntityCoordinates()
	{
		EntityId = EntityUid.Invalid;
		Position = Vector2.Zero;
	}

	public EntityCoordinates(EntityUid entityId, Vector2 position)
	{
		EntityId = entityId;
		Position = position;
	}

	public EntityCoordinates(EntityUid entityId, float x, float y)
	{
		EntityId = entityId;
		Position = new Vector2(x, y);
	}

	public bool IsValid(IEntityManager entityManager)
	{
		if (!EntityId.IsValid() || !entityManager.EntityExists(EntityId))
		{
			return false;
		}
		if (!float.IsFinite(Position.X) || !float.IsFinite(Position.Y))
		{
			return false;
		}
		return true;
	}

	[Obsolete("Use SharedTransformSystem.ToMapCoordinates()")]
	public MapCoordinates ToMap(IEntityManager entityManager, SharedTransformSystem transformSystem)
	{
		return transformSystem.ToMapCoordinates(this);
	}

	[Obsolete("Use SharedTransformSystem.ToMapCoordinates()")]
	public Vector2 ToMapPos(IEntityManager entityManager, SharedTransformSystem transformSystem)
	{
		return ToMap(entityManager, transformSystem).Position;
	}

	[Obsolete("Use SharedTransformSystem.ToCoordinates()")]
	public static EntityCoordinates FromMap(EntityUid entity, MapCoordinates coordinates, SharedTransformSystem transformSystem, IEntityManager? entMan = null)
	{
		return transformSystem.ToCoordinates(entity, coordinates);
	}

	[Obsolete("Use SharedTransformSystem.ToCoordinates()")]
	public static EntityCoordinates FromMap(IMapManager mapManager, MapCoordinates coordinates)
	{
		return IoCManager.Resolve<IEntityManager>().System<SharedTransformSystem>().ToCoordinates(coordinates);
	}

	public Vector2i ToVector2i(IEntityManager entityManager, IMapManager mapManager, SharedTransformSystem transformSystem)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (!IsValid(entityManager))
		{
			return default(Vector2i);
		}
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
		return new Vector2i((int)MathF.Floor(mapCoordinates.X), (int)MathF.Floor(mapCoordinates.Y));
	}

	public EntityCoordinates WithPosition(Vector2 newPosition)
	{
		return new EntityCoordinates(EntityId, newPosition);
	}

	[Obsolete("Use SharedTransformSystem.WithEntityId()")]
	public EntityCoordinates WithEntityId(IEntityManager entityManager, EntityUid entityId)
	{
		if (!entityManager.EntityExists(entityId))
		{
			return new EntityCoordinates(entityId, Vector2.Zero);
		}
		return WithEntityId(entityId);
	}

	[Obsolete("Use SharedTransformSystem.WithEntityId()")]
	public EntityCoordinates WithEntityId(EntityUid entity, IEntityManager? entMan = null)
	{
		IoCManager.Resolve(ref entMan);
		return WithEntityId(entity, entMan.System<SharedTransformSystem>(), entMan);
	}

	[Obsolete("Use SharedTransformSystem.WithEntityId()")]
	public EntityCoordinates WithEntityId(EntityUid entity, SharedTransformSystem transformSystem, IEntityManager? entMan = null)
	{
		return transformSystem.WithEntityId(this, entity);
	}

	[Obsolete("Use SharedTransformSystem.GetGrid()")]
	public EntityUid? GetGridUid(IEntityManager entityManager)
	{
		if (IsValid(entityManager))
		{
			return entityManager.GetComponent<TransformComponent>(EntityId).GridUid;
		}
		return null;
	}

	[Obsolete("Use SharedTransformSystem.GetMapId()")]
	public MapId GetMapId(IEntityManager entityManager)
	{
		if (IsValid(entityManager))
		{
			return entityManager.GetComponent<TransformComponent>(EntityId).MapID;
		}
		return MapId.Nullspace;
	}

	[Obsolete("Use SharedTransformSystem.GetMap()")]
	public EntityUid? GetMapUid(IEntityManager entityManager)
	{
		if (IsValid(entityManager))
		{
			return entityManager.GetComponent<TransformComponent>(EntityId).MapUid;
		}
		return null;
	}

	public EntityCoordinates Offset(Vector2 position)
	{
		return new EntityCoordinates(EntityId, Position + position);
	}

	[Obsolete("Use TransformSystem.InRange()")]
	public bool InRange(IEntityManager entityManager, EntityCoordinates otherCoordinates, float range)
	{
		return InRange(entityManager, entityManager.System<SharedTransformSystem>(), otherCoordinates, range);
	}

	[Obsolete("Use TransformSystem.InRange()")]
	public bool InRange(IEntityManager entityManager, SharedTransformSystem transformSystem, EntityCoordinates otherCoordinates, float range)
	{
		return transformSystem.InRange(this, otherCoordinates, range);
	}

	public bool TryDistance(IEntityManager entityManager, EntityCoordinates otherCoordinates, out float distance)
	{
		return TryDistance(entityManager, entityManager.System<SharedTransformSystem>(), otherCoordinates, out distance);
	}

	public bool TryDistance(IEntityManager entityManager, SharedTransformSystem transformSystem, EntityCoordinates otherCoordinates, out float distance)
	{
		if (TryDelta(entityManager, transformSystem, otherCoordinates, out var delta))
		{
			distance = delta.Length();
			return true;
		}
		distance = 0f;
		return false;
	}

	public bool TryDelta(IEntityManager entityManager, SharedTransformSystem transformSystem, EntityCoordinates otherCoordinates, out Vector2 delta)
	{
		delta = Vector2.Zero;
		if (!IsValid(entityManager) || !otherCoordinates.IsValid(entityManager))
		{
			return false;
		}
		if (EntityId == otherCoordinates.EntityId)
		{
			delta = Position - otherCoordinates.Position;
			return true;
		}
		MapCoordinates mapCoordinates = transformSystem.ToMapCoordinates(this);
		MapCoordinates mapCoordinates2 = transformSystem.ToMapCoordinates(otherCoordinates);
		if (mapCoordinates.MapId != mapCoordinates2.MapId)
		{
			return false;
		}
		delta = mapCoordinates.Position - mapCoordinates2.Position;
		return true;
	}

	public static EntityCoordinates operator +(EntityCoordinates left, EntityCoordinates right)
	{
		if (left.EntityId != right.EntityId)
		{
			throw new ArgumentException("Can't sum EntityCoordinates with different relative entities.");
		}
		return new EntityCoordinates(left.EntityId, left.Position + right.Position);
	}

	public static EntityCoordinates operator -(EntityCoordinates left, EntityCoordinates right)
	{
		if (left.EntityId != right.EntityId)
		{
			throw new ArgumentException("Can't subtract EntityCoordinates with different relative entities.");
		}
		return new EntityCoordinates(left.EntityId, left.Position - right.Position);
	}

	public static EntityCoordinates operator *(EntityCoordinates left, EntityCoordinates right)
	{
		if (left.EntityId != right.EntityId)
		{
			throw new ArgumentException("Can't multiply EntityCoordinates with different relative entities.");
		}
		return new EntityCoordinates(left.EntityId, left.Position * right.Position);
	}

	public static EntityCoordinates operator *(EntityCoordinates left, float right)
	{
		return new EntityCoordinates(left.EntityId, left.Position * right);
	}

	public static EntityCoordinates operator *(EntityCoordinates left, int right)
	{
		return new EntityCoordinates(left.EntityId, left.Position * right);
	}

	public void Deconstruct(out EntityUid entId, out Vector2 localPos)
	{
		entId = EntityId;
		localPos = Position;
	}

	public override string ToString()
	{
		return $"EntId={EntityId}, X={Position.X:N2}, Y={Position.Y:N2}";
	}

	public string ToString(string? format, IFormatProvider? formatProvider)
	{
		return ToString();
	}

	public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
	{
		Unsafe.SkipInit(out BufferInterpolatedStringHandler val);
		((BufferInterpolatedStringHandler)(ref val))._002Ector(14, 3, destination);
		((BufferInterpolatedStringHandler)(ref val)).AppendLiteral("EntId=");
		((BufferInterpolatedStringHandler)(ref val)).AppendFormatted<EntityUid>(EntityId);
		((BufferInterpolatedStringHandler)(ref val)).AppendLiteral(", X=");
		((BufferInterpolatedStringHandler)(ref val)).AppendFormatted<float>(Position.X, "N2");
		((BufferInterpolatedStringHandler)(ref val)).AppendLiteral(", Y=");
		((BufferInterpolatedStringHandler)(ref val)).AppendFormatted<float>(Position.Y, "N2");
		return FormatHelpers.TryFormatInto(destination, ref charsWritten, ref val);
	}

	[CompilerGenerated]
	private bool PrintMembers(StringBuilder builder)
	{
		builder.Append("EntityId = ");
		builder.Append(EntityId.ToString());
		builder.Append(", Position = ");
		builder.Append(Position.ToString());
		builder.Append(", X = ");
		builder.Append(X.ToString());
		builder.Append(", Y = ");
		builder.Append(Y.ToString());
		return true;
	}
}
