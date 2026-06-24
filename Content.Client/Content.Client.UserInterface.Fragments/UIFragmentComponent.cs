using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.UserInterface.Fragments;

[RegisterComponent]
public sealed class UIFragmentComponent : Component, ISerializationGenerated<UIFragmentComponent>, ISerializationGenerated
{
	[DataField("ui", true, 1, false, false, null)]
	public UIFragment? Ui;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref UIFragmentComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component val = (Component)(object)target;
		((Component)this).InternalCopy(ref val, serialization, hookCtx, context);
		target = (UIFragmentComponent)(object)val;
		if (!serialization.TryCustomCopy<UIFragmentComponent>(this, ref target, hookCtx, false, context))
		{
			UIFragment ui = null;
			if (!serialization.TryCustomCopy<UIFragment>(Ui, ref ui, hookCtx, true, context))
			{
				ui = serialization.CreateCopy<UIFragment>(Ui, hookCtx, context, false);
			}
			target.Ui = ui;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref UIFragmentComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		UIFragmentComponent target2 = (UIFragmentComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (Component)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		UIFragmentComponent target2 = (UIFragmentComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		UIFragmentComponent target2 = (UIFragmentComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override UIFragmentComponent Instantiate()
	{
		return new UIFragmentComponent();
	}
}
