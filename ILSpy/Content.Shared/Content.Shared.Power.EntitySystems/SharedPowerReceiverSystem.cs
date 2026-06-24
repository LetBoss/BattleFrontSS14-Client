using System;
using System.Diagnostics.CodeAnalysis;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Power.Components;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;

namespace Content.Shared.Power.EntitySystems;

public abstract class SharedPowerReceiverSystem : EntitySystem
{
	[Dependency]
	private INetManager _netMan;

	[Dependency]
	private ISharedAdminLogManager _adminLogger;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedPowerNetSystem _net;

	public abstract bool ResolveApc(EntityUid entity, [NotNullWhen(true)] ref SharedApcPowerReceiverComponent? component);

	public void SetNeedsPower(EntityUid uid, bool value, SharedApcPowerReceiverComponent? receiver = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (ResolveApc(uid, ref receiver) && receiver.NeedsPower != value)
		{
			receiver.NeedsPower = value;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)receiver, (MetaDataComponent)null);
		}
	}

	public void SetPowerDisabled(EntityUid uid, bool value, SharedApcPowerReceiverComponent? receiver = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (ResolveApc(uid, ref receiver) && receiver.PowerDisabled != value)
		{
			receiver.PowerDisabled = value;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)receiver, (MetaDataComponent)null);
		}
	}

	public bool TogglePower(EntityUid uid, bool playSwitchSound = true, SharedApcPowerReceiverComponent? receiver = null, EntityUid? user = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Expected O, but got Unknown
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		if (!ResolveApc(uid, ref receiver))
		{
			return true;
		}
		if (!receiver.NeedsPower)
		{
			bool powered = _net.IsPoweredCalculate(receiver);
			if (receiver.Powered != powered)
			{
				RaisePower(Entity<SharedApcPowerReceiverComponent>.op_Implicit((uid, receiver)));
			}
			SetPowerDisabled(uid, value: false, receiver);
			return true;
		}
		SetPowerDisabled(uid, !receiver.PowerDisabled, receiver);
		if (user.HasValue)
		{
			ISharedAdminLogManager adminLogger = _adminLogger;
			LogStringHandler handler = new LogStringHandler(32, 3);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user.Value)), "player", "ToPrettyString(user.Value)");
			handler.AppendLiteral(" hit power button on ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "ToPrettyString(uid)");
			handler.AppendLiteral(", it's now ");
			handler.AppendFormatted((!receiver.PowerDisabled) ? "on" : "off");
			adminLogger.Add(LogType.Action, LogImpact.Low, ref handler);
		}
		if (playSwitchSound)
		{
			_audio.PlayPredicted((SoundSpecifier)new SoundPathSpecifier("/Audio/Machines/machine_switch.ogg", (AudioParams?)null), uid, user, (AudioParams?)((AudioParams)(ref AudioParams.Default)).WithVolume(-2f));
		}
		if (_netMan.IsClient && receiver.PowerDisabled)
		{
			bool powered2 = _net.IsPoweredCalculate(receiver);
			if (receiver.Powered != powered2)
			{
				receiver.Powered = powered2;
				RaisePower(Entity<SharedApcPowerReceiverComponent>.op_Implicit((uid, receiver)));
			}
		}
		return !receiver.PowerDisabled;
	}

	protected virtual void RaisePower(Entity<SharedApcPowerReceiverComponent> entity)
	{
	}

	public bool IsPowered(Entity<SharedApcPowerReceiverComponent?> entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!ResolveApc(entity.Owner, ref entity.Comp))
		{
			return true;
		}
		return entity.Comp.Powered;
	}

	protected string GetExamineText(bool powered)
	{
		return base.Loc.GetString("power-receiver-component-on-examine-main", (ValueTuple<string, object>)("stateText", base.Loc.GetString(powered ? "power-receiver-component-on-examine-powered" : "power-receiver-component-on-examine-unpowered")));
	}
}
