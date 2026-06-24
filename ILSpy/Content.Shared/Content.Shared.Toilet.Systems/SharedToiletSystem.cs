using System;
using Content.Shared.Buckle.Components;
using Content.Shared.Interaction;
using Content.Shared.Plunger.Components;
using Content.Shared.Toilet.Components;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;
using Robust.Shared.Utility;

namespace Content.Shared.Toilet.Systems;

public abstract class SharedToiletSystem : EntitySystem
{
	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ToiletComponent, MapInitEvent>((ComponentEventHandler<ToiletComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ToiletComponent, GetVerbsEvent<AlternativeVerb>>((ComponentEventHandler<ToiletComponent, GetVerbsEvent<AlternativeVerb>>)OnToggleSeatVerb, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ToiletComponent, ActivateInWorldEvent>((ComponentEventHandler<ToiletComponent, ActivateInWorldEvent>)OnActivateInWorld, (Type[])null, (Type[])null);
	}

	private void OnMapInit(EntityUid uid, ToiletComponent component, MapInitEvent args)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (RandomExtensions.Prob(_random, 0.5f))
		{
			component.ToggleSeat = true;
		}
		if (RandomExtensions.Prob(_random, 0.3f))
		{
			PlungerUseComponent plunger = default(PlungerUseComponent);
			((EntitySystem)this).TryComp<PlungerUseComponent>(uid, ref plunger);
			if (plunger == null)
			{
				return;
			}
			plunger.NeedsPlunger = true;
		}
		UpdateAppearance(uid);
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
	}

	public bool CanToggle(EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		StrapComponent strap = default(StrapComponent);
		if (((EntitySystem)this).TryComp<StrapComponent>(uid, ref strap))
		{
			return strap.BuckledEntities.Count == 0;
		}
		return false;
	}

	private void OnToggleSeatVerb(EntityUid uid, ToiletComponent component, GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Expected O, but got Unknown
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Expected O, but got Unknown
		if (args.CanInteract && args.CanAccess && CanToggle(uid) && args.Hands != null)
		{
			AlternativeVerb toggleVerb = new AlternativeVerb
			{
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					ToggleToiletSeat(uid, args.User, component);
				}
			};
			if (component.ToggleSeat)
			{
				toggleVerb.Text = base.Loc.GetString("toilet-seat-close");
				toggleVerb.Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/VerbIcons/close.svg.192dpi.png"));
			}
			else
			{
				toggleVerb.Text = base.Loc.GetString("toilet-seat-open");
				toggleVerb.Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/VerbIcons/open.svg.192dpi.png"));
			}
			args.Verbs.Add(toggleVerb);
		}
	}

	private void OnActivateInWorld(EntityUid uid, ToiletComponent comp, ActivateInWorldEvent args)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && args.Complex)
		{
			((HandledEntityEventArgs)args).Handled = true;
			ToggleToiletSeat(uid, args.User, comp);
		}
	}

	public void ToggleToiletSeat(EntityUid uid, EntityUid? user = null, ToiletComponent? component = null, MetaDataComponent? meta = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<ToiletComponent>(uid, ref component, true))
		{
			component.ToggleSeat = !component.ToggleSeat;
			_audio.PlayPredicted(component.SeatSound, uid, (EntityUid?)uid, (AudioParams?)null);
			UpdateAppearance(uid, component);
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, meta);
		}
	}

	private void UpdateAppearance(EntityUid uid, ToiletComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<ToiletComponent>(uid, ref component, true))
		{
			_appearance.SetData(uid, (Enum)ToiletVisuals.SeatVisualState, (object)((!component.ToggleSeat) ? SeatVisualState.SeatDown : SeatVisualState.SeatUp), (AppearanceComponent)null);
		}
	}
}
