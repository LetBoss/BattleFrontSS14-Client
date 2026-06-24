using System;
using System.Diagnostics.CodeAnalysis;
using Content.Shared._RMC14.Dropship.AttachmentPoint;
using Content.Shared._RMC14.Dropship.Utility.Components;
using Content.Shared._RMC14.Dropship.Weapon;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared.Interaction;
using Content.Shared.Shuttles.Components;
using Content.Shared.Shuttles.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Dropship.Utility.Systems;

public sealed class DropshipUtilitySystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedDropshipSystem _dropship;

	[Dependency]
	private SkillsSystem _skills;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedDropshipWeaponSystem _dropshipWeapon;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<DropshipUtilityPointComponent, DropshipTargetChangedEvent>((EntityEventRefHandler<DropshipUtilityPointComponent, DropshipTargetChangedEvent>)OnTargetChange, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipUtilityPointComponent, InteractHandEvent>((EntityEventRefHandler<DropshipUtilityPointComponent, InteractHandEvent>)OnInteract, (Type[])null, (Type[])null);
	}

	private void OnTargetChange(Entity<DropshipUtilityPointComponent> ent, ref DropshipTargetChangedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		ContainerSlot slot = _container.EnsureContainer<ContainerSlot>(Entity<DropshipUtilityPointComponent>.op_Implicit(ent), ent.Comp.UtilitySlotId, (ContainerManagerComponent)null);
		DropshipUtilityComponent utilityComp = default(DropshipUtilityComponent);
		if (((EntitySystem)this).TryComp<DropshipUtilityComponent>(slot.ContainedEntity, ref utilityComp))
		{
			utilityComp.Target = ((EntitySystem)this).GetEntity(args.DropshipTarget);
		}
	}

	private void OnInteract(Entity<DropshipUtilityPointComponent> ent, ref InteractHandEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? utilityEntity = _container.EnsureContainer<ContainerSlot>(Entity<DropshipUtilityPointComponent>.op_Implicit(ent), ent.Comp.UtilitySlotId, (ContainerManagerComponent)null).ContainedEntity;
		if (((EntitySystem)this).HasComp<DropshipUtilityComponent>(utilityEntity))
		{
			InteractHandEvent ev = new InteractHandEvent(args.User, args.Target);
			((EntitySystem)this).RaiseLocalEvent<InteractHandEvent>(utilityEntity.Value, ev, false);
			((HandledEntityEventArgs)args).Handled = ((HandledEntityEventArgs)ev).Handled;
		}
	}

	public bool IsActivatable(Entity<DropshipUtilityComponent> ent, EntityUid user, [NotNullWhen(false)] out string? popup)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Skills != null && !_skills.HasSkills(Entity<SkillsComponent>.op_Implicit(user), ent.Comp.Skills))
		{
			popup = base.Loc.GetString("rmc-dropship-utility-not-skilled");
			return false;
		}
		if (!_dropship.TryGetGridDropship(Entity<DropshipUtilityComponent>.op_Implicit(ent), out Entity<DropshipComponent> dropship))
		{
			popup = "";
			return false;
		}
		TimeSpan curTime = _timing.CurTime;
		TimeSpan? nextActivateAt = ent.Comp.NextActivateAt;
		if (curTime < nextActivateAt)
		{
			popup = base.Loc.GetString("rmc-dropship-utility-cooldown", (ValueTuple<string, object>)("utility", ent.Owner));
			return false;
		}
		if (_dropshipWeapon.CasDebug)
		{
			popup = null;
			return true;
		}
		FTLComponent ftl = default(FTLComponent);
		if (!((EntitySystem)this).TryComp<FTLComponent>(Entity<DropshipComponent>.op_Implicit(dropship), ref ftl) || (ftl.State != FTLState.Travelling && ftl.State != FTLState.Arriving))
		{
			popup = base.Loc.GetString("rmc-dropship-utility-activate-not-flying");
			return false;
		}
		if (!ent.Comp.ActivateInTransport && !((EntitySystem)this).HasComp<DropshipInFlyByComponent>(Entity<DropshipComponent>.op_Implicit(dropship)))
		{
			popup = base.Loc.GetString("rmc-dropship-utility-not-flyby", (ValueTuple<string, object>)("utility", ent.Owner));
			return false;
		}
		popup = null;
		return true;
	}

	public void ResetActivationCooldown(Entity<DropshipUtilityComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.NextActivateAt = _timing.CurTime + ent.Comp.ActivateDelay;
	}
}
