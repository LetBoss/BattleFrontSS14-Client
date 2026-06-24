using System;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.LandMines;

[RegisterComponent]
public sealed class LandMineComponent : Component, ISerializationGenerated<LandMineComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public LocId? TriggerText = LocId.op_Implicit("land-mine-triggered");

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier? Sound;

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan HandDisarmDelay = TimeSpan.FromSeconds(8L);

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan ShovelDisarmDelay = TimeSpan.FromSeconds(5L);

	[DataField(null, false, 1, false, false, null)]
	public EntProtoId? DisarmedItemSpawn;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref LandMineComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (LandMineComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<LandMineComponent>(this, ref target, hookCtx, false, context))
		{
			LocId? TriggerTextTemp = null;
			if (!serialization.TryCustomCopy<LocId?>(TriggerText, ref TriggerTextTemp, hookCtx, false, context))
			{
				TriggerTextTemp = serialization.CreateCopy<LocId?>(TriggerText, hookCtx, context, false);
			}
			target.TriggerText = TriggerTextTemp;
			SoundSpecifier SoundTemp = null;
			if (!serialization.TryCustomCopy<SoundSpecifier>(Sound, ref SoundTemp, hookCtx, true, context))
			{
				SoundTemp = serialization.CreateCopy<SoundSpecifier>(Sound, hookCtx, context, false);
			}
			target.Sound = SoundTemp;
			TimeSpan HandDisarmDelayTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(HandDisarmDelay, ref HandDisarmDelayTemp, hookCtx, false, context))
			{
				HandDisarmDelayTemp = serialization.CreateCopy<TimeSpan>(HandDisarmDelay, hookCtx, context, false);
			}
			target.HandDisarmDelay = HandDisarmDelayTemp;
			TimeSpan ShovelDisarmDelayTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(ShovelDisarmDelay, ref ShovelDisarmDelayTemp, hookCtx, false, context))
			{
				ShovelDisarmDelayTemp = serialization.CreateCopy<TimeSpan>(ShovelDisarmDelay, hookCtx, context, false);
			}
			target.ShovelDisarmDelay = ShovelDisarmDelayTemp;
			EntProtoId? DisarmedItemSpawnTemp = null;
			if (!serialization.TryCustomCopy<EntProtoId?>(DisarmedItemSpawn, ref DisarmedItemSpawnTemp, hookCtx, false, context))
			{
				DisarmedItemSpawnTemp = serialization.CreateCopy<EntProtoId?>(DisarmedItemSpawn, hookCtx, context, false);
			}
			target.DisarmedItemSpawn = DisarmedItemSpawnTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref LandMineComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		LandMineComponent cast = (LandMineComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		LandMineComponent cast = (LandMineComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		LandMineComponent def = (LandMineComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override LandMineComponent Instantiate()
	{
		return new LandMineComponent();
	}
}
