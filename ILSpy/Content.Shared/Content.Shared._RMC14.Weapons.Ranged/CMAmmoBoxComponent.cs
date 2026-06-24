using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._RMC14.Weapons.Ranged;

[RegisterComponent]
[NetworkedComponent]
public sealed class CMAmmoBoxComponent : Component, ISerializationGenerated<CMAmmoBoxComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public Enum AmmoLayer = CMAmmoBoxLayers.Ammo;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref CMAmmoBoxComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (CMAmmoBoxComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<CMAmmoBoxComponent>(this, ref target, hookCtx, false, context))
		{
			Enum AmmoLayerTemp = null;
			if (AmmoLayer == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Enum>(AmmoLayer, ref AmmoLayerTemp, hookCtx, true, context))
			{
				AmmoLayerTemp = AmmoLayer;
			}
			target.AmmoLayer = AmmoLayerTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref CMAmmoBoxComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CMAmmoBoxComponent cast = (CMAmmoBoxComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CMAmmoBoxComponent cast = (CMAmmoBoxComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CMAmmoBoxComponent def = (CMAmmoBoxComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override CMAmmoBoxComponent Instantiate()
	{
		return new CMAmmoBoxComponent();
	}
}
