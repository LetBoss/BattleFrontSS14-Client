using System;
using System.Linq;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.UniformAccessories;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Damage;
using Content.Shared.Examine;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Inventory;
using Content.Shared.Mobs.Components;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Medical.Scanner;

public sealed class RMCStethoscopeSystem : EntitySystem
{
	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private ExamineSystemShared _examine;

	[Dependency]
	private SkillsSystem _skills;

	[Dependency]
	private InventorySystem _inventorySystem;

	[Dependency]
	private SharedContainerSystem _containerSystem;

	private static readonly EntProtoId<SkillDefinitionComponent> MedicalSkill = EntProtoId<SkillDefinitionComponent>.op_Implicit("RMCSkillMedical");

	private static readonly string[] AccessorySlots = new string[2] { "jumpsuit", "outerClothing" };

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<GetVerbsEvent<ExamineVerb>>((EntityEventHandler<GetVerbsEvent<ExamineVerb>>)OnGlobalStethoscopeExamineVerb, (Type[])null, new Type[1] { typeof(SharedPopupSystem) });
		((EntitySystem)this).SubscribeLocalEvent<RMCStethoscopeComponent, AfterInteractEvent>((ComponentEventRefHandler<RMCStethoscopeComponent, AfterInteractEvent>)OnStethoAfterInteract, (Type[])null, (Type[])null);
	}

	private void OnStethoAfterInteract(EntityUid uid, RMCStethoscopeComponent comp, ref AfterInteractEvent args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && HasStethoscope(args.User, out var _) && args.Target.HasValue)
		{
			ShowStethoPopup(args.User, args.Target.Value);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void ShowStethoPopup(EntityUid user, EntityUid target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		string popupText = ((object)GetStethoscopeResults(target, user)).ToString();
		_popup.PopupClient(popupText, user, user);
	}

	private void OnGlobalStethoscopeExamineVerb(GetVerbsEvent<ExamineVerb> args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (args.CanInteract && args.CanAccess && !((EntitySystem)this).HasComp<XenoComponent>(args.Target) && HasStethoscope(args.User, out var stethoscope))
		{
			FormattedMessage examineMarkup = GetStethoscopeResults(args.Target, args.User);
			_examine.AddDetailedExamineVerb(args, (Component)(object)((EntitySystem)this).Comp<RMCStethoscopeComponent>(stethoscope), examineMarkup, base.Loc.GetString("rmc-stethoscope-verb-text"), "/Textures/_RMC14/Objects/Medical/stethoscope.rsi/icon.png", base.Loc.GetString("rmc-stethoscope-verb-message"));
		}
	}

	private bool HasStethoscope(EntityUid user, out EntityUid stethoscope)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		stethoscope = EntityUid.Invalid;
		if (_hands.TryGetActiveItem(Entity<HandsComponent>.op_Implicit(user), out var held) && ((EntitySystem)this).HasComp<RMCStethoscopeComponent>(held.Value))
		{
			stethoscope = held.Value;
			return true;
		}
		string[] accessorySlots = AccessorySlots;
		UniformAccessoryHolderComponent holder = default(UniformAccessoryHolderComponent);
		BaseContainer container = default(BaseContainer);
		foreach (string slot in accessorySlots)
		{
			if (!_inventorySystem.TryGetSlotEntity(user, slot, out var slotEntity) || !((EntitySystem)this).TryComp<UniformAccessoryHolderComponent>(slotEntity.Value, ref holder))
			{
				continue;
			}
			string containerId = holder.ContainerId;
			if (!_containerSystem.TryGetContainer(slotEntity.Value, containerId, ref container, (ContainerManagerComponent)null))
			{
				continue;
			}
			foreach (EntityUid accessory in container.ContainedEntities)
			{
				if (((EntitySystem)this).HasComp<RMCStethoscopeComponent>(accessory))
				{
					stethoscope = accessory;
					return true;
				}
			}
		}
		return false;
	}

	private FormattedMessage GetStethoscopeResults(EntityUid target, EntityUid? user = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Expected O, but got Unknown
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		float? totalHealth = GetPercentHealth(target);
		FormattedMessage msg = new FormattedMessage();
		if (user.HasValue && !_skills.HasSkill(Entity<SkillsComponent>.op_Implicit(user.Value), MedicalSkill, 2))
		{
			msg.AddMarkupOrThrow(base.Loc.GetString("rmc-stethoscope-unskilled"));
			return msg;
		}
		if (!totalHealth.HasValue)
		{
			msg.AddMarkupOrThrow(base.Loc.GetString("rmc-stethoscope-nothing"));
		}
		else if (totalHealth >= 87.5f)
		{
			msg.AddMarkupOrThrow(base.Loc.GetString("rmc-stethoscope-normal", (ValueTuple<string, object>)("target", target)));
		}
		else if (totalHealth >= 62.5f)
		{
			msg.AddMarkupOrThrow(base.Loc.GetString("rmc-stethoscope-raggedy", (ValueTuple<string, object>)("target", target)));
		}
		else if (totalHealth >= 37.5f)
		{
			msg.AddMarkupOrThrow(base.Loc.GetString("rmc-stethoscope-hyper"));
		}
		else if (totalHealth >= 1f)
		{
			msg.AddMarkupOrThrow(base.Loc.GetString("rmc-stethoscope-irregular", (ValueTuple<string, object>)("target", target)));
		}
		else if (totalHealth >= 0f)
		{
			msg.AddMarkupOrThrow(base.Loc.GetString("rmc-stethoscope-dead"));
		}
		return msg;
	}

	private float? GetPercentHealth(EntityUid target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		DamageableComponent damage = default(DamageableComponent);
		MobThresholdsComponent thresholds = default(MobThresholdsComponent);
		if (!((EntitySystem)this).TryComp<DamageableComponent>(target, ref damage) || !((EntitySystem)this).TryComp<MobThresholdsComponent>(target, ref thresholds))
		{
			return null;
		}
		float totalDamage = damage.Damage.GetTotal().Float();
		float maxThreshold = ((thresholds.Thresholds.Count > 0) ? ((float)thresholds.Thresholds.Keys.Max()) : 100f);
		float healthPercent = 100f - MathF.Min(totalDamage / maxThreshold * 100f, 100f);
		if (healthPercent > 100f)
		{
			healthPercent = 100f;
		}
		return healthPercent;
	}
}
