using System;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._PUBG.Emote;

[RegisterComponent]
public sealed class PubgEmoteComponent : Component, ISerializationGenerated<PubgEmoteComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public string EmoteId = string.Empty;

	[DataField(null, false, 1, false, false, null)]
	public string Description = string.Empty;

	[DataField(null, false, 1, false, false, null)]
	public EmotePlayMode PlayMode;

	[DataField(null, false, 1, false, false, null)]
	public float? Duration;

	[DataField(null, false, 1, false, false, null)]
	public int? RepeatCount;

	[DataField(null, false, 1, true, false, null)]
	public string RsiPath = string.Empty;

	[DataField(null, false, 1, true, false, null)]
	public string StateName = string.Empty;

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier? Sound;

	[DataField(null, false, 1, false, false, null)]
	public bool Disabled;

	[DataField(null, false, 1, false, false, null)]
	public float Cooldown = 20f;

	[DataField(null, false, 1, false, false, null)]
	public float Scale = 1f;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PubgEmoteComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (PubgEmoteComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<PubgEmoteComponent>(this, ref target, hookCtx, false, context))
		{
			string EmoteIdTemp = null;
			if (EmoteId == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(EmoteId, ref EmoteIdTemp, hookCtx, false, context))
			{
				EmoteIdTemp = EmoteId;
			}
			target.EmoteId = EmoteIdTemp;
			string DescriptionTemp = null;
			if (Description == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Description, ref DescriptionTemp, hookCtx, false, context))
			{
				DescriptionTemp = Description;
			}
			target.Description = DescriptionTemp;
			EmotePlayMode PlayModeTemp = EmotePlayMode.Once;
			if (!serialization.TryCustomCopy<EmotePlayMode>(PlayMode, ref PlayModeTemp, hookCtx, false, context))
			{
				PlayModeTemp = PlayMode;
			}
			target.PlayMode = PlayModeTemp;
			float? DurationTemp = null;
			if (!serialization.TryCustomCopy<float?>(Duration, ref DurationTemp, hookCtx, false, context))
			{
				DurationTemp = Duration;
			}
			target.Duration = DurationTemp;
			int? RepeatCountTemp = null;
			if (!serialization.TryCustomCopy<int?>(RepeatCount, ref RepeatCountTemp, hookCtx, false, context))
			{
				RepeatCountTemp = RepeatCount;
			}
			target.RepeatCount = RepeatCountTemp;
			string RsiPathTemp = null;
			if (RsiPath == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(RsiPath, ref RsiPathTemp, hookCtx, false, context))
			{
				RsiPathTemp = RsiPath;
			}
			target.RsiPath = RsiPathTemp;
			string StateNameTemp = null;
			if (StateName == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(StateName, ref StateNameTemp, hookCtx, false, context))
			{
				StateNameTemp = StateName;
			}
			target.StateName = StateNameTemp;
			SoundSpecifier SoundTemp = null;
			if (!serialization.TryCustomCopy<SoundSpecifier>(Sound, ref SoundTemp, hookCtx, true, context))
			{
				SoundTemp = serialization.CreateCopy<SoundSpecifier>(Sound, hookCtx, context, false);
			}
			target.Sound = SoundTemp;
			bool DisabledTemp = false;
			if (!serialization.TryCustomCopy<bool>(Disabled, ref DisabledTemp, hookCtx, false, context))
			{
				DisabledTemp = Disabled;
			}
			target.Disabled = DisabledTemp;
			float CooldownTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Cooldown, ref CooldownTemp, hookCtx, false, context))
			{
				CooldownTemp = Cooldown;
			}
			target.Cooldown = CooldownTemp;
			float ScaleTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Scale, ref ScaleTemp, hookCtx, false, context))
			{
				ScaleTemp = Scale;
			}
			target.Scale = ScaleTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PubgEmoteComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgEmoteComponent cast = (PubgEmoteComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgEmoteComponent cast = (PubgEmoteComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgEmoteComponent def = (PubgEmoteComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PubgEmoteComponent Instantiate()
	{
		return new PubgEmoteComponent();
	}
}
