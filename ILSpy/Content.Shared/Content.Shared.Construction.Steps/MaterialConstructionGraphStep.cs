using System;
using System.Diagnostics.CodeAnalysis;
using Content.Shared.Examine;
using Content.Shared.Stacks;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Construction.Steps;

[DataDefinition]
public sealed class MaterialConstructionGraphStep : EntityInsertConstructionGraphStep, ISerializationGenerated<MaterialConstructionGraphStep>, ISerializationGenerated
{
	[DataField("material", false, 1, true, false, null)]
	public ProtoId<StackPrototype> MaterialPrototypeId { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public int Amount { get; private set; } = 1;

	public override void DoExamine(ExaminedEvent examinedEvent)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		string materialName = Loc.GetString(IoCManager.Resolve<IPrototypeManager>().Index<StackPrototype>(MaterialPrototypeId).Name, new(string, object)[1] { ("amount", Amount) });
		examinedEvent.PushMarkup(Loc.GetString("construction-insert-material-entity", new(string, object)[2]
		{
			("amount", Amount),
			("materialName", materialName)
		}));
	}

	public override bool EntityValid(EntityUid uid, IEntityManager entityManager, IComponentFactory compFactory)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		StackComponent stack = default(StackComponent);
		if (entityManager.TryGetComponent<StackComponent>(uid, ref stack) && ProtoId<StackPrototype>.op_Implicit(stack.StackTypeId) == MaterialPrototypeId)
		{
			return stack.Count >= Amount;
		}
		return false;
	}

	public bool EntityValid(EntityUid entity, [NotNullWhen(true)] out StackComponent? stack)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		StackComponent otherStack = default(StackComponent);
		if (IoCManager.Resolve<IEntityManager>().TryGetComponent<StackComponent>(entity, ref otherStack) && ProtoId<StackPrototype>.op_Implicit(otherStack.StackTypeId) == MaterialPrototypeId && otherStack.Count >= Amount)
		{
			stack = otherStack;
		}
		else
		{
			stack = null;
		}
		return stack != null;
	}

	public override ConstructionGuideEntry GenerateGuideEntry()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		StackPrototype material = IoCManager.Resolve<IPrototypeManager>().Index<StackPrototype>(MaterialPrototypeId);
		string materialName = Loc.GetString(material.Name, new(string, object)[1] { ("amount", Amount) });
		ConstructionGuideEntry constructionGuideEntry = new ConstructionGuideEntry();
		constructionGuideEntry.Localization = "construction-presenter-material-step";
		constructionGuideEntry.Arguments = new(string, object)[2]
		{
			("amount", Amount),
			("material", materialName)
		};
		constructionGuideEntry.Icon = material.Icon;
		return constructionGuideEntry;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref MaterialConstructionGraphStep target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		EntityInsertConstructionGraphStep definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (MaterialConstructionGraphStep)definitionCast;
		if (!serialization.TryCustomCopy<MaterialConstructionGraphStep>(this, ref target, hookCtx, false, context))
		{
			ProtoId<StackPrototype> MaterialPrototypeIdTemp = default(ProtoId<StackPrototype>);
			if (!serialization.TryCustomCopy<ProtoId<StackPrototype>>(MaterialPrototypeId, ref MaterialPrototypeIdTemp, hookCtx, false, context))
			{
				MaterialPrototypeIdTemp = serialization.CreateCopy<ProtoId<StackPrototype>>(MaterialPrototypeId, hookCtx, context, false);
			}
			target.MaterialPrototypeId = MaterialPrototypeIdTemp;
			int AmountTemp = 0;
			if (!serialization.TryCustomCopy<int>(Amount, ref AmountTemp, hookCtx, false, context))
			{
				AmountTemp = Amount;
			}
			target.Amount = AmountTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref MaterialConstructionGraphStep target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityInsertConstructionGraphStep target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MaterialConstructionGraphStep cast = (MaterialConstructionGraphStep)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MaterialConstructionGraphStep cast = (MaterialConstructionGraphStep)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override MaterialConstructionGraphStep Instantiate()
	{
		return new MaterialConstructionGraphStep();
	}
}
