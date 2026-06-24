using System;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Emag.Systems;
using Content.Shared.Interaction;
using Content.Shared.Ninja.Components;
using Content.Shared.Tag;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Ninja.Systems;

public sealed class EmagProviderSystem : EntitySystem
{
	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private EntityWhitelistSystem _whitelist;

	[Dependency]
	private ISharedAdminLogManager _adminLogger;

	[Dependency]
	private SharedNinjaGlovesSystem _gloves;

	[Dependency]
	private TagSystem _tag;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<EmagProviderComponent, BeforeInteractHandEvent>((EntityEventRefHandler<EmagProviderComponent, BeforeInteractHandEvent>)OnBeforeInteractHand, (Type[])null, (Type[])null);
	}

	private void OnBeforeInteractHand(Entity<EmagProviderComponent> ent, ref BeforeInteractHandEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || !_gloves.AbilityCheck(Entity<EmagProviderComponent>.op_Implicit(ent), args, out var target))
		{
			return;
		}
		Entity<EmagProviderComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		EmagProviderComponent emagProviderComponent = default(EmagProviderComponent);
		val.Deconstruct(ref val2, ref emagProviderComponent);
		EntityUid uid = val2;
		EmagProviderComponent comp = emagProviderComponent;
		if (!_whitelist.IsWhitelistFail(comp.Whitelist, target) && !_tag.HasTag(target, comp.AccessBreakerImmuneTag))
		{
			GotEmaggedEvent emagEv = new GotEmaggedEvent(uid, EmagType.Access);
			((EntitySystem)this).RaiseLocalEvent<GotEmaggedEvent>(args.Target, ref emagEv, false);
			if (emagEv.Handled)
			{
				_audio.PlayPredicted(comp.EmagSound, uid, (EntityUid?)uid, (AudioParams?)null);
				ISharedAdminLogManager adminLogger = _adminLogger;
				LogStringHandler handler = new LogStringHandler(24, 3);
				handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "player", "ToPrettyString(uid)");
				handler.AppendLiteral(" emagged ");
				handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(target)), "target", "ToPrettyString(target)");
				handler.AppendLiteral(" with flag(s): ");
				handler.AppendFormatted(ent.Comp.EmagType, "ent.Comp.EmagType");
				adminLogger.Add(LogType.Emag, LogImpact.High, ref handler);
				EmaggedSomethingEvent ev = new EmaggedSomethingEvent(target);
				((EntitySystem)this).RaiseLocalEvent<EmaggedSomethingEvent>(uid, ref ev, false);
				((HandledEntityEventArgs)args).Handled = true;
			}
		}
	}
}
