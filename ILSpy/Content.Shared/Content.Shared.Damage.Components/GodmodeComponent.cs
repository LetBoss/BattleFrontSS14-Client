using System;
using Content.Shared.Damage.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Damage.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedGodmodeSystem) })]
public sealed class GodmodeComponent : Component, ISerializationGenerated<GodmodeComponent>, ISerializationGenerated
{
	[DataField("wasMovedByPressure", false, 1, false, false, null)]
	public bool WasMovedByPressure;

	[DataField("oldDamage", false, 1, false, false, null)]
	public DamageSpecifier? OldDamage;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref GodmodeComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (GodmodeComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<GodmodeComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		bool WasMovedByPressureTemp = false;
		if (!serialization.TryCustomCopy<bool>(WasMovedByPressure, ref WasMovedByPressureTemp, hookCtx, false, context))
		{
			WasMovedByPressureTemp = WasMovedByPressure;
		}
		target.WasMovedByPressure = WasMovedByPressureTemp;
		DamageSpecifier OldDamageTemp = null;
		if (!serialization.TryCustomCopy<DamageSpecifier>(OldDamage, ref OldDamageTemp, hookCtx, false, context))
		{
			if (OldDamage == null)
			{
				OldDamageTemp = null;
			}
			else
			{
				serialization.CopyTo<DamageSpecifier>(OldDamage, ref OldDamageTemp, hookCtx, context, false);
			}
		}
		target.OldDamage = OldDamageTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref GodmodeComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GodmodeComponent cast = (GodmodeComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GodmodeComponent cast = (GodmodeComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GodmodeComponent def = (GodmodeComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override GodmodeComponent Instantiate()
	{
		return new GodmodeComponent();
	}
}
