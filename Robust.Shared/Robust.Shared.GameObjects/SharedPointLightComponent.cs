using System;
using System.Numerics;
using Robust.Shared.Analyzers;
using Robust.Shared.Animations;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Robust.Shared.GameObjects;

[NetworkedComponent]
[Access(new Type[] { typeof(SharedPointLightSystem) })]
public abstract class SharedPointLightComponent : Component, ISerializationGenerated<SharedPointLightComponent>, ISerializationGenerated
{
	[ViewVariables(VVAccess.ReadWrite)]
	[DataField("offset", false, 1, false, false, null)]
	[Access(new Type[] { }, Other = AccessPermissions.ReadWriteExecute)]
	public Vector2 Offset = Vector2.Zero;

	[DataField("castShadows", false, 1, false, false, null)]
	public bool CastShadows = true;

	[Access(new Type[] { typeof(SharedPointLightSystem) })]
	[DataField("enabled", false, 1, false, false, null)]
	public bool Enabled = true;

	[DataField("radius", false, 1, false, false, null)]
	[Access(new Type[] { typeof(SharedPointLightSystem) })]
	public float Radius = 5f;

	[ViewVariables]
	public bool ContainerOccluded;

	[ViewVariables(VVAccess.ReadWrite)]
	[DataField("autoRot", false, 1, false, false, null)]
	public bool MaskAutoRotate;

	[ViewVariables(VVAccess.ReadWrite)]
	[DataField("mask", false, 1, false, false, null)]
	public string? MaskPath;

	[ViewVariables(VVAccess.ReadWrite)]
	[DataField("color", false, 1, false, false, null)]
	[Animatable]
	public Color Color { get; set; } = Color.White;

	[ViewVariables(VVAccess.ReadWrite)]
	[DataField("energy", false, 1, false, false, null)]
	[Animatable]
	public float Energy { get; set; } = 1f;

	[DataField("softness", false, 1, false, false, null)]
	[Animatable]
	public float Softness { get; set; } = 1f;

	[DataField(null, false, 1, false, false, null)]
	[Animatable]
	public float Falloff { get; set; } = 6.8f;

	[DataField(null, false, 1, false, false, null)]
	[Animatable]
	public float CurveFactor { get; set; }

	[Animatable]
	public bool AnimatedEnable
	{
		[Obsolete]
		get
		{
			return Enabled;
		}
		[Obsolete]
		set
		{
			IoCManager.Resolve<IEntityManager>().System<SharedPointLightSystem>().SetEnabled(base.Owner, value, this);
		}
	}

	[Animatable]
	public float AnimatedRadius
	{
		[Obsolete]
		get
		{
			return Radius;
		}
		[Obsolete]
		set
		{
			IoCManager.Resolve<IEntityManager>().System<SharedPointLightSystem>().SetRadius(base.Owner, value, this);
		}
	}

	[ViewVariables(VVAccess.ReadWrite)]
	[Animatable]
	public Angle Rotation { get; set; }

	public SharedPointLightComponent()
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)
	//IL_0006: Unknown result type (might be due to invalid IL or missing references)


	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref SharedPointLightComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		Component target2 = target;
		base.InternalCopy(ref target2, serialization, hookCtx, context);
		target = (SharedPointLightComponent)target2;
		if (!serialization.TryCustomCopy(this, ref target, hookCtx, hasHooks: false, context))
		{
			Color target3 = default(Color);
			if (!serialization.TryCustomCopy(Color, ref target3, hookCtx, hasHooks: false, context))
			{
				target3 = serialization.CreateCopy<Color>(Color, hookCtx, context);
			}
			target.Color = target3;
			Vector2 target4 = default(Vector2);
			if (!serialization.TryCustomCopy(Offset, ref target4, hookCtx, hasHooks: false, context))
			{
				target4 = serialization.CreateCopy(Offset, hookCtx, context);
			}
			target.Offset = target4;
			float target5 = 0f;
			if (!serialization.TryCustomCopy(Energy, ref target5, hookCtx, hasHooks: false, context))
			{
				target5 = Energy;
			}
			target.Energy = target5;
			float target6 = 0f;
			if (!serialization.TryCustomCopy(Softness, ref target6, hookCtx, hasHooks: false, context))
			{
				target6 = Softness;
			}
			target.Softness = target6;
			float target7 = 0f;
			if (!serialization.TryCustomCopy(Falloff, ref target7, hookCtx, hasHooks: false, context))
			{
				target7 = Falloff;
			}
			target.Falloff = target7;
			float target8 = 0f;
			if (!serialization.TryCustomCopy(CurveFactor, ref target8, hookCtx, hasHooks: false, context))
			{
				target8 = CurveFactor;
			}
			target.CurveFactor = target8;
			bool target9 = false;
			if (!serialization.TryCustomCopy(CastShadows, ref target9, hookCtx, hasHooks: false, context))
			{
				target9 = CastShadows;
			}
			target.CastShadows = target9;
			bool target10 = false;
			if (!serialization.TryCustomCopy(Enabled, ref target10, hookCtx, hasHooks: false, context))
			{
				target10 = Enabled;
			}
			target.Enabled = target10;
			float target11 = 0f;
			if (!serialization.TryCustomCopy(Radius, ref target11, hookCtx, hasHooks: false, context))
			{
				target11 = Radius;
			}
			target.Radius = target11;
			bool target12 = false;
			if (!serialization.TryCustomCopy(MaskAutoRotate, ref target12, hookCtx, hasHooks: false, context))
			{
				target12 = MaskAutoRotate;
			}
			target.MaskAutoRotate = target12;
			string target13 = null;
			if (!serialization.TryCustomCopy(MaskPath, ref target13, hookCtx, hasHooks: false, context))
			{
				target13 = MaskPath;
			}
			target.MaskPath = target13;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref SharedPointLightComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SharedPointLightComponent target2 = (SharedPointLightComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SharedPointLightComponent target2 = (SharedPointLightComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SharedPointLightComponent target2 = (SharedPointLightComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SharedPointLightComponent Instantiate()
	{
		throw new NotImplementedException();
	}
}
