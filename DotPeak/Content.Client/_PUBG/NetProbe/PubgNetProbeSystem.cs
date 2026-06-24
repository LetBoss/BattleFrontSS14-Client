// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.NetProbe.PubgNetProbeSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._PUBG.NetProbe;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client._PUBG.NetProbe;

public sealed class PubgNetProbeSystem : EntitySystem
{
  [Dependency]
  private IConsoleHost _console;
  [Dependency]
  private IGameTiming _timing;
  private readonly Dictionary<int, PubgNetProbeSystem.PendingProbe> _local = new Dictionary<int, PubgNetProbeSystem.PendingProbe>();
  private readonly List<int> _expired = new List<int>();
  private int _nextId = 1;

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<PubgNetProbeRequestEvent>(new EntityEventHandler<PubgNetProbeRequestEvent>(this.OnRequest), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<PubgNetProbeAckEvent>(new EntityEventHandler<PubgNetProbeAckEvent>(this.OnAck), (Type[]) null, (Type[]) null);
  }

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    if (this._local.Count == 0)
      return;
    TimeSpan curTime = this._timing.CurTime;
    foreach ((int key, PubgNetProbeSystem.PendingProbe pendingProbe) in this._local)
    {
      if (curTime >= pendingProbe.Deadline)
        this._expired.Add(key);
    }
    if (this._expired.Count == 0)
      return;
    foreach (int key in this._expired)
    {
      PubgNetProbeSystem.PendingProbe pendingProbe;
      if (this._local.Remove(key, out pendingProbe))
        this._console.WriteError((ICommonSession) null, this.Loc.GetString("cmd-pubgnetprobe-timeout", ("kb", (object) pendingProbe.Kb), ("seconds", (object) (int) PubgNetProbeConsts.Timeout.TotalSeconds)));
    }
    this._expired.Clear();
  }

  public void StartLocal(int kb)
  {
    int num = this._nextId++;
    this._local[num] = new PubgNetProbeSystem.PendingProbe(kb, this._timing.CurTime + PubgNetProbeConsts.Timeout);
    this.RaiseNetworkEvent((EntityEventArgs) new PubgNetProbeUploadEvent(num, kb, PubgNetProbeSystem.BuildPayload(kb)));
  }

  private void OnRequest(PubgNetProbeRequestEvent ev)
  {
    if (!PubgNetProbeConsts.IsValidKb(ev.Kb))
      return;
    this.RaiseNetworkEvent((EntityEventArgs) new PubgNetProbeUploadEvent(ev.RequestId, ev.Kb, PubgNetProbeSystem.BuildPayload(ev.Kb)));
  }

  private void OnAck(PubgNetProbeAckEvent ev)
  {
    if (!this._local.Remove(ev.RequestId, out PubgNetProbeSystem.PendingProbe _))
      return;
    string str = this.Loc.GetString(ev.LocKey, new (string, object)[4]
    {
      ("kb", (object) ev.Kb),
      ("bytes", (object) ev.ReceivedBytes),
      ("expectedBytes", (object) ev.ExpectedBytes),
      ("limitKb", (object) 3072 /*0x0C00*/)
    });
    if (ev.Status == PubgNetProbeAckStatus.Ok)
      this._console.WriteLine((ICommonSession) null, str);
    else
      this._console.WriteError((ICommonSession) null, str);
  }

  private static byte[] BuildPayload(int kb)
  {
    byte[] numArray = new byte[PubgNetProbeConsts.ToBytes(kb)];
    for (int index = 0; index < numArray.Length; ++index)
      numArray[index] = (byte) (index % 251);
    return numArray;
  }

  private sealed record PendingProbe(int Kb, TimeSpan Deadline);
}
