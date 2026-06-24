using System;
using Content.Shared.ActionBlocker;
using Content.Shared.Burial.Components;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Movement.Events;
using Content.Shared.Placeable;
using Content.Shared.Popups;
using Content.Shared.Storage.Components;
using Content.Shared.Storage.EntitySystems;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Burial;

public sealed class BurialSystem : EntitySystem
{
	[Dependency]
	private SharedDoAfterSystem _doAfterSystem;

	[Dependency]
	private SharedEntityStorageSystem _storageSystem;

	[Dependency]
	private SharedAudioSystem _audioSystem;

	[Dependency]
	private SharedPopupSystem _popupSystem;

	[Dependency]
	private ActionBlockerSystem _actionBlocker;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<GraveComponent, InteractUsingEvent>((ComponentEventHandler<GraveComponent, InteractUsingEvent>)OnInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GraveComponent, ActivateInWorldEvent>((ComponentEventHandler<GraveComponent, ActivateInWorldEvent>)OnActivate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GraveComponent, AfterInteractUsingEvent>((ComponentEventHandler<GraveComponent, AfterInteractUsingEvent>)OnAfterInteractUsing, new Type[1] { typeof(PlaceableSurfaceSystem) }, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GraveComponent, GraveDiggingDoAfterEvent>((ComponentEventHandler<GraveComponent, GraveDiggingDoAfterEvent>)OnGraveDigging, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GraveComponent, StorageOpenAttemptEvent>((ComponentEventRefHandler<GraveComponent, StorageOpenAttemptEvent>)OnOpenAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GraveComponent, StorageCloseAttemptEvent>((ComponentEventRefHandler<GraveComponent, StorageCloseAttemptEvent>)OnCloseAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GraveComponent, StorageAfterOpenEvent>((ComponentEventRefHandler<GraveComponent, StorageAfterOpenEvent>)OnAfterOpen, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GraveComponent, StorageAfterCloseEvent>((ComponentEventRefHandler<GraveComponent, StorageAfterCloseEvent>)OnAfterClose, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GraveComponent, ContainerRelayMovementEntityEvent>((ComponentEventRefHandler<GraveComponent, ContainerRelayMovementEntityEvent>)OnRelayMovement, (Type[])null, (Type[])null);
	}

	private void OnInteractUsing(EntityUid uid, GraveComponent component, InteractUsingEvent args)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || component.ActiveShovelDigging)
		{
			return;
		}
		ShovelComponent shovel = default(ShovelComponent);
		if (((EntitySystem)this).TryComp<ShovelComponent>(args.Used, ref shovel))
		{
			DoAfterArgs doAfterEventArgs = new DoAfterArgs((IEntityManager)(object)base.EntityManager, args.User, component.DigDelay / shovel.SpeedModifier, new GraveDiggingDoAfterEvent(), uid, args.Target, uid)
			{
				BreakOnMove = true,
				BreakOnDamage = true,
				NeedHand = true
			};
			if (!component.Stream.HasValue)
			{
				component.Stream = _audioSystem.PlayPredicted((SoundSpecifier)(object)component.DigSound, uid, (EntityUid?)args.User, (AudioParams?)null)?.Item1;
			}
			if (!_doAfterSystem.TryStartDoAfter(doAfterEventArgs))
			{
				_audioSystem.Stop(component.Stream, (AudioComponent)null);
				return;
			}
			StartDigging(uid, args.User, args.Used, component);
		}
		else
		{
			_popupSystem.PopupClient(base.Loc.GetString("grave-digging-requires-tool", (ValueTuple<string, object>)("grave", args.Target)), uid, args.User);
		}
		((HandledEntityEventArgs)args).Handled = true;
	}

	private void OnAfterInteractUsing(EntityUid uid, GraveComponent component, AfterInteractUsingEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && ((EntitySystem)this).HasComp<ShovelComponent>(args.Used))
		{
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnActivate(EntityUid uid, GraveComponent component, ActivateInWorldEvent args)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && args.Complex)
		{
			_popupSystem.PopupClient(base.Loc.GetString("grave-digging-requires-tool", (ValueTuple<string, object>)("grave", args.Target)), uid, args.User);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnGraveDigging(EntityUid uid, GraveComponent component, GraveDiggingDoAfterEvent args)
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		if (args.Used.HasValue)
		{
			component.ActiveShovelDigging = false;
			component.Stream = _audioSystem.Stop(component.Stream, (AudioComponent)null);
		}
		else
		{
			component.HandDiggingDoAfter = null;
		}
		if (!args.Cancelled && !((HandledEntityEventArgs)args).Handled)
		{
			component.DiggingComplete = true;
			if (args.Used.HasValue)
			{
				_storageSystem.ToggleOpen(args.User, uid);
			}
			else
			{
				_storageSystem.TryOpenStorage(args.User, uid);
			}
		}
	}

	private void StartDigging(EntityUid uid, EntityUid user, EntityUid? used, GraveComponent component)
	{
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		if (used.HasValue)
		{
			string selfMessage = base.Loc.GetString("grave-start-digging-user", (ValueTuple<string, object>)("grave", uid), (ValueTuple<string, object>)("tool", used));
			string othersMessage = base.Loc.GetString("grave-start-digging-others", new(string, object)[3]
			{
				("user", user),
				("grave", uid),
				("tool", used)
			});
			_popupSystem.PopupPredicted(selfMessage, othersMessage, user, user);
			component.ActiveShovelDigging = true;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		}
		else
		{
			_popupSystem.PopupClient(base.Loc.GetString("grave-start-digging-user-trapped", (ValueTuple<string, object>)("grave", uid)), user, user, PopupType.Medium);
		}
	}

	private void OnOpenAttempt(EntityUid uid, GraveComponent component, ref StorageOpenAttemptEvent args)
	{
		if (!component.DiggingComplete)
		{
			args.Cancelled = true;
		}
	}

	private void OnCloseAttempt(EntityUid uid, GraveComponent component, ref StorageCloseAttemptEvent args)
	{
		if (!component.DiggingComplete)
		{
			args.Cancelled = true;
		}
	}

	private void OnAfterOpen(EntityUid uid, GraveComponent component, ref StorageAfterOpenEvent args)
	{
		component.DiggingComplete = false;
	}

	private void OnAfterClose(EntityUid uid, GraveComponent component, ref StorageAfterCloseEvent args)
	{
		component.DiggingComplete = false;
	}

	private void OnRelayMovement(EntityUid uid, GraveComponent component, ref ContainerRelayMovementEntityEvent args)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		if (!_doAfterSystem.IsRunning(component.HandDiggingDoAfter) && _actionBlocker.CanMove(args.Entity))
		{
			DoAfterArgs doAfterEventArgs = new DoAfterArgs((IEntityManager)(object)base.EntityManager, args.Entity, component.DigDelay / component.DigOutByHandModifier, new GraveDiggingDoAfterEvent(), uid, uid)
			{
				NeedHand = false,
				BreakOnMove = true,
				BreakOnHandChange = false,
				BreakOnDamage = false
			};
			if (!component.Stream.HasValue)
			{
				component.Stream = _audioSystem.PlayPredicted((SoundSpecifier)(object)component.DigSound, uid, (EntityUid?)args.Entity, (AudioParams?)null)?.Item1;
			}
			if (!_doAfterSystem.TryStartDoAfter(doAfterEventArgs, out component.HandDiggingDoAfter))
			{
				_audioSystem.Stop(component.Stream, (AudioComponent)null);
			}
			else
			{
				StartDigging(uid, args.Entity, null, component);
			}
		}
	}
}
