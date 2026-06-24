using System;
using Content.Shared._RMC14.Body;
using Content.Shared._RMC14.Hands;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Temperature;
using Content.Shared.Chemistry.Components;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Mobs.Components;
using Content.Shared.Popups;
using Content.Shared.Storage.Components;
using Content.Shared.Storage.EntitySystems;
using Content.Shared.Timing;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Medical.Scanner;

public sealed class HealthScannerSystem : EntitySystem
{
	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private SharedEntityStorageSystem _entityStorage;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedRMCBloodstreamSystem _rmcBloodstream;

	[Dependency]
	private RMCHandsSystem _rmcHands;

	[Dependency]
	private SharedRMCTemperatureSystem _rmcTemperature;

	[Dependency]
	private SkillsSystem _skills;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedUserInterfaceSystem _ui;

	[Dependency]
	private UseDelaySystem _useDelay;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<HealthScannerComponent, AfterInteractEvent>((EntityEventRefHandler<HealthScannerComponent, AfterInteractEvent>)OnAfterInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HealthScannerComponent, DoAfterAttemptEvent<HealthScannerDoAfterEvent>>((EntityEventRefHandler<HealthScannerComponent, DoAfterAttemptEvent<HealthScannerDoAfterEvent>>)OnDoAfterAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HealthScannerComponent, HealthScannerDoAfterEvent>((EntityEventRefHandler<HealthScannerComponent, HealthScannerDoAfterEvent>)OnDoAfter, (Type[])null, (Type[])null);
	}

	private void OnAfterInteract(Entity<HealthScannerComponent> scanner, ref AfterInteractEvent args)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		if (!args.CanReach)
		{
			return;
		}
		EntityUid? target = args.Target;
		if (!target.HasValue)
		{
			return;
		}
		EntityUid target2 = target.GetValueOrDefault();
		if (CanUseHealthScannerPopup(scanner, args.User, ref target2))
		{
			TimeSpan delay = _skills.GetDelay(args.User, Entity<HealthScannerComponent>.op_Implicit(scanner));
			HealthScannerDoAfterEvent ev = new HealthScannerDoAfterEvent();
			DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, args.User, delay, ev, Entity<HealthScannerComponent>.op_Implicit(scanner), target2, Entity<HealthScannerComponent>.op_Implicit(scanner))
			{
				BreakOnMove = true,
				AttemptFrequency = AttemptFrequency.EveryTick
			};
			if (delay > TimeSpan.Zero)
			{
				string name = base.Loc.GetString("zzzz-the", (ValueTuple<string, object>)("ent", target2));
				_popup.PopupClient("You start fumbling around with " + name + "...", target2, args.User);
			}
			_doAfter.TryStartDoAfter(doAfter);
		}
	}

	private void OnDoAfterAttempt(Entity<HealthScannerComponent> ent, ref DoAfterAttemptEvent<HealthScannerDoAfterEvent> args)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		DoAfterArgs doAfter = args.DoAfter.Args;
		EntityUid? target = doAfter.Target;
		if (!target.HasValue)
		{
			return;
		}
		EntityUid target2 = target.GetValueOrDefault();
		if (!CanUseHealthScannerPopup(ent, doAfter.User, ref target2))
		{
			((CancellableEntityEventArgs)args).Cancel();
			return;
		}
		EntityCoordinates userCoords = ((EntitySystem)this).Transform(doAfter.User).Coordinates;
		if (!_transform.InRange(userCoords, args.DoAfter.UserPosition, doAfter.MovementThreshold))
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnDoAfter(Entity<HealthScannerComponent> scanner, ref HealthScannerDoAfterEvent args)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled || ((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		EntityUid? target = args.Target;
		if (target.HasValue)
		{
			EntityUid target2 = target.GetValueOrDefault();
			((HandledEntityEventArgs)args).Handled = true;
			UseDelayComponent useDelay = default(UseDelayComponent);
			if (((EntitySystem)this).TryComp<UseDelayComponent>(Entity<HealthScannerComponent>.op_Implicit(scanner), ref useDelay))
			{
				_useDelay.TryResetDelay(Entity<UseDelayComponent>.op_Implicit((Entity<HealthScannerComponent>.op_Implicit(scanner), useDelay)));
			}
			scanner.Comp.Target = target2;
			((EntitySystem)this).Dirty<HealthScannerComponent>(scanner, (MetaDataComponent)null);
			_audio.PlayPredicted(scanner.Comp.Sound, Entity<HealthScannerComponent>.op_Implicit(scanner), (EntityUid?)args.User, (AudioParams?)null);
			_ui.OpenUi(Entity<UserInterfaceComponent>.op_Implicit(scanner.Owner), (Enum)HealthScannerUIKey.Key, (EntityUid?)args.User, false);
			UpdateUI(scanner);
		}
	}

	private bool CanUseHealthScannerPopup(Entity<HealthScannerComponent> scanner, EntityUid user, ref EntityUid target)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		SharedEntityStorageComponent entityStorage = null;
		if (((EntitySystem)this).HasComp<HealthScannableContainerComponent>(target) && _entityStorage.ResolveStorage(target, ref entityStorage))
		{
			foreach (EntityUid entity in ((BaseContainer)entityStorage.Contents).ContainedEntities)
			{
				if (((EntitySystem)this).HasComp<DamageableComponent>(entity) && ((EntitySystem)this).HasComp<MobStateComponent>(entity) && ((EntitySystem)this).HasComp<MobThresholdsComponent>(entity))
				{
					target = entity;
					break;
				}
			}
		}
		if (!((EntitySystem)this).HasComp<DamageableComponent>(target) || !((EntitySystem)this).HasComp<MobStateComponent>(target) || !((EntitySystem)this).HasComp<MobThresholdsComponent>(target))
		{
			_popup.PopupClient("You can't analyze that!", target, user);
			return false;
		}
		UseDelayComponent useDelay = default(UseDelayComponent);
		if (((EntitySystem)this).TryComp<UseDelayComponent>(Entity<HealthScannerComponent>.op_Implicit(scanner), ref useDelay) && _useDelay.IsDelayed(Entity<UseDelayComponent>.op_Implicit((Entity<HealthScannerComponent>.op_Implicit(scanner), useDelay))))
		{
			return false;
		}
		HealthScannerAttemptTargetEvent ev = default(HealthScannerAttemptTargetEvent);
		((EntitySystem)this).RaiseLocalEvent<HealthScannerAttemptTargetEvent>(target, ref ev, false);
		if (ev.Cancelled)
		{
			if (ev.Popup != null)
			{
				_popup.PopupClient(ev.Popup, target, user);
			}
			return false;
		}
		return true;
	}

	private void UpdateUI(Entity<HealthScannerComponent> scanner)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? target = scanner.Comp.Target;
		if (!target.HasValue)
		{
			return;
		}
		EntityUid target2 = target.GetValueOrDefault();
		EntityUid user;
		if (((EntitySystem)this).TerminatingOrDeleted(target2, (MetaDataComponent)null))
		{
			if (!((EntitySystem)this).TerminatingOrDeleted(Entity<HealthScannerComponent>.op_Implicit(scanner), (MetaDataComponent)null))
			{
				_ui.CloseUi(Entity<UserInterfaceComponent>.op_Implicit(scanner.Owner), (Enum)HealthScannerUIKey.Key);
			}
			scanner.Comp.Target = null;
		}
		else if (_rmcHands.TryGetHolder(Entity<HealthScannerComponent>.op_Implicit(scanner), out user))
		{
			FixedPoint2 blood = 0;
			FixedPoint2 maxBlood = 0;
			if (_rmcBloodstream.TryGetBloodSolution(target2, out Solution bloodstream))
			{
				blood = bloodstream.Volume;
				maxBlood = bloodstream.MaxVolume;
			}
			_rmcBloodstream.TryGetChemicalSolution(target2, out Entity<SolutionComponent> _, out Solution chemicals);
			_rmcTemperature.TryGetCurrentTemperature(target2, out var temperature);
			bool bleeding = _rmcBloodstream.IsBleeding(target2);
			HealthScannerBuiState state = new HealthScannerBuiState(((EntitySystem)this).GetNetEntity(target2, (MetaDataComponent)null), blood, maxBlood, temperature, chemicals, bleeding);
			_ui.SetUiState(Entity<UserInterfaceComponent>.op_Implicit(scanner.Owner), (Enum)HealthScannerUIKey.Key, (BoundUserInterfaceState)(object)state);
		}
	}

	public override void Update(float frameTime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<HealthScannerComponent> scanners = ((EntitySystem)this).EntityQueryEnumerator<HealthScannerComponent>();
		EntityUid uid = default(EntityUid);
		HealthScannerComponent active = default(HealthScannerComponent);
		while (scanners.MoveNext(ref uid, ref active))
		{
			if (!(time < active.UpdateAt))
			{
				active.UpdateAt = time + active.UpdateCooldown;
				UpdateUI(Entity<HealthScannerComponent>.op_Implicit((uid, active)));
			}
		}
	}
}
