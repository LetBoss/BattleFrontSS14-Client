using System;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;

namespace Content.Client._RMC14.Dropship.Weapon;

public readonly record struct DropshipWeaponsButtonData(LocId Text, Action<ButtonEventArgs> OnPressed, NetEntity? Weapon = null);
