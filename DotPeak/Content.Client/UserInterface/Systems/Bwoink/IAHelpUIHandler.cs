// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.Bwoink.IAHelpUIHandler
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Administration;
using Robust.Shared.Network;
using System;

#nullable enable
namespace Content.Client.UserInterface.Systems.Bwoink;

public interface IAHelpUIHandler : IDisposable
{
  bool IsAdmin { get; }

  bool IsOpen { get; }

  void Receive(SharedBwoinkSystem.BwoinkTextMessage message);

  void Close();

  void Open(NetUserId netUserId, bool relayActive);

  void ToggleWindow();

  void DiscordRelayChanged(bool active);

  void PeopleTypingUpdated(BwoinkPlayerTypingUpdated args);

  event Action OnClose;

  event Action OnOpen;

  Action<NetUserId, string, bool, bool>? SendMessageAction { get; set; }

  event Action<NetUserId, string>? InputTextChanged;
}
