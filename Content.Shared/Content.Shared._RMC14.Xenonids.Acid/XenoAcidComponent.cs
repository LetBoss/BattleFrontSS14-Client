using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Xenonids.Acid;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedXenoAcidSystem) })]
public sealed class XenoAcidComponent : Component, ISerializationGenerated<XenoAcidComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public bool CanMeltStructures = true;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref XenoAcidComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (XenoAcidComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<XenoAcidComponent>(this, ref target, hookCtx, false, context))
		{
			bool CanMeltStructuresTemp = false;
			if (!serialization.TryCustomCopy<bool>(CanMeltStructures, ref CanMeltStructuresTemp, hookCtx, false, context))
			{
				CanMeltStructuresTemp = CanMeltStructures;
			}
			target.CanMeltStructures = CanMeltStructuresTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref XenoAcidComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoAcidComponent cast = (XenoAcidComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoAcidComponent cast = (XenoAcidComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoAcidComponent def = (XenoAcidComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override XenoAcidComponent Instantiate()
	{
		return new XenoAcidComponent();
	}
}
