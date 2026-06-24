using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared.Alert;

public abstract class AlertsSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private IPrototypeManager _prototypeManager;

	private FrozenDictionary<ProtoId<AlertPrototype>, AlertPrototype> _typeToAlert;

	public IReadOnlyDictionary<AlertKey, AlertState>? GetActiveAlerts(EntityUid euid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		AlertsComponent comp = default(AlertsComponent);
		if (!((EntitySystem)this).TryComp<AlertsComponent>(euid, ref comp))
		{
			return null;
		}
		return comp.Alerts;
	}

	public short GetSeverityRange(ProtoId<AlertPrototype> alertType)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		short minSeverity = _typeToAlert[alertType].MinSeverity;
		return (short)MathF.Max(minSeverity, _typeToAlert[alertType].MaxSeverity - minSeverity);
	}

	public short GetMaxSeverity(ProtoId<AlertPrototype> alertType)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return _typeToAlert[alertType].MaxSeverity;
	}

	public short GetMinSeverity(ProtoId<AlertPrototype> alertType)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return _typeToAlert[alertType].MinSeverity;
	}

	public bool IsShowingAlert(EntityUid euid, ProtoId<AlertPrototype> alertType)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		AlertsComponent alertsComponent = default(AlertsComponent);
		if (!((EntitySystem)this).TryComp<AlertsComponent>(euid, ref alertsComponent))
		{
			return false;
		}
		if (TryGet(alertType, out AlertPrototype alert))
		{
			return alertsComponent.Alerts.ContainsKey(alert.AlertKey);
		}
		((EntitySystem)this).Log.Debug("Unknown alert type {0}", new object[1] { alertType });
		return false;
	}

	public bool IsShowingAlertCategory(EntityUid euid, ProtoId<AlertCategoryPrototype> alertCategory)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		AlertsComponent alertsComponent = default(AlertsComponent);
		if (((EntitySystem)this).TryComp<AlertsComponent>(euid, ref alertsComponent))
		{
			return alertsComponent.Alerts.ContainsKey(AlertKey.ForCategory(alertCategory));
		}
		return false;
	}

	public bool TryGetAlertState(EntityUid euid, AlertKey key, out AlertState alertState)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		AlertsComponent alertsComponent = default(AlertsComponent);
		if (((EntitySystem)this).TryComp<AlertsComponent>(euid, ref alertsComponent))
		{
			return alertsComponent.Alerts.TryGetValue(key, out alertState);
		}
		alertState = default(AlertState);
		return false;
	}

	public void ShowAlert(EntityUid euid, ProtoId<AlertPrototype> alertType, short? severity = null, (TimeSpan, TimeSpan)? cooldown = null, bool autoRemove = false, bool showCooldown = true, string? dynamicMessage = null)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		AlertsComponent alertsComponent = default(AlertsComponent);
		if (_timing.ApplyingState || !((EntitySystem)this).TryComp<AlertsComponent>(euid, ref alertsComponent))
		{
			return;
		}
		if (TryGet(alertType, out AlertPrototype alert))
		{
			if (alertsComponent.Alerts.TryGetValue(alert.AlertKey, out var alertStateCallback) && alertStateCallback.Type == alertType && alertStateCallback.Severity == severity)
			{
				(TimeSpan, TimeSpan)? cooldown2 = alertStateCallback.Cooldown;
				(TimeSpan, TimeSpan)? tuple = cooldown;
				bool hasValue = cooldown2.HasValue;
				if (hasValue == tuple.HasValue)
				{
					if (hasValue)
					{
						(TimeSpan, TimeSpan) valueOrDefault = cooldown2.GetValueOrDefault();
						(TimeSpan, TimeSpan) valueOrDefault2 = tuple.GetValueOrDefault();
						if (!(valueOrDefault.Item1 == valueOrDefault2.Item1) || !(valueOrDefault.Item2 == valueOrDefault2.Item2))
						{
							goto IL_0155;
						}
					}
					if (alertStateCallback.AutoRemove == autoRemove && alertStateCallback.ShowCooldown == showCooldown && alertStateCallback.DynamicMessage == dynamicMessage)
					{
						return;
					}
				}
			}
			goto IL_0155;
		}
		((EntitySystem)this).Log.Error("Unable to show alert {0}, please ensure this alertType has a corresponding YML alert prototype", new object[1] { alertType });
		return;
		IL_0155:
		alertsComponent.Alerts.Remove(alert.AlertKey);
		AlertState state = new AlertState
		{
			Cooldown = cooldown,
			Severity = severity,
			Type = alertType,
			AutoRemove = autoRemove,
			ShowCooldown = showCooldown,
			DynamicMessage = dynamicMessage
		};
		alertsComponent.Alerts[alert.AlertKey] = state;
		if (autoRemove)
		{
			AlertAutoRemoveComponent autoComp = ((EntitySystem)this).EnsureComp<AlertAutoRemoveComponent>(euid);
			if (!autoComp.AlertKeys.Contains(alert.AlertKey))
			{
				autoComp.AlertKeys.Add(alert.AlertKey);
			}
		}
		AfterShowAlert(Entity<AlertsComponent>.op_Implicit((euid, alertsComponent)));
		((EntitySystem)this).Dirty(euid, (IComponent)(object)alertsComponent, (MetaDataComponent)null);
	}

	public void ClearAlertCategory(EntityUid euid, ProtoId<AlertCategoryPrototype> category)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		AlertsComponent alertsComponent = default(AlertsComponent);
		if (((EntitySystem)this).TryComp<AlertsComponent>(euid, ref alertsComponent))
		{
			AlertKey key = AlertKey.ForCategory(category);
			if (alertsComponent.Alerts.Remove(key))
			{
				AfterClearAlert(Entity<AlertsComponent>.op_Implicit((euid, alertsComponent)));
				((EntitySystem)this).Dirty(euid, (IComponent)(object)alertsComponent, (MetaDataComponent)null);
			}
		}
	}

	public void ClearAlert(EntityUid euid, ProtoId<AlertPrototype> alertType)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		AlertsComponent alertsComponent = default(AlertsComponent);
		if (_timing.ApplyingState || !((EntitySystem)this).TryComp<AlertsComponent>(euid, ref alertsComponent))
		{
			return;
		}
		if (TryGet(alertType, out AlertPrototype alert))
		{
			if (alertsComponent.Alerts.Remove(alert.AlertKey))
			{
				AfterClearAlert(Entity<AlertsComponent>.op_Implicit((euid, alertsComponent)));
				((EntitySystem)this).Dirty(euid, (IComponent)(object)alertsComponent, (MetaDataComponent)null);
			}
		}
		else
		{
			((EntitySystem)this).Log.Error("Unable to clear alert, unknown alertType {0}", new object[1] { alertType });
		}
	}

	protected virtual void AfterShowAlert(Entity<AlertsComponent> alerts)
	{
	}

	protected virtual void AfterClearAlert(Entity<AlertsComponent> alerts)
	{
	}

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<AlertsComponent, ComponentStartup>((ComponentEventHandler<AlertsComponent, ComponentStartup>)HandleComponentStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AlertsComponent, ComponentShutdown>((ComponentEventHandler<AlertsComponent, ComponentShutdown>)HandleComponentShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AlertsComponent, PlayerAttachedEvent>((ComponentEventHandler<AlertsComponent, PlayerAttachedEvent>)OnPlayerAttached, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AlertAutoRemoveComponent, EntityUnpausedEvent>((ComponentEventHandler<AlertAutoRemoveComponent, EntityUnpausedEvent>)OnAutoRemoveUnPaused, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeAllEvent<ClickAlertEvent>((EntitySessionEventHandler<ClickAlertEvent>)HandleClickAlert, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeAllEvent<ClickAlertAltEvent>((EntitySessionEventHandler<ClickAlertAltEvent>)HandleClickAlertAlt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PrototypesReloadedEventArgs>((EntityEventHandler<PrototypesReloadedEventArgs>)HandlePrototypesReloaded, (Type[])null, (Type[])null);
		LoadPrototypes();
	}

	private void OnAutoRemoveUnPaused(EntityUid uid, AlertAutoRemoveComponent comp, EntityUnpausedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		AlertsComponent alertComp = default(AlertsComponent);
		if (!((EntitySystem)this).TryComp<AlertsComponent>(uid, ref alertComp))
		{
			return;
		}
		bool dirty = false;
		foreach (KeyValuePair<AlertKey, AlertState> alert in alertComp.Alerts)
		{
			(TimeSpan, TimeSpan)? cooldown = alert.Value.Cooldown;
			if (cooldown.HasValue)
			{
				(TimeSpan, TimeSpan) cooldown2 = (alert.Value.Cooldown.Value.Item1, alert.Value.Cooldown.Value.Item2 + args.PausedTime);
				AlertState state = new AlertState
				{
					Severity = alert.Value.Severity,
					Cooldown = cooldown2,
					ShowCooldown = alert.Value.ShowCooldown,
					AutoRemove = alert.Value.AutoRemove,
					Type = alert.Value.Type
				};
				alertComp.Alerts[alert.Key] = state;
				dirty = true;
			}
		}
		if (dirty)
		{
			((EntitySystem)this).Dirty(uid, (IComponent)(object)comp, (MetaDataComponent)null);
		}
	}

	public override void Update(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		EntityQueryEnumerator<AlertAutoRemoveComponent> query = ((EntitySystem)this).EntityQueryEnumerator<AlertAutoRemoveComponent>();
		EntityUid uid = default(EntityUid);
		AlertAutoRemoveComponent autoComp = default(AlertAutoRemoveComponent);
		AlertsComponent alertComp = default(AlertsComponent);
		while (query.MoveNext(ref uid, ref autoComp))
		{
			bool dirtyComp = false;
			if (autoComp.AlertKeys.Count <= 0 || !((EntitySystem)this).TryComp<AlertsComponent>(uid, ref alertComp))
			{
				((EntitySystem)this).RemCompDeferred(uid, (IComponent)(object)autoComp);
				continue;
			}
			List<AlertKey> removeList = new List<AlertKey>();
			foreach (AlertKey alertKey in autoComp.AlertKeys)
			{
				alertComp.Alerts.TryGetValue(alertKey, out var alertState);
				(TimeSpan, TimeSpan)? cooldown = alertState.Cooldown;
				if (cooldown.HasValue && !(alertState.Cooldown.Value.Item2 >= _timing.CurTime))
				{
					removeList.Add(alertKey);
					alertComp.Alerts.Remove(alertKey);
					dirtyComp = true;
				}
			}
			foreach (AlertKey alertKey2 in removeList)
			{
				autoComp.AlertKeys.Remove(alertKey2);
			}
			if (dirtyComp)
			{
				((EntitySystem)this).Dirty(uid, (IComponent)(object)alertComp, (MetaDataComponent)null);
			}
		}
	}

	protected virtual void HandleComponentShutdown(EntityUid uid, AlertsComponent component, ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RaiseLocalEvent<AlertSyncEvent>(uid, new AlertSyncEvent(uid), true);
	}

	private void HandleComponentStartup(EntityUid uid, AlertsComponent component, ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RaiseLocalEvent<AlertSyncEvent>(uid, new AlertSyncEvent(uid), true);
	}

	private void HandlePrototypesReloaded(PrototypesReloadedEventArgs obj)
	{
		if (obj.WasModified<AlertPrototype>())
		{
			LoadPrototypes();
		}
	}

	protected virtual void LoadPrototypes()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<ProtoId<AlertPrototype>, AlertPrototype> dict = new Dictionary<ProtoId<AlertPrototype>, AlertPrototype>();
		foreach (AlertPrototype alert in _prototypeManager.EnumeratePrototypes<AlertPrototype>())
		{
			if (!dict.TryAdd(ProtoId<AlertPrototype>.op_Implicit(alert.ID), alert))
			{
				((EntitySystem)this).Log.Error("Found alert with duplicate alertType {0} - all alerts must have a unique alertType, this one will be skipped", new object[1] { alert.ID });
			}
		}
		_typeToAlert = dict.ToFrozenDictionary();
	}

	public bool TryGet(ProtoId<AlertPrototype> alertType, [NotNullWhen(true)] out AlertPrototype? alert)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return _typeToAlert.TryGetValue(alertType, out alert);
	}

	private bool TryGetAlert(ProtoId<AlertPrototype> alertType, EntityUid? player, out AlertPrototype? alert, bool activate = true)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		alert = null;
		if (!player.HasValue || !((EntitySystem)this).HasComp<AlertsComponent>(player))
		{
			return false;
		}
		if (!IsShowingAlert(player.Value, alertType))
		{
			((EntitySystem)this).Log.Debug("User {0} attempted to click alert {1} which is not currently showing for them", new object[2]
			{
				((EntitySystem)this).Comp<MetaDataComponent>(player.Value).EntityName,
				alertType
			});
			return false;
		}
		if (!TryGet(alertType, out alert))
		{
			((EntitySystem)this).Log.Warning("Unrecognized encoded alert {0}", new object[1] { alert });
			return false;
		}
		if (!activate)
		{
			return true;
		}
		if (ActivateAlert(player.Value, alert) && _timing.IsFirstTimePredicted)
		{
			HandledAlert();
		}
		return true;
	}

	protected virtual void HandledAlert()
	{
	}

	private void HandleClickAlert(ClickAlertEvent ev, EntitySessionEventArgs args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		ProtoId<AlertPrototype> type = ev.Type;
		ICommonSession senderSession = ((EntitySessionEventArgs)(ref args)).SenderSession;
		TryGetAlert(type, (senderSession != null) ? senderSession.AttachedEntity : ((EntityUid?)null), out AlertPrototype _);
	}

	private void HandleClickAlertAlt(ClickAlertAltEvent msg, EntitySessionEventArgs args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? player = ((EntitySessionEventArgs)(ref args)).SenderSession.AttachedEntity;
		if (TryGetAlert(msg.Type, player, out AlertPrototype alert, activate: false) && alert != null && player.HasValue)
		{
			ActivateAlertAlt(player.Value, alert);
		}
	}

	public bool ActivateAlert(EntityUid user, AlertPrototype alert)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		BaseAlertEvent clickEvent = alert.ClickEvent;
		if (clickEvent == null)
		{
			return false;
		}
		((HandledEntityEventArgs)clickEvent).Handled = false;
		clickEvent.User = user;
		clickEvent.AlertId = ProtoId<AlertPrototype>.op_Implicit(alert.ID);
		((EntitySystem)this).RaiseLocalEvent(user, (object)clickEvent, true);
		return ((HandledEntityEventArgs)clickEvent).Handled;
	}

	public bool ActivateAlertAlt(EntityUid user, AlertPrototype alert)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		BaseAlertEvent altClickEvent = alert.AltClickEvent;
		if (altClickEvent == null)
		{
			return false;
		}
		((HandledEntityEventArgs)altClickEvent).Handled = false;
		altClickEvent.User = user;
		altClickEvent.AlertId = ProtoId<AlertPrototype>.op_Implicit(alert.ID);
		((EntitySystem)this).RaiseLocalEvent(user, (object)altClickEvent, true);
		return ((HandledEntityEventArgs)altClickEvent).Handled;
	}

	private void OnPlayerAttached(EntityUid uid, AlertsComponent component, PlayerAttachedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
	}
}
