using System;
using Content.Shared._RMC14.Dialog;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Marines.ControlComputer;

[Serializable]
[NetSerializable]
public sealed record MarineControlComputerShipAnnouncementDialogEvent(NetEntity User, string Message = "") : DialogInputEvent(Message);
