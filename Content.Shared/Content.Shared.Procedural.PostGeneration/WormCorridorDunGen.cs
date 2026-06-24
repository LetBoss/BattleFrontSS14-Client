using System;
using Content.Shared.Maps;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Procedural.PostGeneration;

public sealed class WormCorridorDunGen : IDunGenLayer, ISerializationGenerated<IDunGenLayer>, ISerializationGenerated, ISerializationGenerated<WormCorridorDunGen>
{
	[DataField(null, false, 1, false, false, null)]
	public int PathLimit = 2048;

	[DataField(null, false, 1, false, false, null)]
	public int Count = 20;

	[DataField(null, false, 1, false, false, null)]
	public int Length = 20;

	[DataField(null, false, 1, false, false, null)]
	public Angle MaxAngleChange = Angle.FromDegrees(45.0);

	[DataField(null, false, 1, false, false, null)]
	public float Width = 3f;

	[DataField(null, false, 1, true, false, null)]
	public ProtoId<ContentTileDefinition> Tile;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref WormCorridorDunGen target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<WormCorridorDunGen>(this, ref target, hookCtx, false, context))
		{
			int PathLimitTemp = 0;
			if (!serialization.TryCustomCopy<int>(PathLimit, ref PathLimitTemp, hookCtx, false, context))
			{
				PathLimitTemp = PathLimit;
			}
			target.PathLimit = PathLimitTemp;
			int CountTemp = 0;
			if (!serialization.TryCustomCopy<int>(Count, ref CountTemp, hookCtx, false, context))
			{
				CountTemp = Count;
			}
			target.Count = CountTemp;
			int LengthTemp = 0;
			if (!serialization.TryCustomCopy<int>(Length, ref LengthTemp, hookCtx, false, context))
			{
				LengthTemp = Length;
			}
			target.Length = LengthTemp;
			Angle MaxAngleChangeTemp = default(Angle);
			if (!serialization.TryCustomCopy<Angle>(MaxAngleChange, ref MaxAngleChangeTemp, hookCtx, false, context))
			{
				MaxAngleChangeTemp = serialization.CreateCopy<Angle>(MaxAngleChange, hookCtx, context, false);
			}
			target.MaxAngleChange = MaxAngleChangeTemp;
			float WidthTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Width, ref WidthTemp, hookCtx, false, context))
			{
				WidthTemp = Width;
			}
			target.Width = WidthTemp;
			ProtoId<ContentTileDefinition> TileTemp = default(ProtoId<ContentTileDefinition>);
			if (!serialization.TryCustomCopy<ProtoId<ContentTileDefinition>>(Tile, ref TileTemp, hookCtx, false, context))
			{
				TileTemp = serialization.CreateCopy<ProtoId<ContentTileDefinition>>(Tile, hookCtx, context, false);
			}
			target.Tile = TileTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref WormCorridorDunGen target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		WormCorridorDunGen cast = (WormCorridorDunGen)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref IDunGenLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		WormCorridorDunGen def = (WormCorridorDunGen)target;
		Copy(ref def, serialization, hookCtx, context);
		target = def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref IDunGenLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public WormCorridorDunGen Instantiate()
	{
		return new WormCorridorDunGen();
	}

	IDunGenLayer IDunGenLayer.Instantiate()
	{
		return Instantiate();
	}

	IDunGenLayer ISerializationGenerated<IDunGenLayer>.Instantiate()
	{
		return Instantiate();
	}
}
