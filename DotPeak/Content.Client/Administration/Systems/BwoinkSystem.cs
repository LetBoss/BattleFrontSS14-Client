// Decompiled with JetBrains decompiler
// Type: Content.Client.Administration.Systems.BwoinkSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Administration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Client.Administration.Systems;

public sealed class BwoinkSystem : SharedBwoinkSystem
{
  [Dependency]
  private IGameTiming _timing;
  private (TimeSpan Timestamp, bool Typing) _lastTypingUpdateSent;

  public event EventHandler<SharedBwoinkSystem.BwoinkTextMessage>? OnBwoinkTextMessageRecieved;

  protected override void OnBwoinkTextMessage(
    SharedBwoinkSystem.BwoinkTextMessage message,
    EntitySessionEventArgs eventArgs)
  {
    EventHandler<SharedBwoinkSystem.BwoinkTextMessage> textMessageRecieved = this.OnBwoinkTextMessageRecieved;
    if (textMessageRecieved == null)
      return;
    textMessageRecieved((object) this, message);
  }

  public void Send(NetUserId channelId, string text, bool playSound, bool adminOnly)
  {
    NetUserId userId = channelId;
    NetUserId trueSender = channelId;
    string text1 = text;
    bool flag1 = playSound;
    bool flag2 = adminOnly;
    DateTime? sentAt = new DateTime?();
    int num1 = flag1 ? 1 : 0;
    int num2 = flag2 ? 1 : 0;
    this.RaiseNetworkEvent((EntityEventArgs) new SharedBwoinkSystem.BwoinkTextMessage(userId, trueSender, text1, sentAt, num1 != 0, num2 != 0));
    this.SendInputTextUpdated(channelId, false);
  }

  public void SendInputTextUpdated(NetUserId channel, bool typing)
  {
    if (this._lastTypingUpdateSent.Typing == typing && this._lastTypingUpdateSent.Timestamp + TimeSpan.FromSeconds(1L) > this._timing.RealTime)
      return;
    this._lastTypingUpdateSent = (this._timing.RealTime, typing);
    this.RaiseNetworkEvent((EntityEventArgs) new BwoinkClientTypingUpdated(channel, typing));
  }
}
