using System;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Ensnaring.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class EnsnaringComponent : Component, ISerializationGenerated<EnsnaringComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float FreeTime = 3.5f;

	[DataField(null, false, 1, false, false, null)]
	public float BreakoutTime = 30f;

	[DataField(null, false, 1, false, false, null)]
	public float WalkSpeed = 0.9f;

	[DataField(null, false, 1, false, false, null)]
	public float SprintSpeed = 0.9f;

	[DataField(null, false, 1, false, false, null)]
	public float StaminaDamage = 55f;

	[DataField(null, false, 1, false, false, null)]
	public float MaxEnsnares = 1f;

	[DataField(null, false, 1, false, false, null)]
	public bool CanThrowTrigger;

	[DataField(null, false, 1, false, false, null)]
	public EntityUid? Ensnared;

	[DataField(null, false, 1, false, false, null)]
	public bool CanMoveBreakout;

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier? EnsnareSound = (SoundSpecifier?)new SoundPathSpecifier("/Audio/Effects/snap.ogg", (AudioParams?)null);

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref EnsnaringComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (EnsnaringComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<EnsnaringComponent>(this, ref target, hookCtx, false, context))
		{
			float FreeTimeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(FreeTime, ref FreeTimeTemp, hookCtx, false, context))
			{
				FreeTimeTemp = FreeTime;
			}
			target.FreeTime = FreeTimeTemp;
			float BreakoutTimeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(BreakoutTime, ref BreakoutTimeTemp, hookCtx, false, context))
			{
				BreakoutTimeTemp = BreakoutTime;
			}
			target.BreakoutTime = BreakoutTimeTemp;
			float WalkSpeedTemp = 0f;
			if (!serialization.TryCustomCopy<float>(WalkSpeed, ref WalkSpeedTemp, hookCtx, false, context))
			{
				WalkSpeedTemp = WalkSpeed;
			}
			target.WalkSpeed = WalkSpeedTemp;
			float SprintSpeedTemp = 0f;
			if (!serialization.TryCustomCopy<float>(SprintSpeed, ref SprintSpeedTemp, hookCtx, false, context))
			{
				SprintSpeedTemp = SprintSpeed;
			}
			target.SprintSpeed = SprintSpeedTemp;
			float StaminaDamageTemp = 0f;
			if (!serialization.TryCustomCopy<float>(StaminaDamage, ref StaminaDamageTemp, hookCtx, false, context))
			{
				StaminaDamageTemp = StaminaDamage;
			}
			target.StaminaDamage = StaminaDamageTemp;
			float MaxEnsnaresTemp = 0f;
			if (!serialization.TryCustomCopy<float>(MaxEnsnares, ref MaxEnsnaresTemp, hookCtx, false, context))
			{
				MaxEnsnaresTemp = MaxEnsnares;
			}
			target.MaxEnsnares = MaxEnsnaresTemp;
			bool CanThrowTriggerTemp = false;
			if (!serialization.TryCustomCopy<bool>(CanThrowTrigger, ref CanThrowTriggerTemp, hookCtx, false, context))
			{
				CanThrowTriggerTemp = CanThrowTrigger;
			}
			target.CanThrowTrigger = CanThrowTriggerTemp;
			EntityUid? EnsnaredTemp = null;
			if (!serialization.TryCustomCopy<EntityUid?>(Ensnared, ref EnsnaredTemp, hookCtx, false, context))
			{
				EnsnaredTemp = serialization.CreateCopy<EntityUid?>(Ensnared, hookCtx, context, false);
			}
			target.Ensnared = EnsnaredTemp;
			bool CanMoveBreakoutTemp = false;
			if (!serialization.TryCustomCopy<bool>(CanMoveBreakout, ref CanMoveBreakoutTemp, hookCtx, false, context))
			{
				CanMoveBreakoutTemp = CanMoveBreakout;
			}
			target.CanMoveBreakout = CanMoveBreakoutTemp;
			SoundSpecifier EnsnareSoundTemp = null;
			if (!serialization.TryCustomCopy<SoundSpecifier>(EnsnareSound, ref EnsnareSoundTemp, hookCtx, true, context))
			{
				EnsnareSoundTemp = serialization.CreateCopy<SoundSpecifier>(EnsnareSound, hookCtx, context, false);
			}
			target.EnsnareSound = EnsnareSoundTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref EnsnaringComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EnsnaringComponent cast = (EnsnaringComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EnsnaringComponent cast = (EnsnaringComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EnsnaringComponent def = (EnsnaringComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override EnsnaringComponent Instantiate()
	{
		return new EnsnaringComponent();
	}
}
