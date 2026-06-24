using System;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Humanoid.Markings;

[ImplicitDataDefinitionForInheritors]
public abstract class LayerColoringType : ISerializationGenerated<LayerColoringType>, ISerializationGenerated
{
	[DataField("negative", false, 1, false, false, null)]
	public bool Negative { get; private set; }

	public abstract Color? GetCleanColor(Color? skin, Color? eyes, MarkingSet markingSet);

	public Color? GetColor(Color? skin, Color? eyes, MarkingSet markingSet)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		Color? color = GetCleanColor(skin, eyes, markingSet);
		if (color.HasValue && Negative)
		{
			Color rcolor = color.Value;
			rcolor.R = 1f - rcolor.R;
			rcolor.G = 1f - rcolor.G;
			rcolor.B = 1f - rcolor.B;
			return rcolor;
		}
		return color;
	}

	public LayerColoringType()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref LayerColoringType target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		if (!serialization.TryCustomCopy<LayerColoringType>(this, ref target, hookCtx, false, context))
		{
			bool NegativeTemp = false;
			if (!serialization.TryCustomCopy<bool>(Negative, ref NegativeTemp, hookCtx, false, context))
			{
				NegativeTemp = Negative;
			}
			target.Negative = NegativeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref LayerColoringType target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		LayerColoringType cast = (LayerColoringType)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public virtual LayerColoringType Instantiate()
	{
		throw new NotImplementedException();
	}
}
