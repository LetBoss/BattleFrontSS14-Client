using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Nutrition.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class FlavorProfileComponent : Component, ISerializationGenerated<FlavorProfileComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public HashSet<string> Flavors { get; private set; } = new HashSet<string>();

	[DataField(null, false, 1, false, false, null)]
	public HashSet<string> IgnoreReagents { get; private set; } = new HashSet<string> { "Nutriment", "Vitamin", "Protein" };

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref FlavorProfileComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (FlavorProfileComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<FlavorProfileComponent>(this, ref target, hookCtx, false, context))
		{
			HashSet<string> FlavorsTemp = null;
			if (Flavors == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<HashSet<string>>(Flavors, ref FlavorsTemp, hookCtx, true, context))
			{
				FlavorsTemp = serialization.CreateCopy<HashSet<string>>(Flavors, hookCtx, context, false);
			}
			target.Flavors = FlavorsTemp;
			HashSet<string> IgnoreReagentsTemp = null;
			if (IgnoreReagents == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<HashSet<string>>(IgnoreReagents, ref IgnoreReagentsTemp, hookCtx, true, context))
			{
				IgnoreReagentsTemp = serialization.CreateCopy<HashSet<string>>(IgnoreReagents, hookCtx, context, false);
			}
			target.IgnoreReagents = IgnoreReagentsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref FlavorProfileComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FlavorProfileComponent cast = (FlavorProfileComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FlavorProfileComponent cast = (FlavorProfileComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FlavorProfileComponent def = (FlavorProfileComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override FlavorProfileComponent Instantiate()
	{
		return new FlavorProfileComponent();
	}
}
