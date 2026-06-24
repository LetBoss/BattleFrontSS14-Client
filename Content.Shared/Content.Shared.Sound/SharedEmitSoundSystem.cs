using System;
using Content.Shared.Audio;
using Content.Shared.Hands;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Maps;
using Content.Shared.Mobs;
using Content.Shared.Popups;
using Content.Shared.Sound.Components;
using Content.Shared.Throwing;
using Content.Shared.UserInterface;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared.Sound;

public abstract class SharedEmitSoundSystem : EntitySystem
{
	[Dependency]
	protected IGameTiming Timing;

	[Dependency]
	private INetManager _netMan;

	[Dependency]
	protected IRobustRandom Random;

	[Dependency]
	private SharedAmbientSoundSystem _ambient;

	[Dependency]
	private SharedAudioSystem _audioSystem;

	[Dependency]
	protected SharedPopupSystem Popup;

	[Dependency]
	private SharedMapSystem _map;

	[Dependency]
	private EntityWhitelistSystem _whitelistSystem;

	[Dependency]
	private TurfSystem _turf;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<EmitSoundOnSpawnComponent, MapInitEvent>((ComponentEventHandler<EmitSoundOnSpawnComponent, MapInitEvent>)OnEmitSpawnOnInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EmitSoundOnLandComponent, LandEvent>((ComponentEventRefHandler<EmitSoundOnLandComponent, LandEvent>)OnEmitSoundOnLand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EmitSoundOnUseComponent, UseInHandEvent>((ComponentEventHandler<EmitSoundOnUseComponent, UseInHandEvent>)OnEmitSoundOnUseInHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EmitSoundOnThrowComponent, ThrownEvent>((ComponentEventRefHandler<EmitSoundOnThrowComponent, ThrownEvent>)OnEmitSoundOnThrown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EmitSoundOnActivateComponent, ActivateInWorldEvent>((ComponentEventHandler<EmitSoundOnActivateComponent, ActivateInWorldEvent>)OnEmitSoundOnActivateInWorld, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EmitSoundOnPickupComponent, GotEquippedHandEvent>((ComponentEventHandler<EmitSoundOnPickupComponent, GotEquippedHandEvent>)OnEmitSoundOnPickup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EmitSoundOnDropComponent, DroppedEvent>((ComponentEventHandler<EmitSoundOnDropComponent, DroppedEvent>)OnEmitSoundOnDrop, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EmitSoundOnInteractUsingComponent, InteractUsingEvent>((EntityEventRefHandler<EmitSoundOnInteractUsingComponent, InteractUsingEvent>)OnEmitSoundOnInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EmitSoundOnUIOpenComponent, AfterActivatableUIOpenEvent>((ComponentEventHandler<EmitSoundOnUIOpenComponent, AfterActivatableUIOpenEvent>)HandleEmitSoundOnUIOpen, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EmitSoundOnCollideComponent, StartCollideEvent>((ComponentEventRefHandler<EmitSoundOnCollideComponent, StartCollideEvent>)OnEmitSoundOnCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SoundWhileAliveComponent, MobStateChangedEvent>((EntityEventRefHandler<SoundWhileAliveComponent, MobStateChangedEvent>)OnMobState, (Type[])null, (Type[])null);
		SubscribeEmitComponent<EmitSoundOnActivateComponent>();
		SubscribeEmitComponent<EmitSoundOnCollideComponent>();
		SubscribeEmitComponent<EmitSoundOnDropComponent>();
		SubscribeEmitComponent<EmitSoundOnInteractUsingComponent>();
		SubscribeEmitComponent<EmitSoundOnLandComponent>();
		SubscribeEmitComponent<EmitSoundOnPickupComponent>();
		SubscribeEmitComponent<EmitSoundOnSpawnComponent>();
		SubscribeEmitComponent<EmitSoundOnThrowComponent>();
		SubscribeEmitComponent<EmitSoundOnUIOpenComponent>();
		SubscribeEmitComponent<EmitSoundOnUseComponent>();
		void SubscribeEmitComponent<T>() where T : notnull, BaseEmitSoundComponent
		{
			((EntitySystem)this).SubscribeLocalEvent<T, ComponentGetState>((EntityEventRefHandler<T, ComponentGetState>)GetBaseEmitState<T>, (Type[])null, (Type[])null);
			((EntitySystem)this).SubscribeLocalEvent<T, ComponentHandleState>((EntityEventRefHandler<T, ComponentHandleState>)HandleBaseEmitState<T>, (Type[])null, (Type[])null);
		}
	}

	private static void GetBaseEmitState<T>(Entity<T> ent, ref ComponentGetState args) where T : BaseEmitSoundComponent
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((ComponentGetState)(ref args)).State = (IComponentState)(object)new EmitSoundComponentState(ent.Comp.Sound);
	}

	private static void HandleBaseEmitState<T>(Entity<T> ent, ref ComponentHandleState args) where T : BaseEmitSoundComponent
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		if (((ComponentHandleState)(ref args)).Current is EmitSoundComponentState state)
		{
			T comp = ent.Comp;
			SoundSpecifier sound = state.Sound;
			SoundPathSpecifier pathSpec = (SoundPathSpecifier)(object)((sound is SoundPathSpecifier) ? sound : null);
			SoundSpecifier sound2;
			if (pathSpec == null)
			{
				SoundCollectionSpecifier collectionSpec = (SoundCollectionSpecifier)(object)((sound is SoundCollectionSpecifier) ? sound : null);
				sound2 = (SoundSpecifier)((collectionSpec == null) ? ((SoundCollectionSpecifier)null) : ((collectionSpec.Collection == null) ? ((SoundCollectionSpecifier)null) : new SoundCollectionSpecifier(collectionSpec.Collection, (AudioParams?)((SoundSpecifier)collectionSpec).Params)));
			}
			else
			{
				sound2 = (SoundSpecifier)new SoundPathSpecifier(pathSpec.Path, (AudioParams?)((SoundSpecifier)pathSpec).Params);
			}
			comp.Sound = sound2;
		}
	}

	private void HandleEmitSoundOnUIOpen(EntityUid uid, EmitSoundOnUIOpenComponent component, AfterActivatableUIOpenEvent args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (_whitelistSystem.IsBlacklistFail(component.Blacklist, args.User))
		{
			TryEmitSound(uid, component, args.User);
		}
	}

	private void OnMobState(Entity<SoundWhileAliveComponent> entity, ref MobStateChangedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		SpamEmitSoundComponent comp = default(SpamEmitSoundComponent);
		if (((EntitySystem)this).TryComp<SpamEmitSoundComponent>(Entity<SoundWhileAliveComponent>.op_Implicit(entity), ref comp))
		{
			comp.Enabled = args.NewMobState == MobState.Alive;
			((EntitySystem)this).Dirty(entity.Owner, (IComponent)(object)comp, (MetaDataComponent)null);
		}
		_ambient.SetAmbience(entity.Owner, args.NewMobState != MobState.Dead);
	}

	private void OnEmitSpawnOnInit(EntityUid uid, EmitSoundOnSpawnComponent component, MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		TryEmitSound(uid, component, null, predict: false);
	}

	private void OnEmitSoundOnLand(EntityUid uid, BaseEmitSoundComponent component, ref LandEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent xform = default(TransformComponent);
		MapGridComponent grid = default(MapGridComponent);
		if (args.PlaySound && ((EntitySystem)this).TryComp(uid, ref xform) && ((EntitySystem)this).TryComp<MapGridComponent>(xform.GridUid, ref grid))
		{
			TileRef tile = _map.GetTileRef(xform.GridUid.Value, grid, xform.Coordinates);
			EntityUid? gridUid = xform.GridUid;
			EntityUid? mapUid = xform.MapUid;
			if ((gridUid.HasValue == mapUid.HasValue && (!gridUid.HasValue || !(gridUid.GetValueOrDefault() != mapUid.GetValueOrDefault()))) || !_turf.IsSpace(tile))
			{
				TryEmitSound(uid, component, args.User, predict: false);
			}
		}
	}

	private void OnEmitSoundOnUseInHand(EntityUid uid, EmitSoundOnUseComponent component, UseInHandEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		TryEmitSound(uid, component, args.User);
		if (component.Handle)
		{
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnEmitSoundOnThrown(EntityUid uid, BaseEmitSoundComponent component, ref ThrownEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		TryEmitSound(uid, component, args.User, predict: false);
	}

	private void OnEmitSoundOnActivateInWorld(EntityUid uid, EmitSoundOnActivateComponent component, ActivateInWorldEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		TryEmitSound(uid, component, args.User);
		if (component.Handle)
		{
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnEmitSoundOnPickup(EntityUid uid, EmitSoundOnPickupComponent component, GotEquippedHandEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		TryEmitSound(uid, component, args.User);
	}

	private void OnEmitSoundOnDrop(EntityUid uid, EmitSoundOnDropComponent component, DroppedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		TryEmitSound(uid, component, args.User);
	}

	private void OnEmitSoundOnInteractUsing(Entity<EmitSoundOnInteractUsingComponent> ent, ref InteractUsingEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (_whitelistSystem.IsWhitelistPass(ent.Comp.Whitelist, args.Used))
		{
			TryEmitSound(Entity<EmitSoundOnInteractUsingComponent>.op_Implicit(ent), ent.Comp, args.User);
		}
	}

	public void TryEmitSound(EntityUid uid, BaseEmitSoundComponent component, EntityUid? user = null, bool predict = true)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if (component.Sound == null)
		{
			return;
		}
		if (component.Positional)
		{
			EntityCoordinates coords = ((EntitySystem)this).Transform(uid).Coordinates;
			if (predict)
			{
				_audioSystem.PlayPredicted(component.Sound, coords, user, (AudioParams?)null);
			}
			else if (_netMan.IsServer)
			{
				_audioSystem.PlayPvs(component.Sound, coords, (AudioParams?)null);
			}
		}
		else if (predict)
		{
			_audioSystem.PlayPredicted(component.Sound, uid, user, (AudioParams?)null);
		}
		else if (_netMan.IsServer)
		{
			_audioSystem.PlayPvs(component.Sound, uid, (AudioParams?)null);
		}
	}

	private void OnEmitSoundOnCollide(EntityUid uid, EmitSoundOnCollideComponent component, ref StartCollideEvent args)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		PhysicsComponent physics = default(PhysicsComponent);
		if (args.OurFixture.Hard && args.OtherFixture.Hard && ((EntitySystem)this).TryComp<PhysicsComponent>(uid, ref physics) && !(physics.LinearVelocity.Length() < component.MinimumVelocity) && !(Timing.CurTime < component.NextSound) && !((EntitySystem)this).MetaData(uid).EntityPaused)
		{
			float fraction = MathF.Min(1f, (physics.LinearVelocity.Length() - component.MinimumVelocity) / 10f);
			float volume = -10f + 12f * fraction;
			component.NextSound = Timing.CurTime + EmitSoundOnCollideComponent.CollideCooldown;
			SoundSpecifier sound = component.Sound;
			if (_netMan.IsServer && sound != null)
			{
				_audioSystem.PlayPvs(_audioSystem.ResolveSound(sound), uid, (AudioParams?)((AudioParams)(ref AudioParams.Default)).WithVolume(volume));
			}
		}
	}

	public virtual void SetEnabled(Entity<SpamEmitSoundComponent?> entity, bool enabled)
	{
	}
}
