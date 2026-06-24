using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.Atmos.Components;

[RegisterComponent]
public sealed class FireVisualsComponent : Component, ISerializationGenerated<FireVisualsComponent>, ISerializationGenerated
{
	[DataField("fireStackAlternateState", false, 1, false, false, null)]
	public int FireStackAlternateState = 3;

	[DataField("normalState", false, 1, false, false, null)]
	public string? NormalState;

	[DataField("alternateState", false, 1, false, false, null)]
	public string? AlternateState;

	[DataField("sprite", false, 1, false, false, null)]
	public string? Sprite;

	[DataField("lightEnergyPerStack", false, 1, false, false, null)]
	public float LightEnergyPerStack = 0.5f;

	[DataField("lightRadiusPerStack", false, 1, false, false, null)]
	public float LightRadiusPerStack = 0.3f;

	[DataField("maxLightEnergy", false, 1, false, false, null)]
	public float MaxLightEnergy = 10f;

	[DataField("maxLightRadius", false, 1, false, false, null)]
	public float MaxLightRadius = 4f;

	[DataField("lightColor", false, 1, false, false, null)]
	public Color LightColor = Color.Orange;

	public EntityUid? LightEntity;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref FireVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		Component val = (Component)(object)target;
		((Component)this).InternalCopy(ref val, serialization, hookCtx, context);
		target = (FireVisualsComponent)(object)val;
		if (!serialization.TryCustomCopy<FireVisualsComponent>(this, ref target, hookCtx, false, context))
		{
			int fireStackAlternateState = 0;
			if (!serialization.TryCustomCopy<int>(FireStackAlternateState, ref fireStackAlternateState, hookCtx, false, context))
			{
				fireStackAlternateState = FireStackAlternateState;
			}
			target.FireStackAlternateState = fireStackAlternateState;
			string normalState = null;
			if (!serialization.TryCustomCopy<string>(NormalState, ref normalState, hookCtx, false, context))
			{
				normalState = NormalState;
			}
			target.NormalState = normalState;
			string alternateState = null;
			if (!serialization.TryCustomCopy<string>(AlternateState, ref alternateState, hookCtx, false, context))
			{
				alternateState = AlternateState;
			}
			target.AlternateState = alternateState;
			string sprite = null;
			if (!serialization.TryCustomCopy<string>(Sprite, ref sprite, hookCtx, false, context))
			{
				sprite = Sprite;
			}
			target.Sprite = sprite;
			float lightEnergyPerStack = 0f;
			if (!serialization.TryCustomCopy<float>(LightEnergyPerStack, ref lightEnergyPerStack, hookCtx, false, context))
			{
				lightEnergyPerStack = LightEnergyPerStack;
			}
			target.LightEnergyPerStack = lightEnergyPerStack;
			float lightRadiusPerStack = 0f;
			if (!serialization.TryCustomCopy<float>(LightRadiusPerStack, ref lightRadiusPerStack, hookCtx, false, context))
			{
				lightRadiusPerStack = LightRadiusPerStack;
			}
			target.LightRadiusPerStack = lightRadiusPerStack;
			float maxLightEnergy = 0f;
			if (!serialization.TryCustomCopy<float>(MaxLightEnergy, ref maxLightEnergy, hookCtx, false, context))
			{
				maxLightEnergy = MaxLightEnergy;
			}
			target.MaxLightEnergy = maxLightEnergy;
			float maxLightRadius = 0f;
			if (!serialization.TryCustomCopy<float>(MaxLightRadius, ref maxLightRadius, hookCtx, false, context))
			{
				maxLightRadius = MaxLightRadius;
			}
			target.MaxLightRadius = maxLightRadius;
			Color lightColor = default(Color);
			if (!serialization.TryCustomCopy<Color>(LightColor, ref lightColor, hookCtx, false, context))
			{
				lightColor = serialization.CreateCopy<Color>(LightColor, hookCtx, context, false);
			}
			target.LightColor = lightColor;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref FireVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FireVisualsComponent target2 = (FireVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (Component)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FireVisualsComponent target2 = (FireVisualsComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FireVisualsComponent target2 = (FireVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override FireVisualsComponent Instantiate()
	{
		return new FireVisualsComponent();
	}
}
