using System;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Weapons.Ranged;

[RegisterComponent]
[Virtual]
[Access(new Type[] { typeof(SharedGunSystem) })]
public class MagazineAmmoProviderComponent : AmmoProviderComponent, ISerializationGenerated<MagazineAmmoProviderComponent>, ISerializationGenerated
{
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("soundAutoEject", false, 1, false, false, null)]
	public SoundSpecifier? SoundAutoEject = (SoundSpecifier?)new SoundPathSpecifier("/Audio/Weapons/Guns/EmptyAlarm/smg_empty_alarm.ogg", (AudioParams?)null);

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("autoEject", false, 1, false, false, null)]
	public bool AutoEject;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref MagazineAmmoProviderComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AmmoProviderComponent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (MagazineAmmoProviderComponent)definitionCast;
		if (!serialization.TryCustomCopy<MagazineAmmoProviderComponent>(this, ref target, hookCtx, false, context))
		{
			SoundSpecifier SoundAutoEjectTemp = null;
			if (!serialization.TryCustomCopy<SoundSpecifier>(SoundAutoEject, ref SoundAutoEjectTemp, hookCtx, true, context))
			{
				SoundAutoEjectTemp = serialization.CreateCopy<SoundSpecifier>(SoundAutoEject, hookCtx, context, false);
			}
			target.SoundAutoEject = SoundAutoEjectTemp;
			bool AutoEjectTemp = false;
			if (!serialization.TryCustomCopy<bool>(AutoEject, ref AutoEjectTemp, hookCtx, false, context))
			{
				AutoEjectTemp = AutoEject;
			}
			target.AutoEject = AutoEjectTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref MagazineAmmoProviderComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref AmmoProviderComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MagazineAmmoProviderComponent cast = (MagazineAmmoProviderComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MagazineAmmoProviderComponent cast = (MagazineAmmoProviderComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MagazineAmmoProviderComponent def = (MagazineAmmoProviderComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override MagazineAmmoProviderComponent Instantiate()
	{
		return new MagazineAmmoProviderComponent();
	}
}
