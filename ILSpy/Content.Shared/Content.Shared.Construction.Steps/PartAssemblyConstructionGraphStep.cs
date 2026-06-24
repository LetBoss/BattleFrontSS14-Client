using System;
using Content.Shared.Examine;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Construction.Steps;

[DataDefinition]
public sealed class PartAssemblyConstructionGraphStep : ConstructionGraphStep, ISerializationGenerated<PartAssemblyConstructionGraphStep>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public string AssemblyId = string.Empty;

	[DataField(null, false, 1, false, false, null)]
	public LocId GuideString = LocId.op_Implicit("construction-guide-condition-part-assembly");

	public bool Condition(EntityUid uid, IEntityManager entityManager)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return entityManager.System<PartAssemblySystem>().IsAssemblyFinished(uid, AssemblyId);
	}

	public override void DoExamine(ExaminedEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		args.PushMarkup(Loc.GetString(LocId.op_Implicit(GuideString)));
	}

	public override ConstructionGuideEntry GenerateGuideEntry()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return new ConstructionGuideEntry
		{
			Localization = LocId.op_Implicit(GuideString)
		};
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PartAssemblyConstructionGraphStep target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		ConstructionGraphStep definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (PartAssemblyConstructionGraphStep)definitionCast;
		if (!serialization.TryCustomCopy<PartAssemblyConstructionGraphStep>(this, ref target, hookCtx, false, context))
		{
			string AssemblyIdTemp = null;
			if (AssemblyId == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(AssemblyId, ref AssemblyIdTemp, hookCtx, false, context))
			{
				AssemblyIdTemp = AssemblyId;
			}
			target.AssemblyId = AssemblyIdTemp;
			LocId GuideStringTemp = default(LocId);
			if (!serialization.TryCustomCopy<LocId>(GuideString, ref GuideStringTemp, hookCtx, false, context))
			{
				GuideStringTemp = serialization.CreateCopy<LocId>(GuideString, hookCtx, context, false);
			}
			target.GuideString = GuideStringTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PartAssemblyConstructionGraphStep target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref ConstructionGraphStep target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PartAssemblyConstructionGraphStep cast = (PartAssemblyConstructionGraphStep)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PartAssemblyConstructionGraphStep cast = (PartAssemblyConstructionGraphStep)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PartAssemblyConstructionGraphStep Instantiate()
	{
		return new PartAssemblyConstructionGraphStep();
	}
}
