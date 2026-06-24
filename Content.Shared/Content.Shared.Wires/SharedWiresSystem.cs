using System;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Tools;
using Content.Shared.Tools.Systems;
using Content.Shared.UserInterface;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.Shared.Wires;

public abstract class SharedWiresSystem : EntitySystem
{
	[Dependency]
	protected ISharedAdminLogManager AdminLogger;

	[Dependency]
	private ActivatableUISystem _activatableUI;

	[Dependency]
	protected SharedAppearanceSystem Appearance;

	[Dependency]
	protected SharedAudioSystem Audio;

	[Dependency]
	protected SharedToolSystem Tool;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<WiresPanelComponent, ComponentStartup>((EntityEventRefHandler<WiresPanelComponent, ComponentStartup>)OnStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WiresPanelComponent, WirePanelDoAfterEvent>((ComponentEventHandler<WiresPanelComponent, WirePanelDoAfterEvent>)OnPanelDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WiresPanelComponent, InteractUsingEvent>((EntityEventRefHandler<WiresPanelComponent, InteractUsingEvent>)OnInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WiresPanelComponent, ExaminedEvent>((ComponentEventHandler<WiresPanelComponent, ExaminedEvent>)OnExamine, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActivatableUIRequiresPanelComponent, ActivatableUIOpenAttemptEvent>((ComponentEventHandler<ActivatableUIRequiresPanelComponent, ActivatableUIOpenAttemptEvent>)OnAttemptOpenActivatableUI, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActivatableUIRequiresPanelComponent, PanelChangedEvent>((ComponentEventRefHandler<ActivatableUIRequiresPanelComponent, PanelChangedEvent>)OnActivatableUIPanelChanged, (Type[])null, (Type[])null);
	}

	private void OnStartup(Entity<WiresPanelComponent> ent, ref ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		UpdateAppearance(Entity<WiresPanelComponent>.op_Implicit(ent), Entity<WiresPanelComponent>.op_Implicit(ent));
	}

	private void OnPanelDoAfter(EntityUid uid, WiresPanelComponent panel, WirePanelDoAfterEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && TogglePanel(uid, panel, !panel.Open, args.User))
		{
			ISharedAdminLogManager adminLogger = AdminLogger;
			LogStringHandler handler = new LogStringHandler(30, 3);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(args.User)), "user", "ToPrettyString(args.User)");
			handler.AppendLiteral(" screwed ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "target", "ToPrettyString(uid)");
			handler.AppendLiteral("'s maintenance panel ");
			handler.AppendFormatted(panel.Open ? "open" : "closed");
			adminLogger.Add(LogType.Action, LogImpact.Low, ref handler);
			SoundSpecifier sound = (panel.Open ? panel.ScrewdriverOpenSound : panel.ScrewdriverCloseSound);
			Audio.PlayPredicted(sound, uid, (EntityUid?)args.User, (AudioParams?)null);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnInteractUsing(Entity<WiresPanelComponent> ent, ref InteractUsingEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		if (Tool.HasQuality(args.Used, ProtoId<ToolQualityPrototype>.op_Implicit(ent.Comp.OpeningTool)) && CanTogglePanel(ent, args.User) && Tool.UseTool(args.Used, args.User, Entity<WiresPanelComponent>.op_Implicit(ent), (float)ent.Comp.OpenDelay.TotalSeconds, ProtoId<ToolQualityPrototype>.op_Implicit(ent.Comp.OpeningTool), new WirePanelDoAfterEvent()))
		{
			ISharedAdminLogManager adminLogger = AdminLogger;
			LogStringHandler handler = new LogStringHandler(38, 4);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(args.User)), "user", "ToPrettyString(args.User)");
			handler.AppendLiteral(" is screwing ");
			handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<WiresPanelComponent>.op_Implicit(ent), (MetaDataComponent)null), "target", "ToPrettyString(ent)");
			handler.AppendLiteral("'s ");
			handler.AppendFormatted(ent.Comp.Open ? "open" : "closed");
			handler.AppendLiteral(" maintenance panel at ");
			handler.AppendFormatted<EntityCoordinates>(((EntitySystem)this).Transform(Entity<WiresPanelComponent>.op_Implicit(ent)).Coordinates, "targetlocation", "Transform(ent).Coordinates");
			adminLogger.Add(LogType.Action, LogImpact.Low, ref handler);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnExamine(EntityUid uid, WiresPanelComponent component, ExaminedEvent args)
	{
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		using (args.PushGroup("WiresPanelComponent"))
		{
			LocId? examineTextClosed;
			if (!component.Open)
			{
				examineTextClosed = component.ExamineTextClosed;
				if (!string.IsNullOrEmpty(examineTextClosed.HasValue ? LocId.op_Implicit(examineTextClosed.GetValueOrDefault()) : null))
				{
					ILocalizationManager loc = base.Loc;
					examineTextClosed = component.ExamineTextClosed;
					args.PushMarkup(loc.GetString(examineTextClosed.HasValue ? LocId.op_Implicit(examineTextClosed.GetValueOrDefault()) : null));
				}
				return;
			}
			examineTextClosed = component.ExamineTextOpen;
			if (!string.IsNullOrEmpty(examineTextClosed.HasValue ? LocId.op_Implicit(examineTextClosed.GetValueOrDefault()) : null))
			{
				ILocalizationManager loc2 = base.Loc;
				examineTextClosed = component.ExamineTextOpen;
				args.PushMarkup(loc2.GetString(examineTextClosed.HasValue ? LocId.op_Implicit(examineTextClosed.GetValueOrDefault()) : null));
			}
			WiresPanelSecurityComponent wiresPanelSecurity = default(WiresPanelSecurityComponent);
			if (((EntitySystem)this).TryComp<WiresPanelSecurityComponent>(uid, ref wiresPanelSecurity) && wiresPanelSecurity.Examine != null)
			{
				args.PushMarkup(base.Loc.GetString(wiresPanelSecurity.Examine));
			}
		}
	}

	public void ChangePanelVisibility(EntityUid uid, WiresPanelComponent component, bool visible)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		component.Visible = visible;
		UpdateAppearance(uid, component);
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
	}

	protected void UpdateAppearance(EntityUid uid, WiresPanelComponent panel)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		AppearanceComponent appearance = default(AppearanceComponent);
		if (((EntitySystem)this).TryComp<AppearanceComponent>(uid, ref appearance))
		{
			Appearance.SetData(uid, (Enum)WiresVisuals.MaintenancePanelState, (object)(panel.Open && panel.Visible), appearance);
		}
	}

	public bool TogglePanel(EntityUid uid, WiresPanelComponent component, bool open, EntityUid? user = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if (!CanTogglePanel(Entity<WiresPanelComponent>.op_Implicit((uid, component)), user))
		{
			return false;
		}
		component.Open = open;
		UpdateAppearance(uid, component);
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		PanelChangedEvent ev = new PanelChangedEvent(component.Open);
		((EntitySystem)this).RaiseLocalEvent<PanelChangedEvent>(uid, ref ev, false);
		return true;
	}

	public bool CanTogglePanel(Entity<WiresPanelComponent> ent, EntityUid? user)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		AttemptChangePanelEvent attempt = new AttemptChangePanelEvent(ent.Comp.Open, user);
		((EntitySystem)this).RaiseLocalEvent<AttemptChangePanelEvent>(Entity<WiresPanelComponent>.op_Implicit(ent), ref attempt, false);
		return !attempt.Cancelled;
	}

	public bool IsPanelOpen(Entity<WiresPanelComponent?> entity, EntityUid? tool = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<WiresPanelComponent>(Entity<WiresPanelComponent>.op_Implicit(entity), ref entity.Comp, false))
		{
			return true;
		}
		if (tool.HasValue)
		{
			PanelOverrideEvent ev = new PanelOverrideEvent();
			((EntitySystem)this).RaiseLocalEvent<PanelOverrideEvent>(tool.Value, ref ev, false);
			if (ev.Allowed)
			{
				return true;
			}
		}
		WiresPanelSecurityComponent wiresPanelSecurity = default(WiresPanelSecurityComponent);
		if (((EntitySystem)this).TryComp<WiresPanelSecurityComponent>(Entity<WiresPanelComponent>.op_Implicit(entity), ref wiresPanelSecurity) && !wiresPanelSecurity.WiresAccessible)
		{
			return false;
		}
		return entity.Comp.Open;
	}

	private void OnAttemptOpenActivatableUI(EntityUid uid, ActivatableUIRequiresPanelComponent component, ActivatableUIOpenAttemptEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		WiresPanelComponent wires = default(WiresPanelComponent);
		if (!((CancellableEntityEventArgs)args).Cancelled && ((EntitySystem)this).TryComp<WiresPanelComponent>(uid, ref wires) && component.RequireOpen != wires.Open)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnActivatableUIPanelChanged(EntityUid uid, ActivatableUIRequiresPanelComponent component, ref PanelChangedEvent args)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (args.Open != component.RequireOpen)
		{
			_activatableUI.CloseAll(uid);
		}
	}
}
