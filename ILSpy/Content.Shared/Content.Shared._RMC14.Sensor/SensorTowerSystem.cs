using System;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.TacticalMap;
using Content.Shared._RMC14.Tools;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Hands.Components;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Tools;
using Content.Shared.Tools.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Sensor;

public sealed class SensorTowerSystem : EntitySystem
{
	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private SkillsSystem _skills;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedToolSystem _tool;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<TacticalMapIncludeXenosEvent>((EntityEventRefHandler<TacticalMapIncludeXenosEvent>)OnTacticalMapIncludeXenos, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SensorTowerComponent, MapInitEvent>((EntityEventRefHandler<SensorTowerComponent, MapInitEvent>)OnSensorTowerMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SensorTowerComponent, InteractUsingEvent>((EntityEventRefHandler<SensorTowerComponent, InteractUsingEvent>)OnSensorTowerInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SensorTowerComponent, InteractHandEvent>((EntityEventRefHandler<SensorTowerComponent, InteractHandEvent>)OnSensorTowerInteractHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SensorTowerComponent, ExaminedEvent>((EntityEventRefHandler<SensorTowerComponent, ExaminedEvent>)OnSensorTowerExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SensorTowerComponent, SensorTowerRepairDoAfterEvent>((EntityEventRefHandler<SensorTowerComponent, SensorTowerRepairDoAfterEvent>)OnSensorTowerRepairDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SensorTowerComponent, SensorTowerDestroyDoAfterEvent>((EntityEventRefHandler<SensorTowerComponent, SensorTowerDestroyDoAfterEvent>)OnSensorTowerDestroyDoAfter, (Type[])null, (Type[])null);
	}

	private void OnTacticalMapIncludeXenos(ref TacticalMapIncludeXenosEvent ev)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<SensorTowerComponent> towers = ((EntitySystem)this).EntityQueryEnumerator<SensorTowerComponent>();
		SensorTowerComponent tower = default(SensorTowerComponent);
		while (towers.MoveNext(ref tower))
		{
			if (tower.State == SensorTowerState.On)
			{
				ev.Include = true;
				break;
			}
		}
	}

	private void OnSensorTowerMapInit(Entity<SensorTowerComponent> ent, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateAppearance(ent);
	}

	private void OnSensorTowerInteractUsing(Entity<SensorTowerComponent> ent, ref InteractUsingEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = args.User;
		if (!_skills.HasSkill(Entity<SkillsComponent>.op_Implicit(user), ent.Comp.Skill, ent.Comp.SkillLevel))
		{
			string msg = base.Loc.GetString("rmc-skills-no-training", (ValueTuple<string, object>)("target", ent));
			_popup.PopupClient(msg, Entity<SensorTowerComponent>.op_Implicit(ent), user, PopupType.SmallCaution);
			return;
		}
		EntityUid used = args.Used;
		RMCDeviceBreakerComponent breaker = default(RMCDeviceBreakerComponent);
		if (((EntitySystem)this).TryComp<RMCDeviceBreakerComponent>(args.Used, ref breaker) && ent.Comp.State != SensorTowerState.Weld)
		{
			DoAfterArgs doafter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, args.User, breaker.DoAfterTime, new RMCDeviceBreakerDoAfterEvent(), args.Used, args.Target, args.Used)
			{
				BreakOnMove = true,
				RequireCanInteract = true,
				BreakOnHandChange = true,
				DuplicateCondition = DuplicateConditions.SameTool
			};
			((HandledEntityEventArgs)args).Handled = true;
			_doAfter.TryStartDoAfter(doafter);
			return;
		}
		ProtoId<ToolQualityPrototype> correctQuality = (ProtoId<ToolQualityPrototype>)(ent.Comp.State switch
		{
			SensorTowerState.Weld => ent.Comp.WeldingQuality, 
			SensorTowerState.Wire => ent.Comp.CuttingQuality, 
			SensorTowerState.Wrench => ent.Comp.WrenchQuality, 
			_ => throw new ArgumentOutOfRangeException(), 
		});
		((HandledEntityEventArgs)args).Handled = true;
		if (_tool.HasQuality(used, ProtoId<ToolQualityPrototype>.op_Implicit(correctQuality)))
		{
			TryRepair(ent, user, used, ent.Comp.State);
		}
	}

	private void OnSensorTowerInteractHand(Entity<SensorTowerComponent> ent, ref InteractHandEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = args.User;
		if (((EntitySystem)this).HasComp<XenoComponent>(user))
		{
			if (((EntitySystem)this).HasComp<HandsComponent>(user))
			{
				Destroy(ent, user);
			}
			return;
		}
		if (!_skills.HasSkill(Entity<SkillsComponent>.op_Implicit(user), ent.Comp.Skill, ent.Comp.SkillLevel))
		{
			_popup.PopupClient("You have no clue how this thing works...", Entity<SensorTowerComponent>.op_Implicit(ent), user, PopupType.SmallCaution);
			return;
		}
		ref SensorTowerState state = ref ent.Comp.State;
		string popup = state switch
		{
			SensorTowerState.Weld => "Use a blowtorch, then wirecutters, then wrench to repair it.", 
			SensorTowerState.Wire => "Use some wirecutters, then wrench to repair it.", 
			SensorTowerState.Wrench => "Use a wrench to repair it.", 
			SensorTowerState.Off => "The " + ((EntitySystem)this).Name(Entity<SensorTowerComponent>.op_Implicit(ent), (MetaDataComponent)null) + " lights up.", 
			SensorTowerState.On => "The " + ((EntitySystem)this).Name(Entity<SensorTowerComponent>.op_Implicit(ent), (MetaDataComponent)null) + " goes dark.", 
			_ => throw new ArgumentOutOfRangeException(), 
		};
		_popup.PopupClient(popup, Entity<SensorTowerComponent>.op_Implicit(ent), user, PopupType.Medium);
		if (state >= SensorTowerState.Off)
		{
			if (state == SensorTowerState.Off)
			{
				state = SensorTowerState.On;
			}
			else if (state == SensorTowerState.On)
			{
				state = SensorTowerState.Off;
			}
			((EntitySystem)this).Dirty<SensorTowerComponent>(ent, (MetaDataComponent)null);
			UpdateAppearance(ent);
		}
	}

	private void OnSensorTowerExamined(Entity<SensorTowerComponent> ent, ref ExaminedEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<XenoComponent>(args.Examiner))
		{
			return;
		}
		using (args.PushGroup("SensorTowerComponent"))
		{
			string text = ent.Comp.State switch
			{
				SensorTowerState.Weld => "This one is heavily damaged. Use a blowtorch, wirecutters, then a wrench to repair it.", 
				SensorTowerState.Wire => "This one is heavily damaged. Use wirecutters, then a wrench to repair it.", 
				SensorTowerState.Wrench => "This one is heavily damaged. Use a wrench to repair it.", 
				SensorTowerState.Off => "It looks like it is offline.", 
				SensorTowerState.On => "It looks like it is online.", 
				_ => throw new ArgumentOutOfRangeException(), 
			};
			args.PushText(text);
			if (ent.Comp.State < SensorTowerState.Off)
			{
				string tool = ent.Comp.State switch
				{
					SensorTowerState.Wrench => "a [color=cyan]Wrench[/color]", 
					SensorTowerState.Wire => "[color=cyan]Wirecutters[/color]", 
					SensorTowerState.Weld => "a [color=cyan]Welder[/color]", 
					_ => throw new ArgumentOutOfRangeException(), 
				};
				args.PushMarkup("Use " + tool + " to repair it!");
			}
		}
	}

	private void OnSensorTowerRepairDoAfter(Entity<SensorTowerComponent> ent, ref SensorTowerRepairDoAfterEvent args)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && !((HandledEntityEventArgs)args).Handled)
		{
			((HandledEntityEventArgs)args).Handled = true;
			if (ent.Comp.State == args.State)
			{
				SensorTowerComponent comp = ent.Comp;
				comp.State = args.State switch
				{
					SensorTowerState.Weld => SensorTowerState.Wire, 
					SensorTowerState.Wire => SensorTowerState.Wrench, 
					SensorTowerState.Wrench => SensorTowerState.Off, 
					_ => throw new ArgumentOutOfRangeException(), 
				};
				((EntitySystem)this).Dirty<SensorTowerComponent>(ent, (MetaDataComponent)null);
				UpdateAppearance(ent);
			}
		}
	}

	private void OnSensorTowerDestroyDoAfter(Entity<SensorTowerComponent> ent, ref SensorTowerDestroyDoAfterEvent args)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && !((HandledEntityEventArgs)args).Handled)
		{
			((HandledEntityEventArgs)args).Handled = true;
			ent.Comp.State = SensorTowerState.Weld;
			((EntitySystem)this).Dirty<SensorTowerComponent>(ent, (MetaDataComponent)null);
			UpdateAppearance(ent);
		}
	}

	public void SensorTowerIncrementalDestroy(Entity<SensorTowerComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		SensorTowerComponent comp = ent.Comp;
		comp.State = ent.Comp.State switch
		{
			SensorTowerState.On => SensorTowerState.Wrench, 
			SensorTowerState.Off => SensorTowerState.Wrench, 
			SensorTowerState.Wrench => SensorTowerState.Wire, 
			SensorTowerState.Wire => SensorTowerState.Weld, 
			_ => throw new ArgumentOutOfRangeException(), 
		};
		((EntitySystem)this).Dirty<SensorTowerComponent>(ent, (MetaDataComponent)null);
		UpdateAppearance(ent);
	}

	private void TryRepair(Entity<SensorTowerComponent> tower, EntityUid user, EntityUid used, SensorTowerState state)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		ProtoId<ToolQualityPrototype> quality = (ProtoId<ToolQualityPrototype>)(state switch
		{
			SensorTowerState.Weld => tower.Comp.WeldingQuality, 
			SensorTowerState.Wire => tower.Comp.CuttingQuality, 
			SensorTowerState.Wrench => tower.Comp.WrenchQuality, 
			_ => throw new ArgumentOutOfRangeException("state", state, null), 
		});
		TimeSpan delay = state switch
		{
			SensorTowerState.Weld => tower.Comp.WeldingDelay, 
			SensorTowerState.Wire => tower.Comp.CuttingDelay, 
			SensorTowerState.Wrench => tower.Comp.WrenchDelay, 
			_ => throw new ArgumentOutOfRangeException("state", state, null), 
		};
		_tool.UseTool(used, user, Entity<SensorTowerComponent>.op_Implicit(tower), (float)delay.TotalSeconds, ProtoId<ToolQualityPrototype>.op_Implicit(quality), new SensorTowerRepairDoAfterEvent(state), tower.Comp.WeldingCost);
	}

	private void UpdateAppearance(Entity<SensorTowerComponent> tower)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		_appearance.SetData(Entity<SensorTowerComponent>.op_Implicit(tower), (Enum)SensorTowerLayers.Layer, (object)tower.Comp.State, (AppearanceComponent)null);
	}

	private void Destroy(Entity<SensorTowerComponent> tower, EntityUid user)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		if (tower.Comp.State == SensorTowerState.Weld)
		{
			_popup.PopupClient("We stare at the experimental sensor tower cluelessly.", user, user, PopupType.SmallCaution);
			return;
		}
		SensorTowerDestroyDoAfterEvent ev = new SensorTowerDestroyDoAfterEvent();
		DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, tower.Comp.DestroyDelay, ev, Entity<SensorTowerComponent>.op_Implicit(tower), Entity<SensorTowerComponent>.op_Implicit(tower), user)
		{
			ForceVisible = true
		};
		if (_doAfter.TryStartDoAfter(doAfter))
		{
			_popup.PopupClient("You start wrenching apart the " + ((EntitySystem)this).Name(Entity<SensorTowerComponent>.op_Implicit(tower), (MetaDataComponent)null) + "'s panels and reaching inside it!", Entity<SensorTowerComponent>.op_Implicit(tower), user, PopupType.Medium);
		}
	}

	public override void Update(float frameTime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<SensorTowerComponent> query = ((EntitySystem)this).EntityQueryEnumerator<SensorTowerComponent>();
		EntityUid uid = default(EntityUid);
		SensorTowerComponent tower = default(SensorTowerComponent);
		while (query.MoveNext(ref uid, ref tower))
		{
			if (tower.State != SensorTowerState.On || time < tower.NextBreakAt)
			{
				continue;
			}
			if (!RandomExtensions.Prob(_random, tower.BreakChance))
			{
				tower.NextBreakAt = time + tower.BreakEvery;
				((EntitySystem)this).Dirty(uid, (IComponent)(object)tower, (MetaDataComponent)null);
				continue;
			}
			if (RandomExtensions.Prob(_random, 0.75f))
			{
				_popup.PopupEntity("The " + ((EntitySystem)this).Name(uid, (MetaDataComponent)null) + " beeps wildly and sprays random pieces everywhere! Use a wrench to repair it.", uid, uid, PopupType.LargeCaution);
				tower.State = SensorTowerState.Wrench;
				((EntitySystem)this).Dirty(uid, (IComponent)(object)tower, (MetaDataComponent)null);
			}
			else
			{
				_popup.PopupEntity("The " + ((EntitySystem)this).Name(uid, (MetaDataComponent)null) + " beeps wildly and a fuse blows! Use wirecutters, then a wrench to repair it.", uid, uid, PopupType.LargeCaution);
				tower.State = SensorTowerState.Wire;
				((EntitySystem)this).Dirty(uid, (IComponent)(object)tower, (MetaDataComponent)null);
			}
			UpdateAppearance(Entity<SensorTowerComponent>.op_Implicit((uid, tower)));
		}
	}
}
