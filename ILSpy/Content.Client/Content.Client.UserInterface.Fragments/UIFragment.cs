using System;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.UserInterface.Fragments;

[ImplicitDataDefinitionForInheritors]
public abstract class UIFragment : ISerializationGenerated<UIFragment>, ISerializationGenerated
{
	public abstract Control GetUIFragmentRoot();

	public abstract void Setup(BoundUserInterface userInterface, EntityUid? fragmentOwner);

	public abstract void UpdateState(BoundUserInterfaceState state);

	public UIFragment()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref UIFragment target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		serialization.TryCustomCopy<UIFragment>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref UIFragment target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		UIFragment target2 = (UIFragment)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public virtual UIFragment Instantiate()
	{
		throw new NotImplementedException();
	}
}
