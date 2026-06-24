using System;
using System.Collections.Generic;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.Weapons.Ranged.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class RevolverAmmoProviderComponent : AmmoProviderComponent, ISerializationGenerated<RevolverAmmoProviderComponent>, ISerializationGenerated
{
	[DataField("whitelist", false, 1, false, false, null)]
	public EntityWhitelist? Whitelist;

	public Container AmmoContainer;

	[DataField("currentSlot", false, 1, false, false, null)]
	public int CurrentIndex;

	[DataField("capacity", false, 1, false, false, null)]
	public int Capacity = 6;

	[DataField("ammoSlots", false, 1, false, false, null)]
	public List<EntityUid?> AmmoSlots = new List<EntityUid?>();

	[DataField("chambers", false, 1, false, false, null)]
	public bool?[] Chambers = Array.Empty<bool?>();

	[DataField("proto", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
	public string? FillPrototype = "CartridgeMagnum";

	[DataField("soundEject", false, 1, false, false, null)]
	public SoundSpecifier? SoundEject = (SoundSpecifier?)new SoundPathSpecifier("/Audio/Weapons/Guns/MagOut/revolver_magout.ogg", (AudioParams?)null);

	[DataField("soundInsert", false, 1, false, false, null)]
	public SoundSpecifier? SoundInsert = (SoundSpecifier?)new SoundPathSpecifier("/Audio/Weapons/Guns/MagIn/revolver_magin.ogg", (AudioParams?)null);

	[DataField("soundSpin", false, 1, false, false, null)]
	public SoundSpecifier? SoundSpin = (SoundSpecifier?)new SoundPathSpecifier("/Audio/Weapons/Guns/Misc/revolver_spin.ogg", (AudioParams?)null);

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RevolverAmmoProviderComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		AmmoProviderComponent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (RevolverAmmoProviderComponent)definitionCast;
		if (serialization.TryCustomCopy<RevolverAmmoProviderComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		EntityWhitelist WhitelistTemp = null;
		if (!serialization.TryCustomCopy<EntityWhitelist>(Whitelist, ref WhitelistTemp, hookCtx, false, context))
		{
			if (Whitelist == null)
			{
				WhitelistTemp = null;
			}
			else
			{
				serialization.CopyTo<EntityWhitelist>(Whitelist, ref WhitelistTemp, hookCtx, context, false);
			}
		}
		target.Whitelist = WhitelistTemp;
		int CurrentIndexTemp = 0;
		if (!serialization.TryCustomCopy<int>(CurrentIndex, ref CurrentIndexTemp, hookCtx, false, context))
		{
			CurrentIndexTemp = CurrentIndex;
		}
		target.CurrentIndex = CurrentIndexTemp;
		int CapacityTemp = 0;
		if (!serialization.TryCustomCopy<int>(Capacity, ref CapacityTemp, hookCtx, false, context))
		{
			CapacityTemp = Capacity;
		}
		target.Capacity = CapacityTemp;
		List<EntityUid?> AmmoSlotsTemp = null;
		if (AmmoSlots == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<List<EntityUid?>>(AmmoSlots, ref AmmoSlotsTemp, hookCtx, true, context))
		{
			AmmoSlotsTemp = serialization.CreateCopy<List<EntityUid?>>(AmmoSlots, hookCtx, context, false);
		}
		target.AmmoSlots = AmmoSlotsTemp;
		bool?[] ChambersTemp = null;
		if (Chambers == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<bool?[]>(Chambers, ref ChambersTemp, hookCtx, true, context))
		{
			ChambersTemp = serialization.CreateCopy<bool?[]>(Chambers, hookCtx, context, false);
		}
		target.Chambers = ChambersTemp;
		string FillPrototypeTemp = null;
		if (!serialization.TryCustomCopy<string>(FillPrototype, ref FillPrototypeTemp, hookCtx, false, context))
		{
			FillPrototypeTemp = FillPrototype;
		}
		target.FillPrototype = FillPrototypeTemp;
		SoundSpecifier SoundEjectTemp = null;
		if (!serialization.TryCustomCopy<SoundSpecifier>(SoundEject, ref SoundEjectTemp, hookCtx, true, context))
		{
			SoundEjectTemp = serialization.CreateCopy<SoundSpecifier>(SoundEject, hookCtx, context, false);
		}
		target.SoundEject = SoundEjectTemp;
		SoundSpecifier SoundInsertTemp = null;
		if (!serialization.TryCustomCopy<SoundSpecifier>(SoundInsert, ref SoundInsertTemp, hookCtx, true, context))
		{
			SoundInsertTemp = serialization.CreateCopy<SoundSpecifier>(SoundInsert, hookCtx, context, false);
		}
		target.SoundInsert = SoundInsertTemp;
		SoundSpecifier SoundSpinTemp = null;
		if (!serialization.TryCustomCopy<SoundSpecifier>(SoundSpin, ref SoundSpinTemp, hookCtx, true, context))
		{
			SoundSpinTemp = serialization.CreateCopy<SoundSpecifier>(SoundSpin, hookCtx, context, false);
		}
		target.SoundSpin = SoundSpinTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RevolverAmmoProviderComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref AmmoProviderComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RevolverAmmoProviderComponent cast = (RevolverAmmoProviderComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RevolverAmmoProviderComponent cast = (RevolverAmmoProviderComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RevolverAmmoProviderComponent def = (RevolverAmmoProviderComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RevolverAmmoProviderComponent Instantiate()
	{
		return new RevolverAmmoProviderComponent();
	}
}
