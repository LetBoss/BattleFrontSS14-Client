using System;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Teleportation.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class HandTeleporterComponent : Component, ISerializationGenerated<HandTeleporterComponent>, ISerializationGenerated
{
	[ViewVariables]
	[DataField("firstPortal", false, 1, false, false, null)]
	public EntityUid? FirstPortal;

	[ViewVariables]
	[DataField("secondPortal", false, 1, false, false, null)]
	public EntityUid? SecondPortal;

	[DataField(null, false, 1, false, false, null)]
	public bool AllowPortalsOnDifferentGrids;

	[DataField(null, false, 1, false, false, null)]
	public bool AllowPortalsOnDifferentMaps;

	[DataField("firstPortalPrototype", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
	public string FirstPortalPrototype = "PortalRed";

	[DataField("secondPortalPrototype", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
	public string SecondPortalPrototype = "PortalBlue";

	[DataField("newPortalSound", false, 1, false, false, null)]
	public SoundSpecifier NewPortalSound = (SoundSpecifier)new SoundPathSpecifier("/Audio/Machines/high_tech_confirm.ogg", (AudioParams?)null)
	{
		Params = ((AudioParams)(ref AudioParams.Default)).WithVolume(-2f)
	};

	[DataField("clearPortalsSound", false, 1, false, false, null)]
	public SoundSpecifier ClearPortalsSound = (SoundSpecifier)new SoundPathSpecifier("/Audio/Machines/button.ogg", (AudioParams?)null);

	[DataField("portalCreationDelay", false, 1, false, false, null)]
	public float PortalCreationDelay = 1f;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref HandTeleporterComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (HandTeleporterComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<HandTeleporterComponent>(this, ref target, hookCtx, false, context))
		{
			EntityUid? FirstPortalTemp = null;
			if (!serialization.TryCustomCopy<EntityUid?>(FirstPortal, ref FirstPortalTemp, hookCtx, false, context))
			{
				FirstPortalTemp = serialization.CreateCopy<EntityUid?>(FirstPortal, hookCtx, context, false);
			}
			target.FirstPortal = FirstPortalTemp;
			EntityUid? SecondPortalTemp = null;
			if (!serialization.TryCustomCopy<EntityUid?>(SecondPortal, ref SecondPortalTemp, hookCtx, false, context))
			{
				SecondPortalTemp = serialization.CreateCopy<EntityUid?>(SecondPortal, hookCtx, context, false);
			}
			target.SecondPortal = SecondPortalTemp;
			bool AllowPortalsOnDifferentGridsTemp = false;
			if (!serialization.TryCustomCopy<bool>(AllowPortalsOnDifferentGrids, ref AllowPortalsOnDifferentGridsTemp, hookCtx, false, context))
			{
				AllowPortalsOnDifferentGridsTemp = AllowPortalsOnDifferentGrids;
			}
			target.AllowPortalsOnDifferentGrids = AllowPortalsOnDifferentGridsTemp;
			bool AllowPortalsOnDifferentMapsTemp = false;
			if (!serialization.TryCustomCopy<bool>(AllowPortalsOnDifferentMaps, ref AllowPortalsOnDifferentMapsTemp, hookCtx, false, context))
			{
				AllowPortalsOnDifferentMapsTemp = AllowPortalsOnDifferentMaps;
			}
			target.AllowPortalsOnDifferentMaps = AllowPortalsOnDifferentMapsTemp;
			string FirstPortalPrototypeTemp = null;
			if (FirstPortalPrototype == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(FirstPortalPrototype, ref FirstPortalPrototypeTemp, hookCtx, false, context))
			{
				FirstPortalPrototypeTemp = FirstPortalPrototype;
			}
			target.FirstPortalPrototype = FirstPortalPrototypeTemp;
			string SecondPortalPrototypeTemp = null;
			if (SecondPortalPrototype == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(SecondPortalPrototype, ref SecondPortalPrototypeTemp, hookCtx, false, context))
			{
				SecondPortalPrototypeTemp = SecondPortalPrototype;
			}
			target.SecondPortalPrototype = SecondPortalPrototypeTemp;
			SoundSpecifier NewPortalSoundTemp = null;
			if (NewPortalSound == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(NewPortalSound, ref NewPortalSoundTemp, hookCtx, true, context))
			{
				NewPortalSoundTemp = serialization.CreateCopy<SoundSpecifier>(NewPortalSound, hookCtx, context, false);
			}
			target.NewPortalSound = NewPortalSoundTemp;
			SoundSpecifier ClearPortalsSoundTemp = null;
			if (ClearPortalsSound == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(ClearPortalsSound, ref ClearPortalsSoundTemp, hookCtx, true, context))
			{
				ClearPortalsSoundTemp = serialization.CreateCopy<SoundSpecifier>(ClearPortalsSound, hookCtx, context, false);
			}
			target.ClearPortalsSound = ClearPortalsSoundTemp;
			float PortalCreationDelayTemp = 0f;
			if (!serialization.TryCustomCopy<float>(PortalCreationDelay, ref PortalCreationDelayTemp, hookCtx, false, context))
			{
				PortalCreationDelayTemp = PortalCreationDelay;
			}
			target.PortalCreationDelay = PortalCreationDelayTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref HandTeleporterComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HandTeleporterComponent cast = (HandTeleporterComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HandTeleporterComponent cast = (HandTeleporterComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HandTeleporterComponent def = (HandTeleporterComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override HandTeleporterComponent Instantiate()
	{
		return new HandTeleporterComponent();
	}
}
