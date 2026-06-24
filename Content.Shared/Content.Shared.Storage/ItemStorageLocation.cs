using System;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Storage;

[Serializable]
[DataDefinition]
[NetSerializable]
public record struct ItemStorageLocation : ISerializationGenerated<ItemStorageLocation>, ISerializationGenerated
{
	public Angle Rotation
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return DirectionExtensions.ToAngle(Direction);
		}
		set
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			Direction = ((Angle)(ref value)).GetCardinalDir();
		}
	}

	[DataField("_rotation", false, 1, false, false, null)]
	public Direction Direction;

	[DataField(null, false, 1, false, false, null)]
	public Vector2i Position;

	public ItemStorageLocation(Angle rotation, Vector2i position)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		Direction = (Direction)0;
		Position = default(Vector2i);
		Rotation = rotation;
		Position = position;
	}

	public bool Equals(ItemStorageLocation? other)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		Angle rotation = Rotation;
		Angle? val = other?.Rotation;
		if (val.HasValue && rotation == val.GetValueOrDefault())
		{
			return Position == other.Value.Position;
		}
		return false;
	}

	public ItemStorageLocation()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		Direction = (Direction)0;
		Position = default(Vector2i);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ItemStorageLocation target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<ItemStorageLocation>(this, ref target, hookCtx, false, context))
		{
			Direction DirectionTemp = (Direction)0;
			if (!serialization.TryCustomCopy<Direction>(Direction, ref DirectionTemp, hookCtx, false, context))
			{
				DirectionTemp = Direction;
			}
			Vector2i PositionTemp = default(Vector2i);
			if (!serialization.TryCustomCopy<Vector2i>(Position, ref PositionTemp, hookCtx, false, context))
			{
				PositionTemp = serialization.CreateCopy<Vector2i>(Position, hookCtx, context, false);
			}
			target = target with
			{
				Direction = DirectionTemp,
				Position = PositionTemp
			};
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ItemStorageLocation target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ItemStorageLocation cast = (ItemStorageLocation)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public ItemStorageLocation Instantiate()
	{
		return new ItemStorageLocation();
	}

	[CompilerGenerated]
	private bool PrintMembers(StringBuilder builder)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		builder.Append("Direction = ");
		builder.Append(((object)Unsafe.As<Direction, Direction>(ref Direction)/*cast due to constrained. prefix*/).ToString());
		builder.Append(", Rotation = ");
		builder.Append(((object)Rotation/*cast due to constrained. prefix*/).ToString());
		builder.Append(", Position = ");
		builder.Append(((object)Unsafe.As<Vector2i, Vector2i>(ref Position)/*cast due to constrained. prefix*/).ToString());
		return true;
	}
}
