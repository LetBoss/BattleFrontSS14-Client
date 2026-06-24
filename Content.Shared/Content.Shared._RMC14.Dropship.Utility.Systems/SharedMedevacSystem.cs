using System;
using Content.Shared._RMC14.Dropship.AttachmentPoint;
using Content.Shared._RMC14.Dropship.Utility.Components;
using Content.Shared._RMC14.Dropship.Utility.Events;
using Content.Shared._RMC14.Medical.MedevacStretcher;
using Content.Shared.Coordinates;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Timing;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Network;

namespace Content.Shared._RMC14.Dropship.Utility.Systems;

public abstract class SharedMedevacSystem : EntitySystem
{
	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private DropshipUtilitySystem _dropshipUtility;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private MedevacStretcherSystem _stretcher;

	[Dependency]
	private UseDelaySystem _useDelay;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<MedevacComponent, MapInitEvent>((EntityEventRefHandler<MedevacComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MedevacComponent, InteractHandEvent>((EntityEventRefHandler<MedevacComponent, InteractHandEvent>)OnInteract, (Type[])null, (Type[])null);
	}

	private void OnMapInit(Entity<MedevacComponent> ent, ref MapInitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		_useDelay.SetLength(Entity<UseDelayComponent>.op_Implicit(ent.Owner), ent.Comp.DelayLength, "medevac_system_delay");
	}

	private void OnInteract(Entity<MedevacComponent> ent, ref InteractHandEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		DropshipUtilityComponent utilComp = default(DropshipUtilityComponent);
		if (args.Target == ent.Owner || ent.Comp.IsActivated || !((EntitySystem)this).TryComp<DropshipUtilityComponent>(ent.Owner, ref utilComp) || !((EntitySystem)this).HasComp<DropshipUtilityPointComponent>(args.Target))
		{
			return;
		}
		EntityCoordinates targetCoord = ent.Owner.ToCoordinates();
		if (!utilComp.Target.HasValue)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-medevac-no-target"), targetCoord, args.User);
			return;
		}
		Entity<DropshipUtilityComponent> dropshipUtilEnt = default(Entity<DropshipUtilityComponent>);
		dropshipUtilEnt._002Ector(ent.Owner, utilComp);
		if (!_dropshipUtility.IsActivatable(dropshipUtilEnt, args.User, out string popup))
		{
			if (_net.IsServer)
			{
				_popup.PopupCoordinates(popup, targetCoord, args.User);
			}
			return;
		}
		PrepareMedevacEvent ev = new PrepareMedevacEvent(((EntitySystem)this).GetNetEntity(args.Target, (MetaDataComponent)null));
		((EntitySystem)this).RaiseLocalEvent<PrepareMedevacEvent>(utilComp.Target.Value, ev, false);
		if (ev.ReadyForMedevac)
		{
			_appearance.SetData(args.Target, (Enum)DropshipUtilityVisuals.State, (object)"medevac_system_active", (AppearanceComponent)null);
			ent.Comp.IsActivated = true;
			_useDelay.TryResetDelay(ent.Owner, checkDelayed: false, null, "medevac_system_delay");
		}
		else
		{
			_popup.PopupClient(base.Loc.GetString("rmc-medevac-stretcher-failure"), targetCoord, args.User);
		}
	}

	private void AfterMedevac(Entity<MedevacComponent> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		DropshipUtilityComponent utilComp = default(DropshipUtilityComponent);
		if (!((EntitySystem)this).TryComp<DropshipUtilityComponent>(ent.Owner, ref utilComp))
		{
			return;
		}
		EntityUid? target = utilComp.Target;
		MedevacStretcherComponent stretcherComp = default(MedevacStretcherComponent);
		if (target.HasValue && ((EntitySystem)this).TryComp<MedevacStretcherComponent>(target, ref stretcherComp))
		{
			EntityUid? utilId = utilComp.AttachmentPoint;
			(EntityUid, DropshipUtilityComponent) utilEnt = (ent.Owner, utilComp);
			(EntityUid, MedevacStretcherComponent) stretcherEnt = (target.Value, stretcherComp);
			ent.Comp.IsActivated = false;
			DropshipAttachedSpriteComponent sprite = default(DropshipAttachedSpriteComponent);
			if (((EntitySystem)this).TryComp<DropshipAttachedSpriteComponent>(Entity<MedevacComponent>.op_Implicit(ent), ref sprite) && sprite.Sprite != null && utilId.HasValue)
			{
				_appearance.SetData(utilId.Value, (Enum)DropshipUtilityVisuals.State, (object)sprite.Sprite.RsiState, (AppearanceComponent)null);
			}
			_stretcher.Medevac(Entity<MedevacStretcherComponent>.op_Implicit(stretcherEnt), ent.Owner);
			_dropshipUtility.ResetActivationCooldown(Entity<DropshipUtilityComponent>.op_Implicit(utilEnt));
		}
	}

	public override void Update(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		AllEntityQueryEnumerator<MedevacComponent> medevacQuery = ((EntitySystem)this).AllEntityQuery<MedevacComponent>();
		EntityUid ent = default(EntityUid);
		MedevacComponent medicomp = default(MedevacComponent);
		UseDelayComponent delayComp = default(UseDelayComponent);
		while (medevacQuery.MoveNext(ref ent, ref medicomp) && ((EntitySystem)this).TryComp<UseDelayComponent>(ent, ref delayComp))
		{
			if (medicomp.IsActivated && !_useDelay.IsDelayed(Entity<UseDelayComponent>.op_Implicit((ent, delayComp)), "medevac_system_delay"))
			{
				AfterMedevac(Entity<MedevacComponent>.op_Implicit((ent, medicomp)));
			}
		}
	}
}
