// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.BuiPredictionState
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Timing;
using Robust.Shared.GameObjects;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Client.UserInterface;

public sealed class BuiPredictionState
{
  private readonly BoundUserInterface _parent;
  private readonly IClientGameTiming _gameTiming;
  private readonly Queue<BuiPredictionState.MessageData> _queuedMessages = new Queue<BuiPredictionState.MessageData>();

  public BuiPredictionState(BoundUserInterface parent, IClientGameTiming gameTiming)
  {
    this._parent = parent;
    this._gameTiming = gameTiming;
  }

  public void SendMessage(BoundUserInterfaceMessage message)
  {
    if (((IGameTiming) this._gameTiming).IsFirstTimePredicted)
      this._queuedMessages.Enqueue(new BuiPredictionState.MessageData()
      {
        TickSent = ((IGameTiming) this._gameTiming).CurTick,
        Message = message
      });
    this._parent.SendPredictedMessage(message);
  }

  public IEnumerable<BoundUserInterfaceMessage> MessagesToReplay()
  {
    GameTick lastRealTick = this._gameTiming.LastRealTick;
    BuiPredictionState.MessageData result;
    while (this._queuedMessages.TryPeek(out result) && GameTick.op_LessThanOrEqual(result.TickSent, lastRealTick))
      this._queuedMessages.Dequeue();
    return this._queuedMessages.Count == 0 ? (IEnumerable<BoundUserInterfaceMessage>) Array.Empty<BoundUserInterfaceMessage>() : this._queuedMessages.Select<BuiPredictionState.MessageData, BoundUserInterfaceMessage>((Func<BuiPredictionState.MessageData, BoundUserInterfaceMessage>) (c => c.Message));
  }

  private struct MessageData
  {
    public GameTick TickSent;
    public required BoundUserInterfaceMessage Message;

    public override string ToString() => $"{this.Message} @ {this.TickSent}";
  }
}
