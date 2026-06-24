using System;
using Content.Shared._RMC14.Dialog;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Megaphone;

[Serializable]
[NetSerializable]
public sealed record MegaphoneInputEvent(NetEntity Actor, string Message = "") : DialogInputEvent(Message);
