using System;
using Content.Shared._RMC14.Xenonids.Evolution;
using Content.Shared.Examine;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Network;

namespace Content.Shared._RMC14.Xenonids.Strain;

public sealed class XenoStrainSystem : EntitySystem
{
	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPopupSystem _popup;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoStrainComponent, ExaminedEvent>((EntityEventRefHandler<XenoStrainComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoStrainComponent, NewXenoEvolvedEvent>((EntityEventRefHandler<XenoStrainComponent, NewXenoEvolvedEvent>)OnNewXenoEvolved, (Type[])null, (Type[])null);
	}

	private void OnExamined(Entity<XenoStrainComponent> ent, ref ExaminedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		if (string.IsNullOrWhiteSpace(LocId.op_Implicit(ent.Comp.Name)))
		{
			return;
		}
		using (args.PushGroup("XenoStrainComponent"))
		{
			args.PushText(base.Loc.GetString("rmc-xeno-strain-specialized-into", (ValueTuple<string, object>)("strain", base.Loc.GetString(LocId.op_Implicit(ent.Comp.Name)))));
		}
	}

	private void OnNewXenoEvolved(Entity<XenoStrainComponent> ent, ref NewXenoEvolvedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		LocId? popup = ent.Comp.Popup;
		if (popup.HasValue)
		{
			LocId popup2 = popup.GetValueOrDefault();
			if (!_net.IsClient)
			{
				_popup.PopupEntity(base.Loc.GetString(LocId.op_Implicit(popup2)), Entity<XenoStrainComponent>.op_Implicit(ent), Entity<XenoStrainComponent>.op_Implicit(ent), PopupType.MediumXeno);
			}
		}
	}

	public bool AreSameStrain(Entity<XenoStrainComponent?> one, Entity<XenoStrainComponent?> two)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<XenoStrainComponent>(Entity<XenoStrainComponent>.op_Implicit(one), ref one.Comp, false) || !((EntitySystem)this).Resolve<XenoStrainComponent>(Entity<XenoStrainComponent>.op_Implicit(two), ref two.Comp, false))
		{
			return false;
		}
		return one.Comp.Name == two.Comp.Name;
	}
}
