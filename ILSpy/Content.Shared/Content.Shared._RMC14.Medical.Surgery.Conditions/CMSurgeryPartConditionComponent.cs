using System;
using Content.Shared.Body.Part;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Medical.Surgery.Conditions;

[RegisterComponent]
[NetworkedComponent]
public sealed class CMSurgeryPartConditionComponent : Component, ISerializationGenerated<CMSurgeryPartConditionComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public BodyPartType Part;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref CMSurgeryPartConditionComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (CMSurgeryPartConditionComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<CMSurgeryPartConditionComponent>(this, ref target, hookCtx, false, context))
		{
			BodyPartType PartTemp = BodyPartType.Other;
			if (!serialization.TryCustomCopy<BodyPartType>(Part, ref PartTemp, hookCtx, false, context))
			{
				PartTemp = Part;
			}
			target.Part = PartTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref CMSurgeryPartConditionComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CMSurgeryPartConditionComponent cast = (CMSurgeryPartConditionComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CMSurgeryPartConditionComponent cast = (CMSurgeryPartConditionComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CMSurgeryPartConditionComponent def = (CMSurgeryPartConditionComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override CMSurgeryPartConditionComponent Instantiate()
	{
		return new CMSurgeryPartConditionComponent();
	}
}
