using System;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Client.Weapons.Ranged.Components;

[RegisterComponent]
public sealed class AmmoCounterComponent : SharedAmmoCounterComponent, ISerializationGenerated<AmmoCounterComponent>, ISerializationGenerated
{
	public Control? Control;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref AmmoCounterComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SharedAmmoCounterComponent target2 = target;
		base.InternalCopy(ref target2, serialization, hookCtx, context);
		target = (AmmoCounterComponent)target2;
		serialization.TryCustomCopy<AmmoCounterComponent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref AmmoCounterComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref SharedAmmoCounterComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AmmoCounterComponent target2 = (AmmoCounterComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AmmoCounterComponent target2 = (AmmoCounterComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AmmoCounterComponent target2 = (AmmoCounterComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override AmmoCounterComponent Instantiate()
	{
		return new AmmoCounterComponent();
	}
}
