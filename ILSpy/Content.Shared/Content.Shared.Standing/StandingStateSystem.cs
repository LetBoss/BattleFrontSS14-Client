using System;
using System.Collections.Generic;
using Content.Shared.Hands.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Rotation;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;

namespace Content.Shared.Standing;

public sealed class StandingStateSystem : EntitySystem
{
	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedPhysicsSystem _physics;

	private const int StandingCollisionLayer = 4;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<StandingStateComponent, AttemptMobCollideEvent>((EntityEventRefHandler<StandingStateComponent, AttemptMobCollideEvent>)OnMobCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StandingStateComponent, AttemptMobTargetCollideEvent>((EntityEventRefHandler<StandingStateComponent, AttemptMobTargetCollideEvent>)OnMobTargetCollide, (Type[])null, (Type[])null);
	}

	private void OnMobTargetCollide(Entity<StandingStateComponent> ent, ref AttemptMobTargetCollideEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		if (!ent.Comp.Standing)
		{
			args.Cancelled = true;
		}
	}

	private void OnMobCollide(Entity<StandingStateComponent> ent, ref AttemptMobCollideEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		if (!ent.Comp.Standing)
		{
			args.Cancelled = true;
		}
	}

	public bool IsDown(EntityUid uid, StandingStateComponent? standingState = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<StandingStateComponent>(uid, ref standingState, false))
		{
			return false;
		}
		return !standingState.Standing;
	}

	public bool Down(EntityUid uid, bool playSound = true, bool dropHeldItems = true, bool force = false, bool changeCollision = false, StandingStateComponent? standingState = null, AppearanceComponent? appearance = null, HandsComponent? hands = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Invalid comparison between Unknown and I4
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<StandingStateComponent>(uid, ref standingState, false))
		{
			return false;
		}
		((EntitySystem)this).Resolve<AppearanceComponent, HandsComponent>(uid, ref appearance, ref hands, false);
		if (!standingState.Standing)
		{
			return true;
		}
		if (dropHeldItems && hands != null)
		{
			DropHandItemsEvent ev = new DropHandItemsEvent();
			((EntitySystem)this).RaiseLocalEvent<DropHandItemsEvent>(uid, ref ev, false);
		}
		if (!force)
		{
			DownAttemptEvent msg = new DownAttemptEvent();
			((EntitySystem)this).RaiseLocalEvent<DownAttemptEvent>(uid, msg, false);
			if (((CancellableEntityEventArgs)msg).Cancelled)
			{
				return false;
			}
		}
		standingState.Standing = false;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)standingState, (MetaDataComponent)null);
		((EntitySystem)this).RaiseLocalEvent<DownedEvent>(uid, new DownedEvent(), false);
		_appearance.SetData(uid, (Enum)RotationVisuals.RotationState, (object)RotationState.Horizontal, appearance);
		FixturesComponent fixtureComponent = default(FixturesComponent);
		if (changeCollision && ((EntitySystem)this).TryComp<FixturesComponent>(uid, ref fixtureComponent))
		{
			foreach (var (key, fixture) in fixtureComponent.Fixtures)
			{
				if ((fixture.CollisionMask & 4) != 0)
				{
					standingState.ChangedFixtures.Add(key);
					_physics.SetCollisionMask(uid, key, fixture, fixture.CollisionMask & -5, fixtureComponent, (PhysicsComponent)null);
				}
			}
		}
		if ((int)((Component)standingState).LifeStage <= 5)
		{
			return true;
		}
		if (playSound)
		{
			_audio.PlayPredicted(standingState.DownSound, uid, (EntityUid?)uid, (AudioParams?)null);
		}
		return true;
	}

	public bool Stand(EntityUid uid, StandingStateComponent? standingState = null, AppearanceComponent? appearance = null, bool force = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<StandingStateComponent>(uid, ref standingState, false))
		{
			return false;
		}
		((EntitySystem)this).Resolve<AppearanceComponent>(uid, ref appearance, false);
		if (standingState.Standing)
		{
			return true;
		}
		if (!force)
		{
			StandAttemptEvent msg = new StandAttemptEvent();
			((EntitySystem)this).RaiseLocalEvent<StandAttemptEvent>(uid, msg, false);
			if (((CancellableEntityEventArgs)msg).Cancelled)
			{
				return false;
			}
		}
		standingState.Standing = true;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)standingState, (MetaDataComponent)null);
		((EntitySystem)this).RaiseLocalEvent<StoodEvent>(uid, new StoodEvent(), false);
		_appearance.SetData(uid, (Enum)RotationVisuals.RotationState, (object)RotationState.Vertical, appearance);
		FixturesComponent fixtureComponent = default(FixturesComponent);
		if (((EntitySystem)this).TryComp<FixturesComponent>(uid, ref fixtureComponent))
		{
			foreach (string key in standingState.ChangedFixtures)
			{
				if (fixtureComponent.Fixtures.TryGetValue(key, out var fixture))
				{
					_physics.SetCollisionMask(uid, key, fixture, fixture.CollisionMask | 4, fixtureComponent, (PhysicsComponent)null);
				}
			}
		}
		standingState.ChangedFixtures.Clear();
		return true;
	}
}
