using System;
using Content.Shared.Atmos.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Atmos.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedFirestarterSystem) })]
public sealed class FirestarterComponent : Component, ISerializationGenerated<FirestarterComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float IgnitionRadius = 4f;

	[DataField(null, false, 1, false, false, null)]
	public EntProtoId? FireStarterAction = EntProtoId.op_Implicit("ActionFireStarter");

	[DataField(null, false, 1, false, false, null)]
	public EntityUid? FireStarterActionEntity;

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier IgniteSound = (SoundSpecifier)new SoundPathSpecifier("/Audio/Magic/rumble.ogg", (AudioParams?)null);

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref FirestarterComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (FirestarterComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<FirestarterComponent>(this, ref target, hookCtx, false, context))
		{
			float IgnitionRadiusTemp = 0f;
			if (!serialization.TryCustomCopy<float>(IgnitionRadius, ref IgnitionRadiusTemp, hookCtx, false, context))
			{
				IgnitionRadiusTemp = IgnitionRadius;
			}
			target.IgnitionRadius = IgnitionRadiusTemp;
			EntProtoId? FireStarterActionTemp = null;
			if (!serialization.TryCustomCopy<EntProtoId?>(FireStarterAction, ref FireStarterActionTemp, hookCtx, false, context))
			{
				FireStarterActionTemp = serialization.CreateCopy<EntProtoId?>(FireStarterAction, hookCtx, context, false);
			}
			target.FireStarterAction = FireStarterActionTemp;
			EntityUid? FireStarterActionEntityTemp = null;
			if (!serialization.TryCustomCopy<EntityUid?>(FireStarterActionEntity, ref FireStarterActionEntityTemp, hookCtx, false, context))
			{
				FireStarterActionEntityTemp = serialization.CreateCopy<EntityUid?>(FireStarterActionEntity, hookCtx, context, false);
			}
			target.FireStarterActionEntity = FireStarterActionEntityTemp;
			SoundSpecifier IgniteSoundTemp = null;
			if (IgniteSound == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(IgniteSound, ref IgniteSoundTemp, hookCtx, true, context))
			{
				IgniteSoundTemp = serialization.CreateCopy<SoundSpecifier>(IgniteSound, hookCtx, context, false);
			}
			target.IgniteSound = IgniteSoundTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref FirestarterComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FirestarterComponent cast = (FirestarterComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FirestarterComponent cast = (FirestarterComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FirestarterComponent def = (FirestarterComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override FirestarterComponent Instantiate()
	{
		return new FirestarterComponent();
	}
}
