using System;
using Content.Shared._RMC14.Dialog;
using Content.Shared.Administration;
using Content.Shared.Administration.Logs;
using Content.Shared.Administration.Managers;
using Content.Shared.Database;
using Content.Shared.DragDrop;
using Content.Shared.Mind;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared._RMC14.Mobs;

public sealed class RMCAdminGhostDragPossessionSystem : EntitySystem
{
	[Dependency]
	private ISharedAdminLogManager _adminLog;

	[Dependency]
	private ISharedAdminManager _adminManager;

	[Dependency]
	private DialogSystem _dialog;

	[Dependency]
	private SharedMindSystem _mind;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<CMGhostComponent, CanDragEvent>((EntityEventRefHandler<CMGhostComponent, CanDragEvent>)OnCanDrag, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CMGhostComponent, CanDropDraggedEvent>((EntityEventRefHandler<CMGhostComponent, CanDropDraggedEvent>)OnCanDropDragged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CMGhostComponent, DragDropDraggedEvent>((EntityEventRefHandler<CMGhostComponent, DragDropDraggedEvent>)OnGhostDraggedDropped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CMGhostComponent, CanDropTargetEvent>((EntityEventRefHandler<CMGhostComponent, CanDropTargetEvent>)OnCanDropTarget, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CMGhostComponent, GhostPossessionConfirmEvent>((EntityEventRefHandler<CMGhostComponent, GhostPossessionConfirmEvent>)OnPossessionConfirmation, (Type[])null, (Type[])null);
	}

	private void OnCanDrag(Entity<CMGhostComponent> ent, ref CanDragEvent args)
	{
		args.Handled = true;
	}

	private void OnCanDropDragged(Entity<CMGhostComponent> ent, ref CanDropDraggedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Exists(Entity<CMGhostComponent>.op_Implicit(ent)) && _adminManager.HasAdminFlag(args.User, AdminFlags.Fun))
		{
			args.CanDrop = true;
			args.Handled = true;
		}
	}

	private void OnCanDropTarget(Entity<CMGhostComponent> ent, ref CanDropTargetEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Exists(Entity<CMGhostComponent>.op_Implicit(ent)) && _adminManager.HasAdminFlag(args.User, AdminFlags.Fun))
		{
			args.CanDrop = true;
			args.Handled = true;
		}
	}

	private void OnGhostDraggedDropped(Entity<CMGhostComponent> ent, ref DragDropDraggedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Exists(Entity<CMGhostComponent>.op_Implicit(ent)) && _adminManager.HasAdminFlag(args.User, AdminFlags.Fun) && !(ent.Owner == args.Target))
		{
			args.Handled = true;
			GhostPossessionConfirmEvent ev = new GhostPossessionConfirmEvent(((EntitySystem)this).GetNetEntity(args.User, (MetaDataComponent)null), ((EntitySystem)this).GetNetEntity(Entity<CMGhostComponent>.op_Implicit(ent), (MetaDataComponent)null), ((EntitySystem)this).GetNetEntity(args.Target, (MetaDataComponent)null));
			_dialog.OpenConfirmation(args.User, "Are you sure?", $"Are you sure you want [Bold][Italic]{((EntitySystem)this).MetaData(Entity<CMGhostComponent>.op_Implicit(ent)).EntityName} | {ent.Owner.Id}[/Bold][/Italic] to possess [Bold][Italic]{((EntitySystem)this).MetaData(args.Target).EntityName} | {args.Target.Id}[/Bold][/Italic]", ev);
		}
	}

	private void OnPossessionConfirmation(Entity<CMGhostComponent> ent, ref GhostPossessionConfirmEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		EntityUid actor = ((EntitySystem)this).GetEntity(args.Actor);
		EntityUid possessor = ((EntitySystem)this).GetEntity(args.Possessor);
		EntityUid toPossess = ((EntitySystem)this).GetEntity(args.ToPossess);
		_mind.ControlMob(possessor, toPossess);
		ISharedAdminLogManager adminLog = _adminLog;
		LogStringHandler handler = new LogStringHandler(24, 3);
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(actor)), "player", "ToPrettyString(actor)");
		handler.AppendLiteral(" has forced ");
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(possessor)), "entity", "ToPrettyString(possessor)");
		handler.AppendLiteral(" to possess ");
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(toPossess)), "player", "ToPrettyString(toPossess)");
		adminLog.Add(LogType.RMCAdminCommandLogging, LogImpact.High, ref handler);
	}
}
