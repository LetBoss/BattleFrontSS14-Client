using System;
using System.Collections.Generic;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Humanoid.Markings;

[Serializable]
[DataDefinition]
[NetSerializable]
public sealed class MarkingPoints : ISerializationGenerated<MarkingPoints>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public int Points;

	[DataField(null, false, 1, true, false, null)]
	public bool Required;

	[DataField(null, false, 1, false, false, null)]
	public bool OnlyWhitelisted;

	[DataField(null, false, 1, false, false, null)]
	public List<ProtoId<MarkingPrototype>> DefaultMarkings = new List<ProtoId<MarkingPrototype>>();

	public static Dictionary<MarkingCategories, MarkingPoints> CloneMarkingPointDictionary(Dictionary<MarkingCategories, MarkingPoints> self)
	{
		Dictionary<MarkingCategories, MarkingPoints> clone = new Dictionary<MarkingCategories, MarkingPoints>();
		foreach (KeyValuePair<MarkingCategories, MarkingPoints> item in self)
		{
			item.Deconstruct(out var key, out var value);
			MarkingCategories category = key;
			MarkingPoints points = value;
			clone[category] = new MarkingPoints
			{
				Points = points.Points,
				Required = points.Required,
				OnlyWhitelisted = points.OnlyWhitelisted,
				DefaultMarkings = points.DefaultMarkings
			};
		}
		return clone;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref MarkingPoints target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<MarkingPoints>(this, ref target, hookCtx, false, context))
		{
			int PointsTemp = 0;
			if (!serialization.TryCustomCopy<int>(Points, ref PointsTemp, hookCtx, false, context))
			{
				PointsTemp = Points;
			}
			target.Points = PointsTemp;
			bool RequiredTemp = false;
			if (!serialization.TryCustomCopy<bool>(Required, ref RequiredTemp, hookCtx, false, context))
			{
				RequiredTemp = Required;
			}
			target.Required = RequiredTemp;
			bool OnlyWhitelistedTemp = false;
			if (!serialization.TryCustomCopy<bool>(OnlyWhitelisted, ref OnlyWhitelistedTemp, hookCtx, false, context))
			{
				OnlyWhitelistedTemp = OnlyWhitelisted;
			}
			target.OnlyWhitelisted = OnlyWhitelistedTemp;
			List<ProtoId<MarkingPrototype>> DefaultMarkingsTemp = null;
			if (DefaultMarkings == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<ProtoId<MarkingPrototype>>>(DefaultMarkings, ref DefaultMarkingsTemp, hookCtx, true, context))
			{
				DefaultMarkingsTemp = serialization.CreateCopy<List<ProtoId<MarkingPrototype>>>(DefaultMarkings, hookCtx, context, false);
			}
			target.DefaultMarkings = DefaultMarkingsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref MarkingPoints target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MarkingPoints cast = (MarkingPoints)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public MarkingPoints Instantiate()
	{
		return new MarkingPoints();
	}
}
