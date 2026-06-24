using System;
using System.Collections.Generic;
using Content.Shared.Storage;
using Content.Shared.Tools.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Tools.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(ToolRefinablSystem) })]
public sealed class ToolRefinableComponent : Component, ISerializationGenerated<ToolRefinableComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public List<EntitySpawnEntry> RefineResult = new List<EntitySpawnEntry>();

	[DataField(null, false, 1, false, false, null)]
	public float RefineTime = 2f;

	[DataField(null, false, 1, false, false, null)]
	public float RefineFuel = 3f;

	[DataField(null, false, 1, false, false, null)]
	public ProtoId<ToolQualityPrototype> QualityNeeded = ProtoId<ToolQualityPrototype>.op_Implicit("Welding");

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ToolRefinableComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ToolRefinableComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ToolRefinableComponent>(this, ref target, hookCtx, false, context))
		{
			List<EntitySpawnEntry> RefineResultTemp = null;
			if (RefineResult == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<EntitySpawnEntry>>(RefineResult, ref RefineResultTemp, hookCtx, true, context))
			{
				RefineResultTemp = serialization.CreateCopy<List<EntitySpawnEntry>>(RefineResult, hookCtx, context, false);
			}
			target.RefineResult = RefineResultTemp;
			float RefineTimeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(RefineTime, ref RefineTimeTemp, hookCtx, false, context))
			{
				RefineTimeTemp = RefineTime;
			}
			target.RefineTime = RefineTimeTemp;
			float RefineFuelTemp = 0f;
			if (!serialization.TryCustomCopy<float>(RefineFuel, ref RefineFuelTemp, hookCtx, false, context))
			{
				RefineFuelTemp = RefineFuel;
			}
			target.RefineFuel = RefineFuelTemp;
			ProtoId<ToolQualityPrototype> QualityNeededTemp = default(ProtoId<ToolQualityPrototype>);
			if (!serialization.TryCustomCopy<ProtoId<ToolQualityPrototype>>(QualityNeeded, ref QualityNeededTemp, hookCtx, false, context))
			{
				QualityNeededTemp = serialization.CreateCopy<ProtoId<ToolQualityPrototype>>(QualityNeeded, hookCtx, context, false);
			}
			target.QualityNeeded = QualityNeededTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ToolRefinableComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ToolRefinableComponent cast = (ToolRefinableComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ToolRefinableComponent cast = (ToolRefinableComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ToolRefinableComponent def = (ToolRefinableComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ToolRefinableComponent Instantiate()
	{
		return new ToolRefinableComponent();
	}
}
