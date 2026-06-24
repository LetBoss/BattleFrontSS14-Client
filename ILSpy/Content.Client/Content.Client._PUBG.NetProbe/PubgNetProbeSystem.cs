using System;
using System.Collections.Generic;
using Content.Shared._PUBG.NetProbe;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Client._PUBG.NetProbe;

public sealed class PubgNetProbeSystem : EntitySystem
{
	private sealed record PendingProbe(int Kb, TimeSpan Deadline);

	[Dependency]
	private IConsoleHost _console;

	[Dependency]
	private IGameTiming _timing;

	private readonly Dictionary<int, PendingProbe> _local = new Dictionary<int, PendingProbe>();

	private readonly List<int> _expired = new List<int>();

	private int _nextId = 1;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<PubgNetProbeRequestEvent>((EntityEventHandler<PubgNetProbeRequestEvent>)OnRequest, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<PubgNetProbeAckEvent>((EntityEventHandler<PubgNetProbeAckEvent>)OnAck, (Type[])null, (Type[])null);
	}

	public override void Update(float frameTime)
	{
		((EntitySystem)this).Update(frameTime);
		if (_local.Count == 0)
		{
			return;
		}
		TimeSpan curTime = _timing.CurTime;
		foreach (var (item, pendingProbe2) in _local)
		{
			if (curTime >= pendingProbe2.Deadline)
			{
				_expired.Add(item);
			}
		}
		if (_expired.Count == 0)
		{
			return;
		}
		foreach (int item2 in _expired)
		{
			if (_local.Remove(item2, out PendingProbe value))
			{
				_console.WriteError((ICommonSession)null, base.Loc.GetString("cmd-pubgnetprobe-timeout", (ValueTuple<string, object>)("kb", value.Kb), (ValueTuple<string, object>)("seconds", (int)PubgNetProbeConsts.Timeout.TotalSeconds)));
			}
		}
		_expired.Clear();
	}

	public void StartLocal(int kb)
	{
		int num = _nextId++;
		_local[num] = new PendingProbe(kb, _timing.CurTime + PubgNetProbeConsts.Timeout);
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new PubgNetProbeUploadEvent(num, kb, BuildPayload(kb)));
	}

	private void OnRequest(PubgNetProbeRequestEvent ev)
	{
		if (PubgNetProbeConsts.IsValidKb(ev.Kb))
		{
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new PubgNetProbeUploadEvent(ev.RequestId, ev.Kb, BuildPayload(ev.Kb)));
		}
	}

	private void OnAck(PubgNetProbeAckEvent ev)
	{
		if (_local.Remove(ev.RequestId, out PendingProbe _))
		{
			string text = base.Loc.GetString(ev.LocKey, new(string, object)[4]
			{
				("kb", ev.Kb),
				("bytes", ev.ReceivedBytes),
				("expectedBytes", ev.ExpectedBytes),
				("limitKb", 3072)
			});
			if (ev.Status == PubgNetProbeAckStatus.Ok)
			{
				_console.WriteLine((ICommonSession)null, text);
			}
			else
			{
				_console.WriteError((ICommonSession)null, text);
			}
		}
	}

	private static byte[] BuildPayload(int kb)
	{
		byte[] array = new byte[PubgNetProbeConsts.ToBytes(kb)];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = (byte)(i % 251);
		}
		return array;
	}
}
