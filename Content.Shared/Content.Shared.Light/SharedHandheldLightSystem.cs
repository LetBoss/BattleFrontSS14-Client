using System;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Clothing.EntitySystems;
using Content.Shared.Item;
using Content.Shared.Light.Components;
using Content.Shared.Toggleable;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Utility;

namespace Content.Shared.Light;

public abstract class SharedHandheldLightSystem : EntitySystem
{
	[Dependency]
	private SharedItemSystem _itemSys;

	[Dependency]
	private ClothingSystem _clothingSys;

	[Dependency]
	private SharedActionsSystem _actionSystem;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedAudioSystem _audio;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<HandheldLightComponent, ComponentInit>((ComponentEventHandler<HandheldLightComponent, ComponentInit>)OnInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HandheldLightComponent, ComponentHandleState>((ComponentEventRefHandler<HandheldLightComponent, ComponentHandleState>)OnHandleState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HandheldLightComponent, GetVerbsEvent<ActivationVerb>>((EntityEventRefHandler<HandheldLightComponent, GetVerbsEvent<ActivationVerb>>)AddToggleLightVerb, (Type[])null, (Type[])null);
	}

	private void OnInit(EntityUid uid, HandheldLightComponent component, ComponentInit args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		UpdateVisuals(uid, component);
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
	}

	private void OnHandleState(EntityUid uid, HandheldLightComponent component, ref ComponentHandleState args)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (((ComponentHandleState)(ref args)).Current is HandheldLightComponent.HandheldLightComponentState state)
		{
			component.Level = state.Charge;
			SetActivated(uid, state.Activated, component, makeNoise: false);
		}
	}

	public void SetActivated(EntityUid uid, bool activated, HandheldLightComponent? component = null, bool makeNoise = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<HandheldLightComponent>(uid, ref component, true) && component.Activated != activated)
		{
			component.Activated = activated;
			if (makeNoise)
			{
				SoundSpecifier sound = (component.Activated ? component.TurnOnSound : component.TurnOffSound);
				_audio.PlayPvs(sound, uid, (AudioParams?)null);
			}
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
			UpdateVisuals(uid, component);
			LightToggleEvent ev = new LightToggleEvent(activated);
			((EntitySystem)this).RaiseLocalEvent<LightToggleEvent>(uid, ev, false);
		}
	}

	public void UpdateVisuals(EntityUid uid, HandheldLightComponent? component = null, AppearanceComponent? appearance = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<HandheldLightComponent, AppearanceComponent>(uid, ref component, ref appearance, false))
		{
			if (component.AddPrefix)
			{
				string prefix = (component.Activated ? "on" : "off");
				_itemSys.SetHeldPrefix(uid, prefix);
				_clothingSys.SetEquippedPrefix(uid, prefix);
			}
			if (component.ToggleActionEntity.HasValue)
			{
				SharedActionsSystem actionSystem = _actionSystem;
				EntityUid? toggleActionEntity = component.ToggleActionEntity;
				actionSystem.SetToggled(toggleActionEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(toggleActionEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null), component.Activated);
			}
			_appearance.SetData(uid, (Enum)ToggleableVisuals.Enabled, (object)component.Activated, appearance);
		}
	}

	private void AddToggleLightVerb(Entity<HandheldLightComponent> ent, ref GetVerbsEvent<ActivationVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Expected O, but got Unknown
		if (args.CanAccess && args.CanInteract && ent.Comp.ToggleOnInteract)
		{
			GetVerbsEvent<ActivationVerb> @event = args;
			ActivationVerb verb = new ActivationVerb
			{
				Text = base.Loc.GetString("verb-common-toggle-light"),
				Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/VerbIcons/light.svg.192dpi.png")),
				Act = (ent.Comp.Activated ? ((Action)delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					TurnOff(ent);
				}) : ((Action)delegate
				{
					//IL_000c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					TurnOn(@event.User, ent);
				}))
			};
			args.Verbs.Add(verb);
		}
	}

	public abstract bool TurnOff(Entity<HandheldLightComponent> ent, bool makeNoise = true);

	public abstract bool TurnOn(EntityUid user, Entity<HandheldLightComponent> uid);
}
