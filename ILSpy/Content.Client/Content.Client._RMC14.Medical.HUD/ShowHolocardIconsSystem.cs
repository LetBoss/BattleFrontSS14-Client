using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Content.Client.Overlays;
using Content.Shared._RMC14.Medical.HUD;
using Content.Shared._RMC14.Medical.HUD.Components;
using Content.Shared.StatusIcon;
using Content.Shared.StatusIcon.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Client._RMC14.Medical.HUD;

public sealed class ShowHolocardIconsSystem : EquipmentHudSystem<HolocardScannerComponent>
{
	[Dependency]
	private IPrototypeManager _prototypes;

	private static readonly ProtoId<HealthIconPrototype> Urgent = ProtoId<HealthIconPrototype>.op_Implicit("UrgentHolocardIcon");

	private static readonly ProtoId<HealthIconPrototype> Emergency = ProtoId<HealthIconPrototype>.op_Implicit("EmergencyHolocardIcon");

	private static readonly ProtoId<HealthIconPrototype> Xeno = ProtoId<HealthIconPrototype>.op_Implicit("XenoHolocardIcon");

	private static readonly ProtoId<HealthIconPrototype> Permadead = ProtoId<HealthIconPrototype>.op_Implicit("PermaHolocardIcon");

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<HolocardStateComponent, GetStatusIconsEvent>((EntityEventRefHandler<HolocardStateComponent, GetStatusIconsEvent>)OnGetStatusIconsEvent, (Type[])null, (Type[])null);
	}

	private void OnGetStatusIconsEvent(Entity<HolocardStateComponent> entity, ref GetStatusIconsEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (IsActive)
		{
			IReadOnlyList<StatusIconData> icons = GetIcons(entity);
			args.StatusIcons.AddRange(icons);
		}
	}

	public IReadOnlyList<StatusIconData> GetIcons(Entity<HolocardStateComponent> entity)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		List<StatusIconData> list = new List<StatusIconData>();
		if (TryGetHolocardData(entity.Comp.HolocardStatus, out var data) && data.HolocardIcon.HasValue)
		{
			HealthIconPrototype item = _prototypes.Index<HealthIconPrototype>(data.HolocardIcon.Value);
			list.Add(item);
		}
		return list;
	}

	public bool TryGetHolocardData(HolocardStatus holocardStatus, out HolocardData data)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		data = default(HolocardData);
		switch (holocardStatus)
		{
		case HolocardStatus.None:
			data.HolocardIcon = null;
			data.Description = LocId.op_Implicit(((EntitySystem)this).Loc.GetString("hc-none-description"));
			break;
		case HolocardStatus.Urgent:
			data.HolocardIcon = Urgent;
			data.Description = LocId.op_Implicit(((EntitySystem)this).Loc.GetString("hc-urgent-description"));
			break;
		case HolocardStatus.Emergency:
			data.HolocardIcon = Emergency;
			data.Description = LocId.op_Implicit(((EntitySystem)this).Loc.GetString("hc-emergency-description"));
			break;
		case HolocardStatus.Xeno:
			data.HolocardIcon = Xeno;
			data.Description = LocId.op_Implicit(((EntitySystem)this).Loc.GetString("hc-xeno-description"));
			break;
		case HolocardStatus.Permadead:
			data.HolocardIcon = Permadead;
			data.Description = LocId.op_Implicit(((EntitySystem)this).Loc.GetString("hc-permadead-description"));
			break;
		default:
			data = default(HolocardData);
			return false;
		}
		return true;
	}

	public bool TryGetHolocardName(HolocardStatus holocardStatus, [NotNullWhen(true)] out string? holocardName)
	{
		holocardName = null;
		switch (holocardStatus)
		{
		case HolocardStatus.None:
			holocardName = ((EntitySystem)this).Loc.GetString("hc-none-name");
			break;
		case HolocardStatus.Urgent:
			holocardName = ((EntitySystem)this).Loc.GetString("hc-urgent-name");
			break;
		case HolocardStatus.Emergency:
			holocardName = ((EntitySystem)this).Loc.GetString("hc-emergency-name");
			break;
		case HolocardStatus.Xeno:
			holocardName = ((EntitySystem)this).Loc.GetString("hc-xeno-name");
			break;
		case HolocardStatus.Permadead:
			holocardName = ((EntitySystem)this).Loc.GetString("hc-permadead-name");
			break;
		default:
			return false;
		}
		return true;
	}

	public bool TryGetHolocardColor(HolocardStatus holocardStatus, [NotNullWhen(true)] out Color? holocardColor)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		holocardColor = null;
		switch (holocardStatus)
		{
		case HolocardStatus.Urgent:
			holocardColor = Color.Chocolate;
			break;
		case HolocardStatus.Emergency:
			holocardColor = Color.DarkRed;
			break;
		case HolocardStatus.Xeno:
			holocardColor = Color.Purple;
			break;
		case HolocardStatus.Permadead:
			holocardColor = Color.Black;
			break;
		default:
			return false;
		}
		return true;
	}

	public bool TryGetHolocardColor(Entity<HolocardStateComponent> entity, [NotNullWhen(true)] out Color? holocardColor)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		holocardColor = null;
		if (TryGetHolocardColor(entity.Comp.HolocardStatus, out var holocardColor2))
		{
			holocardColor = holocardColor2;
			return true;
		}
		return false;
	}

	public bool TryGetDescription(Entity<HolocardStateComponent> entity, [NotNullWhen(true)] out string? description)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		description = null;
		if (TryGetHolocardData(entity.Comp.HolocardStatus, out var data))
		{
			description = LocId.op_Implicit(data.Description);
			return true;
		}
		return false;
	}
}
