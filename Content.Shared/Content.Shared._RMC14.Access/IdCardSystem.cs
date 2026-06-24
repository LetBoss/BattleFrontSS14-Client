using System;
using Content.Shared._RMC14.Marines;
using Content.Shared.Access.Components;
using Content.Shared.Administration.Logs;
using Content.Shared.Clothing.EntitySystems;
using Content.Shared.Database;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared._RMC14.Access;

public sealed class IdCardSystem : EntitySystem
{
	[Dependency]
	private ISharedAdminLogManager _adminLogger;

	[Dependency]
	private SharedPopupSystem _popup;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<IdCardComponent, UseInHandEvent>((EntityEventRefHandler<IdCardComponent, UseInHandEvent>)OnUseInHand, new Type[1] { typeof(ClothingSystem) }, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<IdCardComponent, ExaminedEvent>((EntityEventRefHandler<IdCardComponent, ExaminedEvent>)OnExamine, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MarineComponent, InteractUsingEvent>((EntityEventRefHandler<MarineComponent, InteractUsingEvent>)OnInteractUsing, (Type[])null, (Type[])null);
	}

	private void OnInteractUsing(Entity<MarineComponent> ent, ref InteractUsingEvent args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		IdCardComponent idCard = default(IdCardComponent);
		if (!((HandledEntityEventArgs)args).Handled && ((EntitySystem)this).TryComp<IdCardComponent>(args.Used, ref idCard) && !idCard.OriginalOwner.HasValue)
		{
			idCard.OriginalOwner = args.Target;
			string popupMessage = ((EntitySystem)this).Name(args.User, (MetaDataComponent)null) + " bound an ID to " + ((EntitySystem)this).Name(args.Target, (MetaDataComponent)null) + ".";
			_popup.PopupPredicted(popupMessage, args.Target, args.User);
			ISharedAdminLogManager adminLogger = _adminLogger;
			LogStringHandler handler = new LogStringHandler(22, 3);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(args.User)), "player", "ToPrettyString(args.User)");
			handler.AppendLiteral(" has bound the ID ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(args.Used)), "entity", "ToPrettyString(args.Used)");
			handler.AppendLiteral(" to ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(args.Target)), "player", "ToPrettyString(args.Target)");
			adminLogger.Add(LogType.RMCIdModify, LogImpact.High, ref handler);
			((HandledEntityEventArgs)args).Handled = true;
			((EntitySystem)this).Dirty<MarineComponent>(ent, (MetaDataComponent)null);
		}
	}

	private void OnExamine(Entity<IdCardComponent> ent, ref ExaminedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		if (!ent.Comp.OriginalOwner.HasValue)
		{
			args.PushMarkup("[color=orange]To claim ownership, interact with the card or another person to bind it to them.[/color]");
		}
	}

	private void OnUseInHand(Entity<IdCardComponent> ent, ref UseInHandEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		if (!ent.Comp.OriginalOwner.HasValue && !((HandledEntityEventArgs)args).Handled)
		{
			ent.Comp.OriginalOwner = args.User;
			((HandledEntityEventArgs)args).Handled = true;
			string popupMessage = "Bound ID to yourself.";
			_popup.PopupClient(popupMessage, args.User);
			ISharedAdminLogManager adminLogger = _adminLogger;
			LogStringHandler handler = new LogStringHandler(33, 2);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(args.User)), "player", "ToPrettyString(args.User)");
			handler.AppendLiteral(" has bound the ID ");
			handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<IdCardComponent>.op_Implicit(ent), (MetaDataComponent)null), "entity", "ToPrettyString(ent)");
			handler.AppendLiteral(" to themselves.");
			adminLogger.Add(LogType.RMCIdModify, LogImpact.Medium, ref handler);
			((EntitySystem)this).Dirty<IdCardComponent>(ent, (MetaDataComponent)null);
		}
	}
}
