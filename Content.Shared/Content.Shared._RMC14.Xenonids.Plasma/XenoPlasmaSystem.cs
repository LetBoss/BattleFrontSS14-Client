using System;
using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Xenonids.Egg;
using Content.Shared._RMC14.Xenonids.Evolution;
using Content.Shared.Alert;
using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Content.Shared.Jittering;
using Content.Shared.Popups;
using Content.Shared.Rejuvenate;
using Content.Shared.Rounding;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Xenonids.Plasma;

public sealed class XenoPlasmaSystem : EntitySystem
{
	[Dependency]
	private AlertsSystem _alerts;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedJitteringSystem _jitter;

	private EntityQuery<XenoPlasmaComponent> _xenoPlasmaQuery;

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_xenoPlasmaQuery = ((EntitySystem)this).GetEntityQuery<XenoPlasmaComponent>();
		((EntitySystem)this).SubscribeLocalEvent<XenoPlasmaComponent, MapInitEvent>((EntityEventRefHandler<XenoPlasmaComponent, MapInitEvent>)OnXenoPlasmaMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoPlasmaComponent, ComponentRemove>((EntityEventRefHandler<XenoPlasmaComponent, ComponentRemove>)OnXenoPlasmaRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoPlasmaComponent, RejuvenateEvent>((EntityEventRefHandler<XenoPlasmaComponent, RejuvenateEvent>)OnXenoRejuvenate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoPlasmaComponent, XenoTransferPlasmaActionEvent>((EntityEventRefHandler<XenoPlasmaComponent, XenoTransferPlasmaActionEvent>)OnXenoTransferPlasmaAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoPlasmaComponent, XenoTransferPlasmaDoAfterEvent>((EntityEventRefHandler<XenoPlasmaComponent, XenoTransferPlasmaDoAfterEvent>)OnXenoTransferDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoPlasmaComponent, NewXenoEvolvedEvent>((EntityEventRefHandler<XenoPlasmaComponent, NewXenoEvolvedEvent>)OnNewXenoEvolved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoPlasmaComponent, XenoDevolvedEvent>((EntityEventRefHandler<XenoPlasmaComponent, XenoDevolvedEvent>)OnXenoDevolved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoActionPlasmaComponent, RMCActionUseAttemptEvent>((EntityEventRefHandler<XenoActionPlasmaComponent, RMCActionUseAttemptEvent>)OnXenoActionEnergyUseAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoActionPlasmaComponent, RMCActionUseEvent>((EntityEventRefHandler<XenoActionPlasmaComponent, RMCActionUseEvent>)OnXenoActionEnergyUse, (Type[])null, (Type[])null);
	}

	private void OnXenoPlasmaMapInit(Entity<XenoPlasmaComponent> ent, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateAlert(ent);
	}

	private void OnXenoPlasmaRemove(Entity<XenoPlasmaComponent> ent, ref ComponentRemove args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		_alerts.ClearAlert(Entity<XenoPlasmaComponent>.op_Implicit(ent), ent.Comp.Alert);
	}

	private void OnXenoRejuvenate(Entity<XenoPlasmaComponent> xeno, ref RejuvenateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		RegenPlasma(Entity<XenoPlasmaComponent>.op_Implicit((Entity<XenoPlasmaComponent>.op_Implicit(xeno), Entity<XenoPlasmaComponent>.op_Implicit(xeno))), xeno.Comp.MaxPlasma);
	}

	private void OnXenoTransferPlasmaAction(Entity<XenoPlasmaComponent> xeno, ref XenoTransferPlasmaActionEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		XenoPlasmaComponent targetPlasma = default(XenoPlasmaComponent);
		if (xeno.Owner == args.Target)
		{
			_popup.PopupClient(base.Loc.GetString("cm-xeno-plasma-cannot-self"), Entity<XenoPlasmaComponent>.op_Implicit(xeno), Entity<XenoPlasmaComponent>.op_Implicit(xeno));
		}
		else if (((EntitySystem)this).HasComp<XenoAttachedOvipositorComponent>(args.Target))
		{
			_popup.PopupClient(base.Loc.GetString("rmc-xeno-plasma-ovipositor"), Entity<XenoPlasmaComponent>.op_Implicit(xeno), Entity<XenoPlasmaComponent>.op_Implicit(xeno));
		}
		else if (!((EntitySystem)this).TryComp<XenoPlasmaComponent>(args.Target, ref targetPlasma) || targetPlasma.MaxPlasma == 0)
		{
			_popup.PopupClient(base.Loc.GetString("cm-xeno-plasma-other-max-zero", (ValueTuple<string, object>)("target", args.Target)), Entity<XenoPlasmaComponent>.op_Implicit(xeno), Entity<XenoPlasmaComponent>.op_Implicit(xeno));
		}
		else if (HasPlasmaPopup(Entity<XenoPlasmaComponent>.op_Implicit((Entity<XenoPlasmaComponent>.op_Implicit(xeno), Entity<XenoPlasmaComponent>.op_Implicit(xeno))), args.Amount))
		{
			((HandledEntityEventArgs)args).Handled = true;
			XenoTransferPlasmaDoAfterEvent ev = new XenoTransferPlasmaDoAfterEvent(args.Amount);
			DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, Entity<XenoPlasmaComponent>.op_Implicit(xeno), xeno.Comp.PlasmaTransferDelay, ev, Entity<XenoPlasmaComponent>.op_Implicit(xeno), args.Target)
			{
				BreakOnMove = true,
				DistanceThreshold = args.Range,
				TargetEffect = EntProtoId.op_Implicit("RMCEffectHealPlasma")
			};
			_doAfter.TryStartDoAfter(doAfter);
		}
	}

	private void OnXenoTransferDoAfter(Entity<XenoPlasmaComponent> self, ref XenoTransferPlasmaDoAfterEvent args)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled || ((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		EntityUid? target = args.Target;
		if (!target.HasValue)
		{
			return;
		}
		EntityUid target2 = target.GetValueOrDefault();
		XenoPlasmaComponent otherXeno = default(XenoPlasmaComponent);
		if (self.Owner == target2 || ((EntitySystem)this).HasComp<XenoAttachedOvipositorComponent>(args.Target) || !((EntitySystem)this).TryComp<XenoPlasmaComponent>(target2, ref otherXeno) || otherXeno.Plasma == otherXeno.MaxPlasma || !TryRemovePlasma(Entity<XenoPlasmaComponent>.op_Implicit((Entity<XenoPlasmaComponent>.op_Implicit(self), Entity<XenoPlasmaComponent>.op_Implicit(self))), args.Amount))
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		RegenPlasma(Entity<XenoPlasmaComponent>.op_Implicit(target2), args.Amount);
		_jitter.DoJitter(target2, TimeSpan.FromSeconds(1L), refresh: true, 80f, 8f, forceValueChange: true);
		if (!_net.IsClient)
		{
			_popup.PopupEntity(base.Loc.GetString("cm-xeno-plasma-transferred-to-other", new(string, object)[3]
			{
				("plasma", args.Amount),
				("target", target2),
				("total", self.Comp.Plasma)
			}), Entity<XenoPlasmaComponent>.op_Implicit(self), Entity<XenoPlasmaComponent>.op_Implicit(self));
			_popup.PopupEntity(base.Loc.GetString("cm-xeno-plasma-transferred-to-self", new(string, object)[3]
			{
				("plasma", args.Amount),
				("target", self.Owner),
				("total", otherXeno.Plasma)
			}), target2, target2);
			_audio.PlayPredicted(self.Comp.PlasmaTransferSound, Entity<XenoPlasmaComponent>.op_Implicit(self), (EntityUid?)Entity<XenoPlasmaComponent>.op_Implicit(self), (AudioParams?)null);
			if (otherXeno.Plasma != otherXeno.MaxPlasma)
			{
				args.Repeat = true;
			}
		}
	}

	private void OnNewXenoEvolved(Entity<XenoPlasmaComponent> newXeno, ref NewXenoEvolvedEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		EvolutionTransferPlasma(Entity<XenoEvolutionComponent>.op_Implicit(args.OldXeno), newXeno);
	}

	private void OnXenoDevolved(Entity<XenoPlasmaComponent> newXeno, ref XenoDevolvedEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		EvolutionTransferPlasma(args.OldXeno, newXeno);
	}

	private void OnXenoActionEnergyUseAttempt(Entity<XenoActionPlasmaComponent> action, ref RMCActionUseAttemptEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && !HasPlasmaPopup(Entity<XenoPlasmaComponent>.op_Implicit(args.User), action.Comp.Cost))
		{
			args.Cancelled = true;
		}
	}

	private void OnXenoActionEnergyUse(Entity<XenoActionPlasmaComponent> action, ref RMCActionUseEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		XenoPlasmaComponent plasma = default(XenoPlasmaComponent);
		if (((EntitySystem)this).TryComp<XenoPlasmaComponent>(args.User, ref plasma))
		{
			RemovePlasma(Entity<XenoPlasmaComponent>.op_Implicit((args.User, plasma)), action.Comp.Cost);
		}
	}

	private void EvolutionTransferPlasma(EntityUid oldXeno, Entity<XenoPlasmaComponent> newXeno)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		XenoPlasmaComponent oldXenoPlasma = default(XenoPlasmaComponent);
		if (((EntitySystem)this).TryComp<XenoPlasmaComponent>(oldXeno, ref oldXenoPlasma))
		{
			FixedPoint2 newPlasma = newXeno.Comp.MaxPlasma;
			if (oldXenoPlasma.MaxPlasma > 0)
			{
				newPlasma *= oldXenoPlasma.Plasma / oldXenoPlasma.MaxPlasma;
			}
			SetPlasma(newXeno, newPlasma);
		}
	}

	private void UpdateAlert(Entity<XenoPlasmaComponent> xeno)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		if (xeno.Comp.MaxPlasma != 0)
		{
			float level = MathF.Max(0f, xeno.Comp.Plasma.Float());
			short max = _alerts.GetMaxSeverity(xeno.Comp.Alert);
			int severity = max - ContentHelpers.RoundToLevels(level, xeno.Comp.MaxPlasma, max + 1);
			string plasmaResourceMessage = (int)xeno.Comp.Plasma + " / " + xeno.Comp.MaxPlasma;
			AlertsSystem alerts = _alerts;
			EntityUid euid = Entity<XenoPlasmaComponent>.op_Implicit(xeno);
			ProtoId<AlertPrototype> alert = xeno.Comp.Alert;
			short? severity2 = (short)severity;
			string dynamicMessage = plasmaResourceMessage;
			alerts.ShowAlert(euid, alert, severity2, null, autoRemove: false, showCooldown: true, dynamicMessage);
		}
	}

	public bool HasPlasma(Entity<XenoPlasmaComponent> xeno, FixedPoint2 plasma)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return xeno.Comp.Plasma >= plasma;
	}

	public bool HasPlasmaPopup(Entity<XenoPlasmaComponent?> xeno, FixedPoint2 plasma, bool predicted = true)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<XenoPlasmaComponent>(Entity<XenoPlasmaComponent>.op_Implicit(xeno), ref xeno.Comp, false))
		{
			DoPopup();
			return false;
		}
		if (!HasPlasma(Entity<XenoPlasmaComponent>.op_Implicit((Entity<XenoPlasmaComponent>.op_Implicit(xeno), xeno.Comp)), plasma))
		{
			DoPopup();
			return false;
		}
		return true;
		void DoPopup()
		{
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			string popup = base.Loc.GetString("cm-xeno-not-enough-plasma");
			if (predicted)
			{
				_popup.PopupClient(popup, Entity<XenoPlasmaComponent>.op_Implicit(xeno), Entity<XenoPlasmaComponent>.op_Implicit(xeno));
			}
			else
			{
				_popup.PopupEntity(popup, Entity<XenoPlasmaComponent>.op_Implicit(xeno), Entity<XenoPlasmaComponent>.op_Implicit(xeno));
			}
		}
	}

	public void RegenPlasma(Entity<XenoPlasmaComponent?> xeno, FixedPoint2 amount)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		if (_xenoPlasmaQuery.Resolve(Entity<XenoPlasmaComponent>.op_Implicit(xeno), ref xeno.Comp, false))
		{
			FixedPoint2 plasma = xeno.Comp.Plasma;
			xeno.Comp.Plasma = FixedPoint2.Min(xeno.Comp.Plasma + amount, xeno.Comp.MaxPlasma);
			if (!(plasma == xeno.Comp.Plasma))
			{
				((EntitySystem)this).Dirty<XenoPlasmaComponent>(xeno, (MetaDataComponent)null);
				UpdateAlert(Entity<XenoPlasmaComponent>.op_Implicit((Entity<XenoPlasmaComponent>.op_Implicit(xeno), xeno.Comp)));
			}
		}
	}

	public void RemovePlasma(Entity<XenoPlasmaComponent> xeno, FixedPoint2 plasma)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		xeno.Comp.Plasma = FixedPoint2.Max(FixedPoint2.Zero, xeno.Comp.Plasma - plasma);
		((EntitySystem)this).Dirty<XenoPlasmaComponent>(xeno, (MetaDataComponent)null);
		UpdateAlert(xeno);
	}

	public void SetPlasma(Entity<XenoPlasmaComponent> xeno, FixedPoint2 plasma)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		xeno.Comp.Plasma = plasma;
		((EntitySystem)this).Dirty<XenoPlasmaComponent>(xeno, (MetaDataComponent)null);
		UpdateAlert(xeno);
	}

	public bool TryRemovePlasma(Entity<XenoPlasmaComponent?> xeno, FixedPoint2 plasma)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<XenoPlasmaComponent>(Entity<XenoPlasmaComponent>.op_Implicit(xeno), ref xeno.Comp, false))
		{
			return false;
		}
		if (!HasPlasma(Entity<XenoPlasmaComponent>.op_Implicit((Entity<XenoPlasmaComponent>.op_Implicit(xeno), xeno.Comp)), plasma))
		{
			return false;
		}
		RemovePlasma(Entity<XenoPlasmaComponent>.op_Implicit((Entity<XenoPlasmaComponent>.op_Implicit(xeno), xeno.Comp)), plasma);
		return true;
	}

	public bool TryRemovePlasmaPopup(Entity<XenoPlasmaComponent?> xeno, FixedPoint2 plasma, bool predicted = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<XenoPlasmaComponent>(Entity<XenoPlasmaComponent>.op_Implicit(xeno), ref xeno.Comp, false))
		{
			return false;
		}
		if (TryRemovePlasma(Entity<XenoPlasmaComponent>.op_Implicit((Entity<XenoPlasmaComponent>.op_Implicit(xeno), xeno.Comp)), plasma))
		{
			return true;
		}
		if (predicted)
		{
			_popup.PopupClient(base.Loc.GetString("cm-xeno-not-enough-plasma"), Entity<XenoPlasmaComponent>.op_Implicit(xeno), Entity<XenoPlasmaComponent>.op_Implicit(xeno));
		}
		else
		{
			_popup.PopupEntity(base.Loc.GetString("cm-xeno-not-enough-plasma"), Entity<XenoPlasmaComponent>.op_Implicit(xeno), Entity<XenoPlasmaComponent>.op_Implicit(xeno));
		}
		return false;
	}
}
