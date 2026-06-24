using System;
using System.Numerics;
using Content.Shared.Radiation.Components;
using Content.Shared.Singularity.Components;
using Content.Shared.Singularity.Events;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Serialization;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Singularity.EntitySystems;

public abstract class SharedSingularitySystem : EntitySystem
{
	[Serializable]
	[NetSerializable]
	protected sealed class SingularityComponentState : ComponentState
	{
		public readonly byte Level;

		public SingularityComponentState(SingularityComponent singulo)
		{
			Level = singulo.Level;
		}
	}

	[Dependency]
	private SharedAppearanceSystem _visualizer;

	[Dependency]
	private SharedContainerSystem _containers;

	[Dependency]
	private SharedEventHorizonSystem _horizons;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	protected IViewVariablesManager Vvm;

	public const byte MinSingularityLevel = 0;

	public const byte MaxSingularityLevel = 6;

	public const float DistortionContainerScaling = 4f;

	public const float BaseGravityWellRadius = 2f;

	public const float BaseGravityWellAcceleration = 10f;

	public const byte SingularityBreachThreshold = 5;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<SingularityComponent, ComponentStartup>((ComponentEventHandler<SingularityComponent, ComponentStartup>)OnSingularityStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SingularityDistortionComponent, SingularityLevelChangedEvent>((ComponentEventHandler<SingularityDistortionComponent, SingularityLevelChangedEvent>)UpdateDistortion, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SingularityDistortionComponent, EntGotInsertedIntoContainerMessage>((ComponentEventHandler<SingularityDistortionComponent, EntGotInsertedIntoContainerMessage>)UpdateDistortion, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SingularityDistortionComponent, EntGotRemovedFromContainerMessage>((ComponentEventHandler<SingularityDistortionComponent, EntGotRemovedFromContainerMessage>)UpdateDistortion, (Type[])null, (Type[])null);
		ViewVariablesTypeHandler<SingularityComponent> typeHandler = Vvm.GetTypeHandler<SingularityComponent>();
		typeHandler.AddPath<byte>("Level", (ComponentPropertyGetter<SingularityComponent, byte>)((EntityUid _, SingularityComponent comp) => comp.Level), (ComponentPropertySetter<SingularityComponent, byte>)SetLevel);
		typeHandler.AddPath<float>("RadsPerLevel", (ComponentPropertyGetter<SingularityComponent, float>)((EntityUid _, SingularityComponent comp) => comp.RadsPerLevel), (ComponentPropertySetter<SingularityComponent, float>)SetRadsPerLevel);
	}

	public override void Shutdown()
	{
		ViewVariablesTypeHandler<SingularityComponent> typeHandler = Vvm.GetTypeHandler<SingularityComponent>();
		typeHandler.RemovePath("Level");
		typeHandler.RemovePath("RadsPerLevel");
		((EntitySystem)this).Shutdown();
	}

	public void SetLevel(EntityUid uid, byte value, SingularityComponent? singularity = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<SingularityComponent>(uid, ref singularity, true))
		{
			return;
		}
		value = MathHelper.Clamp(value, (byte)0, (byte)6);
		byte oldValue = singularity.Level;
		if (oldValue != value)
		{
			singularity.Level = value;
			UpdateSingularityLevel(uid, oldValue, singularity);
			if (!((EntitySystem)this).Deleted(uid, (MetaDataComponent)null))
			{
				((EntitySystem)this).Dirty(uid, (IComponent)(object)singularity, (MetaDataComponent)null);
			}
		}
	}

	public void SetRadsPerLevel(EntityUid uid, float value, SingularityComponent? singularity = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<SingularityComponent>(uid, ref singularity, true) && singularity.RadsPerLevel != value)
		{
			singularity.RadsPerLevel = value;
			UpdateRadiation(uid, singularity);
		}
	}

	public void UpdateSingularityLevel(EntityUid uid, byte oldValue, SingularityComponent? singularity = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<SingularityComponent>(uid, ref singularity, true))
		{
			EventHorizonComponent eventHorizon = default(EventHorizonComponent);
			if (((EntitySystem)this).TryComp<EventHorizonComponent>(uid, ref eventHorizon))
			{
				_horizons.SetRadius(uid, EventHorizonRadius(singularity), updateFixture: false, eventHorizon);
				_horizons.SetCanBreachContainment(uid, CanBreachContainment(singularity), updateFixture: false, eventHorizon);
				_horizons.UpdateEventHorizonFixture(uid, null, eventHorizon);
			}
			PhysicsComponent body = default(PhysicsComponent);
			if (((EntitySystem)this).TryComp<PhysicsComponent>(uid, ref body) && singularity.Level <= 1 && oldValue > 1)
			{
				_physics.SetLinearVelocity(uid, Vector2.Zero, true, true, (FixturesComponent)null, body);
			}
			AppearanceComponent appearance = default(AppearanceComponent);
			if (((EntitySystem)this).TryComp<AppearanceComponent>(uid, ref appearance))
			{
				_visualizer.SetData(uid, (Enum)SingularityAppearanceKeys.Singularity, (object)singularity.Level, appearance);
			}
			RadiationSourceComponent radiationSource = default(RadiationSourceComponent);
			if (((EntitySystem)this).TryComp<RadiationSourceComponent>(uid, ref radiationSource))
			{
				UpdateRadiation(uid, singularity, radiationSource);
			}
			((EntitySystem)this).RaiseLocalEvent<SingularityLevelChangedEvent>(uid, new SingularityLevelChangedEvent(singularity.Level, oldValue, singularity), false);
			if (singularity.Level <= 0)
			{
				((EntitySystem)this).QueueDel((EntityUid?)uid);
			}
		}
	}

	public void UpdateSingularityLevel(EntityUid uid, SingularityComponent? singularity = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<SingularityComponent>(uid, ref singularity, true))
		{
			UpdateSingularityLevel(uid, singularity.Level, singularity);
		}
	}

	private void UpdateRadiation(EntityUid uid, SingularityComponent? singularity = null, RadiationSourceComponent? rads = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<SingularityComponent, RadiationSourceComponent>(uid, ref singularity, ref rads, false))
		{
			rads.Intensity = (float)(int)singularity.Level * singularity.RadsPerLevel;
		}
	}

	public float GravPulseRange(SingularityComponent singulo)
	{
		return 2f * (float)(singulo.Level + 1);
	}

	public (float, float) GravPulseAcceleration(SingularityComponent singulo)
	{
		return (10f * (float)(int)singulo.Level, 0f);
	}

	public float EventHorizonRadius(SingularityComponent singulo)
	{
		return (float)(int)singulo.Level - 0.5f;
	}

	public bool CanBreachContainment(SingularityComponent singulo)
	{
		return singulo.Level >= 5;
	}

	public float GetFalloff(float level)
	{
		if (level != 0f)
		{
			if (level != 1f)
			{
				if (level != 2f)
				{
					if (level != 3f)
					{
						if (level != 4f)
						{
							if (level != 5f)
							{
								if (level == 6f)
								{
									return MathF.Sqrt(12f);
								}
								return -1f;
							}
							return MathF.Sqrt(12f);
						}
						return MathF.Sqrt(10f);
					}
					return MathF.Sqrt(8f);
				}
				return MathF.Sqrt(7f);
			}
			return MathF.Sqrt(6.4f);
		}
		return 9999f;
	}

	public float GetIntensity(float level)
	{
		if (level != 0f)
		{
			if (level != 1f)
			{
				if (level != 2f)
				{
					if (level != 3f)
					{
						if (level != 4f)
						{
							if (level != 5f)
							{
								if (level == 6f)
								{
									return 180000000f;
								}
								return -1f;
							}
							return 180000000f;
						}
						return 16200000f;
					}
					return 1113920f;
				}
				return 103680f;
			}
			return 3645f;
		}
		return 0f;
	}

	protected virtual void OnSingularityStartup(EntityUid uid, SingularityComponent comp, ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateSingularityLevel(uid, comp);
	}

	private void UpdateDistortion(EntityUid uid, SingularityDistortionComponent comp, SingularityLevelChangedEvent args)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		float newFalloffPower = GetFalloff((int)args.NewValue);
		float newIntensity = GetIntensity((int)args.NewValue);
		if (_containers.IsEntityInContainer(uid, (MetaDataComponent)null))
		{
			float absFalloffPower = MathF.Abs(newFalloffPower);
			float absIntensity = MathF.Abs(newIntensity);
			float factor = -0.75f;
			newFalloffPower = ((absFalloffPower > 1f) ? (newFalloffPower * MathF.Pow(absFalloffPower, factor)) : newFalloffPower);
			newIntensity = ((absIntensity > 1f) ? (newIntensity * MathF.Pow(absIntensity, factor)) : newIntensity);
		}
		comp.FalloffPower = newFalloffPower;
		comp.Intensity = newIntensity;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)comp, (MetaDataComponent)null);
	}

	private void UpdateDistortion(EntityUid uid, SingularityDistortionComponent comp, EntGotInsertedIntoContainerMessage args)
	{
		float absFalloffPower = MathF.Abs(comp.FalloffPower);
		float absIntensity = MathF.Abs(comp.Intensity);
		float factor = -0.75f;
		comp.FalloffPower = ((absFalloffPower > 1f) ? (comp.FalloffPower * MathF.Pow(absFalloffPower, factor)) : comp.FalloffPower);
		comp.Intensity = ((absIntensity > 1f) ? (comp.Intensity * MathF.Pow(absIntensity, factor)) : comp.Intensity);
	}

	private void UpdateDistortion(EntityUid uid, SingularityDistortionComponent comp, EntGotRemovedFromContainerMessage args)
	{
		float absFalloffPower = MathF.Abs(comp.FalloffPower);
		float absIntensity = MathF.Abs(comp.Intensity);
		float factor = 3f;
		comp.FalloffPower = ((absFalloffPower > 1f) ? (comp.FalloffPower * MathF.Pow(absFalloffPower, factor)) : comp.FalloffPower);
		comp.Intensity = ((absIntensity > 1f) ? (comp.Intensity * MathF.Pow(absIntensity, factor)) : comp.Intensity);
	}
}
