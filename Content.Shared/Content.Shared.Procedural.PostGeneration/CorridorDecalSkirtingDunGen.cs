using System;
using System.Collections.Generic;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Procedural.PostGeneration;

public sealed class CorridorDecalSkirtingDunGen : IDunGenLayer, ISerializationGenerated<IDunGenLayer>, ISerializationGenerated, ISerializationGenerated<CorridorDecalSkirtingDunGen>
{
	[DataField(null, false, 1, false, false, null)]
	public Dictionary<DirectionFlag, string> CardinalDecals = new Dictionary<DirectionFlag, string>();

	[DataField(null, false, 1, false, false, null)]
	public Dictionary<Direction, string> PocketDecals = new Dictionary<Direction, string>();

	[DataField(null, false, 1, false, false, null)]
	public Dictionary<DirectionFlag, string> CornerDecals = new Dictionary<DirectionFlag, string>();

	[DataField(null, false, 1, false, false, null)]
	public Color? Color;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref CorridorDecalSkirtingDunGen target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<CorridorDecalSkirtingDunGen>(this, ref target, hookCtx, false, context))
		{
			Dictionary<DirectionFlag, string> CardinalDecalsTemp = null;
			if (CardinalDecals == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<DirectionFlag, string>>(CardinalDecals, ref CardinalDecalsTemp, hookCtx, true, context))
			{
				CardinalDecalsTemp = serialization.CreateCopy<Dictionary<DirectionFlag, string>>(CardinalDecals, hookCtx, context, false);
			}
			target.CardinalDecals = CardinalDecalsTemp;
			Dictionary<Direction, string> PocketDecalsTemp = null;
			if (PocketDecals == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<Direction, string>>(PocketDecals, ref PocketDecalsTemp, hookCtx, true, context))
			{
				PocketDecalsTemp = serialization.CreateCopy<Dictionary<Direction, string>>(PocketDecals, hookCtx, context, false);
			}
			target.PocketDecals = PocketDecalsTemp;
			Dictionary<DirectionFlag, string> CornerDecalsTemp = null;
			if (CornerDecals == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<DirectionFlag, string>>(CornerDecals, ref CornerDecalsTemp, hookCtx, true, context))
			{
				CornerDecalsTemp = serialization.CreateCopy<Dictionary<DirectionFlag, string>>(CornerDecals, hookCtx, context, false);
			}
			target.CornerDecals = CornerDecalsTemp;
			Color? ColorTemp = null;
			if (!serialization.TryCustomCopy<Color?>(Color, ref ColorTemp, hookCtx, false, context))
			{
				ColorTemp = serialization.CreateCopy<Color?>(Color, hookCtx, context, false);
			}
			target.Color = ColorTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref CorridorDecalSkirtingDunGen target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CorridorDecalSkirtingDunGen cast = (CorridorDecalSkirtingDunGen)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref IDunGenLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CorridorDecalSkirtingDunGen def = (CorridorDecalSkirtingDunGen)target;
		Copy(ref def, serialization, hookCtx, context);
		target = def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref IDunGenLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public CorridorDecalSkirtingDunGen Instantiate()
	{
		return new CorridorDecalSkirtingDunGen();
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
