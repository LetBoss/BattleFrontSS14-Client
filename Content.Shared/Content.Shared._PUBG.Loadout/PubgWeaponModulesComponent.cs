using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._PUBG.Loadout;

[RegisterComponent]
public sealed class PubgWeaponModulesComponent : Component, ISerializationGenerated<PubgWeaponModulesComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public List<PubgWeaponModuleSlotDefinition> Slots = new List<PubgWeaponModuleSlotDefinition>();

	[DataField(null, false, 1, false, false, null)]
	public string FlashlightToggleAction = "ActionTogglePubgWeaponLight";

	[DataField(null, false, 1, false, false, null)]
	public EntityUid? FlashlightToggleActionEntity;

	[DataField(null, false, 1, false, false, null)]
	public string BipodToggleAction = "ActionTogglePubgBipod";

	[DataField(null, false, 1, false, false, null)]
	public EntityUid? BipodToggleActionEntity;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PubgWeaponModulesComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (PubgWeaponModulesComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<PubgWeaponModulesComponent>(this, ref target, hookCtx, false, context))
		{
			List<PubgWeaponModuleSlotDefinition> SlotsTemp = null;
			if (Slots == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<PubgWeaponModuleSlotDefinition>>(Slots, ref SlotsTemp, hookCtx, true, context))
			{
				SlotsTemp = serialization.CreateCopy<List<PubgWeaponModuleSlotDefinition>>(Slots, hookCtx, context, false);
			}
			target.Slots = SlotsTemp;
			string FlashlightToggleActionTemp = null;
			if (FlashlightToggleAction == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(FlashlightToggleAction, ref FlashlightToggleActionTemp, hookCtx, false, context))
			{
				FlashlightToggleActionTemp = FlashlightToggleAction;
			}
			target.FlashlightToggleAction = FlashlightToggleActionTemp;
			EntityUid? FlashlightToggleActionEntityTemp = null;
			if (!serialization.TryCustomCopy<EntityUid?>(FlashlightToggleActionEntity, ref FlashlightToggleActionEntityTemp, hookCtx, false, context))
			{
				FlashlightToggleActionEntityTemp = serialization.CreateCopy<EntityUid?>(FlashlightToggleActionEntity, hookCtx, context, false);
			}
			target.FlashlightToggleActionEntity = FlashlightToggleActionEntityTemp;
			string BipodToggleActionTemp = null;
			if (BipodToggleAction == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(BipodToggleAction, ref BipodToggleActionTemp, hookCtx, false, context))
			{
				BipodToggleActionTemp = BipodToggleAction;
			}
			target.BipodToggleAction = BipodToggleActionTemp;
			EntityUid? BipodToggleActionEntityTemp = null;
			if (!serialization.TryCustomCopy<EntityUid?>(BipodToggleActionEntity, ref BipodToggleActionEntityTemp, hookCtx, false, context))
			{
				BipodToggleActionEntityTemp = serialization.CreateCopy<EntityUid?>(BipodToggleActionEntity, hookCtx, context, false);
			}
			target.BipodToggleActionEntity = BipodToggleActionEntityTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PubgWeaponModulesComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgWeaponModulesComponent cast = (PubgWeaponModulesComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgWeaponModulesComponent cast = (PubgWeaponModulesComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgWeaponModulesComponent def = (PubgWeaponModulesComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PubgWeaponModulesComponent Instantiate()
	{
		return new PubgWeaponModulesComponent();
	}
}
