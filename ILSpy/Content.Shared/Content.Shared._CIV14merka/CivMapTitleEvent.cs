using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka;

[Serializable]
[NetSerializable]
public sealed class CivMapTitleEvent : EntityEventArgs
{
	public string Title { get; }

	public CivMapTitleEvent(string title)
	{
		Title = title;
	}
}
