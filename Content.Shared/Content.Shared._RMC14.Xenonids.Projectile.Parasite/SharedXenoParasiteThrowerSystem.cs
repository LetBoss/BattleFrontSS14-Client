using System;
using Content.Shared._RMC14.Xenonids.Egg;
using Content.Shared.Database;
using Content.Shared.Examine;
using Content.Shared.Ghost;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;

namespace Content.Shared._RMC14.Xenonids.Projectile.Parasite;

public abstract class SharedXenoParasiteThrowerSystem : EntitySystem
{
	[Dependency]
	protected SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedUserInterfaceSystem _ui;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<XenoParasiteThrowerComponent, ExaminedEvent>((EntityEventRefHandler<XenoParasiteThrowerComponent, ExaminedEvent>)OnParasiteThrowerExamine, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoParasiteThrowerComponent, XenoChangeParasiteReserveMessage>((EntityEventRefHandler<XenoParasiteThrowerComponent, XenoChangeParasiteReserveMessage>)OnParasiteReserveChange, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoParasiteThrowerComponent, XenoReserveParasiteActionEvent>((EntityEventRefHandler<XenoParasiteThrowerComponent, XenoReserveParasiteActionEvent>)OnSetReserve, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoParasiteThrowerComponent, GetVerbsEvent<ActivationVerb>>((EntityEventRefHandler<XenoParasiteThrowerComponent, GetVerbsEvent<ActivationVerb>>)OnGetVerbs, (Type[])null, (Type[])null);
	}

	private void OnParasiteThrowerExamine(Entity<XenoParasiteThrowerComponent> thrower, ref ExaminedEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<XenoComponent>(args.Examiner) && !((EntitySystem)this).HasComp<GhostComponent>(args.Examiner))
		{
			return;
		}
		if (((EntitySystem)this).HasComp<GhostComponent>(args.Examiner))
		{
			int paras = Math.Max(thrower.Comp.CurParasites - thrower.Comp.ReservedParasites, 0);
			using (args.PushGroup("XenoParasiteThrowerComponent"))
			{
				args.PushMarkup(base.Loc.GetString("rmc-xeno-throw-parasite-reserves", (ValueTuple<string, object>)("xeno", thrower), (ValueTuple<string, object>)("rev_paras", paras)));
				return;
			}
		}
		using (args.PushGroup("XenoParasiteThrowerComponent"))
		{
			args.PushMarkup(base.Loc.GetString("rmc-xeno-throw-parasite-current", new(string, object)[3]
			{
				("xeno", thrower),
				("cur_paras", thrower.Comp.CurParasites),
				("max_paras", thrower.Comp.MaxParasites)
			}));
		}
	}

	private void OnParasiteReserveChange(Entity<XenoParasiteThrowerComponent> thrower, ref XenoChangeParasiteReserveMessage args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		int newVal = Math.Clamp(args.NewReserve, 0, thrower.Comp.MaxParasites);
		thrower.Comp.ReservedParasites = newVal;
		((EntitySystem)this).Dirty<XenoParasiteThrowerComponent>(thrower, (MetaDataComponent)null);
	}

	private void OnSetReserve(Entity<XenoParasiteThrowerComponent> xeno, ref XenoReserveParasiteActionEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			_ui.OpenUi(Entity<UserInterfaceComponent>.op_Implicit(xeno.Owner), (Enum)XenoReserveParasiteChangeUI.Key, (EntityUid?)Entity<XenoParasiteThrowerComponent>.op_Implicit(xeno), false);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnGetVerbs(Entity<XenoParasiteThrowerComponent> xeno, ref GetVerbsEvent<ActivationVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		EntityUid uid = args.User;
		if (((EntitySystem)this).HasComp<ActorComponent>(uid) && ((EntitySystem)this).HasComp<GhostComponent>(uid) && xeno.Comp.CurParasites != 0 && xeno.Comp.ReservedParasites < xeno.Comp.CurParasites)
		{
			ActivationVerb parasiteVerb = new ActivationVerb
			{
				Text = base.Loc.GetString("rmc-xeno-egg-ghost-verb"),
				Act = delegate
				{
					//IL_0011: Unknown result type (might be due to invalid IL or missing references)
					//IL_0016: Unknown result type (might be due to invalid IL or missing references)
					//IL_0022: Unknown result type (might be due to invalid IL or missing references)
					_ui.TryOpenUi(Entity<UserInterfaceComponent>.op_Implicit(xeno.Owner), (Enum)XenoParasiteGhostUI.Key, uid, false);
				},
				Impact = LogImpact.High
			};
			args.Verbs.Add(parasiteVerb);
		}
	}
}
