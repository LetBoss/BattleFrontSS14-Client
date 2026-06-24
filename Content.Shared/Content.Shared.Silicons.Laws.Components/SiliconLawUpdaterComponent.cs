using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Silicons.Laws.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class SiliconLawUpdaterComponent : Component, ISerializationGenerated<SiliconLawUpdaterComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public ComponentRegistry Components;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SiliconLawUpdaterComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (SiliconLawUpdaterComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<SiliconLawUpdaterComponent>(this, ref target, hookCtx, false, context))
		{
			ComponentRegistry ComponentsTemp = null;
			if (Components == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<ComponentRegistry>(Components, ref ComponentsTemp, hookCtx, false, context))
			{
				ComponentsTemp = serialization.CreateCopy<ComponentRegistry>(Components, hookCtx, context, false);
			}
			target.Components = ComponentsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SiliconLawUpdaterComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SiliconLawUpdaterComponent cast = (SiliconLawUpdaterComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SiliconLawUpdaterComponent cast = (SiliconLawUpdaterComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SiliconLawUpdaterComponent def = (SiliconLawUpdaterComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SiliconLawUpdaterComponent Instantiate()
	{
		return new SiliconLawUpdaterComponent();
	}
}
