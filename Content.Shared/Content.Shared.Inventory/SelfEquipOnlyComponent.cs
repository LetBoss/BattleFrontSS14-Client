using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Inventory;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SelfEquipOnlySystem) })]
public sealed class SelfEquipOnlyComponent : Component, ISerializationGenerated<SelfEquipOnlyComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public bool UnequipRequireConscious = true;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SelfEquipOnlyComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (SelfEquipOnlyComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<SelfEquipOnlyComponent>(this, ref target, hookCtx, false, context))
		{
			bool UnequipRequireConsciousTemp = false;
			if (!serialization.TryCustomCopy<bool>(UnequipRequireConscious, ref UnequipRequireConsciousTemp, hookCtx, false, context))
			{
				UnequipRequireConsciousTemp = UnequipRequireConscious;
			}
			target.UnequipRequireConscious = UnequipRequireConsciousTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SelfEquipOnlyComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SelfEquipOnlyComponent cast = (SelfEquipOnlyComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SelfEquipOnlyComponent cast = (SelfEquipOnlyComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SelfEquipOnlyComponent def = (SelfEquipOnlyComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SelfEquipOnlyComponent Instantiate()
	{
		return new SelfEquipOnlyComponent();
	}
}
