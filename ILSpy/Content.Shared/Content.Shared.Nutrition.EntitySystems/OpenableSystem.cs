using System;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Lock;
using Content.Shared.Nutrition.Components;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Utility;

namespace Content.Shared.Nutrition.EntitySystems;

public sealed class OpenableSystem : EntitySystem
{
	[Dependency]
	private LockSystem _lock;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedPopupSystem _popup;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<OpenableComponent, ComponentInit>((EntityEventRefHandler<OpenableComponent, ComponentInit>)OnInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<OpenableComponent, UseInHandEvent>((EntityEventRefHandler<OpenableComponent, UseInHandEvent>)OnUse, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<OpenableComponent, ActivateInWorldEvent>((EntityEventRefHandler<OpenableComponent, ActivateInWorldEvent>)OnActivated, (Type[])null, new Type[1] { typeof(LockSystem) });
		((EntitySystem)this).SubscribeLocalEvent<OpenableComponent, ExaminedEvent>((ComponentEventHandler<OpenableComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<OpenableComponent, MeleeHitEvent>((ComponentEventHandler<OpenableComponent, MeleeHitEvent>)HandleIfClosed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<OpenableComponent, AfterInteractEvent>((ComponentEventHandler<OpenableComponent, AfterInteractEvent>)HandleIfClosed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<OpenableComponent, GetVerbsEvent<AlternativeVerb>>((ComponentEventHandler<OpenableComponent, GetVerbsEvent<AlternativeVerb>>)OnGetVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<OpenableComponent, SolutionTransferAttemptEvent>((EntityEventRefHandler<OpenableComponent, SolutionTransferAttemptEvent>)OnTransferAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<OpenableComponent, AttemptShakeEvent>((EntityEventRefHandler<OpenableComponent, AttemptShakeEvent>)OnAttemptShake, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<OpenableComponent, AttemptAddFizzinessEvent>((EntityEventRefHandler<OpenableComponent, AttemptAddFizzinessEvent>)OnAttemptAddFizziness, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<OpenableComponent, LockToggleAttemptEvent>((EntityEventRefHandler<OpenableComponent, LockToggleAttemptEvent>)OnLockToggleAttempt, (Type[])null, (Type[])null);
	}

	private void OnInit(Entity<OpenableComponent> ent, ref ComponentInit args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		UpdateAppearance(Entity<OpenableComponent>.op_Implicit(ent), ent.Comp);
	}

	private void OnUse(Entity<OpenableComponent> ent, ref UseInHandEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && ent.Comp.OpenableByHand)
		{
			((HandledEntityEventArgs)args).Handled = TryOpen(Entity<OpenableComponent>.op_Implicit(ent), Entity<OpenableComponent>.op_Implicit(ent), args.User);
		}
	}

	private void OnActivated(Entity<OpenableComponent> ent, ref ActivateInWorldEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && ent.Comp.OpenOnActivate)
		{
			((HandledEntityEventArgs)args).Handled = TryToggle(ent, args.User);
		}
	}

	private void OnExamined(EntityUid uid, OpenableComponent comp, ExaminedEvent args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (comp.Opened && args.IsInDetailsRange)
		{
			string text = base.Loc.GetString(LocId.op_Implicit(comp.ExamineText));
			args.PushMarkup(text);
		}
	}

	private void HandleIfClosed(EntityUid uid, OpenableComponent comp, HandledEntityEventArgs args)
	{
		args.Handled = !comp.Opened;
	}

	private void OnGetVerbs(EntityUid uid, OpenableComponent comp, GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Expected O, but got Unknown
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Expected O, but got Unknown
		if (args.Hands == null || !args.CanAccess || !args.CanInteract || _lock.IsLocked(Entity<LockComponent>.op_Implicit(uid)))
		{
			return;
		}
		AlternativeVerb verb;
		if (comp.Opened)
		{
			if (!comp.Closeable)
			{
				return;
			}
			verb = new AlternativeVerb
			{
				Text = base.Loc.GetString(LocId.op_Implicit(comp.CloseVerbText)),
				Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/VerbIcons/close.svg.192dpi.png")),
				Act = delegate
				{
					//IL_000c: Unknown result type (might be due to invalid IL or missing references)
					//IL_001d: Unknown result type (might be due to invalid IL or missing references)
					TryClose(args.Target, comp, args.User);
				}
			};
		}
		else
		{
			verb = new AlternativeVerb
			{
				Text = base.Loc.GetString(LocId.op_Implicit(comp.OpenVerbText)),
				Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/VerbIcons/open.svg.192dpi.png")),
				Act = delegate
				{
					//IL_000c: Unknown result type (might be due to invalid IL or missing references)
					//IL_001d: Unknown result type (might be due to invalid IL or missing references)
					TryOpen(args.Target, comp, args.User);
				}
			};
		}
		args.Verbs.Add(verb);
	}

	private void OnTransferAttempt(Entity<OpenableComponent> ent, ref SolutionTransferAttemptEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (!ent.Comp.Opened)
		{
			args.Cancel(base.Loc.GetString("drink-component-try-use-drink-not-open", (ValueTuple<string, object>)("owner", ent.Owner)));
		}
	}

	private void OnAttemptShake(Entity<OpenableComponent> entity, ref AttemptShakeEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		if (entity.Comp.Opened)
		{
			args.Cancelled = true;
		}
	}

	private void OnAttemptAddFizziness(Entity<OpenableComponent> entity, ref AttemptAddFizzinessEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		if (entity.Comp.Opened)
		{
			args.Cancelled = true;
		}
	}

	private void OnLockToggleAttempt(Entity<OpenableComponent> ent, ref LockToggleAttemptEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Opened)
		{
			args.Cancelled = true;
		}
	}

	public bool IsOpen(EntityUid uid, OpenableComponent? comp = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<OpenableComponent>(uid, ref comp, false))
		{
			return true;
		}
		return comp.Opened;
	}

	public bool IsClosed(EntityUid uid, EntityUid? user = null, OpenableComponent? comp = null, bool predicted = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<OpenableComponent>(uid, ref comp, false))
		{
			return false;
		}
		if (comp.Opened)
		{
			return false;
		}
		if (user.HasValue)
		{
			if (predicted)
			{
				_popup.PopupClient(base.Loc.GetString(LocId.op_Implicit(comp.ClosedPopup), (ValueTuple<string, object>)("owner", uid)), user.Value, user.Value);
			}
			else
			{
				_popup.PopupEntity(base.Loc.GetString(LocId.op_Implicit(comp.ClosedPopup), (ValueTuple<string, object>)("owner", uid)), user.Value, user.Value);
			}
		}
		return true;
	}

	public void UpdateAppearance(EntityUid uid, OpenableComponent? comp = null, AppearanceComponent? appearance = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<OpenableComponent>(uid, ref comp, true))
		{
			_appearance.SetData(uid, (Enum)OpenableVisuals.Opened, (object)comp.Opened, appearance);
		}
	}

	public void SetOpen(EntityUid uid, bool opened = true, OpenableComponent? comp = null, EntityUid? user = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<OpenableComponent>(uid, ref comp, false) && opened != comp.Opened)
		{
			comp.Opened = opened;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)comp, (MetaDataComponent)null);
			if (opened)
			{
				OpenableOpenedEvent ev = new OpenableOpenedEvent(user);
				((EntitySystem)this).RaiseLocalEvent<OpenableOpenedEvent>(uid, ref ev, false);
			}
			else
			{
				OpenableClosedEvent ev2 = new OpenableClosedEvent(user);
				((EntitySystem)this).RaiseLocalEvent<OpenableClosedEvent>(uid, ref ev2, false);
			}
			UpdateAppearance(uid, comp);
		}
	}

	public bool TryOpen(EntityUid uid, OpenableComponent? comp = null, EntityUid? user = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<OpenableComponent>(uid, ref comp, false) || comp.Opened || _lock.IsLocked(Entity<LockComponent>.op_Implicit(uid)))
		{
			return false;
		}
		OpenableOpenAttemptEvent ev = new OpenableOpenAttemptEvent(user);
		((EntitySystem)this).RaiseLocalEvent<OpenableOpenAttemptEvent>(uid, ref ev, false);
		if (ev.Cancelled)
		{
			return false;
		}
		SetOpen(uid, opened: true, comp, user);
		_audio.PlayPredicted(comp.Sound, uid, user, (AudioParams?)null);
		return true;
	}

	public bool TryClose(EntityUid uid, OpenableComponent? comp = null, EntityUid? user = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<OpenableComponent>(uid, ref comp, false) || !comp.Opened || !comp.Closeable)
		{
			return false;
		}
		SetOpen(uid, opened: false, comp, user);
		if (comp.CloseSound != null)
		{
			_audio.PlayPredicted(comp.CloseSound, uid, user, (AudioParams?)null);
		}
		return true;
	}

	public bool TryToggle(Entity<OpenableComponent> ent, EntityUid? user)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Opened && ent.Comp.Closeable)
		{
			return TryClose(Entity<OpenableComponent>.op_Implicit(ent), ent.Comp, user);
		}
		return TryOpen(Entity<OpenableComponent>.op_Implicit(ent), ent.Comp, user);
	}
}
