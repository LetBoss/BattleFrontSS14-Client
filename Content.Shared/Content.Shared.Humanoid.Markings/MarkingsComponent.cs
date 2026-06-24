using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Humanoid.Markings;

[RegisterComponent]
public sealed class MarkingsComponent : Component, ISerializationGenerated<MarkingsComponent>, ISerializationGenerated
{
	public Dictionary<HumanoidVisualLayers, List<Marking>> ActiveMarkings = new Dictionary<HumanoidVisualLayers, List<Marking>>();

	[DataField("layerPoints", false, 1, false, false, null)]
	public Dictionary<MarkingCategories, MarkingPoints> LayerPoints = new Dictionary<MarkingCategories, MarkingPoints>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref MarkingsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (MarkingsComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<MarkingsComponent>(this, ref target, hookCtx, false, context))
		{
			Dictionary<MarkingCategories, MarkingPoints> LayerPointsTemp = null;
			if (LayerPoints == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<MarkingCategories, MarkingPoints>>(LayerPoints, ref LayerPointsTemp, hookCtx, true, context))
			{
				LayerPointsTemp = serialization.CreateCopy<Dictionary<MarkingCategories, MarkingPoints>>(LayerPoints, hookCtx, context, false);
			}
			target.LayerPoints = LayerPointsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref MarkingsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MarkingsComponent cast = (MarkingsComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MarkingsComponent cast = (MarkingsComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MarkingsComponent def = (MarkingsComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override MarkingsComponent Instantiate()
	{
		return new MarkingsComponent();
	}
}
