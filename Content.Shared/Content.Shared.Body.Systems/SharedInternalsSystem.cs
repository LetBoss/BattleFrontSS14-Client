using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Alert;
using Content.Shared.Atmos.Components;
using Content.Shared.Atmos.EntitySystems;
using Content.Shared.Body.Components;
using Content.Shared.DoAfter;
using Content.Shared.Hands.Components;
using Content.Shared.IdentityManagement;
using Content.Shared.Internals;
using Content.Shared.Inventory;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Utility;

namespace Content.Shared.Body.Systems;

public abstract class SharedInternalsSystem : EntitySystem
{
	[Dependency]
	private AlertsSystem _alerts;

	[Dependency]
	private InventorySystem _inventory;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private SharedGasTankSystem _gasTank;

	[Dependency]
	private SharedPopupSystem _popupSystem;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<InternalsComponent, GetVerbsEvent<InteractionVerb>>((EntityEventRefHandler<InternalsComponent, GetVerbsEvent<InteractionVerb>>)OnGetInteractionVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InternalsComponent, ComponentStartup>((EntityEventRefHandler<InternalsComponent, ComponentStartup>)OnInternalsStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InternalsComponent, ComponentShutdown>((EntityEventRefHandler<InternalsComponent, ComponentShutdown>)OnInternalsShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InternalsComponent, InternalsDoAfterEvent>((EntityEventRefHandler<InternalsComponent, InternalsDoAfterEvent>)OnDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InternalsComponent, ToggleInternalsAlertEvent>((EntityEventRefHandler<InternalsComponent, ToggleInternalsAlertEvent>)OnToggleInternalsAlert, (Type[])null, (Type[])null);
	}

	private void OnGetInteractionVerbs(Entity<InternalsComponent> ent, ref GetVerbsEvent<InteractionVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Expected O, but got Unknown
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<XenoComponent>(args.User) || !args.CanAccess || !args.CanInteract || args.Hands == null || (!AreInternalsWorking(Entity<InternalsComponent>.op_Implicit(ent)) && ent.Comp.BreathTools.Count == 0))
		{
			return;
		}
		EntityUid user = args.User;
		InteractionVerb verb = new InteractionVerb
		{
			Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/VerbIcons/dot.svg.192dpi.png"))
		};
		if (AreInternalsWorking(Entity<InternalsComponent>.op_Implicit(ent)))
		{
			verb.Act = delegate
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				ToggleInternals(Entity<InternalsComponent>.op_Implicit(ent), user, force: false, Entity<InternalsComponent>.op_Implicit(ent), ToggleMode.Off);
			};
			verb.Message = base.Loc.GetString("action-description-internals-toggle-off");
			verb.Text = base.Loc.GetString("action-name-internals-toggle-off");
		}
		else
		{
			verb.Act = delegate
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				ToggleInternals(Entity<InternalsComponent>.op_Implicit(ent), user, force: false, Entity<InternalsComponent>.op_Implicit(ent), ToggleMode.On);
			};
			verb.Message = base.Loc.GetString("action-description-internals-toggle-on");
			verb.Text = base.Loc.GetString("action-name-internals-toggle-on");
		}
		args.Verbs.Add(verb);
	}

	protected bool ToggleInternals(EntityUid target, EntityUid user, bool force, InternalsComponent? internals = null, ToggleMode mode = ToggleMode.Toggle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<InternalsComponent>(target, ref internals, false))
		{
			return false;
		}
		if (internals.BreathTools.Count == 0)
		{
			string message = ((user == target) ? base.Loc.GetString("internals-self-no-breath-tool") : base.Loc.GetString("internals-other-no-breath-tool", (ValueTuple<string, object>)("ent", Identity.Name(target, (IEntityManager)(object)base.EntityManager, user))));
			_popupSystem.PopupClient(message, target, user);
			return false;
		}
		Entity<GasTankComponent>? tank = FindBestGasTank(Entity<HandsComponent, InventoryComponent, ContainerManagerComponent>.op_Implicit(target));
		if (!tank.HasValue)
		{
			string message2 = ((user == target) ? base.Loc.GetString("internals-self-no-tank") : base.Loc.GetString("internals-other-no-tank", (ValueTuple<string, object>)("ent", Identity.Name(target, (IEntityManager)(object)base.EntityManager, user))));
			_popupSystem.PopupClient(message2, target, user);
			return false;
		}
		if (!force && user != target)
		{
			return StartToggleInternalsDoAfter(user, Entity<InternalsComponent>.op_Implicit((target, internals)), mode);
		}
		GasTankComponent gas = default(GasTankComponent);
		if (((EntitySystem)this).TryComp<GasTankComponent>(internals.GasTankEntity, ref gas))
		{
			if (mode == ToggleMode.On)
			{
				return false;
			}
			return _gasTank.DisconnectFromInternals(Entity<GasTankComponent>.op_Implicit((internals.GasTankEntity.Value, gas)), user);
		}
		if (mode == ToggleMode.Off)
		{
			return false;
		}
		return _gasTank.ConnectToInternals(tank.Value, user);
	}

	private bool StartToggleInternalsDoAfter(EntityUid user, Entity<InternalsComponent> targetEnt, ToggleMode mode)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan delay = ((!(user == targetEnt.Owner)) ? targetEnt.Comp.Delay : TimeSpan.Zero);
		return _doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, delay, new InternalsDoAfterEvent(mode), Entity<InternalsComponent>.op_Implicit(targetEnt), Entity<InternalsComponent>.op_Implicit(targetEnt))
		{
			BreakOnDamage = true,
			BreakOnMove = true,
			MovementThreshold = 0.1f
		});
	}

	private void OnDoAfter(Entity<InternalsComponent> ent, ref InternalsDoAfterEvent args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && !((HandledEntityEventArgs)args).Handled)
		{
			ToggleInternals(Entity<InternalsComponent>.op_Implicit(ent), args.User, force: true, Entity<InternalsComponent>.op_Implicit(ent), args.ToggleMode);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnToggleInternalsAlert(Entity<InternalsComponent> ent, ref ToggleInternalsAlertEvent args)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			ToggleInternalsAlertEvent obj = args;
			((HandledEntityEventArgs)obj).Handled = ((HandledEntityEventArgs)obj).Handled | ToggleInternals(Entity<InternalsComponent>.op_Implicit(ent), Entity<InternalsComponent>.op_Implicit(ent), force: false, ent.Comp);
		}
	}

	private void OnInternalsStartup(Entity<InternalsComponent> ent, ref ComponentStartup args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		_alerts.ShowAlert(Entity<InternalsComponent>.op_Implicit(ent), ent.Comp.InternalsAlert, GetSeverity(Entity<InternalsComponent>.op_Implicit(ent)));
	}

	private void OnInternalsShutdown(Entity<InternalsComponent> ent, ref ComponentShutdown args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		_alerts.ClearAlert(Entity<InternalsComponent>.op_Implicit(ent), ent.Comp.InternalsAlert);
	}

	public void ConnectBreathTool(Entity<InternalsComponent> ent, EntityUid toolEntity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.BreathTools.Add(toolEntity))
		{
			BreathToolComponent breathTool = default(BreathToolComponent);
			if (((EntitySystem)this).TryComp<BreathToolComponent>(toolEntity, ref breathTool))
			{
				breathTool.ConnectedInternalsEntity = ent.Owner;
				((EntitySystem)this).Dirty(toolEntity, (IComponent)(object)breathTool, (MetaDataComponent)null);
			}
			((EntitySystem)this).Dirty<InternalsComponent>(ent, (MetaDataComponent)null);
			_alerts.ShowAlert(Entity<InternalsComponent>.op_Implicit(ent), ent.Comp.InternalsAlert, GetSeverity(Entity<InternalsComponent>.op_Implicit(ent)));
		}
	}

	public void DisconnectBreathTool(Entity<InternalsComponent> ent, EntityUid toolEntity, bool forced = false)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.BreathTools.Remove(toolEntity))
		{
			((EntitySystem)this).Dirty<InternalsComponent>(ent, (MetaDataComponent)null);
			BreathToolComponent breathTool = default(BreathToolComponent);
			if (((EntitySystem)this).TryComp<BreathToolComponent>(toolEntity, ref breathTool))
			{
				breathTool.ConnectedInternalsEntity = null;
				((EntitySystem)this).Dirty(toolEntity, (IComponent)(object)breathTool, (MetaDataComponent)null);
			}
			if (ent.Comp.BreathTools.Count == 0)
			{
				DisconnectTank(ent, forced);
			}
			_alerts.ShowAlert(Entity<InternalsComponent>.op_Implicit(ent), ent.Comp.InternalsAlert, GetSeverity(Entity<InternalsComponent>.op_Implicit(ent)));
		}
	}

	public void DisconnectTank(Entity<InternalsComponent> ent, bool forced = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		GasTankComponent tank = default(GasTankComponent);
		if (((EntitySystem)this).TryComp<GasTankComponent>(ent.Comp.GasTankEntity, ref tank))
		{
			SharedGasTankSystem gasTank = _gasTank;
			Entity<GasTankComponent> ent2 = Entity<GasTankComponent>.op_Implicit((ent.Comp.GasTankEntity.Value, tank));
			bool forced2 = forced;
			gasTank.DisconnectFromInternals(ent2, null, forced2);
		}
		ent.Comp.GasTankEntity = null;
		((EntitySystem)this).Dirty<InternalsComponent>(ent, (MetaDataComponent)null);
		_alerts.ShowAlert(ent.Owner, ent.Comp.InternalsAlert, GetSeverity(ent.Comp));
	}

	public bool TryConnectTank(Entity<InternalsComponent> ent, EntityUid tankEntity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.BreathTools.Count == 0)
		{
			return false;
		}
		GasTankComponent tank = default(GasTankComponent);
		if (((EntitySystem)this).TryComp<GasTankComponent>(ent.Comp.GasTankEntity, ref tank))
		{
			_gasTank.DisconnectFromInternals(Entity<GasTankComponent>.op_Implicit((ent.Comp.GasTankEntity.Value, tank)));
		}
		ent.Comp.GasTankEntity = tankEntity;
		((EntitySystem)this).Dirty<InternalsComponent>(ent, (MetaDataComponent)null);
		_alerts.ShowAlert(Entity<InternalsComponent>.op_Implicit(ent), ent.Comp.InternalsAlert, GetSeverity(Entity<InternalsComponent>.op_Implicit(ent)));
		return true;
	}

	public bool AreInternalsWorking(EntityUid uid, InternalsComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<InternalsComponent>(uid, ref component, false))
		{
			return AreInternalsWorking(component);
		}
		return false;
	}

	public bool AreInternalsWorking(InternalsComponent component)
	{
		BreathToolComponent breathTool = default(BreathToolComponent);
		if (((EntitySystem)this).TryComp<BreathToolComponent>(Extensions.FirstOrNull<EntityUid>((IEnumerable<EntityUid>)component.BreathTools), ref breathTool) && breathTool.IsFunctional)
		{
			return ((EntitySystem)this).HasComp<GasTankComponent>(component.GasTankEntity);
		}
		return false;
	}

	protected short GetSeverity(InternalsComponent component)
	{
		if (component.BreathTools.Count == 0 || !AreInternalsWorking(component))
		{
			return 2;
		}
		GasTankComponent gasTank = default(GasTankComponent);
		if (((EntitySystem)this).TryComp<GasTankComponent>(component.GasTankEntity, ref gasTank) && gasTank.IsLowPressure)
		{
			return 0;
		}
		return 1;
	}

	public Entity<GasTankComponent>? FindBestGasTank(Entity<HandsComponent?, InventoryComponent?, ContainerManagerComponent?> user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<InventoryComponent, ContainerManagerComponent>(Entity<HandsComponent, InventoryComponent, ContainerManagerComponent>.op_Implicit(user), ref user.Comp2, ref user.Comp3, true))
		{
			return null;
		}
		GasTankComponent backGasTank = default(GasTankComponent);
		if (_inventory.TryGetSlotEntity(Entity<HandsComponent, InventoryComponent, ContainerManagerComponent>.op_Implicit(user), "back", out var backEntity, user.Comp2, user.Comp3) && ((EntitySystem)this).TryComp<GasTankComponent>(backEntity, ref backGasTank) && _gasTank.CanConnectToInternals(Entity<GasTankComponent>.op_Implicit((backEntity.Value, backGasTank))))
		{
			return Entity<GasTankComponent>.op_Implicit((backEntity.Value, backGasTank));
		}
		GasTankComponent gasTank = default(GasTankComponent);
		if (_inventory.TryGetSlotEntity(Entity<HandsComponent, InventoryComponent, ContainerManagerComponent>.op_Implicit(user), "suitstorage", out var entity, user.Comp2, user.Comp3) && ((EntitySystem)this).TryComp<GasTankComponent>(entity, ref gasTank) && _gasTank.CanConnectToInternals(Entity<GasTankComponent>.op_Implicit((entity.Value, gasTank))))
		{
			return Entity<GasTankComponent>.op_Implicit((entity.Value, gasTank));
		}
		foreach (EntityUid item in _inventory.GetHandOrInventoryEntities(Entity<HandsComponent, InventoryComponent>.op_Implicit((user.Owner, user.Comp1, user.Comp2))))
		{
			if (((EntitySystem)this).TryComp<GasTankComponent>(item, ref gasTank) && _gasTank.CanConnectToInternals(Entity<GasTankComponent>.op_Implicit((item, gasTank))))
			{
				return Entity<GasTankComponent>.op_Implicit((item, gasTank));
			}
		}
		return null;
	}
}
