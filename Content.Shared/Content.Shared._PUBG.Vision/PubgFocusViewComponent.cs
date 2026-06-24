using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._PUBG.Vision;

[RegisterComponent]
public sealed class PubgFocusViewComponent : Component, ISerializationGenerated<PubgFocusViewComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float OffsetTiles = 4f;

	[DataField(null, false, 1, false, false, null)]
	public float PvsIncrease = 0.4f;

	[DataField(null, false, 1, false, false, null)]
	public bool FixedOffset;

	[DataField(null, false, 1, false, false, null)]
	public bool AdjustShotCoordinates = true;

	[DataField(null, false, 1, false, false, null)]
	public bool Active;

	[DataField(null, false, 1, false, false, null)]
	public EntityUid? ActiveUser;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PubgFocusViewComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (PubgFocusViewComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<PubgFocusViewComponent>(this, ref target, hookCtx, false, context))
		{
			float OffsetTilesTemp = 0f;
			if (!serialization.TryCustomCopy<float>(OffsetTiles, ref OffsetTilesTemp, hookCtx, false, context))
			{
				OffsetTilesTemp = OffsetTiles;
			}
			target.OffsetTiles = OffsetTilesTemp;
			float PvsIncreaseTemp = 0f;
			if (!serialization.TryCustomCopy<float>(PvsIncrease, ref PvsIncreaseTemp, hookCtx, false, context))
			{
				PvsIncreaseTemp = PvsIncrease;
			}
			target.PvsIncrease = PvsIncreaseTemp;
			bool FixedOffsetTemp = false;
			if (!serialization.TryCustomCopy<bool>(FixedOffset, ref FixedOffsetTemp, hookCtx, false, context))
			{
				FixedOffsetTemp = FixedOffset;
			}
			target.FixedOffset = FixedOffsetTemp;
			bool AdjustShotCoordinatesTemp = false;
			if (!serialization.TryCustomCopy<bool>(AdjustShotCoordinates, ref AdjustShotCoordinatesTemp, hookCtx, false, context))
			{
				AdjustShotCoordinatesTemp = AdjustShotCoordinates;
			}
			target.AdjustShotCoordinates = AdjustShotCoordinatesTemp;
			bool ActiveTemp = false;
			if (!serialization.TryCustomCopy<bool>(Active, ref ActiveTemp, hookCtx, false, context))
			{
				ActiveTemp = Active;
			}
			target.Active = ActiveTemp;
			EntityUid? ActiveUserTemp = null;
			if (!serialization.TryCustomCopy<EntityUid?>(ActiveUser, ref ActiveUserTemp, hookCtx, false, context))
			{
				ActiveUserTemp = serialization.CreateCopy<EntityUid?>(ActiveUser, hookCtx, context, false);
			}
			target.ActiveUser = ActiveUserTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PubgFocusViewComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgFocusViewComponent cast = (PubgFocusViewComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgFocusViewComponent cast = (PubgFocusViewComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgFocusViewComponent def = (PubgFocusViewComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PubgFocusViewComponent Instantiate()
	{
		return new PubgFocusViewComponent();
	}
}
