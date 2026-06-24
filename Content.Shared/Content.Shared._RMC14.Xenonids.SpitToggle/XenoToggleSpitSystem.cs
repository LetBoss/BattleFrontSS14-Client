using System;
using Content.Shared._RMC14.Xenonids.Projectile.Spit.Standard;
using Content.Shared.Actions;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Xenonids.SpitToggle;

public sealed class XenoToggleSpitSystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _actions;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoToggleSpitComponent, XenoSpitToggleActionEvent>((EntityEventRefHandler<XenoToggleSpitComponent, XenoSpitToggleActionEvent>)OnToggleSpit, (Type[])null, (Type[])null);
	}

	private void OnToggleSpit(Entity<XenoToggleSpitComponent> xeno, ref XenoSpitToggleActionEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		XenoSpitComponent spit = default(XenoSpitComponent);
		if (!((HandledEntityEventArgs)args).Handled && ((EntitySystem)this).TryComp<XenoSpitComponent>(Entity<XenoToggleSpitComponent>.op_Implicit(xeno), ref spit))
		{
			((HandledEntityEventArgs)args).Handled = true;
			xeno.Comp.UseAcid = !xeno.Comp.UseAcid;
			_actions.SetToggled(args.Action.AsNullable(), xeno.Comp.UseAcid);
			EntProtoId proto = (xeno.Comp.UseAcid ? xeno.Comp.AcidProto : xeno.Comp.NeuroProto);
			FixedPoint2 cost = (xeno.Comp.UseAcid ? xeno.Comp.AcidCost : xeno.Comp.NeuroCost);
			spit.PlasmaCost = cost;
			spit.ProjectileId = proto;
			((EntitySystem)this).Dirty<XenoToggleSpitComponent>(xeno, (MetaDataComponent)null);
		}
	}
}
