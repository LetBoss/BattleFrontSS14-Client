using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Preferences;

[Serializable]
[NetSerializable]
public sealed class GameSettings
{
	private int _maxCharacterSlots;

	public int MaxCharacterSlots
	{
		get
		{
			return _maxCharacterSlots;
		}
		set
		{
			_maxCharacterSlots = value;
		}
	}
}
