using System;
using System.Collections.Generic;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Client.Damage;

[RegisterComponent]
public sealed class DamageVisualsComponent : Component, ISerializationGenerated<DamageVisualsComponent>, ISerializationGenerated
{
	[DataField("thresholds", false, 1, true, false, null)]
	public List<FixedPoint2> Thresholds = new List<FixedPoint2>();

	[DataField("targetLayers", false, 1, false, false, null)]
	public List<Enum>? TargetLayers;

	[DataField("damageOverlayGroups", false, 1, false, false, null)]
	public Dictionary<string, DamageVisualizerSprite>? DamageOverlayGroups;

	[DataField("overlay", false, 1, false, false, null)]
	public bool Overlay = true;

	[DataField("damageGroup", false, 1, false, false, null)]
	public string? DamageGroup;

	[DataField("damageDivisor", false, 1, false, false, null)]
	public float Divisor = 1f;

	[DataField("trackAllDamage", false, 1, false, false, null)]
	public bool TrackAllDamage;

	[DataField("damageOverlay", false, 1, false, false, null)]
	public DamageVisualizerSprite? DamageOverlay;

	public readonly List<Enum> TargetLayerMapKeys = new List<Enum>();

	public bool Disabled;

	public bool Valid = true;

	public FixedPoint2 LastDamageThreshold = FixedPoint2.Zero;

	public readonly Dictionary<object, bool> DisabledLayers = new Dictionary<object, bool>();

	public readonly Dictionary<object, string> LayerMapKeyStates = new Dictionary<object, string>();

	public readonly Dictionary<string, FixedPoint2> LastThresholdPerGroup = new Dictionary<string, FixedPoint2>();

	public string TopMostLayerKey;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref DamageVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component val = (Component)(object)target;
		((Component)this).InternalCopy(ref val, serialization, hookCtx, context);
		target = (DamageVisualsComponent)(object)val;
		if (serialization.TryCustomCopy<DamageVisualsComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		List<FixedPoint2> thresholds = null;
		if (Thresholds == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<List<FixedPoint2>>(Thresholds, ref thresholds, hookCtx, true, context))
		{
			thresholds = serialization.CreateCopy<List<FixedPoint2>>(Thresholds, hookCtx, context, false);
		}
		target.Thresholds = thresholds;
		List<Enum> targetLayers = null;
		if (!serialization.TryCustomCopy<List<Enum>>(TargetLayers, ref targetLayers, hookCtx, true, context))
		{
			targetLayers = serialization.CreateCopy<List<Enum>>(TargetLayers, hookCtx, context, false);
		}
		target.TargetLayers = targetLayers;
		Dictionary<string, DamageVisualizerSprite> damageOverlayGroups = null;
		if (!serialization.TryCustomCopy<Dictionary<string, DamageVisualizerSprite>>(DamageOverlayGroups, ref damageOverlayGroups, hookCtx, true, context))
		{
			damageOverlayGroups = serialization.CreateCopy<Dictionary<string, DamageVisualizerSprite>>(DamageOverlayGroups, hookCtx, context, false);
		}
		target.DamageOverlayGroups = damageOverlayGroups;
		bool overlay = false;
		if (!serialization.TryCustomCopy<bool>(Overlay, ref overlay, hookCtx, false, context))
		{
			overlay = Overlay;
		}
		target.Overlay = overlay;
		string damageGroup = null;
		if (!serialization.TryCustomCopy<string>(DamageGroup, ref damageGroup, hookCtx, false, context))
		{
			damageGroup = DamageGroup;
		}
		target.DamageGroup = damageGroup;
		float divisor = 0f;
		if (!serialization.TryCustomCopy<float>(Divisor, ref divisor, hookCtx, false, context))
		{
			divisor = Divisor;
		}
		target.Divisor = divisor;
		bool trackAllDamage = false;
		if (!serialization.TryCustomCopy<bool>(TrackAllDamage, ref trackAllDamage, hookCtx, false, context))
		{
			trackAllDamage = TrackAllDamage;
		}
		target.TrackAllDamage = trackAllDamage;
		DamageVisualizerSprite damageOverlay = null;
		if (!serialization.TryCustomCopy<DamageVisualizerSprite>(DamageOverlay, ref damageOverlay, hookCtx, false, context))
		{
			if (DamageOverlay == null)
			{
				damageOverlay = null;
			}
			else
			{
				serialization.CopyTo<DamageVisualizerSprite>(DamageOverlay, ref damageOverlay, hookCtx, context, false);
			}
		}
		target.DamageOverlay = damageOverlay;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref DamageVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DamageVisualsComponent target2 = (DamageVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (Component)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DamageVisualsComponent target2 = (DamageVisualsComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DamageVisualsComponent target2 = (DamageVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override DamageVisualsComponent Instantiate()
	{
		return new DamageVisualsComponent();
	}
}
