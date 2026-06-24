using System;
using Content.Shared.Chat.Prototypes;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Cluwne;

[RegisterComponent]
[NetworkedComponent]
public sealed class CluwneComponent : Component, ISerializationGenerated<CluwneComponent>, ISerializationGenerated
{
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public TimeSpan DamageGiggleCooldown = TimeSpan.FromSeconds(2L);

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float KnockChance = 0.05f;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float GiggleRandomChance = 0.1f;

	[DataField("emoteId", false, 1, false, false, typeof(PrototypeIdSerializer<EmoteSoundsPrototype>))]
	public string? EmoteSoundsId = "Cluwne";

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float ParalyzeTime = 2f;

	[DataField("spawnsound", false, 1, false, false, null)]
	public SoundSpecifier SpawnSound = (SoundSpecifier)new SoundPathSpecifier("/Audio/Items/bikehorn.ogg", (AudioParams?)null);

	[DataField("knocksound", false, 1, false, false, null)]
	public SoundSpecifier KnockSound = (SoundSpecifier)new SoundPathSpecifier("/Audio/Items/airhorn.ogg", (AudioParams?)null);

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref CluwneComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (CluwneComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<CluwneComponent>(this, ref target, hookCtx, false, context))
		{
			string EmoteSoundsIdTemp = null;
			if (!serialization.TryCustomCopy<string>(EmoteSoundsId, ref EmoteSoundsIdTemp, hookCtx, false, context))
			{
				EmoteSoundsIdTemp = EmoteSoundsId;
			}
			target.EmoteSoundsId = EmoteSoundsIdTemp;
			SoundSpecifier SpawnSoundTemp = null;
			if (SpawnSound == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(SpawnSound, ref SpawnSoundTemp, hookCtx, true, context))
			{
				SpawnSoundTemp = serialization.CreateCopy<SoundSpecifier>(SpawnSound, hookCtx, context, false);
			}
			target.SpawnSound = SpawnSoundTemp;
			SoundSpecifier KnockSoundTemp = null;
			if (KnockSound == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(KnockSound, ref KnockSoundTemp, hookCtx, true, context))
			{
				KnockSoundTemp = serialization.CreateCopy<SoundSpecifier>(KnockSound, hookCtx, context, false);
			}
			target.KnockSound = KnockSoundTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref CluwneComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CluwneComponent cast = (CluwneComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CluwneComponent cast = (CluwneComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CluwneComponent def = (CluwneComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override CluwneComponent Instantiate()
	{
		return new CluwneComponent();
	}
}
