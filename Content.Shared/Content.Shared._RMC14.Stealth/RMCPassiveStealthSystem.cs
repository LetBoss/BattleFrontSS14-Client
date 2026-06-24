using System;
using Content.Shared.Foldable;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Storage.Components;
using Content.Shared.Storage.EntitySystems;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Stealth;

public sealed class RMCPassiveStealthSystem : EntitySystem
{
	[Dependency]
	private SharedEntityStorageSystem _entityStorage;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private EntityWhitelistSystem _whitelist;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RMCPassiveStealthComponent, ComponentInit>((EntityEventRefHandler<RMCPassiveStealthComponent, ComponentInit>)OnInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCPassiveStealthComponent, StorageAfterOpenEvent>((EntityEventRefHandler<RMCPassiveStealthComponent, StorageAfterOpenEvent>)OnStorageAfterOpen, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCPassiveStealthComponent, FoldedEvent>((EntityEventRefHandler<RMCPassiveStealthComponent, FoldedEvent>)OnFolded, (Type[])null, new Type[1] { typeof(SharedEntityStorageSystem) });
		((EntitySystem)this).SubscribeLocalEvent<RMCPassiveStealthComponent, ActivateInWorldEvent>((EntityEventRefHandler<RMCPassiveStealthComponent, ActivateInWorldEvent>)OnToggle, (Type[])null, (Type[])null);
	}

	private void OnInit(Entity<RMCPassiveStealthComponent> ent, ref ComponentInit args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState && !((EntitySystem)this).Paused(ent.Owner, (MetaDataComponent)null))
		{
			ent.Comp.Enabled = false;
			((EntitySystem)this).EnsureComp<EntityTurnInvisibleComponent>(ent.Owner);
		}
	}

	private void OnStorageAfterOpen(Entity<RMCPassiveStealthComponent> ent, ref StorageAfterOpenEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState && ent.Comp.Enabled.HasValue)
		{
			ent.Comp.Enabled = false;
			ent.Comp.ToggleTime = _timing.CurTime;
			((EntitySystem)this).Dirty(ent.Owner, (IComponent)(object)ent.Comp, (MetaDataComponent)null);
		}
	}

	private void OnFolded(Entity<RMCPassiveStealthComponent> ent, ref FoldedEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState && ent.Comp.Enabled.HasValue)
		{
			if (!args.IsFolded)
			{
				_entityStorage.OpenStorage(ent.Owner);
				ent.Comp.Enabled = false;
			}
			else
			{
				ent.Comp.Enabled = false;
				((EntitySystem)this).RemCompDeferred<EntityActiveInvisibleComponent>(ent.Owner);
			}
		}
	}

	private void OnToggle(Entity<RMCPassiveStealthComponent> ent, ref ActivateInWorldEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		FoldableComponent fold = default(FoldableComponent);
		if (!_timing.ApplyingState && ent.Comp.Toggleable && (!((EntitySystem)this).TryComp<FoldableComponent>(ent.Owner, ref fold) || !fold.IsFolded))
		{
			RMCPassiveStealthComponent comp = ent.Comp;
			bool valueOrDefault = comp.Enabled == true;
			if (!comp.Enabled.HasValue)
			{
				valueOrDefault = false;
				comp.Enabled = valueOrDefault;
			}
			if (!ent.Comp.Enabled.Value && !_whitelist.IsValid(ent.Comp.Whitelist, args.User))
			{
				string popup = base.Loc.GetString("rmc-skills-cant-use", (ValueTuple<string, object>)("item", ent.Owner));
				_popup.PopupClient(popup, args.User, args.User, PopupType.SmallCaution);
				((HandledEntityEventArgs)args).Handled = true;
			}
			else if (ent.Comp.Enabled.Value)
			{
				ent.Comp.Enabled = false;
				ent.Comp.ToggleTime = _timing.CurTime;
				((EntitySystem)this).Dirty(ent.Owner, (IComponent)(object)ent.Comp, (MetaDataComponent)null);
			}
			else
			{
				ent.Comp.Enabled = true;
				ent.Comp.ToggleTime = _timing.CurTime;
				((EntitySystem)this).Dirty(ent.Owner, (IComponent)(object)ent.Comp, (MetaDataComponent)null);
			}
		}
	}

	public override void Update(float frameTime)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.IsFirstTimePredicted)
		{
			return;
		}
		EntityQueryEnumerator<RMCPassiveStealthComponent> stealth = ((EntitySystem)this).EntityQueryEnumerator<RMCPassiveStealthComponent>();
		EntityUid uid = default(EntityUid);
		RMCPassiveStealthComponent stealthComp = default(RMCPassiveStealthComponent);
		EntityActiveInvisibleComponent invis2 = default(EntityActiveInvisibleComponent);
		while (stealth.MoveNext(ref uid, ref stealthComp))
		{
			if (!stealthComp.Enabled.HasValue || _net.IsClient)
			{
				continue;
			}
			TimeSpan time = _timing.CurTime - stealthComp.ToggleTime;
			if (stealthComp.Enabled.Value)
			{
				EntityActiveInvisibleComponent invis = ((EntitySystem)this).EnsureComp<EntityActiveInvisibleComponent>(uid);
				if (time < stealthComp.Delay)
				{
					invis.Opacity = (float)((double)stealthComp.MaxOpacity - time / stealthComp.Delay * (double)(stealthComp.MaxOpacity - stealthComp.MinOpacity));
				}
				else
				{
					invis.Opacity = stealthComp.MinOpacity;
				}
				((EntitySystem)this).Dirty(uid, (IComponent)(object)invis, (MetaDataComponent)null);
			}
			else if (((EntitySystem)this).TryComp<EntityActiveInvisibleComponent>(uid, ref invis2))
			{
				if (time < stealthComp.UnCloakDelay)
				{
					invis2.Opacity = (float)((double)stealthComp.MinOpacity + time / stealthComp.UnCloakDelay * (double)(stealthComp.MaxOpacity - stealthComp.MinOpacity));
					((EntitySystem)this).Dirty(uid, (IComponent)(object)invis2, (MetaDataComponent)null);
				}
				else
				{
					((EntitySystem)this).RemCompDeferred<EntityActiveInvisibleComponent>(uid);
				}
			}
		}
	}
}
