using System;
using Content.Shared.Chat.Prototypes;
using Content.Shared.Damage;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._RMC14.Xenonids.Rend;

[RegisterComponent]
[NetworkedComponent]
public sealed class XenoRendComponent : Component, ISerializationGenerated<XenoRendComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public DamageSpecifier Damage = new DamageSpecifier();

	[DataField(null, false, 1, false, false, null)]
	public float Range = 1.5f;

	[DataField(null, false, 1, false, false, null)]
	public EntProtoId Effect = EntProtoId.op_Implicit("RMCEffectExtraSlash");

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier Sound = (SoundSpecifier)new SoundCollectionSpecifier("AlienClaw", (AudioParams?)null);

	[DataField(null, false, 1, false, false, null)]
	public ProtoId<EmotePrototype> HissEmote = ProtoId<EmotePrototype>.op_Implicit("Hiss");

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref XenoRendComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (XenoRendComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<XenoRendComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		DamageSpecifier DamageTemp = null;
		if (Damage == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<DamageSpecifier>(Damage, ref DamageTemp, hookCtx, false, context))
		{
			if (Damage == null)
			{
				DamageTemp = null;
			}
			else
			{
				serialization.CopyTo<DamageSpecifier>(Damage, ref DamageTemp, hookCtx, context, true);
			}
		}
		target.Damage = DamageTemp;
		float RangeTemp = 0f;
		if (!serialization.TryCustomCopy<float>(Range, ref RangeTemp, hookCtx, false, context))
		{
			RangeTemp = Range;
		}
		target.Range = RangeTemp;
		EntProtoId EffectTemp = default(EntProtoId);
		if (!serialization.TryCustomCopy<EntProtoId>(Effect, ref EffectTemp, hookCtx, false, context))
		{
			EffectTemp = serialization.CreateCopy<EntProtoId>(Effect, hookCtx, context, false);
		}
		target.Effect = EffectTemp;
		SoundSpecifier SoundTemp = null;
		if (Sound == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<SoundSpecifier>(Sound, ref SoundTemp, hookCtx, true, context))
		{
			SoundTemp = serialization.CreateCopy<SoundSpecifier>(Sound, hookCtx, context, false);
		}
		target.Sound = SoundTemp;
		ProtoId<EmotePrototype> HissEmoteTemp = default(ProtoId<EmotePrototype>);
		if (!serialization.TryCustomCopy<ProtoId<EmotePrototype>>(HissEmote, ref HissEmoteTemp, hookCtx, false, context))
		{
			HissEmoteTemp = serialization.CreateCopy<ProtoId<EmotePrototype>>(HissEmote, hookCtx, context, false);
		}
		target.HissEmote = HissEmoteTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref XenoRendComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoRendComponent cast = (XenoRendComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoRendComponent cast = (XenoRendComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoRendComponent def = (XenoRendComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override XenoRendComponent Instantiate()
	{
		return new XenoRendComponent();
	}
}
