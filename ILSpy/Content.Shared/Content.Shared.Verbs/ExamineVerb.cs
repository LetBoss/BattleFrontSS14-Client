using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Verbs;

[Serializable]
[NetSerializable]
public sealed class ExamineVerb : Verb
{
	public bool ShowOnExamineTooltip = true;

	public bool HoverVerb;

	public override int TypePriority => 0;

	public override bool CloseMenuDefault => false;
}
