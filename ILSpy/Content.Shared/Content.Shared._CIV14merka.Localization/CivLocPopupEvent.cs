using System;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Localization;

[Serializable]
[NetSerializable]
public sealed class CivLocPopupEvent : EntityEventArgs
{
	public CivLocMessage Message;

	public NetEntity? At;

	public PopupType Type;
}
