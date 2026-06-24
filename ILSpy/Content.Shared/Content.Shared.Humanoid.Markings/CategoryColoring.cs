using System;
using System.Collections.Generic;
using System.Linq;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Humanoid.Markings;

public sealed class CategoryColoring : LayerColoringType, ISerializationGenerated<CategoryColoring>, ISerializationGenerated
{
	[DataField("category", false, 1, true, false, null)]
	public MarkingCategories Category;

	public override Color? GetCleanColor(Color? skin, Color? eyes, MarkingSet markingSet)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		Color? outColor = null;
		if (markingSet.TryGetCategory(Category, out IReadOnlyList<Marking> markings) && markings.Count > 0)
		{
			outColor = markings[0].MarkingColors.FirstOrDefault();
		}
		return outColor;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref CategoryColoring target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		LayerColoringType definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (CategoryColoring)definitionCast;
		if (!serialization.TryCustomCopy<CategoryColoring>(this, ref target, hookCtx, false, context))
		{
			MarkingCategories CategoryTemp = MarkingCategories.Special;
			if (!serialization.TryCustomCopy<MarkingCategories>(Category, ref CategoryTemp, hookCtx, false, context))
			{
				CategoryTemp = Category;
			}
			target.Category = CategoryTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref CategoryColoring target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref LayerColoringType target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CategoryColoring cast = (CategoryColoring)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CategoryColoring cast = (CategoryColoring)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override CategoryColoring Instantiate()
	{
		return new CategoryColoring();
	}
}
