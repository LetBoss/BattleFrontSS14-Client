using System;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Medical.HUD.Components;
using Content.Shared._RMC14.Medical.HUD.Events;
using Content.Shared._RMC14.Medical.Scanner;
using Content.Shared._RMC14.Overwatch;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Verbs;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Medical.HUD.Systems;

public sealed class HolocardSystem : EntitySystem
{
	[Dependency]
	private SkillsSystem _skills;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedUserInterfaceSystem _ui;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	public const int MinimumRequiredSkill = 2;

	public static readonly EntProtoId<SkillDefinitionComponent> SkillType = EntProtoId<SkillDefinitionComponent>.op_Implicit("RMCSkillMedical");

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<HolocardStateComponent, HolocardChangeEvent>((EntityEventRefHandler<HolocardStateComponent, HolocardChangeEvent>)ChangeHolocard, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HolocardStateComponent, GetVerbsEvent<ExamineVerb>>((EntityEventRefHandler<HolocardStateComponent, GetVerbsEvent<ExamineVerb>>)OnHolocardExaminableVerb, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HealthScannerComponent, OpenChangeHolocardUIEvent>((ComponentEventRefHandler<HealthScannerComponent, OpenChangeHolocardUIEvent>)OpenChangeHolocardUI, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HealthScannerComponent, RefreshEquipmentHudEvent<HealthScannerComponent>>((EntityEventRefHandler<HealthScannerComponent, RefreshEquipmentHudEvent<HealthScannerComponent>>)OnRefreshEquipmentHud, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HolocardContainerComponent, HolocardContainerStatusUpdateEvent>((EntityEventRefHandler<HolocardContainerComponent, HolocardContainerStatusUpdateEvent>)OnHolocardContainerStatusUpdate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HolocardContainerComponent, EntInsertedIntoContainerMessage>((EntityEventRefHandler<HolocardContainerComponent, EntInsertedIntoContainerMessage>)OnHolocardContainerEntInserted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HolocardContainerComponent, EntRemovedFromContainerMessage>((EntityEventRefHandler<HolocardContainerComponent, EntRemovedFromContainerMessage>)OnHolocardContainerEntRemoved, (Type[])null, (Type[])null);
	}

	private void ChangeHolocard(Entity<HolocardStateComponent> ent, ref HolocardChangeEvent args)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		Enum uiKey = ((BaseBoundUserInterfaceEvent)args).UiKey;
		EntityUid? viewer = default(EntityUid?);
		if (uiKey is HolocardChangeUIKey && (HolocardChangeUIKey)(object)uiKey == HolocardChangeUIKey.Key && ((EntitySystem)this).TryGetEntity(args.Owner, ref viewer) && (_transform.InRange(Entity<TransformComponent>.op_Implicit(ent.Owner), Entity<TransformComponent>.op_Implicit(viewer.Value), 15f) || ((EntitySystem)this).HasComp<OverwatchWatchingComponent>(viewer.Value)) && _skills.HasSkill(Entity<SkillsComponent>.op_Implicit(viewer.Value), SkillType, 2))
		{
			ent.Comp.HolocardStatus = args.NewHolocardStatus;
			BaseContainer container = default(BaseContainer);
			if (_container.TryGetOuterContainer(Entity<HolocardStateComponent>.op_Implicit(ent), ((EntitySystem)this).Transform(Entity<HolocardStateComponent>.op_Implicit(ent)), ref container))
			{
				HolocardContainerStatusUpdateEvent ev = new HolocardContainerStatusUpdateEvent(args.NewHolocardStatus);
				((EntitySystem)this).RaiseLocalEvent<HolocardContainerStatusUpdateEvent>(container.Owner, ref ev, false);
			}
			((EntitySystem)this).Dirty<HolocardStateComponent>(ent, (MetaDataComponent)null);
		}
	}

	private void OnHolocardExaminableVerb(Entity<HolocardStateComponent> entity, ref GetVerbsEvent<ExamineVerb> args)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Expected O, but got Unknown
		if (!args.CanInteract || !_skills.HasSkill(Entity<SkillsComponent>.op_Implicit(args.User), SkillType, 2))
		{
			return;
		}
		HolocardScanEvent scanEvent = new HolocardScanEvent(CanScan: false, SlotFlags.HEAD | SlotFlags.EYES);
		((EntitySystem)this).RaiseLocalEvent<HolocardScanEvent>(args.User, ref scanEvent, false);
		if (scanEvent.CanScan)
		{
			EntityUid target = args.Target;
			EntityUid user = args.User;
			ExamineVerb verb = new ExamineVerb
			{
				Act = delegate
				{
					//IL_000c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0011: Unknown result type (might be due to invalid IL or missing references)
					//IL_001d: Unknown result type (might be due to invalid IL or missing references)
					_ui.OpenUi(Entity<UserInterfaceComponent>.op_Implicit(target), (Enum)HolocardChangeUIKey.Key, (EntityUid?)user, false);
				},
				Text = base.Loc.GetString("scannable-holocard-verb-text"),
				Message = base.Loc.GetString("scannable-holocard-verb-message"),
				Category = VerbCategory.Examine,
				Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/_RMC14/Interface/VerbIcons/ambulance.png"))
			};
			args.Verbs.Add(verb);
		}
	}

	private void OpenChangeHolocardUI(EntityUid entity, HealthScannerComponent comp, ref OpenChangeHolocardUIEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		EntityUid localOwner = ((EntitySystem)this).GetEntity(args.Owner);
		EntityUid localTarget = ((EntitySystem)this).GetEntity(args.Target);
		_ui.OpenUi(Entity<UserInterfaceComponent>.op_Implicit(localTarget), (Enum)HolocardChangeUIKey.Key, (EntityUid?)localOwner, false);
	}

	private void OnRefreshEquipmentHud(Entity<HealthScannerComponent> ent, ref RefreshEquipmentHudEvent<HealthScannerComponent> args)
	{
		args.Active = true;
	}

	private void OnHolocardContainerStatusUpdate(Entity<HolocardContainerComponent> container, ref HolocardContainerStatusUpdateEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_appearance.SetData(Entity<HolocardContainerComponent>.op_Implicit(container), (Enum)HolocardContainerVisuals.State, (object)args.NewStatus, (AppearanceComponent)null);
	}

	private void OnHolocardContainerEntInserted(Entity<HolocardContainerComponent> container, ref EntInsertedIntoContainerMessage args)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		HolocardStatus state = HolocardStatus.None;
		HolocardStateComponent holocard = default(HolocardStateComponent);
		if (((EntitySystem)this).TryComp<HolocardStateComponent>(((ContainerModifiedMessage)args).Entity, ref holocard))
		{
			state = holocard.HolocardStatus;
		}
		_appearance.SetData(Entity<HolocardContainerComponent>.op_Implicit(container), (Enum)HolocardContainerVisuals.State, (object)state, (AppearanceComponent)null);
	}

	private void OnHolocardContainerEntRemoved(Entity<HolocardContainerComponent> container, ref EntRemovedFromContainerMessage args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_appearance.SetData(Entity<HolocardContainerComponent>.op_Implicit(container), (Enum)HolocardContainerVisuals.State, (object)HolocardStatus.None, (AppearanceComponent)null);
	}
}
