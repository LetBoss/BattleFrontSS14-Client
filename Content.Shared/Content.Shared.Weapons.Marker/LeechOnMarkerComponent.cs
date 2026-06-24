using System;
using Content.Shared.Damage;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Weapons.Marker;

[RegisterComponent]
[NetworkedComponent]
public sealed class LeechOnMarkerComponent : Component, ISerializationGenerated<LeechOnMarkerComponent>, ISerializationGenerated
{
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("leech", false, 1, true, false, null)]
	public DamageSpecifier Leech = new DamageSpecifier();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref LeechOnMarkerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (LeechOnMarkerComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<LeechOnMarkerComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		DamageSpecifier LeechTemp = null;
		if (Leech == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<DamageSpecifier>(Leech, ref LeechTemp, hookCtx, false, context))
		{
			if (Leech == null)
			{
				LeechTemp = null;
			}
			else
			{
				serialization.CopyTo<DamageSpecifier>(Leech, ref LeechTemp, hookCtx, context, true);
			}
		}
		target.Leech = LeechTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref LeechOnMarkerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		LeechOnMarkerComponent cast = (LeechOnMarkerComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		LeechOnMarkerComponent cast = (LeechOnMarkerComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		LeechOnMarkerComponent def = (LeechOnMarkerComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override LeechOnMarkerComponent Instantiate()
	{
		return new LeechOnMarkerComponent();
	}
}
