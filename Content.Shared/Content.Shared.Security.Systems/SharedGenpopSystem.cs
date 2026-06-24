using System;
using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared.CCVar;
using Content.Shared.Database;
using Content.Shared.Examine;
using Content.Shared.Lock;
using Content.Shared.Popups;
using Content.Shared.Security.Components;
using Content.Shared.Storage.Components;
using Content.Shared.Storage.EntitySystems;
using Content.Shared.Verbs;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Shared.Security.Systems;

public abstract class SharedGenpopSystem : EntitySystem
{
	[Dependency]
	private IConfigurationManager _cfgManager;

	[Dependency]
	protected IGameTiming Timing;

	[Dependency]
	private AccessReaderSystem _accessReader;

	[Dependency]
	private SharedEntityStorageSystem _entityStorage;

	[Dependency]
	protected SharedIdCardSystem IdCard;

	[Dependency]
	private LockSystem _lock;

	[Dependency]
	protected MetaDataSystem MetaDataSystem;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedUserInterfaceSystem _userInterface;

	private int _maxIdJobLength;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<GenpopLockerComponent, GenpopLockerIdConfiguredMessage>((EntityEventRefHandler<GenpopLockerComponent, GenpopLockerIdConfiguredMessage>)OnIdConfigured, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GenpopLockerComponent, StorageCloseAttemptEvent>((EntityEventRefHandler<GenpopLockerComponent, StorageCloseAttemptEvent>)OnCloseAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GenpopLockerComponent, LockToggleAttemptEvent>((EntityEventRefHandler<GenpopLockerComponent, LockToggleAttemptEvent>)OnLockToggleAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GenpopLockerComponent, LockToggledEvent>((EntityEventRefHandler<GenpopLockerComponent, LockToggledEvent>)OnLockToggled, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GenpopLockerComponent, GetVerbsEvent<Verb>>((EntityEventRefHandler<GenpopLockerComponent, GetVerbsEvent<Verb>>)OnGetVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GenpopIdCardComponent, ExaminedEvent>((EntityEventRefHandler<GenpopIdCardComponent, ExaminedEvent>)OnExamine, (Type[])null, (Type[])null);
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _cfgManager, CCVars.MaxIdJobLength, (Action<int>)delegate(int value)
		{
			_maxIdJobLength = value;
		}, true);
	}

	private void OnIdConfigured(Entity<GenpopLockerComponent> ent, ref GenpopLockerIdConfiguredMessage args)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		if (!string.IsNullOrWhiteSpace(args.Name) && args.Name.Length <= _maxIdJobLength && !(args.Sentence < 0f) && !string.IsNullOrWhiteSpace(args.Crime) && args.Crime.Length <= 48 && _accessReader.IsAllowed(((BaseBoundUserInterfaceEvent)args).Actor, Entity<GenpopLockerComponent>.op_Implicit(ent)))
		{
			ent.Comp.LinkedId = EntityUid.Invalid;
			_lock.Lock(ent.Owner, null);
			_entityStorage.CloseStorage(Entity<GenpopLockerComponent>.op_Implicit(ent));
			CreateId(ent, args.Name, args.Sentence, args.Crime);
		}
	}

	private void OnCloseAttempt(Entity<GenpopLockerComponent> ent, ref StorageCloseAttemptEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled)
		{
			return;
		}
		if (!ent.Comp.LinkedId.HasValue)
		{
			args.Cancelled = true;
		}
		EntityUid? user = args.User;
		if (user.HasValue)
		{
			EntityUid user2 = user.GetValueOrDefault();
			if (!_accessReader.IsAllowed(user2, Entity<GenpopLockerComponent>.op_Implicit(ent)))
			{
				_popup.PopupClient(base.Loc.GetString("lock-comp-has-user-access-fail"), user2);
			}
			else
			{
				_userInterface.TryOpenUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)GenpopLockerUiKey.Key, user2, false);
			}
		}
	}

	private void OnLockToggleAttempt(Entity<GenpopLockerComponent> ent, ref LockToggleAttemptEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled)
		{
			return;
		}
		ExpireIdCardComponent expireIdCard = default(ExpireIdCardComponent);
		if (!ent.Comp.LinkedId.HasValue)
		{
			args.Cancelled = true;
		}
		else if (!_accessReader.FindPotentialAccessItems(args.User).Contains(ent.Comp.LinkedId.Value))
		{
			if (!args.Silent)
			{
				_popup.PopupClient(base.Loc.GetString("lock-comp-has-user-access-fail"), Entity<GenpopLockerComponent>.op_Implicit(ent), args.User);
			}
			args.Cancelled = true;
		}
		else if (!((EntitySystem)this).TryComp<ExpireIdCardComponent>(ent.Comp.LinkedId.Value, ref expireIdCard) || !expireIdCard.Expired)
		{
			if (!args.Silent)
			{
				_popup.PopupClient(base.Loc.GetString("genpop-prisoner-id-popup-not-served"), Entity<GenpopLockerComponent>.op_Implicit(ent), args.User);
			}
			args.Cancelled = true;
		}
	}

	private void OnLockToggled(Entity<GenpopLockerComponent> ent, ref LockToggledEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Locked)
		{
			CancelIdCard(ent);
		}
	}

	private void OnGetVerbs(Entity<GenpopLockerComponent> ent, ref GetVerbsEvent<Verb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		ExpireIdCardComponent expire = default(ExpireIdCardComponent);
		GenpopIdCardComponent genpopId = default(GenpopIdCardComponent);
		if (!ent.Comp.LinkedId.HasValue || !args.CanAccess || !args.CanComplexInteract || !args.CanInteract || !((EntitySystem)this).TryComp<ExpireIdCardComponent>(ent.Comp.LinkedId, ref expire) || !((EntitySystem)this).TryComp<GenpopIdCardComponent>(ent.Comp.LinkedId, ref genpopId))
		{
			return;
		}
		EntityUid user = args.User;
		bool hasAccess = _accessReader.IsAllowed(args.User, Entity<GenpopLockerComponent>.op_Implicit(ent));
		args.Verbs.Add(new Verb
		{
			Act = delegate
			{
				//IL_001b: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				IdCard.ExpireId(Entity<ExpireIdCardComponent>.op_Implicit((ent.Comp.LinkedId.Value, expire)));
			},
			Priority = 13,
			Text = base.Loc.GetString("genpop-locker-action-end-early"),
			Impact = LogImpact.Medium,
			DoContactInteraction = true,
			Disabled = !hasAccess
		});
		args.Verbs.Add(new Verb
		{
			Act = delegate
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				CancelIdCard(ent, user);
			},
			Priority = 12,
			Text = base.Loc.GetString("genpop-locker-action-clear-id"),
			Impact = LogImpact.Medium,
			DoContactInteraction = true,
			Disabled = !hasAccess
		});
		double servedTime = 1.0 - (expire.ExpireTime - Timing.CurTime).TotalSeconds / genpopId.SentenceDuration.TotalSeconds;
		if (!expire.Expired)
		{
			args.Verbs.Add(new Verb
			{
				Act = delegate
				{
					//IL_001b: Unknown result type (might be due to invalid IL or missing references)
					//IL_002b: Unknown result type (might be due to invalid IL or missing references)
					IdCard.SetExpireTime(Entity<ExpireIdCardComponent>.op_Implicit((ent.Comp.LinkedId.Value, expire)), Timing.CurTime + genpopId.SentenceDuration);
				},
				Priority = 11,
				Text = base.Loc.GetString("genpop-locker-action-reset-sentence", (ValueTuple<string, object>)("percent", Math.Clamp(servedTime, 0.0, 1.0) * 100.0)),
				Impact = LogImpact.Medium,
				DoContactInteraction = true,
				Disabled = !hasAccess
			});
		}
	}

	private void CancelIdCard(Entity<GenpopLockerComponent> ent, EntityUid? user = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.LinkedId.HasValue)
		{
			MetaDataComponent metaData = ((EntitySystem)this).MetaData(Entity<GenpopLockerComponent>.op_Implicit(ent));
			MetaDataSystem.SetEntityName(Entity<GenpopLockerComponent>.op_Implicit(ent), base.Loc.GetString("genpop-locker-name-default"), metaData, true);
			MetaDataSystem.SetEntityDescription(Entity<GenpopLockerComponent>.op_Implicit(ent), base.Loc.GetString("genpop-locker-desc-default"), metaData);
			ent.Comp.LinkedId = null;
			_lock.Unlock(ent.Owner, user);
			_entityStorage.OpenStorage(ent.Owner);
			ExpireIdCardComponent expire = default(ExpireIdCardComponent);
			if (((EntitySystem)this).TryComp<ExpireIdCardComponent>(ent.Comp.LinkedId, ref expire))
			{
				IdCard.ExpireId(Entity<ExpireIdCardComponent>.op_Implicit((ent.Comp.LinkedId.Value, expire)));
			}
			((EntitySystem)this).Dirty<GenpopLockerComponent>(ent, (MetaDataComponent)null);
		}
	}

	private void OnExamine(Entity<GenpopIdCardComponent> ent, ref ExaminedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		ExpireIdCardComponent expireIdCard = default(ExpireIdCardComponent);
		if (((EntitySystem)this).TryComp<ExpireIdCardComponent>(Entity<GenpopIdCardComponent>.op_Implicit(ent), ref expireIdCard))
		{
			if (expireIdCard.Permanent)
			{
				args.PushText(base.Loc.GetString("genpop-prisoner-id-examine-wait-perm", (ValueTuple<string, object>)("crime", ent.Comp.Crime)));
				return;
			}
			if (expireIdCard.Expired)
			{
				args.PushText(base.Loc.GetString("genpop-prisoner-id-examine-served", (ValueTuple<string, object>)("crime", ent.Comp.Crime)));
				return;
			}
			TimeSpan sentence = ent.Comp.SentenceDuration;
			TimeSpan served = ent.Comp.SentenceDuration - (expireIdCard.ExpireTime - Timing.CurTime);
			args.PushText(base.Loc.GetString("genpop-prisoner-id-examine-wait", new(string, object)[4]
			{
				("minutes", served.Minutes),
				("seconds", served.Seconds),
				("sentence", sentence.TotalMinutes),
				("crime", ent.Comp.Crime)
			}));
		}
	}

	protected virtual void CreateId(Entity<GenpopLockerComponent> ent, string name, float sentence, string crime)
	{
	}
}
