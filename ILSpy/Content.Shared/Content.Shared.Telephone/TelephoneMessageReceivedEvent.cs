using Content.Shared.Chat;
using Robust.Shared.GameObjects;

namespace Content.Shared.Telephone;

[ByRefEvent]
public readonly record struct TelephoneMessageReceivedEvent(string Message, MsgChatMessage ChatMsg, EntityUid MessageSource, Entity<TelephoneComponent> TelephoneSource);
