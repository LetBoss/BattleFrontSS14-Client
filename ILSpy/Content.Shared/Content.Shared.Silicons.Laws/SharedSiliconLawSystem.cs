using System;
using Content.Shared.Emag.Systems;
using Content.Shared.Mind;
using Content.Shared.Popups;
using Content.Shared.Silicons.Laws.Components;
using Content.Shared.Stunnable;
using Content.Shared.Wires;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Silicons.Laws;

public abstract class SharedSiliconLawSystem : EntitySystem
{
	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedStunSystem _stunSystem;

	[Dependency]
	private EmagSystem _emag;

	[Dependency]
	private SharedMindSystem _mind;

	public override void Initialize()
	{
		InitializeUpdater();
		((EntitySystem)this).SubscribeLocalEvent<EmagSiliconLawComponent, GotEmaggedEvent>((ComponentEventRefHandler<EmagSiliconLawComponent, GotEmaggedEvent>)OnGotEmagged, (Type[])null, (Type[])null);
	}

	private void OnGotEmagged(EntityUid uid, EmagSiliconLawComponent component, ref GotEmaggedEvent args)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		if (!_emag.CompareFlag(args.Type, EmagType.Interaction) || _emag.CheckFlag(uid, EmagType.Interaction))
		{
			return;
		}
		if (uid == args.UserUid)
		{
			_popup.PopupClient(base.Loc.GetString("law-emag-cannot-emag-self"), uid, args.UserUid);
			return;
		}
		WiresPanelComponent panel = default(WiresPanelComponent);
		if (component.RequireOpenPanel && ((EntitySystem)this).TryComp<WiresPanelComponent>(uid, ref panel) && !panel.Open)
		{
			_popup.PopupClient(base.Loc.GetString("law-emag-require-panel"), uid, args.UserUid);
			return;
		}
		SiliconEmaggedEvent ev = new SiliconEmaggedEvent(args.UserUid);
		((EntitySystem)this).RaiseLocalEvent<SiliconEmaggedEvent>(uid, ref ev, false);
		component.OwnerName = ((EntitySystem)this).Name(args.UserUid, (MetaDataComponent)null);
		NotifyLawsChanged(uid, component.EmaggedSound);
		if (_mind.TryGetMind(uid, out EntityUid mindId, out MindComponent _))
		{
			EnsureSubvertedSiliconRole(mindId);
		}
		_stunSystem.TryParalyze(uid, component.StunTime, refresh: true);
		args.Handled = true;
	}

	public virtual void NotifyLawsChanged(EntityUid uid, SoundSpecifier? cue = null)
	{
	}

	protected virtual void EnsureSubvertedSiliconRole(EntityUid mindId)
	{
	}

	protected virtual void RemoveSubvertedSiliconRole(EntityUid mindId)
	{
	}

	private void InitializeUpdater()
	{
		((EntitySystem)this).SubscribeLocalEvent<SiliconLawUpdaterComponent, EntInsertedIntoContainerMessage>((EntityEventRefHandler<SiliconLawUpdaterComponent, EntInsertedIntoContainerMessage>)OnUpdaterInsert, (Type[])null, (Type[])null);
	}

	protected virtual void OnUpdaterInsert(Entity<SiliconLawUpdaterComponent> ent, ref EntInsertedIntoContainerMessage args)
	{
	}
}
