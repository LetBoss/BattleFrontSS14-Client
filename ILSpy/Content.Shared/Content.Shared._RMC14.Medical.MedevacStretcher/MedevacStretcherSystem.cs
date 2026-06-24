using System;
using System.Linq;
using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.Dropship.Utility.Events;
using Content.Shared._RMC14.Dropship.Weapon;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Buckle.Components;
using Content.Shared.Coordinates;
using Content.Shared.Coordinates.Helpers;
using Content.Shared.Examine;
using Content.Shared.Foldable;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Medical.MedevacStretcher;

public sealed class MedevacStretcherSystem : EntitySystem
{
	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private AreaSystem _areas;

	[Dependency]
	private SharedDropshipWeaponSystem _dropshipWeapon;

	[Dependency]
	private IMapManager _mapManager;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SkillsSystem _skills;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedTransformSystem _transform;

	private static readonly EntProtoId<SkillDefinitionComponent> SkillType = EntProtoId<SkillDefinitionComponent>.op_Implicit("RMCSkillMedical");

	private const int MinimumRequiredSkill = 1;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<MedevacStretcherComponent, GetVerbsEvent<InteractionVerb>>((EntityEventRefHandler<MedevacStretcherComponent, GetVerbsEvent<InteractionVerb>>)AddActivateBeaconVerb, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MedevacStretcherComponent, FoldedEvent>((EntityEventRefHandler<MedevacStretcherComponent, FoldedEvent>)OnFold, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MedevacStretcherComponent, PrepareMedevacEvent>((EntityEventRefHandler<MedevacStretcherComponent, PrepareMedevacEvent>)PrepareMedevac, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MedevacStretcherComponent, ExaminedEvent>((EntityEventRefHandler<MedevacStretcherComponent, ExaminedEvent>)OnExamine, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MedevacStretcherComponent, InteractHandEvent>((EntityEventRefHandler<MedevacStretcherComponent, InteractHandEvent>)OnInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MedevacStretcherComponent, StrappedEvent>((EntityEventRefHandler<MedevacStretcherComponent, StrappedEvent>)OnStrapped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MedevacStretcherComponent, UnstrappedEvent>((EntityEventRefHandler<MedevacStretcherComponent, UnstrappedEvent>)OnStrapped, (Type[])null, (Type[])null);
	}

	public void Medevac(Entity<MedevacStretcherComponent> ent, EntityUid medevacEntity)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		StrapComponent strap = default(StrapComponent);
		if (_net.IsClient || !((EntitySystem)this).TryComp<StrapComponent>(ent.Owner, ref strap) || strap.BuckledEntities.Count == 0)
		{
			return;
		}
		foreach (EntityUid buckled in strap.BuckledEntities)
		{
			_transform.PlaceNextTo(Entity<TransformComponent>.op_Implicit(buckled), Entity<TransformComponent>.op_Implicit(medevacEntity));
		}
		_appearance.SetData(Entity<MedevacStretcherComponent>.op_Implicit(ent), (Enum)MedevacStretcherVisuals.MedevacingState, (object)false, (AppearanceComponent)null);
	}

	private void OnExamine(Entity<MedevacStretcherComponent> ent, ref ExaminedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		StrapComponent strap = default(StrapComponent);
		if (!((EntitySystem)this).TryComp<StrapComponent>(Entity<MedevacStretcherComponent>.op_Implicit(ent), ref strap) || strap.BuckledEntities.Count == 0)
		{
			return;
		}
		string name = ((EntitySystem)this).Name(strap.BuckledEntities.First(), (MetaDataComponent)null);
		using (args.PushGroup("MedevacStretcherComponent"))
		{
			args.PushText(base.Loc.GetString("rmc-medevac-stretcher-examine-id", (ValueTuple<string, object>)("id", name)));
		}
	}

	private void AddActivateBeaconVerb(Entity<MedevacStretcherComponent> ent, ref GetVerbsEvent<InteractionVerb> args)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (args.CanAccess && args.CanInteract && args.Hands != null && _skills.HasSkill(Entity<SkillsComponent>.op_Implicit(args.User), SkillType, 1))
		{
			GetVerbsEvent<InteractionVerb> @event = args;
			args.Verbs.Add(new InteractionVerb
			{
				Text = base.Loc.GetString("rmc-medevac-toggle-beacon-verb"),
				Act = delegate
				{
					//IL_000c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0017: Unknown result type (might be due to invalid IL or missing references)
					ToggleBeacon(@event.Target, @event.User);
				},
				Priority = 1
			});
		}
	}

	private void OnFold(Entity<MedevacStretcherComponent> ent, ref FoldedEvent args)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState)
		{
			if (args.IsFolded)
			{
				DeactivateBeacon(ent.Owner);
			}
			else
			{
				ActivateBeacon(ent.Owner, null);
			}
		}
	}

	private void PrepareMedevac(Entity<MedevacStretcherComponent> ent, ref PrepareMedevacEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		StrapComponent strap = default(StrapComponent);
		if (((EntitySystem)this).TryComp<StrapComponent>(ent.Owner, ref strap) && strap.BuckledEntities.Count != 0)
		{
			_appearance.SetData(ent.Owner, (Enum)MedevacStretcherVisuals.MedevacingState, (object)true, (AppearanceComponent)null);
			args.ReadyForMedevac = true;
		}
	}

	private void OnInteract(Entity<MedevacStretcherComponent> ent, ref InteractHandEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		FoldableComponent foldComp = default(FoldableComponent);
		if (!((EntitySystem)this).HasComp<XenoComponent>(args.User) && !((HandledEntityEventArgs)args).Handled && (!((EntitySystem)this).TryComp<FoldableComponent>(ent.Owner, ref foldComp) || !foldComp.IsFolded))
		{
			ToggleBeacon(args.Target, args.User);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnStrapped<T>(Entity<MedevacStretcherComponent> ent, ref T args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		DropshipTargetComponent dropshipTarget = default(DropshipTargetComponent);
		if (((EntitySystem)this).TryComp<DropshipTargetComponent>(Entity<MedevacStretcherComponent>.op_Implicit(ent), ref dropshipTarget))
		{
			dropshipTarget.Abbreviation = GetName(Entity<StrapComponent>.op_Implicit(ent.Owner));
			((EntitySystem)this).Dirty(Entity<MedevacStretcherComponent>.op_Implicit(ent), (IComponent)(object)dropshipTarget, (MetaDataComponent)null);
			_dropshipWeapon.TargetUpdated(Entity<DropshipTargetComponent>.op_Implicit((Entity<MedevacStretcherComponent>.op_Implicit(ent), dropshipTarget)));
		}
	}

	private void ToggleBeacon(EntityUid stretcher, EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<DropshipTargetComponent>(stretcher))
		{
			DeactivateBeacon(stretcher);
		}
		else
		{
			ActivateBeacon(stretcher, user);
		}
	}

	private void ActivateBeacon(EntityUid stretcher, EntityUid? user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<DropshipTargetComponent>(stretcher))
		{
			EntityCoordinates stretcherCoords = stretcher.ToCoordinates();
			EntityCoordinates snappedCoords = stretcher.ToCoordinates().SnapToGrid((IEntityManager?)(object)base.EntityManager, _mapManager);
			if (!_dropshipWeapon.CasDebug && (!_areas.TryGetArea(snappedCoords, out Entity<AreaComponent>? stretcherArea, out EntityPrototype _) || !stretcherArea.Value.Comp.Medevac))
			{
				_popup.PopupClient(base.Loc.GetString("rmc-medevac-area-not-cas"), stretcherCoords, user);
			}
			else if (((EntitySystem)this).HasComp<MedevacStretcherComponent>(stretcher))
			{
				string name = GetName(Entity<StrapComponent>.op_Implicit(stretcher));
				_dropshipWeapon.MakeTarget(stretcher, name, targetableByWeapons: false);
				_appearance.SetData(stretcher, (Enum)MedevacStretcherVisuals.BeaconState, (object)BeaconVisuals.On, (AppearanceComponent)null);
				_popup.PopupClient(base.Loc.GetString("rmc-medevac-activate-beacon"), stretcherCoords, user);
			}
		}
	}

	private void DeactivateBeacon(EntityUid stretcher)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<DropshipTargetComponent>(stretcher))
		{
			((EntitySystem)this).RemCompDeferred<DropshipTargetComponent>(stretcher);
			_appearance.SetData(stretcher, (Enum)MedevacStretcherVisuals.BeaconState, (object)BeaconVisuals.Off, (AppearanceComponent)null);
			_appearance.SetData(stretcher, (Enum)MedevacStretcherVisuals.MedevacingState, (object)false, (AppearanceComponent)null);
		}
	}

	private string GetName(Entity<StrapComponent?> stretcher)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<StrapComponent>(Entity<StrapComponent>.op_Implicit(stretcher), ref stretcher.Comp, false) || stretcher.Comp.BuckledEntities.Count <= 0)
		{
			return "Empty";
		}
		return ((EntitySystem)this).Name(stretcher.Comp.BuckledEntities.First(), (MetaDataComponent)null);
	}
}
