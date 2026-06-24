using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Mousetrap;

[RegisterComponent]
[NetworkedComponent]
public sealed class MousetrapComponent : Component, ISerializationGenerated<MousetrapComponent>, ISerializationGenerated
{
	[ViewVariables]
	[DataField("isActive", false, 1, false, false, null)]
	public bool IsActive;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("massBalance", false, 1, false, false, null)]
	public int MassBalance = 10;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref MousetrapComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (MousetrapComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<MousetrapComponent>(this, ref target, hookCtx, false, context))
		{
			bool IsActiveTemp = false;
			if (!serialization.TryCustomCopy<bool>(IsActive, ref IsActiveTemp, hookCtx, false, context))
			{
				IsActiveTemp = IsActive;
			}
			target.IsActive = IsActiveTemp;
			int MassBalanceTemp = 0;
			if (!serialization.TryCustomCopy<int>(MassBalance, ref MassBalanceTemp, hookCtx, false, context))
			{
				MassBalanceTemp = MassBalance;
			}
			target.MassBalance = MassBalanceTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref MousetrapComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MousetrapComponent cast = (MousetrapComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MousetrapComponent cast = (MousetrapComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MousetrapComponent def = (MousetrapComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override MousetrapComponent Instantiate()
	{
		return new MousetrapComponent();
	}
}
