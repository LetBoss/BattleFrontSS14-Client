using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Security.Components;

[Serializable]
[NetSerializable]
public sealed class GenpopLockerIdConfiguredMessage : BoundUserInterfaceMessage
{
	public string Name;

	public float Sentence;

	public string Crime;

	public GenpopLockerIdConfiguredMessage(string name, float sentence, string crime)
	{
		Name = name;
		Sentence = sentence;
		Crime = crime;
	}
}
