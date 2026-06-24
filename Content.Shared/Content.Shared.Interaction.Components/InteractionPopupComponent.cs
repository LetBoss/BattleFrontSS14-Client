using System;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Interaction.Components;

[RegisterComponent]
[Access(new Type[] { typeof(InteractionPopupSystem) })]
public sealed class InteractionPopupComponent : Component, ISerializationGenerated<InteractionPopupComponent>, ISerializationGenerated
{
	[DataField("interactDelay", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public TimeSpan InteractDelay = TimeSpan.FromSeconds(1.0);

	[DataField("interactSuccessString", false, 1, false, false, null)]
	public string? InteractSuccessString;

	[DataField("interactFailureString", false, 1, false, false, null)]
	public string? InteractFailureString;

	[DataField("interactSuccessSound", false, 1, false, false, null)]
	public SoundSpecifier? InteractSuccessSound;

	[DataField("interactFailureSound", false, 1, false, false, null)]
	public SoundSpecifier? InteractFailureSound;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public EntProtoId? InteractSuccessSpawn;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public EntProtoId? InteractFailureSpawn;

	[DataField("successChance", false, 1, false, false, null)]
	public float SuccessChance = 1f;

	[DataField("messagePerceivedByOthers", false, 1, false, false, null)]
	public string? MessagePerceivedByOthers;

	[DataField("soundPerceivedByOthers", false, 1, false, false, null)]
	public bool SoundPerceivedByOthers = true;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public TimeSpan LastInteractTime;

	[DataField(null, false, 1, false, false, null)]
	public bool OnActivate;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref InteractionPopupComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (InteractionPopupComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<InteractionPopupComponent>(this, ref target, hookCtx, false, context))
		{
			TimeSpan InteractDelayTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(InteractDelay, ref InteractDelayTemp, hookCtx, false, context))
			{
				InteractDelayTemp = serialization.CreateCopy<TimeSpan>(InteractDelay, hookCtx, context, false);
			}
			target.InteractDelay = InteractDelayTemp;
			string InteractSuccessStringTemp = null;
			if (!serialization.TryCustomCopy<string>(InteractSuccessString, ref InteractSuccessStringTemp, hookCtx, false, context))
			{
				InteractSuccessStringTemp = InteractSuccessString;
			}
			target.InteractSuccessString = InteractSuccessStringTemp;
			string InteractFailureStringTemp = null;
			if (!serialization.TryCustomCopy<string>(InteractFailureString, ref InteractFailureStringTemp, hookCtx, false, context))
			{
				InteractFailureStringTemp = InteractFailureString;
			}
			target.InteractFailureString = InteractFailureStringTemp;
			SoundSpecifier InteractSuccessSoundTemp = null;
			if (!serialization.TryCustomCopy<SoundSpecifier>(InteractSuccessSound, ref InteractSuccessSoundTemp, hookCtx, true, context))
			{
				InteractSuccessSoundTemp = serialization.CreateCopy<SoundSpecifier>(InteractSuccessSound, hookCtx, context, false);
			}
			target.InteractSuccessSound = InteractSuccessSoundTemp;
			SoundSpecifier InteractFailureSoundTemp = null;
			if (!serialization.TryCustomCopy<SoundSpecifier>(InteractFailureSound, ref InteractFailureSoundTemp, hookCtx, true, context))
			{
				InteractFailureSoundTemp = serialization.CreateCopy<SoundSpecifier>(InteractFailureSound, hookCtx, context, false);
			}
			target.InteractFailureSound = InteractFailureSoundTemp;
			EntProtoId? InteractSuccessSpawnTemp = null;
			if (!serialization.TryCustomCopy<EntProtoId?>(InteractSuccessSpawn, ref InteractSuccessSpawnTemp, hookCtx, false, context))
			{
				InteractSuccessSpawnTemp = serialization.CreateCopy<EntProtoId?>(InteractSuccessSpawn, hookCtx, context, false);
			}
			target.InteractSuccessSpawn = InteractSuccessSpawnTemp;
			EntProtoId? InteractFailureSpawnTemp = null;
			if (!serialization.TryCustomCopy<EntProtoId?>(InteractFailureSpawn, ref InteractFailureSpawnTemp, hookCtx, false, context))
			{
				InteractFailureSpawnTemp = serialization.CreateCopy<EntProtoId?>(InteractFailureSpawn, hookCtx, context, false);
			}
			target.InteractFailureSpawn = InteractFailureSpawnTemp;
			float SuccessChanceTemp = 0f;
			if (!serialization.TryCustomCopy<float>(SuccessChance, ref SuccessChanceTemp, hookCtx, false, context))
			{
				SuccessChanceTemp = SuccessChance;
			}
			target.SuccessChance = SuccessChanceTemp;
			string MessagePerceivedByOthersTemp = null;
			if (!serialization.TryCustomCopy<string>(MessagePerceivedByOthers, ref MessagePerceivedByOthersTemp, hookCtx, false, context))
			{
				MessagePerceivedByOthersTemp = MessagePerceivedByOthers;
			}
			target.MessagePerceivedByOthers = MessagePerceivedByOthersTemp;
			bool SoundPerceivedByOthersTemp = false;
			if (!serialization.TryCustomCopy<bool>(SoundPerceivedByOthers, ref SoundPerceivedByOthersTemp, hookCtx, false, context))
			{
				SoundPerceivedByOthersTemp = SoundPerceivedByOthers;
			}
			target.SoundPerceivedByOthers = SoundPerceivedByOthersTemp;
			bool OnActivateTemp = false;
			if (!serialization.TryCustomCopy<bool>(OnActivate, ref OnActivateTemp, hookCtx, false, context))
			{
				OnActivateTemp = OnActivate;
			}
			target.OnActivate = OnActivateTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref InteractionPopupComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InteractionPopupComponent cast = (InteractionPopupComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InteractionPopupComponent cast = (InteractionPopupComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InteractionPopupComponent def = (InteractionPopupComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override InteractionPopupComponent Instantiate()
	{
		return new InteractionPopupComponent();
	}
}
