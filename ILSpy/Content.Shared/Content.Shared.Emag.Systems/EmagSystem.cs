using System;
using Content.Shared.Administration.Logs;
using Content.Shared.Charges.Components;
using Content.Shared.Charges.Systems;
using Content.Shared.Database;
using Content.Shared.Emag.Components;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Tag;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Emag.Systems;

public sealed class EmagSystem : EntitySystem
{
	[Dependency]
	private ISharedAdminLogManager _adminLogger;

	[Dependency]
	private SharedChargesSystem _sharedCharges;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private TagSystem _tag;

	[Dependency]
	private SharedAudioSystem _audio;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<EmagComponent, AfterInteractEvent>((ComponentEventHandler<EmagComponent, AfterInteractEvent>)OnAfterInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EmaggedComponent, OnAccessOverriderAccessUpdatedEvent>((EntityEventRefHandler<EmaggedComponent, OnAccessOverriderAccessUpdatedEvent>)OnAccessOverriderAccessUpdated, (Type[])null, (Type[])null);
	}

	private void OnAccessOverriderAccessUpdated(Entity<EmaggedComponent> entity, ref OnAccessOverriderAccessUpdatedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (CompareFlag(entity.Comp.EmagType, EmagType.Access))
		{
			entity.Comp.EmagType &= ~EmagType.Access;
			((EntitySystem)this).Dirty<EmaggedComponent>(entity, (MetaDataComponent)null);
		}
	}

	private void OnAfterInteract(EntityUid uid, EmagComponent comp, AfterInteractEvent args)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (args.CanReach)
		{
			EntityUid? target = args.Target;
			if (target.HasValue)
			{
				EntityUid target2 = target.GetValueOrDefault();
				((HandledEntityEventArgs)args).Handled = TryEmagEffect(Entity<EmagComponent>.op_Implicit((uid, comp)), args.User, target2);
			}
		}
	}

	public bool TryEmagEffect(Entity<EmagComponent?> ent, EntityUid user, EntityUid target, EmagType? customEmagType = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<EmagComponent>(Entity<EmagComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			return false;
		}
		if (_tag.HasTag(target, ent.Comp.EmagImmuneTag))
		{
			return false;
		}
		Entity<LimitedChargesComponent> chargesEnt = Entity<LimitedChargesComponent>.op_Implicit(ent.Owner);
		if (_sharedCharges.IsEmpty(chargesEnt))
		{
			_popup.PopupClient(base.Loc.GetString("emag-no-charges"), user, user);
			return false;
		}
		EmagType typeToUse = customEmagType ?? ent.Comp.EmagType;
		GotEmaggedEvent emaggedEvent = new GotEmaggedEvent(user, typeToUse);
		((EntitySystem)this).RaiseLocalEvent<GotEmaggedEvent>(target, ref emaggedEvent, false);
		if (!emaggedEvent.Handled)
		{
			return false;
		}
		_popup.PopupPredicted(base.Loc.GetString("emag-success", (ValueTuple<string, object>)("target", Identity.Entity(target, (IEntityManager)(object)base.EntityManager))), user, user, PopupType.Medium);
		_audio.PlayPredicted(ent.Comp.EmagSound, Entity<EmagComponent>.op_Implicit(ent), (EntityUid?)Entity<EmagComponent>.op_Implicit(ent), (AudioParams?)null);
		ISharedAdminLogManager adminLogger = _adminLogger;
		LogStringHandler handler = new LogStringHandler(24, 3);
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "player", "ToPrettyString(user)");
		handler.AppendLiteral(" emagged ");
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(target)), "target", "ToPrettyString(target)");
		handler.AppendLiteral(" with flag(s): ");
		handler.AppendFormatted(typeToUse, "typeToUse");
		adminLogger.Add(LogType.Emag, LogImpact.High, ref handler);
		if (emaggedEvent.Handled)
		{
			_sharedCharges.TryUseCharge(chargesEnt);
		}
		if (!emaggedEvent.Repeatable)
		{
			EmaggedComponent emaggedComp = default(EmaggedComponent);
			((EntitySystem)this).EnsureComp<EmaggedComponent>(target, ref emaggedComp);
			emaggedComp.EmagType |= typeToUse;
			((EntitySystem)this).Dirty(target, (IComponent)(object)emaggedComp, (MetaDataComponent)null);
		}
		return emaggedEvent.Handled;
	}

	public bool CheckFlag(EntityUid target, EmagType flag)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		EmaggedComponent comp = default(EmaggedComponent);
		if (!((EntitySystem)this).TryComp<EmaggedComponent>(target, ref comp))
		{
			return false;
		}
		if ((comp.EmagType & flag) == flag)
		{
			return true;
		}
		return false;
	}

	public bool CompareFlag(EmagType target, EmagType flag)
	{
		if ((target & flag) == flag)
		{
			return true;
		}
		return false;
	}
}
