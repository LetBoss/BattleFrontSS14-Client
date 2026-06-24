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
public sealed class ComponentConstructionGraphStep : ArbitraryInsertConstructionGraphStep, ISerializationGenerated<ComponentConstructionGraphStep>, ISerializationGenerated
{
	[DataField("component", false, 1, false, false, null)]
	public string Component { get; private set; } = string.Empty;

	public override bool EntityValid(EntityUid uid, IEntityManager entityManager, IComponentFactory compFactory)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		foreach (IComponent component in entityManager.GetComponents(uid))
		{
			if (compFactory.GetComponentName(((object)component).GetType()) == Component)
			{
				return true;
			}
		}
		return false;
	}

	public override void DoExamine(ExaminedEvent examinedEvent)
	{
		examinedEvent.PushMarkup(string.IsNullOrEmpty(base.Name) ? Loc.GetString("construction-insert-entity-with-component", new(string, object)[1] { ("componentName", Component) }) : Loc.GetString("construction-insert-exact-entity", new(string, object)[1] { ("entityName", Loc.GetString(base.Name)) }));
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ComponentConstructionGraphStep target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		ArbitraryInsertConstructionGraphStep definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ComponentConstructionGraphStep)definitionCast;
		if (!serialization.TryCustomCopy<ComponentConstructionGraphStep>(this, ref target, hookCtx, false, context))
		{
			string ComponentTemp = null;
			if (Component == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Component, ref ComponentTemp, hookCtx, false, context))
			{
				ComponentTemp = Component;
			}
			target.Component = ComponentTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ComponentConstructionGraphStep target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref ArbitraryInsertConstructionGraphStep target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ComponentConstructionGraphStep cast = (ComponentConstructionGraphStep)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ComponentConstructionGraphStep cast = (ComponentConstructionGraphStep)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ComponentConstructionGraphStep Instantiate()
	{
		return new ComponentConstructionGraphStep();
	}
}
