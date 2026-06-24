using System;
using System.Collections.Generic;
using Content.Shared.Administration.Logs;
using Content.Shared.Body.Components;
using Content.Shared.Body.Systems;
using Content.Shared.Database;
using Content.Shared.Destructible;
using Content.Shared.DoAfter;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Kitchen.Components;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Nutrition.Components;
using Content.Shared.Nutrition.EntitySystems;
using Content.Shared.Popups;
using Content.Shared.Storage;
using Content.Shared.Verbs;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Utility;

namespace Content.Shared.Kitchen;

public sealed class SharpSystem : EntitySystem
{
	[Dependency]
	private SharedBodySystem _bodySystem;

	[Dependency]
	private SharedDestructibleSystem _destructibleSystem;

	[Dependency]
	private SharedDoAfterSystem _doAfterSystem;

	[Dependency]
	private SharedPopupSystem _popupSystem;

	[Dependency]
	private SharedContainerSystem _containerSystem;

	[Dependency]
	private MobStateSystem _mobStateSystem;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private IRobustRandom _robustRandom;

	[Dependency]
	private ISharedAdminLogManager _adminLogger;

	[Dependency]
	private INetManager _net;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<SharpComponent, AfterInteractEvent>((ComponentEventHandler<SharpComponent, AfterInteractEvent>)OnAfterInteract, new Type[1] { typeof(UtensilSystem) }, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SharpComponent, SharpDoAfterEvent>((ComponentEventHandler<SharpComponent, SharpDoAfterEvent>)OnDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ButcherableComponent, GetVerbsEvent<InteractionVerb>>((ComponentEventHandler<ButcherableComponent, GetVerbsEvent<InteractionVerb>>)OnGetInteractionVerbs, (Type[])null, (Type[])null);
	}

	private void OnAfterInteract(EntityUid uid, SharpComponent component, AfterInteractEvent args)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && args.Target.HasValue && args.CanReach && TryStartButcherDoafter(uid, args.Target.Value, args.User))
		{
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private bool TryStartButcherDoafter(EntityUid knife, EntityUid target, EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		ButcherableComponent butcher = default(ButcherableComponent);
		if (!((EntitySystem)this).TryComp<ButcherableComponent>(target, ref butcher))
		{
			return false;
		}
		SharpComponent sharp = default(SharpComponent);
		if (!((EntitySystem)this).TryComp<SharpComponent>(knife, ref sharp))
		{
			return false;
		}
		MobStateComponent mobState = default(MobStateComponent);
		if (((EntitySystem)this).TryComp<MobStateComponent>(target, ref mobState) && !_mobStateSystem.IsDead(target, mobState))
		{
			return false;
		}
		if (butcher.Type != ButcheringType.Knife && target != user)
		{
			_popupSystem.PopupEntity(base.Loc.GetString("butcherable-different-tool", (ValueTuple<string, object>)("target", target)), knife, user);
			return false;
		}
		if (!sharp.Butchering.Add(target))
		{
			return false;
		}
		bool needHand = user != knife;
		DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, sharp.ButcherDelayModifier * butcher.ButcherDelay, new SharpDoAfterEvent(), knife, target, knife)
		{
			BreakOnDamage = true,
			BreakOnMove = true,
			NeedHand = needHand
		};
		_doAfterSystem.TryStartDoAfter(doAfter);
		return true;
	}

	private void OnDoAfter(EntityUid uid, SharpComponent component, DoAfterEvent args)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		ButcherableComponent butcher = default(ButcherableComponent);
		if (((HandledEntityEventArgs)args).Handled || !((EntitySystem)this).TryComp<ButcherableComponent>(args.Args.Target, ref butcher))
		{
			return;
		}
		if (args.Cancelled)
		{
			component.Butchering.Remove(args.Args.Target.Value);
			return;
		}
		component.Butchering.Remove(args.Args.Target.Value);
		if (_containerSystem.IsEntityInContainer(args.Args.Target.Value, (MetaDataComponent)null))
		{
			((HandledEntityEventArgs)args).Handled = true;
		}
		else
		{
			if (_net.IsClient)
			{
				return;
			}
			List<string> spawns = EntitySpawnCollection.GetSpawns(butcher.SpawnedEntities, _robustRandom);
			MapCoordinates coords = _transform.GetMapCoordinates(args.Args.Target.Value, (TransformComponent)null);
			EntityUid popupEnt = default(EntityUid);
			foreach (string proto in spawns)
			{
				popupEnt = ((EntitySystem)this).Spawn(proto, ((MapCoordinates)(ref coords)).Offset(_robustRandom.NextVector2(0.25f)), (ComponentRegistry)null, default(Angle));
			}
			BodyComponent body = default(BodyComponent);
			bool num = ((EntitySystem)this).TryComp<BodyComponent>(args.Args.Target.Value, ref body);
			PopupType popupType = PopupType.Small;
			if (num)
			{
				popupType = PopupType.LargeCaution;
			}
			_popupSystem.PopupEntity(base.Loc.GetString("butcherable-knife-butchered-success", (ValueTuple<string, object>)("target", args.Args.Target.Value), (ValueTuple<string, object>)("knife", Identity.Entity(uid, (IEntityManager)(object)base.EntityManager))), popupEnt, args.Args.User, popupType);
			if (num)
			{
				_bodySystem.GibBody(args.Args.Target.Value, gibOrgans: false, body);
			}
			_destructibleSystem.DestroyEntity(args.Args.Target.Value);
			((HandledEntityEventArgs)args).Handled = true;
			ISharedAdminLogManager adminLogger = _adminLogger;
			LogStringHandler handler = new LogStringHandler(21, 3);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(args.User)), "user", "ToPrettyString(args.User)");
			handler.AppendLiteral(" ");
			handler.AppendLiteral("has butchered ");
			handler.AppendFormatted(((EntitySystem)this).ToPrettyString(args.Target, (MetaDataComponent)null), "target", "ToPrettyString(args.Target)");
			handler.AppendLiteral(" ");
			handler.AppendLiteral("with ");
			handler.AppendFormatted(((EntitySystem)this).ToPrettyString(args.Used, (MetaDataComponent)null), "knife", "ToPrettyString(args.Used)");
			adminLogger.Add(LogType.Gib, ref handler);
		}
	}

	private void OnGetInteractionVerbs(EntityUid uid, ButcherableComponent component, GetVerbsEvent<InteractionVerb> args)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Expected O, but got Unknown
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		SharpComponent userSharpComp = default(SharpComponent);
		if (component.Type != ButcheringType.Knife || !args.CanAccess || !args.CanInteract || (!((EntitySystem)this).TryComp<SharpComponent>(args.User, ref userSharpComp) && args.Hands == null))
		{
			return;
		}
		bool disabled = false;
		string message = null;
		SharpComponent usingSharpComp = default(SharpComponent);
		MobStateComponent state = default(MobStateComponent);
		if (!((EntitySystem)this).TryComp<SharpComponent>(args.Using, ref usingSharpComp) && userSharpComp == null)
		{
			disabled = true;
			message = base.Loc.GetString("butcherable-need-knife", (ValueTuple<string, object>)("target", uid));
		}
		else if (_containerSystem.IsEntityInContainer(uid, (MetaDataComponent)null))
		{
			disabled = true;
			message = base.Loc.GetString("butcherable-not-in-container", (ValueTuple<string, object>)("target", uid));
		}
		else if (((EntitySystem)this).TryComp<MobStateComponent>(uid, ref state) && !_mobStateSystem.IsDead(uid, state))
		{
			disabled = true;
			message = base.Loc.GetString("butcherable-mob-isnt-dead");
		}
		EntityUid sharpObject = default(EntityUid);
		if (usingSharpComp != null)
		{
			sharpObject = args.Using.Value;
		}
		else if (userSharpComp != null)
		{
			sharpObject = args.User;
		}
		InteractionVerb verb = new InteractionVerb
		{
			Act = delegate
			{
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				if (!disabled)
				{
					TryStartButcherDoafter(sharpObject, args.Target, args.User);
				}
			},
			Message = message,
			Disabled = disabled,
			Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/VerbIcons/cutlery.svg.192dpi.png")),
			Text = base.Loc.GetString("butcherable-verb-name")
		};
		args.Verbs.Add(verb);
	}
}
