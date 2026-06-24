using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.ActionBlocker;
using Content.Shared.Body.Components;
using Content.Shared.Destructible;
using Content.Shared.Foldable;
using Content.Shared.Hands.Components;
using Content.Shared.Interaction;
using Content.Shared.Item;
using Content.Shared.Lock;
using Content.Shared.Movement.Events;
using Content.Shared.Popups;
using Content.Shared.Storage.Components;
using Content.Shared.Tools.Systems;
using Content.Shared.Verbs;
using Content.Shared.Wall;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Shared.Storage.EntitySystems;

public abstract class SharedEntityStorageSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private EntityLookupSystem _lookup;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedInteractionSystem _interaction;

	[Dependency]
	private SharedJointSystem _joints;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	protected SharedPopupSystem Popup;

	[Dependency]
	protected SharedTransformSystem TransformSystem;

	[Dependency]
	private WeldableSystem _weldable;

	[Dependency]
	private EntityWhitelistSystem _whitelistSystem;

	[Dependency]
	private ActionBlockerSystem _actionBlocker;

	public const string ContainerName = "entity_storage";

	protected void OnEntityUnpausedEvent(EntityUid uid, SharedEntityStorageComponent component, EntityUnpausedEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		component.NextInternalOpenAttempt += args.PausedTime;
	}

	protected void OnGetState(EntityUid uid, SharedEntityStorageComponent component, ref ComponentGetState args)
	{
		((ComponentGetState)(ref args)).State = (IComponentState)(object)new EntityStorageComponentState(component.Open, component.Capacity, component.IsCollidableWhenOpen, component.OpenOnMove, component.EnteringRange, component.NextInternalOpenAttempt);
	}

	protected void OnHandleState(EntityUid uid, SharedEntityStorageComponent component, ref ComponentHandleState args)
	{
		if (((ComponentHandleState)(ref args)).Current is EntityStorageComponentState state)
		{
			component.Open = state.Open;
			component.Capacity = state.Capacity;
			component.IsCollidableWhenOpen = state.IsCollidableWhenOpen;
			component.OpenOnMove = state.OpenOnMove;
			component.EnteringRange = state.EnteringRange;
			component.NextInternalOpenAttempt = state.NextInternalOpenAttempt;
		}
	}

	protected virtual void OnComponentInit(EntityUid uid, SharedEntityStorageComponent component, ComponentInit args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		component.Contents = _container.EnsureContainer<Container>(uid, "entity_storage", (ContainerManagerComponent)null);
		((BaseContainer)component.Contents).ShowContents = component.ShowContents;
		((BaseContainer)component.Contents).OccludesLight = component.OccludesLight;
	}

	protected virtual void OnComponentStartup(EntityUid uid, SharedEntityStorageComponent component, ComponentStartup args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		_appearance.SetData(uid, (Enum)StorageVisuals.Open, (object)component.Open, (AppearanceComponent)null);
	}

	protected void OnInteract(EntityUid uid, SharedEntityStorageComponent component, ActivateInWorldEvent args)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && args.Complex)
		{
			((HandledEntityEventArgs)args).Handled = true;
			ToggleOpen(args.User, uid, component);
		}
	}

	public abstract bool ResolveStorage(EntityUid uid, [NotNullWhen(true)] ref SharedEntityStorageComponent? component);

	protected void OnLockToggleAttempt(EntityUid uid, SharedEntityStorageComponent target, ref LockToggleAttemptEvent args)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (target.Open)
		{
			args.Cancelled = true;
		}
		if (((BaseContainer)target.Contents).Contains(args.User))
		{
			args.Cancelled = true;
		}
	}

	protected void OnDestruction(EntityUid uid, SharedEntityStorageComponent component, DestructionEventArgs args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		component.Open = true;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		if (!component.DeleteContentsOnDestruction)
		{
			EmptyContents(uid, component);
			return;
		}
		foreach (EntityUid ent in new List<EntityUid>(((BaseContainer)component.Contents).ContainedEntities))
		{
			((EntitySystem)this).Del((EntityUid?)ent);
		}
	}

	protected void OnRelayMovement(EntityUid uid, SharedEntityStorageComponent component, ref ContainerRelayMovementEntityEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<HandsComponent>(args.Entity) && _actionBlocker.CanMove(args.Entity) && !(_timing.CurTime < component.NextInternalOpenAttempt))
		{
			component.NextInternalOpenAttempt = _timing.CurTime + SharedEntityStorageComponent.InternalOpenAttemptDelay;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
			if (component.OpenOnMove)
			{
				TryOpenStorage(args.Entity, uid);
			}
		}
	}

	protected void OnFoldAttempt(EntityUid uid, SharedEntityStorageComponent component, ref FoldAttemptEvent args)
	{
		if (!args.Cancelled)
		{
			args.Cancelled = component.Open || ((BaseContainer)component.Contents).ContainedEntities.Count != 0;
		}
	}

	protected void AddToggleOpenVerb(EntityUid uid, SharedEntityStorageComponent component, GetVerbsEvent<InteractionVerb> args)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Expected O, but got Unknown
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Expected O, but got Unknown
		if (args.CanAccess && args.CanInteract && CanOpen(args.User, args.Target, silent: true, component))
		{
			InteractionVerb verb = new InteractionVerb();
			if (component.Open)
			{
				verb.Text = base.Loc.GetString("verb-common-close");
				verb.Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/VerbIcons/close.svg.192dpi.png"));
			}
			else
			{
				verb.Text = base.Loc.GetString("verb-common-open");
				verb.Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/VerbIcons/open.svg.192dpi.png"));
			}
			verb.Act = delegate
			{
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				ToggleOpen(args.User, args.Target, component);
			};
			args.Verbs.Add(verb);
		}
	}

	public void ToggleOpen(EntityUid user, EntityUid target, SharedEntityStorageComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (!ResolveStorage(target, ref component))
		{
			return;
		}
		if (component.Open)
		{
			if (!((EntitySystem)this).HasComp<XenoComponent>(user))
			{
				TryCloseStorage(target, user);
			}
		}
		else
		{
			TryOpenStorage(user, target);
		}
	}

	public void EmptyContents(EntityUid uid, SharedEntityStorageComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (ResolveStorage(uid, ref component))
		{
			TransformComponent uidXform = ((EntitySystem)this).Transform(uid);
			EntityUid[] array = ((BaseContainer)component.Contents).ContainedEntities.ToArray();
			foreach (EntityUid contained in array)
			{
				Remove(contained, uid, component, uidXform);
			}
		}
	}

	public void OpenStorage(EntityUid uid, SharedEntityStorageComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		if (ResolveStorage(uid, ref component) && !component.Open)
		{
			StorageBeforeOpenEvent beforeev = default(StorageBeforeOpenEvent);
			((EntitySystem)this).RaiseLocalEvent<StorageBeforeOpenEvent>(uid, ref beforeev, false);
			component.Open = true;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
			EmptyContents(uid, component);
			ModifyComponents(uid, component);
			if (_net.IsClient && _timing.IsFirstTimePredicted)
			{
				_audio.PlayPvs(component.OpenSound, uid, (AudioParams?)null);
			}
			ReleaseGas(uid, component);
			StorageAfterOpenEvent afterev = default(StorageAfterOpenEvent);
			((EntitySystem)this).RaiseLocalEvent<StorageAfterOpenEvent>(uid, ref afterev, false);
		}
	}

	public void CloseStorage(EntityUid uid, SharedEntityStorageComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		if (!ResolveStorage(uid, ref component) || !component.Open || base.EntityManager.IsQueuedForDeletion(uid))
		{
			return;
		}
		component.Open = false;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		HashSet<EntityUid> entities = _lookup.GetEntitiesInRange(new EntityCoordinates(uid, component.EnteringOffset), component.EnteringRange, (LookupFlags)11);
		entities.Remove(uid);
		StorageBeforeCloseEvent ev = new StorageBeforeCloseEvent(entities, new HashSet<EntityUid>());
		((EntitySystem)this).RaiseLocalEvent<StorageBeforeCloseEvent>(uid, ref ev, false);
		foreach (EntityUid entity in ev.Contents)
		{
			if ((ev.BypassChecks.Contains(entity) || CanInsert(entity, uid, component)) && AddToContents(entity, uid, component) && ((BaseContainer)component.Contents).ContainedEntities.Count >= component.Capacity)
			{
				break;
			}
		}
		TakeGas(uid, component);
		ModifyComponents(uid, component);
		if (_net.IsClient && _timing.IsFirstTimePredicted)
		{
			_audio.PlayPvs(component.CloseSound, uid, (AudioParams?)null);
		}
		StorageAfterCloseEvent afterev = default(StorageAfterCloseEvent);
		((EntitySystem)this).RaiseLocalEvent<StorageAfterCloseEvent>(uid, ref afterev, false);
	}

	public bool Insert(EntityUid toInsert, EntityUid container, SharedEntityStorageComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		if (!ResolveStorage(container, ref component))
		{
			return false;
		}
		if (component.Open)
		{
			TransformSystem.DropNextTo(Entity<TransformComponent>.op_Implicit(toInsert), Entity<TransformComponent>.op_Implicit(container));
			return true;
		}
		_joints.RecursiveClearJoints(toInsert, (TransformComponent)null, (JointComponent)null, (JointRelayTargetComponent)null);
		if (!_container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(toInsert), (BaseContainer)(object)component.Contents, (TransformComponent)null, false))
		{
			return false;
		}
		((EntitySystem)this).EnsureComp<InsideEntityStorageComponent>(toInsert).Storage = container;
		return true;
	}

	public bool Remove(EntityUid toRemove, EntityUid container, SharedEntityStorageComponent? component = null, TransformComponent? xform = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve(container, ref xform, false))
		{
			return false;
		}
		if (!ResolveStorage(container, ref component))
		{
			return false;
		}
		_container.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit(toRemove), (BaseContainer)(object)component.Contents, true, false, (EntityCoordinates?)null, (Angle?)null);
		BaseContainer outerContainer = default(BaseContainer);
		if (_container.IsEntityInContainer(container, (MetaDataComponent)null) && _container.TryGetOuterContainer(container, ((EntitySystem)this).Transform(container), ref outerContainer) && !((EntitySystem)this).HasComp<HandsComponent>(outerContainer.Owner))
		{
			_container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(toRemove), outerContainer, (TransformComponent)null, false);
			return true;
		}
		((EntitySystem)this).RemComp<InsideEntityStorageComponent>(toRemove);
		Vector2 worldPosition = TransformSystem.GetWorldPosition(xform);
		Angle worldRotation = TransformSystem.GetWorldRotation(xform);
		Vector2 pos = worldPosition + ((Angle)(ref worldRotation)).RotateVec(ref component.EnteringOffset);
		TransformSystem.SetWorldPosition(toRemove, pos);
		return true;
	}

	public bool CanInsert(EntityUid toInsert, EntityUid container, SharedEntityStorageComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		if (!ResolveStorage(container, ref component))
		{
			return false;
		}
		if (component.Open)
		{
			return true;
		}
		if (((BaseContainer)component.Contents).ContainedEntities.Count >= component.Capacity)
		{
			return false;
		}
		Box2 aabb = _lookup.GetAABBNoContainer(toInsert, Vector2.Zero, Angle.op_Implicit(0f));
		if (component.MaxSize < ((Box2)(ref aabb)).Size.X || component.MaxSize < ((Box2)(ref aabb)).Size.Y)
		{
			return false;
		}
		InsertIntoEntityStorageAttemptEvent attemptEvent = new InsertIntoEntityStorageAttemptEvent(toInsert, container);
		((EntitySystem)this).RaiseLocalEvent<InsertIntoEntityStorageAttemptEvent>(toInsert, ref attemptEvent, false);
		if (attemptEvent.Cancelled)
		{
			return false;
		}
		EntityStorageInsertedIntoAttemptEvent containerAttemptEvent = new EntityStorageInsertedIntoAttemptEvent(toInsert);
		((EntitySystem)this).RaiseLocalEvent<EntityStorageInsertedIntoAttemptEvent>(container, ref containerAttemptEvent, false);
		if (containerAttemptEvent.Cancelled)
		{
			return false;
		}
		if (component.Whitelist != null)
		{
			return _whitelistSystem.IsValid(component.Whitelist, toInsert);
		}
		if (!((EntitySystem)this).HasComp<BodyComponent>(toInsert))
		{
			return ((EntitySystem)this).HasComp<ItemComponent>(toInsert);
		}
		return true;
	}

	public bool TryOpenStorage(EntityUid user, EntityUid target, bool silent = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (!CanOpen(user, target, silent))
		{
			return false;
		}
		OpenStorage(target);
		return true;
	}

	public bool TryCloseStorage(EntityUid target, EntityUid? user = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (!CanClose(target, user))
		{
			return false;
		}
		CloseStorage(target);
		return true;
	}

	public bool IsOpen(EntityUid target, SharedEntityStorageComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (!ResolveStorage(target, ref component))
		{
			return false;
		}
		return component.Open;
	}

	public bool CanOpen(EntityUid user, EntityUid target, bool silent = false, SharedEntityStorageComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		if (!ResolveStorage(target, ref component))
		{
			return false;
		}
		if (!((EntitySystem)this).HasComp<HandsComponent>(user))
		{
			return false;
		}
		if (_weldable.IsWelded(target))
		{
			if (!silent && !((BaseContainer)component.Contents).Contains(user))
			{
				Popup.PopupClient(base.Loc.GetString("entity-storage-component-welded-shut-message"), target, user);
			}
			return false;
		}
		if (component.EnteringOffset != new Vector2(0f, 0f) && !((EntitySystem)this).HasComp<WallMountComponent>(target))
		{
			EntityCoordinates newCoords = default(EntityCoordinates);
			((EntityCoordinates)(ref newCoords))._002Ector(target, component.EnteringOffset);
			if (!_interaction.InRangeUnobstructed(target, newCoords, 0f, component.EnteringOffsetCollisionFlags))
			{
				if (!silent && _net.IsServer)
				{
					Popup.PopupEntity(base.Loc.GetString("entity-storage-component-cannot-open-no-space"), target);
				}
				return false;
			}
		}
		StorageOpenAttemptEvent ev = new StorageOpenAttemptEvent(user, silent);
		((EntitySystem)this).RaiseLocalEvent<StorageOpenAttemptEvent>(target, ref ev, true);
		return !ev.Cancelled;
	}

	public bool CanClose(EntityUid target, EntityUid? user = null, bool silent = false)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		StorageCloseAttemptEvent ev = new StorageCloseAttemptEvent(user);
		((EntitySystem)this).RaiseLocalEvent<StorageCloseAttemptEvent>(target, ref ev, silent);
		return !ev.Cancelled;
	}

	public bool AddToContents(EntityUid toAdd, EntityUid container, SharedEntityStorageComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (!ResolveStorage(container, ref component))
		{
			return false;
		}
		if (toAdd == container)
		{
			return false;
		}
		return Insert(toAdd, container, component);
	}

	private void ModifyComponents(EntityUid uid, SharedEntityStorageComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		if (!ResolveStorage(uid, ref component))
		{
			return;
		}
		FixturesComponent fixtures = default(FixturesComponent);
		if (!component.IsCollidableWhenOpen && ((EntitySystem)this).TryComp<FixturesComponent>(uid, ref fixtures) && fixtures.Fixtures.Count > 0)
		{
			KeyValuePair<string, Fixture> fixture = fixtures.Fixtures.First();
			if (component.Open)
			{
				component.RemovedMasks = fixture.Value.CollisionLayer & component.MasksToRemove;
				_physics.SetCollisionLayer(uid, fixture.Key, fixture.Value, fixture.Value.CollisionLayer & ~component.MasksToRemove, fixtures, (PhysicsComponent)null);
			}
			else
			{
				_physics.SetCollisionLayer(uid, fixture.Key, fixture.Value, fixture.Value.CollisionLayer | component.RemovedMasks, fixtures, (PhysicsComponent)null);
				component.RemovedMasks = 0;
			}
		}
		_appearance.SetData(uid, (Enum)StorageVisuals.Open, (object)component.Open, (AppearanceComponent)null);
		_appearance.SetData(uid, (Enum)StorageVisuals.HasContents, (object)(((BaseContainer)component.Contents).ContainedEntities.Count > 0), (AppearanceComponent)null);
	}

	protected virtual void TakeGas(EntityUid uid, SharedEntityStorageComponent component)
	{
	}

	public virtual void ReleaseGas(EntityUid uid, SharedEntityStorageComponent component)
	{
	}
}
