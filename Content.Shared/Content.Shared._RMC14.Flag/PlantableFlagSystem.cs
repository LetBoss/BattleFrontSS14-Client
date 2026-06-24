using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Content.Shared._RMC14.Map;
using Content.Shared._RMC14.Sprite;
using Content.Shared._RMC14.Weapons.Ranged.IFF;
using Content.Shared.CombatMode;
using Content.Shared.Coordinates;
using Content.Shared.DoAfter;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction.Events;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Flag;

public sealed class PlantableFlagSystem : EntitySystem
{
	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedCombatModeSystem _combatMode;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private EntityLookupSystem _entityLookup;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private RMCMapSystem _rmcMap;

	[Dependency]
	private SharedRMCSpriteSystem _rmcSprite;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private GunIFFSystem _gunIFF;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<PlantableFlagComponent, UseInHandEvent>((EntityEventRefHandler<PlantableFlagComponent, UseInHandEvent>)OnUseInHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PlantableFlagComponent, PlantableFlagPlantDoAfterEvent>((EntityEventRefHandler<PlantableFlagComponent, PlantableFlagPlantDoAfterEvent>)OnPlantDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PlantableFlagComponent, PlantableFlagRemoveDoAfterEvent>((EntityEventRefHandler<PlantableFlagComponent, PlantableFlagRemoveDoAfterEvent>)OnRemoveDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PlantableFlagComponent, GetVerbsEvent<AlternativeVerb>>((EntityEventRefHandler<PlantableFlagComponent, GetVerbsEvent<AlternativeVerb>>)OnGetAlternativeVerbs, (Type[])null, (Type[])null);
	}

	private void OnUseInHand(Entity<PlantableFlagComponent> ent, ref UseInHandEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		if (CanPlantFlagPopup(ent, args.User, out var _))
		{
			((HandledEntityEventArgs)args).Handled = true;
			PlantableFlagPlantDoAfterEvent ev = new PlantableFlagPlantDoAfterEvent();
			DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, args.User, ent.Comp.Delay, ev, Entity<PlantableFlagComponent>.op_Implicit(ent), Entity<PlantableFlagComponent>.op_Implicit(ent), Entity<PlantableFlagComponent>.op_Implicit(ent))
			{
				BreakOnMove = true,
				NeedHand = true
			};
			if (_doAfter.TryStartDoAfter(doAfter))
			{
				_audio.PlayPredicted(ent.Comp.RaiseStartSound, Entity<PlantableFlagComponent>.op_Implicit(ent), (EntityUid?)args.User, (AudioParams?)null);
			}
		}
	}

	private void OnPlantDoAfter(Entity<PlantableFlagComponent> ent, ref PlantableFlagPlantDoAfterEvent args)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || args.Cancelled)
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		if (!CanPlantFlagPopup(ent, args.User, out var target))
		{
			return;
		}
		_transform.SetCoordinates(Entity<PlantableFlagComponent>.op_Implicit(ent), target.Value);
		_transform.SetLocalRotation(Entity<PlantableFlagComponent>.op_Implicit(ent), Angle.Zero, (TransformComponent)null);
		_transform.AnchorEntity(Entity<PlantableFlagComponent>.op_Implicit(ent));
		_appearance.SetData(Entity<PlantableFlagComponent>.op_Implicit(ent), (Enum)PlantableFlagVisuals.Planted, (object)true, (AppearanceComponent)null);
		if (ent.Comp.DeployOffset != Vector2.Zero)
		{
			_rmcSprite.SetOffset(Entity<PlantableFlagComponent>.op_Implicit(ent), ent.Comp.DeployOffset);
		}
		if (_net.IsClient)
		{
			return;
		}
		SoundSpecifier sound = ent.Comp.RaiseEndSound;
		if (_combatMode.IsInCombatMode(args.User))
		{
			sound = ent.Comp.RaisedCombatSound;
			if (_gunIFF.TryGetFaction(Entity<UserIFFComponent>.op_Implicit((args.User, ((EntitySystem)this).CompOrNull<UserIFFComponent>(args.User))), out EntProtoId<IFFFactionComponent> userFaction))
			{
				int allies = 0;
				foreach (Entity<UserIFFComponent> inRangeEnt in _entityLookup.GetEntitiesInRange<UserIFFComponent>(args.User.ToCoordinates(), (float)ent.Comp.AlliesRange, (LookupFlags)110))
				{
					if (_gunIFF.IsInFaction(Entity<UserIFFComponent>.op_Implicit((inRangeEnt.Owner, inRangeEnt.Comp)), userFaction))
					{
						allies++;
					}
					if (allies >= ent.Comp.AlliesRequired)
					{
						sound = ent.Comp.RaisedCombatAlliesSound;
						break;
					}
				}
			}
		}
		_audio.PlayPvs(sound, Entity<PlantableFlagComponent>.op_Implicit(ent), (AudioParams?)null);
	}

	private void OnRemoveDoAfter(Entity<PlantableFlagComponent> ent, ref PlantableFlagRemoveDoAfterEvent args)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && !args.Cancelled)
		{
			((HandledEntityEventArgs)args).Handled = true;
			_transform.Unanchor(Entity<PlantableFlagComponent>.op_Implicit(ent));
			_hands.TryPickupAnyHand(args.User, Entity<PlantableFlagComponent>.op_Implicit(ent));
			_appearance.SetData(Entity<PlantableFlagComponent>.op_Implicit(ent), (Enum)PlantableFlagVisuals.Planted, (object)false, (AppearanceComponent)null);
			_rmcSprite.SetOffset(Entity<PlantableFlagComponent>.op_Implicit(ent), Vector2.Zero);
		}
	}

	private void OnGetAlternativeVerbs(Entity<PlantableFlagComponent> ent, ref GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent transform = default(TransformComponent);
		if (!((EntitySystem)this).TryComp(Entity<PlantableFlagComponent>.op_Implicit(ent), ref transform) || !transform.Anchored)
		{
			return;
		}
		EntityUid user = args.User;
		args.Verbs.Add(new AlternativeVerb
		{
			Text = "Take Down",
			Act = delegate
			{
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0039: Unknown result type (might be due to invalid IL or missing references)
				//IL_003e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0049: Unknown result type (might be due to invalid IL or missing references)
				//IL_004e: Unknown result type (might be due to invalid IL or missing references)
				//IL_009c: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
				PlantableFlagRemoveDoAfterEvent plantableFlagRemoveDoAfterEvent = new PlantableFlagRemoveDoAfterEvent();
				DoAfterArgs args2 = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, ent.Comp.Delay, plantableFlagRemoveDoAfterEvent, Entity<PlantableFlagComponent>.op_Implicit(ent), Entity<PlantableFlagComponent>.op_Implicit(ent), Entity<PlantableFlagComponent>.op_Implicit(ent))
				{
					BreakOnMove = true,
					NeedHand = true
				};
				if (_doAfter.TryStartDoAfter(args2))
				{
					_audio.PlayPredicted(ent.Comp.LowerStartSound, Entity<PlantableFlagComponent>.op_Implicit(ent), (EntityUid?)user, (AudioParams?)null);
				}
			}
		});
	}

	private bool CanPlantFlagPopup(Entity<PlantableFlagComponent> ent, EntityUid user, [NotNullWhen(true)] out EntityCoordinates? target)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		target = null;
		TransformComponent userTransform = default(TransformComponent);
		if (!((EntitySystem)this).TryComp(user, ref userTransform))
		{
			return false;
		}
		ValueTuple<EntityCoordinates, Angle> moverCoordinateRotation = _transform.GetMoverCoordinateRotation(user, userTransform);
		EntityCoordinates coords = moverCoordinateRotation.Item1;
		Angle rot = moverCoordinateRotation.Item2;
		target = ((EntityCoordinates)(ref coords)).Offset(((Angle)(ref rot)).ToWorldVec());
		if (_rmcMap.IsTileBlocked(target.Value))
		{
			_popup.PopupClient("You need a clear, open area to plant the " + ((EntitySystem)this).Name(Entity<PlantableFlagComponent>.op_Implicit(ent), (MetaDataComponent)null) + ", something is blocking the way in front of you!", user, user, PopupType.MediumCaution);
			return false;
		}
		return true;
	}
}
