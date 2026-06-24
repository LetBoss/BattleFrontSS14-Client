using System;
using Content.Client.Weapons.Ranged.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Client.Weapons.Ranged.Components;

[RegisterComponent]
[Access(new Type[] { typeof(GunSystem) })]
public sealed class SpentAmmoVisualsComponent : Component, ISerializationGenerated<SpentAmmoVisualsComponent>, ISerializationGenerated
{
	[DataField("suffix", false, 1, false, false, null)]
	public bool Suffix = true;

	[DataField("state", false, 1, false, false, null)]
	public string State = "base";

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SpentAmmoVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		Component val = (Component)(object)target;
		((Component)this).InternalCopy(ref val, serialization, hookCtx, context);
		target = (SpentAmmoVisualsComponent)(object)val;
		if (!serialization.TryCustomCopy<SpentAmmoVisualsComponent>(this, ref target, hookCtx, false, context))
		{
			bool suffix = false;
			if (!serialization.TryCustomCopy<bool>(Suffix, ref suffix, hookCtx, false, context))
			{
				suffix = Suffix;
			}
			target.Suffix = suffix;
			string state = null;
			if (State == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(State, ref state, hookCtx, false, context))
			{
				state = State;
			}
			target.State = state;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SpentAmmoVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SpentAmmoVisualsComponent target2 = (SpentAmmoVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (Component)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SpentAmmoVisualsComponent target2 = (SpentAmmoVisualsComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SpentAmmoVisualsComponent target2 = (SpentAmmoVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SpentAmmoVisualsComponent Instantiate()
	{
		return new SpentAmmoVisualsComponent();
	}
}
