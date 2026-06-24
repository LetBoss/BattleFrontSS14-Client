using System;
using Content.Shared.Damage;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._RMC14.Xenonids.Impale;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(XenoImpaleSystem) })]
public sealed class XenoSecondImpaleComponent : Component, ISerializationGenerated<XenoSecondImpaleComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public TimeSpan ImpaleAt;

	[DataField(null, false, 1, false, false, null)]
	public DamageSpecifier Damage;

	[DataField(null, false, 1, false, false, null)]
	public EntProtoId Animation = EntProtoId.op_Implicit("RMCEffectTailHit");

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier Sound = (SoundSpecifier)new SoundPathSpecifier("/Audio/_RMC14/Xeno/alien_tail_attack.ogg", (AudioParams?)null);

	[DataField(null, false, 1, false, false, null)]
	public int AP = 10;

	[DataField(null, false, 1, false, false, null)]
	public EntityUid Origin;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref XenoSecondImpaleComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (XenoSecondImpaleComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<XenoSecondImpaleComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		TimeSpan ImpaleAtTemp = default(TimeSpan);
		if (!serialization.TryCustomCopy<TimeSpan>(ImpaleAt, ref ImpaleAtTemp, hookCtx, false, context))
		{
			ImpaleAtTemp = serialization.CreateCopy<TimeSpan>(ImpaleAt, hookCtx, context, false);
		}
		target.ImpaleAt = ImpaleAtTemp;
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
		EntProtoId AnimationTemp = default(EntProtoId);
		if (!serialization.TryCustomCopy<EntProtoId>(Animation, ref AnimationTemp, hookCtx, false, context))
		{
			AnimationTemp = serialization.CreateCopy<EntProtoId>(Animation, hookCtx, context, false);
		}
		target.Animation = AnimationTemp;
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
		int APTemp = 0;
		if (!serialization.TryCustomCopy<int>(AP, ref APTemp, hookCtx, false, context))
		{
			APTemp = AP;
		}
		target.AP = APTemp;
		EntityUid OriginTemp = default(EntityUid);
		if (!serialization.TryCustomCopy<EntityUid>(Origin, ref OriginTemp, hookCtx, false, context))
		{
			OriginTemp = serialization.CreateCopy<EntityUid>(Origin, hookCtx, context, false);
		}
		target.Origin = OriginTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref XenoSecondImpaleComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoSecondImpaleComponent cast = (XenoSecondImpaleComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoSecondImpaleComponent cast = (XenoSecondImpaleComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoSecondImpaleComponent def = (XenoSecondImpaleComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override XenoSecondImpaleComponent Instantiate()
	{
		return new XenoSecondImpaleComponent();
	}
}
