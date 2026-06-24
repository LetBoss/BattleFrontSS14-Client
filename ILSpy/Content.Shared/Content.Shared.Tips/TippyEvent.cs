using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Tips;

[Serializable]
[NetSerializable]
public sealed class TippyEvent : EntityEventArgs
{
	public string Msg;

	public string? Proto;

	public float SpeakTime = 5f;

	public float SlideTime = 3f;

	public float WaddleInterval = 0.5f;

	public TippyEvent(string msg)
	{
		Msg = msg;
	}
}
