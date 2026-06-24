using System;
using System.Collections.Generic;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Decals;
using Content.Shared.Emag.Systems;
using Content.Shared.Light.Components;
using Content.Shared.Mind.Components;
using Content.Shared.Toggleable;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Utility;

namespace Content.Shared.Light.EntitySystems;

public sealed class UnpoweredFlashlightSystem : EntitySystem
{
	[Dependency]
	private IPrototypeManager _prototypeManager;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private SharedActionsSystem _actionsSystem;

	[Dependency]
	private ActionContainerSystem _actionContainer;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedAudioSystem _audioSystem;

	[Dependency]
	private SharedPointLightSystem _light;

	[Dependency]
	private EmagSystem _emag;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<UnpoweredFlashlightComponent, GetVerbsEvent<ActivationVerb>>((ComponentEventHandler<UnpoweredFlashlightComponent, GetVerbsEvent<ActivationVerb>>)AddToggleLightVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<UnpoweredFlashlightComponent, GetItemActionsEvent>((ComponentEventHandler<UnpoweredFlashlightComponent, GetItemActionsEvent>)OnGetActions, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<UnpoweredFlashlightComponent, ToggleActionEvent>((ComponentEventHandler<UnpoweredFlashlightComponent, ToggleActionEvent>)OnToggleAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<UnpoweredFlashlightComponent, MindAddedMessage>((ComponentEventHandler<UnpoweredFlashlightComponent, MindAddedMessage>)OnMindAdded, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<UnpoweredFlashlightComponent, GotEmaggedEvent>((ComponentEventRefHandler<UnpoweredFlashlightComponent, GotEmaggedEvent>)OnGotEmagged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<UnpoweredFlashlightComponent, MapInitEvent>((ComponentEventHandler<UnpoweredFlashlightComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
	}

	private void OnMapInit(EntityUid uid, UnpoweredFlashlightComponent component, MapInitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		_actionContainer.EnsureAction(uid, ref component.ToggleActionEntity, EntProtoId.op_Implicit(component.ToggleAction));
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
	}

	private void OnToggleAction(EntityUid uid, UnpoweredFlashlightComponent component, ToggleActionEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			TryToggleLight(Entity<UnpoweredFlashlightComponent>.op_Implicit((uid, component)), args.Performer);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnGetActions(EntityUid uid, UnpoweredFlashlightComponent component, GetItemActionsEvent args)
	{
		args.AddAction(component.ToggleActionEntity);
	}

	private void AddToggleLightVerbs(EntityUid uid, UnpoweredFlashlightComponent component, GetVerbsEvent<ActivationVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Expected O, but got Unknown
		if (args.CanAccess && args.CanInteract)
		{
			ActivationVerb verb = new ActivationVerb
			{
				Text = base.Loc.GetString("toggle-flashlight-verb-get-data-text"),
				Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/VerbIcons/light.svg.192dpi.png")),
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0017: Unknown result type (might be due to invalid IL or missing references)
					//IL_0022: Unknown result type (might be due to invalid IL or missing references)
					TryToggleLight(Entity<UnpoweredFlashlightComponent>.op_Implicit((uid, component)), args.User);
				},
				Priority = -1
			};
			args.Verbs.Add(verb);
		}
	}

	private void OnMindAdded(EntityUid uid, UnpoweredFlashlightComponent component, MindAddedMessage args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		_actionsSystem.AddAction(uid, ref component.ToggleActionEntity, EntProtoId.op_Implicit(component.ToggleAction));
	}

	private void OnGotEmagged(EntityUid uid, UnpoweredFlashlightComponent component, ref GotEmaggedEvent args)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		SharedPointLightComponent light = default(SharedPointLightComponent);
		if (_emag.CompareFlag(args.Type, EmagType.Interaction) && _light.TryGetLight(uid, ref light))
		{
			ColorPalettePrototype possibleColors = default(ColorPalettePrototype);
			if (_prototypeManager.TryIndex<ColorPalettePrototype>(component.EmaggedColorsPrototype, ref possibleColors))
			{
				Color pick = RandomExtensions.Pick<Color>(_random, (IReadOnlyCollection<Color>)possibleColors.Colors.Values);
				_light.SetColor(uid, pick, light);
			}
			args.Repeatable = true;
			args.Handled = true;
		}
	}

	public void TryToggleLight(Entity<UnpoweredFlashlightComponent?> ent, EntityUid? user = null, bool quiet = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<UnpoweredFlashlightComponent>(Entity<UnpoweredFlashlightComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			SetLight(ent, !ent.Comp.LightOn, user, quiet);
		}
	}

	public void SetLight(Entity<UnpoweredFlashlightComponent?> ent, bool value, EntityUid? user = null, bool quiet = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		SharedPointLightComponent light = default(SharedPointLightComponent);
		if (((EntitySystem)this).Resolve<UnpoweredFlashlightComponent>(Entity<UnpoweredFlashlightComponent>.op_Implicit(ent), ref ent.Comp, true) && ent.Comp.LightOn != value && _light.TryGetLight(Entity<UnpoweredFlashlightComponent>.op_Implicit(ent), ref light))
		{
			((EntitySystem)this).Dirty<UnpoweredFlashlightComponent>(ent, (MetaDataComponent)null);
			ent.Comp.LightOn = value;
			_light.SetEnabled(Entity<UnpoweredFlashlightComponent>.op_Implicit(ent), value, light, (MetaDataComponent)null);
			_appearance.SetData(Entity<UnpoweredFlashlightComponent>.op_Implicit(ent), (Enum)UnpoweredFlashlightVisuals.LightOn, (object)value, (AppearanceComponent)null);
			if (!quiet)
			{
				_audioSystem.PlayPredicted(ent.Comp.ToggleSound, Entity<UnpoweredFlashlightComponent>.op_Implicit(ent), user, (AudioParams?)null);
			}
			SharedActionsSystem actionsSystem = _actionsSystem;
			EntityUid? toggleActionEntity = ent.Comp.ToggleActionEntity;
			actionsSystem.SetToggled(toggleActionEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(toggleActionEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null), value);
			((EntitySystem)this).RaiseLocalEvent<LightToggleEvent>(Entity<UnpoweredFlashlightComponent>.op_Implicit(ent), new LightToggleEvent(value), false);
		}
	}
}
