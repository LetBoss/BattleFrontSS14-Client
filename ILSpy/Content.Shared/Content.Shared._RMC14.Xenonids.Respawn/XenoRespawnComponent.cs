using System;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._RMC14.Xenonids.Respawn;

[RegisterComponent]
[NetworkedComponent]
public sealed class XenoRespawnComponent : Component, ISerializationGenerated<XenoRespawnComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public EntityUid? Hive;

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan RespawnAt;

	[DataField(null, false, 1, false, false, null)]
	public bool RespawnAtCorpse;

	[DataField(null, false, 1, false, false, null)]
	public EntityCoordinates? CorpseLocation;

	[DataField(null, false, 1, false, false, null)]
	public EntProtoId Larva = EntProtoId.op_Implicit("CMXenoLarva");

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier CorpseSound = (SoundSpecifier)new SoundPathSpecifier("/Audio/_RMC14/Xeno/xeno_newlarva.ogg", (AudioParams?)null);

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref XenoRespawnComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (XenoRespawnComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<XenoRespawnComponent>(this, ref target, hookCtx, false, context))
		{
			EntityUid? HiveTemp = null;
			if (!serialization.TryCustomCopy<EntityUid?>(Hive, ref HiveTemp, hookCtx, false, context))
			{
				HiveTemp = serialization.CreateCopy<EntityUid?>(Hive, hookCtx, context, false);
			}
			target.Hive = HiveTemp;
			TimeSpan RespawnAtTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(RespawnAt, ref RespawnAtTemp, hookCtx, false, context))
			{
				RespawnAtTemp = serialization.CreateCopy<TimeSpan>(RespawnAt, hookCtx, context, false);
			}
			target.RespawnAt = RespawnAtTemp;
			bool RespawnAtCorpseTemp = false;
			if (!serialization.TryCustomCopy<bool>(RespawnAtCorpse, ref RespawnAtCorpseTemp, hookCtx, false, context))
			{
				RespawnAtCorpseTemp = RespawnAtCorpse;
			}
			target.RespawnAtCorpse = RespawnAtCorpseTemp;
			EntityCoordinates? CorpseLocationTemp = null;
			if (!serialization.TryCustomCopy<EntityCoordinates?>(CorpseLocation, ref CorpseLocationTemp, hookCtx, false, context))
			{
				CorpseLocationTemp = serialization.CreateCopy<EntityCoordinates?>(CorpseLocation, hookCtx, context, false);
			}
			target.CorpseLocation = CorpseLocationTemp;
			EntProtoId LarvaTemp = default(EntProtoId);
			if (!serialization.TryCustomCopy<EntProtoId>(Larva, ref LarvaTemp, hookCtx, false, context))
			{
				LarvaTemp = serialization.CreateCopy<EntProtoId>(Larva, hookCtx, context, false);
			}
			target.Larva = LarvaTemp;
			SoundSpecifier CorpseSoundTemp = null;
			if (CorpseSound == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(CorpseSound, ref CorpseSoundTemp, hookCtx, true, context))
			{
				CorpseSoundTemp = serialization.CreateCopy<SoundSpecifier>(CorpseSound, hookCtx, context, false);
			}
			target.CorpseSound = CorpseSoundTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref XenoRespawnComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoRespawnComponent cast = (XenoRespawnComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoRespawnComponent cast = (XenoRespawnComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoRespawnComponent def = (XenoRespawnComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override XenoRespawnComponent Instantiate()
	{
		return new XenoRespawnComponent();
	}
}
