using System;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Popups;
using Content.Shared.Temperature;
using Content.Shared.Toggleable;
using Content.Shared.Verbs;
using Content.Shared.Wieldable;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;

namespace Content.Shared.Item.ItemToggle;

public sealed class ItemToggleSystem : EntitySystem
{
	[Dependency]
	private INetManager _netManager;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedPopupSystem _popup;

	private EntityQuery<ItemToggleComponent> _query;

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		_query = ((EntitySystem)this).GetEntityQuery<ItemToggleComponent>();
		((EntitySystem)this).SubscribeLocalEvent<ItemToggleComponent, ComponentStartup>((EntityEventRefHandler<ItemToggleComponent, ComponentStartup>)OnStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ItemToggleComponent, MapInitEvent>((EntityEventRefHandler<ItemToggleComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ItemToggleComponent, ItemUnwieldedEvent>((EntityEventRefHandler<ItemToggleComponent, ItemUnwieldedEvent>)TurnOffOnUnwielded, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ItemToggleComponent, ItemWieldedEvent>((EntityEventRefHandler<ItemToggleComponent, ItemWieldedEvent>)TurnOnOnWielded, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ItemToggleComponent, UseInHandEvent>((EntityEventRefHandler<ItemToggleComponent, UseInHandEvent>)OnUseInHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ItemToggleComponent, GetVerbsEvent<ActivationVerb>>((EntityEventRefHandler<ItemToggleComponent, GetVerbsEvent<ActivationVerb>>)OnActivateVerb, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ItemToggleComponent, ActivateInWorldEvent>((EntityEventRefHandler<ItemToggleComponent, ActivateInWorldEvent>)OnActivate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ItemToggleHotComponent, IsHotEvent>((EntityEventRefHandler<ItemToggleHotComponent, IsHotEvent>)OnIsHotEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ItemToggleActiveSoundComponent, ItemToggledEvent>((EntityEventRefHandler<ItemToggleActiveSoundComponent, ItemToggledEvent>)UpdateActiveSound, (Type[])null, (Type[])null);
	}

	private void OnStartup(Entity<ItemToggleComponent> ent, ref ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateVisuals(ent);
	}

	private void OnMapInit(Entity<ItemToggleComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Activated)
		{
			ItemToggledEvent ev = new ItemToggledEvent(ent.Comp.Predictable, ent.Comp.Activated, null);
			((EntitySystem)this).RaiseLocalEvent<ItemToggledEvent>(Entity<ItemToggleComponent>.op_Implicit(ent), ref ev, false);
		}
	}

	private void OnUseInHand(Entity<ItemToggleComponent> ent, ref UseInHandEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && ent.Comp.OnUse)
		{
			((HandledEntityEventArgs)args).Handled = true;
			Toggle(Entity<ItemToggleComponent>.op_Implicit((Entity<ItemToggleComponent>.op_Implicit(ent), ent.Comp)), args.User, ent.Comp.Predictable);
		}
	}

	private void OnActivateVerb(Entity<ItemToggleComponent> ent, ref GetVerbsEvent<ActivationVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		if (!args.CanAccess || !args.CanInteract || !ent.Comp.OnActivate)
		{
			return;
		}
		EntityUid user = args.User;
		if (ent.Comp.Activated)
		{
			ItemToggleActivateAttemptEvent ev = new ItemToggleActivateAttemptEvent(args.User);
			((EntitySystem)this).RaiseLocalEvent<ItemToggleActivateAttemptEvent>(ent.Owner, ref ev, false);
			if (ev.Cancelled)
			{
				return;
			}
		}
		else
		{
			ItemToggleDeactivateAttemptEvent ev2 = new ItemToggleDeactivateAttemptEvent(args.User);
			((EntitySystem)this).RaiseLocalEvent<ItemToggleDeactivateAttemptEvent>(ent.Owner, ref ev2, false);
			if (ev2.Cancelled)
			{
				return;
			}
		}
		args.Verbs.Add(new ActivationVerb
		{
			Text = ((!ent.Comp.Activated) ? base.Loc.GetString(ent.Comp.VerbToggleOn) : base.Loc.GetString(ent.Comp.VerbToggleOff)),
			Act = delegate
			{
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				Toggle(Entity<ItemToggleComponent>.op_Implicit((ent.Owner, ent.Comp)), user, ent.Comp.Predictable);
			}
		});
	}

	private void OnActivate(Entity<ItemToggleComponent> ent, ref ActivateInWorldEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && ent.Comp.OnActivate)
		{
			((HandledEntityEventArgs)args).Handled = true;
			Toggle(Entity<ItemToggleComponent>.op_Implicit((ent.Owner, ent.Comp)), args.User, ent.Comp.Predictable);
		}
	}

	public bool Toggle(Entity<ItemToggleComponent?> ent, EntityUid? user = null, bool predicted = true)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (!_query.Resolve(Entity<ItemToggleComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			return false;
		}
		return TrySetActive(ent, !ent.Comp.Activated, user, predicted);
	}

	public bool TrySetActive(Entity<ItemToggleComponent?> ent, bool active, EntityUid? user = null, bool predicted = true)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		if (active)
		{
			return TryActivate(ent, user, predicted);
		}
		return TryDeactivate(ent, user, predicted);
	}

	public bool TryActivate(Entity<ItemToggleComponent?> ent, EntityUid? user = null, bool predicted = true)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		if (!_query.Resolve(Entity<ItemToggleComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			return false;
		}
		EntityUid uid = ent.Owner;
		ItemToggleComponent comp = ent.Comp;
		if (comp.Activated)
		{
			return true;
		}
		ItemToggleActivateAttemptEvent attempt = new ItemToggleActivateAttemptEvent(user);
		((EntitySystem)this).RaiseLocalEvent<ItemToggleActivateAttemptEvent>(uid, ref attempt, false);
		if (!comp.Predictable)
		{
			predicted = false;
		}
		if (!predicted && _netManager.IsClient)
		{
			return false;
		}
		if (attempt.Cancelled)
		{
			if (attempt.Silent)
			{
				return false;
			}
			if (predicted)
			{
				_audio.PlayPredicted(comp.SoundFailToActivate, uid, user, (AudioParams?)null);
			}
			else
			{
				_audio.PlayPvs(comp.SoundFailToActivate, uid, (AudioParams?)null);
			}
			if (attempt.Popup != null && user.HasValue)
			{
				if (predicted)
				{
					_popup.PopupClient(attempt.Popup, uid, user.Value);
				}
				else
				{
					_popup.PopupEntity(attempt.Popup, uid, user.Value);
				}
			}
			return false;
		}
		Activate(Entity<ItemToggleComponent>.op_Implicit((uid, comp)), predicted, user);
		return true;
	}

	public bool TryDeactivate(Entity<ItemToggleComponent?> ent, EntityUid? user = null, bool predicted = true)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		if (!_query.Resolve(Entity<ItemToggleComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			return false;
		}
		EntityUid uid = ent.Owner;
		ItemToggleComponent comp = ent.Comp;
		if (!comp.Activated)
		{
			return true;
		}
		if (!comp.Predictable)
		{
			predicted = false;
		}
		ItemToggleDeactivateAttemptEvent attempt = new ItemToggleDeactivateAttemptEvent(user);
		((EntitySystem)this).RaiseLocalEvent<ItemToggleDeactivateAttemptEvent>(uid, ref attempt, false);
		if (!predicted && _netManager.IsClient)
		{
			return false;
		}
		if (attempt.Cancelled)
		{
			if (attempt.Silent)
			{
				return false;
			}
			if (attempt.Popup != null && user.HasValue)
			{
				if (predicted)
				{
					_popup.PopupClient(attempt.Popup, uid, user.Value);
				}
				else
				{
					_popup.PopupEntity(attempt.Popup, uid, user.Value);
				}
			}
			return false;
		}
		Deactivate(Entity<ItemToggleComponent>.op_Implicit((uid, comp)), predicted, user);
		return true;
	}

	private void Activate(Entity<ItemToggleComponent> ent, bool predicted, EntityUid? user = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		Entity<ItemToggleComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		ItemToggleComponent itemToggleComponent = default(ItemToggleComponent);
		val.Deconstruct(ref val2, ref itemToggleComponent);
		EntityUid uid = val2;
		ItemToggleComponent comp = itemToggleComponent;
		SoundSpecifier soundToPlay = comp.SoundActivate;
		if (predicted)
		{
			_audio.PlayPredicted(soundToPlay, uid, user, (AudioParams?)null);
		}
		else
		{
			_audio.PlayPvs(soundToPlay, uid, (AudioParams?)null);
		}
		comp.Activated = true;
		UpdateVisuals(Entity<ItemToggleComponent>.op_Implicit((uid, comp)));
		((EntitySystem)this).Dirty(uid, (IComponent)(object)comp, (MetaDataComponent)null);
		ItemToggledEvent toggleUsed = new ItemToggledEvent(predicted, Activated: true, user);
		((EntitySystem)this).RaiseLocalEvent<ItemToggledEvent>(uid, ref toggleUsed, false);
	}

	private void Deactivate(Entity<ItemToggleComponent> ent, bool predicted, EntityUid? user = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		Entity<ItemToggleComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		ItemToggleComponent itemToggleComponent = default(ItemToggleComponent);
		val.Deconstruct(ref val2, ref itemToggleComponent);
		EntityUid uid = val2;
		ItemToggleComponent comp = itemToggleComponent;
		SoundSpecifier soundToPlay = comp.SoundDeactivate;
		if (predicted)
		{
			_audio.PlayPredicted(soundToPlay, uid, user, (AudioParams?)null);
		}
		else
		{
			_audio.PlayPvs(soundToPlay, uid, (AudioParams?)null);
		}
		comp.Activated = false;
		UpdateVisuals(Entity<ItemToggleComponent>.op_Implicit((uid, comp)));
		((EntitySystem)this).Dirty(uid, (IComponent)(object)comp, (MetaDataComponent)null);
		ItemToggledEvent toggleUsed = new ItemToggledEvent(predicted, Activated: false, user);
		((EntitySystem)this).RaiseLocalEvent<ItemToggledEvent>(uid, ref toggleUsed, false);
	}

	public void SetOnActivate(Entity<ItemToggleComponent?> ent, bool val)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<ItemToggleComponent>(Entity<ItemToggleComponent>.op_Implicit(ent), ref ent.Comp, true) && ent.Comp.OnActivate != val)
		{
			ent.Comp.OnActivate = val;
			((EntitySystem)this).Dirty<ItemToggleComponent>(ent, (MetaDataComponent)null);
		}
	}

	private void UpdateVisuals(Entity<ItemToggleComponent> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		AppearanceComponent appearance = default(AppearanceComponent);
		if (((EntitySystem)this).TryComp<AppearanceComponent>(Entity<ItemToggleComponent>.op_Implicit(ent), ref appearance))
		{
			_appearance.SetData(Entity<ItemToggleComponent>.op_Implicit(ent), (Enum)ToggleableVisuals.Enabled, (object)ent.Comp.Activated, appearance);
		}
	}

	private void TurnOffOnUnwielded(Entity<ItemToggleComponent> ent, ref ItemUnwieldedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		TryDeactivate(Entity<ItemToggleComponent>.op_Implicit((Entity<ItemToggleComponent>.op_Implicit(ent), ent.Comp)), args.User);
	}

	private void TurnOnOnWielded(Entity<ItemToggleComponent> ent, ref ItemWieldedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		TryActivate(Entity<ItemToggleComponent>.op_Implicit((Entity<ItemToggleComponent>.op_Implicit(ent), ent.Comp)));
	}

	public bool IsActivated(Entity<ItemToggleComponent?> ent)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (!_query.Resolve(Entity<ItemToggleComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			return true;
		}
		return ent.Comp.Activated;
	}

	private void OnIsHotEvent(Entity<ItemToggleHotComponent> ent, ref IsHotEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		args.IsHot |= IsActivated(Entity<ItemToggleComponent>.op_Implicit(ent.Owner));
	}

	private void UpdateActiveSound(Entity<ItemToggleActiveSoundComponent> ent, ref ItemToggledEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		Entity<ItemToggleActiveSoundComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		ItemToggleActiveSoundComponent itemToggleActiveSoundComponent = default(ItemToggleActiveSoundComponent);
		val.Deconstruct(ref val2, ref itemToggleActiveSoundComponent);
		EntityUid uid = val2;
		ItemToggleActiveSoundComponent comp = itemToggleActiveSoundComponent;
		if (!args.Activated)
		{
			comp.PlayingStream = _audio.Stop(comp.PlayingStream, (AudioComponent)null);
		}
		else if (comp.ActiveSound != null && !comp.PlayingStream.HasValue)
		{
			AudioParams val3 = comp.ActiveSound.Params;
			AudioParams loop = ((AudioParams)(ref val3)).WithLoop(true);
			EntityUid? val4 = (args.Predicted ? _audio.PlayPredicted(comp.ActiveSound, uid, args.User, (AudioParams?)loop) : _audio.PlayPvs(comp.ActiveSound, uid, (AudioParams?)loop))?.Item1;
			if (val4.HasValue)
			{
				EntityUid entity = val4.GetValueOrDefault();
				comp.PlayingStream = entity;
			}
		}
	}
}
