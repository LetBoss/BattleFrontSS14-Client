using System;
using System.Numerics;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Decals;

[Serializable]
[NetSerializable]
[DataDefinition]
public sealed class Decal : ISerializationGenerated<Decal>, ISerializationGenerated
{
	[DataField("coordinates", false, 1, false, false, null)]
	public Vector2 Coordinates = Vector2.Zero;

	[DataField("id", false, 1, false, false, null)]
	public string Id = string.Empty;

	[DataField("color", false, 1, false, false, null)]
	public Color? Color;

	[DataField("angle", false, 1, false, false, null)]
	public Angle Angle = Angle.Zero;

	[DataField("zIndex", false, 1, false, false, null)]
	public int ZIndex;

	[DataField("cleanable", false, 1, false, false, null)]
	public bool Cleanable;

	public Decal()
	{
	}//IL_0017: Unknown result type (might be due to invalid IL or missing references)
	//IL_001c: Unknown result type (might be due to invalid IL or missing references)


	public Decal(Vector2 coordinates, string id, Color? color, Angle angle, int zIndex, bool cleanable)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		Coordinates = coordinates;
		Id = id;
		Color = color;
		Angle = angle;
		ZIndex = zIndex;
		Cleanable = cleanable;
	}

	public Decal WithCoordinates(Vector2 coordinates)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		return new Decal(coordinates, Id, Color, Angle, ZIndex, Cleanable);
	}

	public Decal WithId(string id)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		return new Decal(Coordinates, id, Color, Angle, ZIndex, Cleanable);
	}

	public Decal WithColor(Color? color)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		return new Decal(Coordinates, Id, color, Angle, ZIndex, Cleanable);
	}

	public Decal WithRotation(Angle angle)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		return new Decal(Coordinates, Id, Color, angle, ZIndex, Cleanable);
	}

	public Decal WithZIndex(int zIndex)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		return new Decal(Coordinates, Id, Color, Angle, zIndex, Cleanable);
	}

	public Decal WithCleanable(bool cleanable)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		return new Decal(Coordinates, Id, Color, Angle, ZIndex, cleanable);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref Decal target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<Decal>(this, ref target, hookCtx, false, context))
		{
			Vector2 CoordinatesTemp = default(Vector2);
			if (!serialization.TryCustomCopy<Vector2>(Coordinates, ref CoordinatesTemp, hookCtx, false, context))
			{
				CoordinatesTemp = serialization.CreateCopy<Vector2>(Coordinates, hookCtx, context, false);
			}
			target.Coordinates = CoordinatesTemp;
			string IdTemp = null;
			if (Id == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Id, ref IdTemp, hookCtx, false, context))
			{
				IdTemp = Id;
			}
			target.Id = IdTemp;
			Color? ColorTemp = null;
			if (!serialization.TryCustomCopy<Color?>(Color, ref ColorTemp, hookCtx, false, context))
			{
				ColorTemp = serialization.CreateCopy<Color?>(Color, hookCtx, context, false);
			}
			target.Color = ColorTemp;
			Angle AngleTemp = default(Angle);
			if (!serialization.TryCustomCopy<Angle>(Angle, ref AngleTemp, hookCtx, false, context))
			{
				AngleTemp = serialization.CreateCopy<Angle>(Angle, hookCtx, context, false);
			}
			target.Angle = AngleTemp;
			int ZIndexTemp = 0;
			if (!serialization.TryCustomCopy<int>(ZIndex, ref ZIndexTemp, hookCtx, false, context))
			{
				ZIndexTemp = ZIndex;
			}
			target.ZIndex = ZIndexTemp;
			bool CleanableTemp = false;
			if (!serialization.TryCustomCopy<bool>(Cleanable, ref CleanableTemp, hookCtx, false, context))
			{
				CleanableTemp = Cleanable;
			}
			target.Cleanable = CleanableTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref Decal target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Decal cast = (Decal)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public Decal Instantiate()
	{
		return new Decal();
	}
}
