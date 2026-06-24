// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chat.ChatMessage
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Chat;

[NetSerializable]
[Serializable]
public sealed class ChatMessage
{
  public ChatChannel Channel;
  public string Message;
  public string WrappedMessage;
  public NetEntity SenderEntity;
  public int? SenderKey;
  public bool HideChat;
  public Color? MessageColorOverride;
  public string? AudioPath;
  public float AudioVolume;
  public bool HidePopup;
  public string? SpeechStyleClass;
  public bool RepeatCheckSender;
  [NonSerialized]
  public bool Read;

  public ChatMessage(
    ChatChannel channel,
    string message,
    string wrappedMessage,
    NetEntity source,
    int? senderKey,
    bool hideChat = false,
    Color? colorOverride = null,
    string? audioPath = null,
    float audioVolume = 0.0f,
    bool hidePopup = false,
    string? speechStyleClass = null,
    bool repeatCheckSender = true)
  {
    this.Channel = channel;
    this.Message = message;
    this.WrappedMessage = wrappedMessage;
    this.SenderEntity = source;
    this.SenderKey = senderKey;
    this.HideChat = hideChat;
    this.MessageColorOverride = colorOverride;
    this.AudioPath = audioPath;
    this.AudioVolume = audioVolume;
    this.HidePopup = hidePopup;
    this.SpeechStyleClass = speechStyleClass;
    this.RepeatCheckSender = repeatCheckSender;
  }
}
