using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Wires;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedWiresSystem) })]
public sealed class ActivatableUIRequiresPanelComponent : Component, ISerializationGenerated<ActivatableUIRequiresPanelComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public bool RequireOpen = true;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ActivatableUIRequiresPanelComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ActivatableUIRequiresPanelComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ActivatableUIRequiresPanelComponent>(this, ref target, hookCtx, false, context))
		{
			bool RequireOpenTemp = false;
			if (!serialization.TryCustomCopy<bool>(RequireOpen, ref RequireOpenTemp, hookCtx, false, context))
			{
				RequireOpenTemp = RequireOpen;
			}
			target.RequireOpen = RequireOpenTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ActivatableUIRequiresPanelComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ActivatableUIRequiresPanelComponent cast = (ActivatableUIRequiresPanelComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ActivatableUIRequiresPanelComponent cast = (ActivatableUIRequiresPanelComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ActivatableUIRequiresPanelComponent def = (ActivatableUIRequiresPanelComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ActivatableUIRequiresPanelComponent Instantiate()
	{
		return new ActivatableUIRequiresPanelComponent();
	}
}
