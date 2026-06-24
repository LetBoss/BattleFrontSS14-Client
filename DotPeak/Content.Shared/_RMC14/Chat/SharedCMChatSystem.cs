// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Chat.SharedCMChatSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Marines;
using Content.Shared._RMC14.Marines.Squads;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Chat;
using Content.Shared.Radio;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;

#nullable enable
namespace Content.Shared._RMC14.Chat;

public abstract class SharedCMChatSystem : EntitySystem
{
  [Dependency]
  private IConfigurationManager _config;
  [Dependency]
  private SquadSystem _squadSystem;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<MarineComponent, ChatGetPrefixEvent>(new EntityEventRefHandler<MarineComponent, ChatGetPrefixEvent>(this.OnMarineGetPrefix));
    this.SubscribeLocalEvent<XenoComponent, ChatGetPrefixEvent>(new EntityEventRefHandler<XenoComponent, ChatGetPrefixEvent>(this.OnXenoGetPrefix));
  }

  private void OnMarineGetPrefix(Entity<MarineComponent> ent, ref ChatGetPrefixEvent args)
  {
    if (!(args.Channel?.ID == SharedChatSystem.HivemindChannel.Id))
      return;
    args.Channel = (RadioChannelPrototype) null;
  }

  private void OnXenoGetPrefix(Entity<XenoComponent> ent, ref ChatGetPrefixEvent args)
  {
    if (!(args.Channel?.ID != SharedChatSystem.HivemindChannel.Id))
      return;
    args.Channel = (RadioChannelPrototype) null;
  }

  public virtual string SanitizeMessageReplaceWords(EntityUid source, string msg) => msg;

  public virtual void ChatMessageToOne(
    ChatChannel channel,
    string message,
    string wrappedMessage,
    EntityUid source,
    bool hideChat,
    INetChannel client,
    Color? colorOverride = null,
    bool recordReplay = false,
    string? audioPath = null,
    float audioVolume = 0.0f,
    NetUserId? author = null)
  {
  }

  public void ChatMessageToOne(
    string message,
    EntityUid target,
    ChatChannel channel = ChatChannel.Local,
    bool hideChat = false,
    Color? colorOverride = null,
    bool recordReplay = false,
    string? audioPath = null,
    float audioVolume = 0.0f,
    NetUserId? author = null)
  {
    ActorComponent comp;
    if (!this.TryComp<ActorComponent>(target, out comp))
      return;
    this.ChatMessageToOne(channel, message, message, new EntityUid(), hideChat, comp.PlayerSession.Channel, colorOverride, recordReplay, audioPath, audioVolume, author);
  }

  public virtual void ChatMessageToMany(
    string message,
    string wrappedMessage,
    Filter filter,
    ChatChannel channel,
    EntityUid source = default (EntityUid),
    bool hideChat = false,
    Color? colorOverride = null,
    bool recordReplay = false,
    string? audioPath = null,
    float audioVolume = 0.0f,
    NetUserId? author = null)
  {
  }

  public virtual void Emote(
    EntityUid source,
    string message,
    string? nameOverride = null,
    bool checkRadioPrefix = true,
    bool ignoreActionBlocker = false)
  {
  }

  public string? ColorizeSpeakerNameBySquadOrNull(ChatMessage msg)
  {
    int num = this._config.GetCVar<bool>(RMCCVars.RMCChatSquadColorMode) ? 1 : 0;
    Color? nullable = new Color?();
    Color color1;
    if (num != 0 && this._squadSystem.TryGetSquadMemberColor(this.GetEntity(msg.SenderEntity), out color1, true))
      nullable = new Color?(color1);
    if (!nullable.HasValue)
      return (string) null;
    ChatMessage chatMessage = msg;
    ChatMessage message = msg;
    Color color2 = nullable.Value;
    string hex = ((Color) ref color2).ToHex();
    string str = SharedChatSystem.InjectTagInsideTag(message, "Name", "color", hex);
    chatMessage.WrappedMessage = str;
    return msg.WrappedMessage;
  }
}
