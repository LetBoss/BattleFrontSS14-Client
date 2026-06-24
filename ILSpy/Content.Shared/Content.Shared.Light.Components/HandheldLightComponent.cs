using System;
using Robust.Shared.Analyzers;
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

namespace Content.Shared.Light.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedHandheldLightSystem) })]
public sealed class HandheldLightComponent : Component, ISerializationGenerated<HandheldLightComponent>, ISerializationGenerated
{
	[Serializable]
	[NetSerializable]
	public sealed class HandheldLightComponentState : ComponentState
	{
		public byte? Charge { get; }

		public bool Activated { get; }

		public HandheldLightComponentState(bool activated, byte? charge)
		{
			Activated = activated;
			Charge = charge;
		}
	}

	public byte? Level;

	public bool Activated;

	[DataField("turnOnSound", false, 1, false, false, null)]
	public SoundSpecifier TurnOnSound = (SoundSpecifier)new SoundPathSpecifier("/Audio/Items/flashlight_on.ogg", (AudioParams?)null);

	[DataField("turnOnFailSound", false, 1, false, false, null)]
	public SoundSpecifier TurnOnFailSound = (SoundSpecifier)new SoundPathSpecifier("/Audio/Machines/button.ogg", (AudioParams?)null);

	[DataField("turnOffSound", false, 1, false, false, null)]
	public SoundSpecifier TurnOffSound = (SoundSpecifier)new SoundPathSpecifier("/Audio/Items/flashlight_off.ogg", (AudioParams?)null);

	[DataField("addPrefix", false, 1, false, false, null)]
	public bool AddPrefix;

	[DataField("toggleAction", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
	public string ToggleAction = "ActionToggleLight";

	[DataField("toggleOnInteract", false, 1, false, false, null)]
	public bool ToggleOnInteract = true;

	[DataField("toggleActionEntity", false, 1, false, false, null)]
	public EntityUid? ToggleActionEntity;

	[DataField(null, false, 1, false, false, null)]
	public EntityUid? SelfToggleActionEntity;

	public const int StatusLevels = 6;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("wattage", false, 1, false, false, null)]
	public float Wattage { get; set; } = 0.8f;

	[DataField("blinkingBehaviourId", false, 1, false, false, null)]
	public string BlinkingBehaviourId { get; set; } = string.Empty;

	[DataField("radiatingBehaviourId", false, 1, false, false, null)]
	public string RadiatingBehaviourId { get; set; } = string.Empty;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref HandheldLightComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (HandheldLightComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<HandheldLightComponent>(this, ref target, hookCtx, false, context))
		{
			float WattageTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Wattage, ref WattageTemp, hookCtx, false, context))
			{
				WattageTemp = Wattage;
			}
			target.Wattage = WattageTemp;
			SoundSpecifier TurnOnSoundTemp = null;
			if (TurnOnSound == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(TurnOnSound, ref TurnOnSoundTemp, hookCtx, true, context))
			{
				TurnOnSoundTemp = serialization.CreateCopy<SoundSpecifier>(TurnOnSound, hookCtx, context, false);
			}
			target.TurnOnSound = TurnOnSoundTemp;
			SoundSpecifier TurnOnFailSoundTemp = null;
			if (TurnOnFailSound == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(TurnOnFailSound, ref TurnOnFailSoundTemp, hookCtx, true, context))
			{
				TurnOnFailSoundTemp = serialization.CreateCopy<SoundSpecifier>(TurnOnFailSound, hookCtx, context, false);
			}
			target.TurnOnFailSound = TurnOnFailSoundTemp;
			SoundSpecifier TurnOffSoundTemp = null;
			if (TurnOffSound == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(TurnOffSound, ref TurnOffSoundTemp, hookCtx, true, context))
			{
				TurnOffSoundTemp = serialization.CreateCopy<SoundSpecifier>(TurnOffSound, hookCtx, context, false);
			}
			target.TurnOffSound = TurnOffSoundTemp;
			bool AddPrefixTemp = false;
			if (!serialization.TryCustomCopy<bool>(AddPrefix, ref AddPrefixTemp, hookCtx, false, context))
			{
				AddPrefixTemp = AddPrefix;
			}
			target.AddPrefix = AddPrefixTemp;
			string ToggleActionTemp = null;
			if (ToggleAction == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(ToggleAction, ref ToggleActionTemp, hookCtx, false, context))
			{
				ToggleActionTemp = ToggleAction;
			}
			target.ToggleAction = ToggleActionTemp;
			bool ToggleOnInteractTemp = false;
			if (!serialization.TryCustomCopy<bool>(ToggleOnInteract, ref ToggleOnInteractTemp, hookCtx, false, context))
			{
				ToggleOnInteractTemp = ToggleOnInteract;
			}
			target.ToggleOnInteract = ToggleOnInteractTemp;
			EntityUid? ToggleActionEntityTemp = null;
			if (!serialization.TryCustomCopy<EntityUid?>(ToggleActionEntity, ref ToggleActionEntityTemp, hookCtx, false, context))
			{
				ToggleActionEntityTemp = serialization.CreateCopy<EntityUid?>(ToggleActionEntity, hookCtx, context, false);
			}
			target.ToggleActionEntity = ToggleActionEntityTemp;
			EntityUid? SelfToggleActionEntityTemp = null;
			if (!serialization.TryCustomCopy<EntityUid?>(SelfToggleActionEntity, ref SelfToggleActionEntityTemp, hookCtx, false, context))
			{
				SelfToggleActionEntityTemp = serialization.CreateCopy<EntityUid?>(SelfToggleActionEntity, hookCtx, context, false);
			}
			target.SelfToggleActionEntity = SelfToggleActionEntityTemp;
			string BlinkingBehaviourIdTemp = null;
			if (BlinkingBehaviourId == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(BlinkingBehaviourId, ref BlinkingBehaviourIdTemp, hookCtx, false, context))
			{
				BlinkingBehaviourIdTemp = BlinkingBehaviourId;
			}
			target.BlinkingBehaviourId = BlinkingBehaviourIdTemp;
			string RadiatingBehaviourIdTemp = null;
			if (RadiatingBehaviourId == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(RadiatingBehaviourId, ref RadiatingBehaviourIdTemp, hookCtx, false, context))
			{
				RadiatingBehaviourIdTemp = RadiatingBehaviourId;
			}
			target.RadiatingBehaviourId = RadiatingBehaviourIdTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref HandheldLightComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HandheldLightComponent cast = (HandheldLightComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HandheldLightComponent cast = (HandheldLightComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HandheldLightComponent def = (HandheldLightComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override HandheldLightComponent Instantiate()
	{
		return new HandheldLightComponent();
	}
}
