using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.Doors.Components;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Roles;
using Content.Shared.SprayPainter.Components;
using Content.Shared.SprayPainter.Prototypes;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.Shared.SprayPainter;

public abstract class SharedSprayPainterSystem : EntitySystem
{
	[Dependency]
	protected IPrototypeManager Proto;

	[Dependency]
	private ISharedAdminLogManager _adminLogger;

	[Dependency]
	protected SharedAppearanceSystem Appearance;

	[Dependency]
	protected SharedAudioSystem Audio;

	[Dependency]
	protected SharedDoAfterSystem DoAfter;

	[Dependency]
	private SharedPopupSystem _popup;

	private static readonly ProtoId<AirlockDepartmentsPrototype> Departments = ProtoId<AirlockDepartmentsPrototype>.op_Implicit("Departments");

	public List<AirlockStyle> Styles { get; private set; } = new List<AirlockStyle>();

	public List<AirlockGroupPrototype> Groups { get; private set; } = new List<AirlockGroupPrototype>();

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		CacheStyles();
		((EntitySystem)this).SubscribeLocalEvent<SprayPainterComponent, MapInitEvent>((EntityEventRefHandler<SprayPainterComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SprayPainterComponent, SprayPainterDoorDoAfterEvent>((EntityEventRefHandler<SprayPainterComponent, SprayPainterDoorDoAfterEvent>)OnDoorDoAfter, (Type[])null, (Type[])null);
		BoundUserInterfaceRegisterExt.BuiEvents<SprayPainterComponent>(((EntitySystem)this).Subs, (object)SprayPainterUiKey.Key, (BuiEventSubscriber<SprayPainterComponent>)delegate(Subscriber<SprayPainterComponent> subs)
		{
			subs.Event<SprayPainterSpritePickedMessage>((EntityEventRefHandler<SprayPainterComponent, SprayPainterSpritePickedMessage>)OnSpritePicked);
			subs.Event<SprayPainterColorPickedMessage>((EntityEventRefHandler<SprayPainterComponent, SprayPainterColorPickedMessage>)OnColorPicked);
		});
		((EntitySystem)this).SubscribeLocalEvent<PaintableAirlockComponent, InteractUsingEvent>((EntityEventRefHandler<PaintableAirlockComponent, InteractUsingEvent>)OnAirlockInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PrototypesReloadedEventArgs>((EntityEventHandler<PrototypesReloadedEventArgs>)OnPrototypesReloaded, (Type[])null, (Type[])null);
	}

	private void OnMapInit(Entity<SprayPainterComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.ColorPalette.Count != 0)
		{
			SetColor(ent, ent.Comp.ColorPalette.First().Key);
		}
	}

	private void OnDoorDoAfter(Entity<SprayPainterComponent> ent, ref SprayPainterDoorDoAfterEvent args)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || args.Cancelled)
		{
			return;
		}
		EntityUid? target = args.Args.Target;
		if (target.HasValue)
		{
			EntityUid target2 = target.GetValueOrDefault();
			PaintableAirlockComponent airlock = default(PaintableAirlockComponent);
			if (((EntitySystem)this).TryComp<PaintableAirlockComponent>(target2, ref airlock))
			{
				airlock.Department = ProtoId<DepartmentPrototype>.op_Implicit(args.Department);
				((EntitySystem)this).Dirty(target2, (IComponent)(object)airlock, (MetaDataComponent)null);
				Audio.PlayPredicted(ent.Comp.SpraySound, Entity<SprayPainterComponent>.op_Implicit(ent), (EntityUid?)args.Args.User, (AudioParams?)null);
				Appearance.SetData(target2, (Enum)DoorVisuals.BaseRSI, (object)args.Sprite, (AppearanceComponent)null);
				ISharedAdminLogManager adminLogger = _adminLogger;
				LogStringHandler handler = new LogStringHandler(9, 2);
				handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(args.Args.User)), "user", "ToPrettyString(args.Args.User)");
				handler.AppendLiteral(" painted ");
				handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(args.Args.Target.Value)), "target", "ToPrettyString(args.Args.Target.Value)");
				adminLogger.Add(LogType.Action, LogImpact.Low, ref handler);
				((HandledEntityEventArgs)args).Handled = true;
			}
		}
	}

	private void OnColorPicked(Entity<SprayPainterComponent> ent, ref SprayPainterColorPickedMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		SetColor(ent, args.Key);
	}

	private void OnSpritePicked(Entity<SprayPainterComponent> ent, ref SprayPainterSpritePickedMessage args)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (args.Index < Styles.Count)
		{
			ent.Comp.Index = args.Index;
			((EntitySystem)this).Dirty(Entity<SprayPainterComponent>.op_Implicit(ent), (IComponent)(object)ent.Comp, (MetaDataComponent)null);
		}
	}

	private void SetColor(Entity<SprayPainterComponent> ent, string? paletteKey)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if (paletteKey != null && !(paletteKey == ent.Comp.PickedColor) && ent.Comp.ColorPalette.ContainsKey(paletteKey))
		{
			ent.Comp.PickedColor = paletteKey;
			((EntitySystem)this).Dirty(Entity<SprayPainterComponent>.op_Implicit(ent), (IComponent)(object)ent.Comp, (MetaDataComponent)null);
		}
	}

	private void OnAirlockInteract(Entity<PaintableAirlockComponent> ent, ref InteractUsingEvent args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		SprayPainterComponent painter = default(SprayPainterComponent);
		if (((HandledEntityEventArgs)args).Handled || !((EntitySystem)this).TryComp<SprayPainterComponent>(args.Used, ref painter))
		{
			return;
		}
		AirlockGroupPrototype airlockGroupPrototype = Proto.Index<AirlockGroupPrototype>(ent.Comp.Group);
		AirlockStyle style = Styles[painter.Index];
		if (!airlockGroupPrototype.StylePaths.TryGetValue(style.Name, out string sprite))
		{
			string msg = base.Loc.GetString("spray-painter-style-not-available");
			_popup.PopupClient(msg, args.User, args.User);
			return;
		}
		DoAfterArgs doAfterEventArgs = new DoAfterArgs((IEntityManager)(object)base.EntityManager, args.User, painter.AirlockSprayTime, new SprayPainterDoorDoAfterEvent(sprite, style.Department), args.Used, Entity<PaintableAirlockComponent>.op_Implicit(ent), args.Used)
		{
			BreakOnMove = true,
			BreakOnDamage = true,
			NeedHand = true
		};
		if (DoAfter.TryStartDoAfter(doAfterEventArgs, out var _))
		{
			((HandledEntityEventArgs)args).Handled = true;
			ISharedAdminLogManager adminLogger = _adminLogger;
			LogStringHandler handler = new LogStringHandler(23, 4);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(args.User)), "user", "ToPrettyString(args.User)");
			handler.AppendLiteral(" is painting ");
			handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<PaintableAirlockComponent>.op_Implicit(ent), (MetaDataComponent)null), "target", "ToPrettyString(ent)");
			handler.AppendLiteral(" to '");
			handler.AppendFormatted(style.Name);
			handler.AppendLiteral("' at ");
			handler.AppendFormatted<EntityCoordinates>(((EntitySystem)this).Transform(Entity<PaintableAirlockComponent>.op_Implicit(ent)).Coordinates, "targetlocation", "Transform(ent).Coordinates");
			adminLogger.Add(LogType.Action, LogImpact.Low, ref handler);
		}
	}

	private void OnPrototypesReloaded(PrototypesReloadedEventArgs args)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		if (!args.WasModified<AirlockGroupPrototype>() && !args.WasModified<AirlockDepartmentsPrototype>())
		{
			return;
		}
		Styles.Clear();
		Groups.Clear();
		CacheStyles();
		int max = Styles.Count - 1;
		AllEntityQueryEnumerator<SprayPainterComponent> query = ((EntitySystem)this).AllEntityQuery<SprayPainterComponent>();
		EntityUid uid = default(EntityUid);
		SprayPainterComponent comp = default(SprayPainterComponent);
		while (query.MoveNext(ref uid, ref comp))
		{
			if (comp.Index > max)
			{
				comp.Index = max;
				((EntitySystem)this).Dirty(uid, (IComponent)(object)comp, (MetaDataComponent)null);
			}
		}
	}

	protected virtual void CacheStyles()
	{
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		SortedSet<string> names = new SortedSet<string>();
		foreach (AirlockGroupPrototype group in Proto.EnumeratePrototypes<AirlockGroupPrototype>())
		{
			Groups.Add(group);
			foreach (string style in group.StylePaths.Keys)
			{
				names.Add(style);
			}
		}
		AirlockDepartmentsPrototype departments = Proto.Index<AirlockDepartmentsPrototype>(Departments);
		Styles.Capacity = names.Count;
		foreach (string name in names)
		{
			departments.Departments.TryGetValue(name, out ProtoId<DepartmentPrototype> department);
			Styles.Add(new AirlockStyle(name, ProtoId<DepartmentPrototype>.op_Implicit(department)));
		}
	}
}
