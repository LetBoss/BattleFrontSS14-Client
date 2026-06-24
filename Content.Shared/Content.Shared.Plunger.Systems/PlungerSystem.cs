using System;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Plunger.Components;
using Content.Shared.Popups;
using Content.Shared.Random;
using Content.Shared.Random.Helpers;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared.Plunger.Systems;

public sealed class PlungerSystem : EntitySystem
{
	[Dependency]
	private IPrototypeManager _proto;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private SharedPopupSystem _popup;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<PlungerComponent, AfterInteractEvent>((ComponentEventHandler<PlungerComponent, AfterInteractEvent>)OnInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PlungerComponent, PlungerDoAfterEvent>((ComponentEventHandler<PlungerComponent, PlungerDoAfterEvent>)OnDoAfter, (Type[])null, (Type[])null);
	}

	private void OnInteract(EntityUid uid, PlungerComponent component, AfterInteractEvent args)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || !args.CanReach)
		{
			return;
		}
		EntityUid? target = args.Target;
		if (target.HasValue)
		{
			EntityUid target2 = target.GetValueOrDefault();
			PlungerUseComponent plunger = default(PlungerUseComponent);
			if (((EntityUid)(ref target2)).Valid && ((EntitySystem)this).TryComp<PlungerUseComponent>(args.Target, ref plunger) && !plunger.NeedsPlunger)
			{
				_doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager)(object)base.EntityManager, args.User, component.PlungeDuration, new PlungerDoAfterEvent(), uid, target2, uid)
				{
					BreakOnMove = true,
					BreakOnDamage = true,
					MovementThreshold = 1f
				});
				((HandledEntityEventArgs)args).Handled = true;
			}
		}
	}

	private void OnDoAfter(EntityUid uid, PlungerComponent component, DoAfterEvent args)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled || ((HandledEntityEventArgs)args).Handled || !args.Args.Target.HasValue)
		{
			return;
		}
		EntityUid? target = args.Target;
		if (target.HasValue)
		{
			EntityUid target2 = target.GetValueOrDefault();
			PlungerUseComponent plunge = default(PlungerUseComponent);
			if (((EntityUid)(ref target2)).Valid && ((EntitySystem)this).TryComp<PlungerUseComponent>(target2, ref plunge))
			{
				_popup.PopupClient(base.Loc.GetString("plunger-unblock", (ValueTuple<string, object>)("target", target2)), args.User, args.User, PopupType.Medium);
				plunge.Plunged = true;
				string spawn = _proto.Index<WeightedRandomEntityPrototype>(plunge.PlungerLoot).Pick(_random);
				_audio.PlayPredicted(plunge.Sound, uid, (EntityUid?)uid, (AudioParams?)null);
				((EntitySystem)this).Spawn(spawn, ((EntitySystem)this).Transform(target2).Coordinates);
				((EntitySystem)this).RemComp<PlungerUseComponent>(target2);
				((EntitySystem)this).Dirty(target2, (IComponent)(object)plunge, (MetaDataComponent)null);
				((HandledEntityEventArgs)args).Handled = true;
			}
		}
	}
}
