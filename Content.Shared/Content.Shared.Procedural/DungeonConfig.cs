using System;
using System.Collections.Generic;
using Robust.Shared.Analyzers;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Procedural;

[Virtual]
[DataDefinition]
public class DungeonConfig : ISerializationGenerated<DungeonConfig>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public List<IDunGenLayer> Layers = new List<IDunGenLayer>();

	[DataField(null, false, 1, false, false, null)]
	public bool ReserveTiles;

	[DataField(null, false, 1, false, false, null)]
	public int MinCount = 1;

	[DataField(null, false, 1, false, false, null)]
	public int MaxCount = 1;

	[DataField(null, false, 1, false, false, null)]
	public int MinOffset;

	[DataField(null, false, 1, false, false, null)]
	public int MaxOffset;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref DungeonConfig target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<DungeonConfig>(this, ref target, hookCtx, false, context))
		{
			List<IDunGenLayer> LayersTemp = null;
			if (Layers == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<IDunGenLayer>>(Layers, ref LayersTemp, hookCtx, true, context))
			{
				LayersTemp = serialization.CreateCopy<List<IDunGenLayer>>(Layers, hookCtx, context, false);
			}
			target.Layers = LayersTemp;
			bool ReserveTilesTemp = false;
			if (!serialization.TryCustomCopy<bool>(ReserveTiles, ref ReserveTilesTemp, hookCtx, false, context))
			{
				ReserveTilesTemp = ReserveTiles;
			}
			target.ReserveTiles = ReserveTilesTemp;
			int MinCountTemp = 0;
			if (!serialization.TryCustomCopy<int>(MinCount, ref MinCountTemp, hookCtx, false, context))
			{
				MinCountTemp = MinCount;
			}
			target.MinCount = MinCountTemp;
			int MaxCountTemp = 0;
			if (!serialization.TryCustomCopy<int>(MaxCount, ref MaxCountTemp, hookCtx, false, context))
			{
				MaxCountTemp = MaxCount;
			}
			target.MaxCount = MaxCountTemp;
			int MinOffsetTemp = 0;
			if (!serialization.TryCustomCopy<int>(MinOffset, ref MinOffsetTemp, hookCtx, false, context))
			{
				MinOffsetTemp = MinOffset;
			}
			target.MinOffset = MinOffsetTemp;
			int MaxOffsetTemp = 0;
			if (!serialization.TryCustomCopy<int>(MaxOffset, ref MaxOffsetTemp, hookCtx, false, context))
			{
				MaxOffsetTemp = MaxOffset;
			}
			target.MaxOffset = MaxOffsetTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref DungeonConfig target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DungeonConfig cast = (DungeonConfig)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public virtual DungeonConfig Instantiate()
	{
		return new DungeonConfig();
	}
}
