using System;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Shared.Animations;
using Robust.Shared.GameObjects;
using Robust.Shared.Random;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Client.Light.Components;

[Serializable]
[ImplicitDataDefinitionForInheritors]
public abstract class LightBehaviourAnimationTrack : AnimationTrackProperty, ISerializationGenerated<LightBehaviourAnimationTrack>, ISerializationGenerated
{
	protected IEntityManager _entMan;

	protected IRobustRandom _random;

	private float _maxTime;

	private EntityUid _parent;

	[DataField("id", false, 1, false, false, null)]
	public string ID { get; set; } = string.Empty;

	[DataField("property", false, 1, false, false, null)]
	public virtual string Property { get; protected set; } = "AnimatedRadius";

	[DataField("isLooped", false, 1, false, false, null)]
	public bool IsLooped { get; set; }

	[DataField("enabled", false, 1, false, false, null)]
	public bool Enabled { get; set; }

	[DataField("startValue", false, 1, false, false, null)]
	public float StartValue { get; set; }

	[DataField("endValue", false, 1, false, false, null)]
	public float EndValue { get; set; } = 2f;

	[DataField("minDuration", false, 1, false, false, null)]
	public float MinDuration { get; set; } = -1f;

	[DataField("maxDuration", false, 1, false, false, null)]
	public float MaxDuration { get; set; } = 2f;

	[DataField("interpolate", false, 1, false, false, null)]
	public AnimationInterpolationMode InterpolateMode { get; set; }

	[ViewVariables]
	protected float MaxTime { get; set; }

	public void Initialize(EntityUid parent, IRobustRandom random, IEntityManager entMan)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		_random = random;
		_entMan = entMan;
		_parent = parent;
		PointLightComponent val = default(PointLightComponent);
		if (Enabled && _entMan.TryGetComponent<PointLightComponent>(_parent, ref val))
		{
			((SharedPointLightSystem)_entMan.System<PointLightSystem>()).SetEnabled(_parent, true, (SharedPointLightComponent)(object)val, (MetaDataComponent)null);
		}
		OnInitialize();
	}

	public void UpdatePlaybackValues(Animation owner)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		PointLightComponent val = default(PointLightComponent);
		if (_entMan.TryGetComponent<PointLightComponent>(_parent, ref val))
		{
			((SharedPointLightSystem)_entMan.System<PointLightSystem>()).SetEnabled(_parent, true, (SharedPointLightComponent)(object)val, (MetaDataComponent)null);
		}
		if (MinDuration > 0f)
		{
			MaxTime = (float)_random.NextDouble() * (MaxDuration - MinDuration) + MinDuration;
		}
		else
		{
			MaxTime = MaxDuration;
		}
		owner.Length = TimeSpan.FromSeconds(MaxTime);
	}

	public override (int KeyFrameIndex, float FramePlayingTime) InitPlayback()
	{
		OnStart();
		return (KeyFrameIndex: -1, FramePlayingTime: _maxTime);
	}

	protected void ApplyProperty(object value)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (Property == null)
		{
			throw new InvalidOperationException("Property parameter is null! Check the prototype!");
		}
		PointLightComponent val = default(PointLightComponent);
		if (_entMan.TryGetComponent<PointLightComponent>(_parent, ref val))
		{
			AnimationHelper.SetAnimatableProperty((object)val, Property, value);
		}
	}

	protected override void ApplyProperty(object context, object value)
	{
		ApplyProperty(value);
	}

	public virtual void OnInitialize()
	{
	}

	public virtual void OnStart()
	{
	}

	public LightBehaviourAnimationTrack()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref LightBehaviourAnimationTrack target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<LightBehaviourAnimationTrack>(this, ref target, hookCtx, false, context))
		{
			string iD = null;
			if (ID == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(ID, ref iD, hookCtx, false, context))
			{
				iD = ID;
			}
			target.ID = iD;
			string property = null;
			if (Property == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Property, ref property, hookCtx, false, context))
			{
				property = Property;
			}
			target.Property = property;
			bool isLooped = false;
			if (!serialization.TryCustomCopy<bool>(IsLooped, ref isLooped, hookCtx, false, context))
			{
				isLooped = IsLooped;
			}
			target.IsLooped = isLooped;
			bool enabled = false;
			if (!serialization.TryCustomCopy<bool>(Enabled, ref enabled, hookCtx, false, context))
			{
				enabled = Enabled;
			}
			target.Enabled = enabled;
			float startValue = 0f;
			if (!serialization.TryCustomCopy<float>(StartValue, ref startValue, hookCtx, false, context))
			{
				startValue = StartValue;
			}
			target.StartValue = startValue;
			float endValue = 0f;
			if (!serialization.TryCustomCopy<float>(EndValue, ref endValue, hookCtx, false, context))
			{
				endValue = EndValue;
			}
			target.EndValue = endValue;
			float minDuration = 0f;
			if (!serialization.TryCustomCopy<float>(MinDuration, ref minDuration, hookCtx, false, context))
			{
				minDuration = MinDuration;
			}
			target.MinDuration = minDuration;
			float maxDuration = 0f;
			if (!serialization.TryCustomCopy<float>(MaxDuration, ref maxDuration, hookCtx, false, context))
			{
				maxDuration = MaxDuration;
			}
			target.MaxDuration = maxDuration;
			AnimationInterpolationMode interpolateMode = (AnimationInterpolationMode)0;
			if (!serialization.TryCustomCopy<AnimationInterpolationMode>(InterpolateMode, ref interpolateMode, hookCtx, false, context))
			{
				interpolateMode = InterpolateMode;
			}
			target.InterpolateMode = interpolateMode;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref LightBehaviourAnimationTrack target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		LightBehaviourAnimationTrack target2 = (LightBehaviourAnimationTrack)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public virtual LightBehaviourAnimationTrack Instantiate()
	{
		throw new NotImplementedException();
	}
}
