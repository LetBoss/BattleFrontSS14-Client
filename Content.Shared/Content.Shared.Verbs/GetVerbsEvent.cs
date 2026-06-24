using System.Collections.Generic;
using Content.Shared.Hands.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Verbs;

public sealed class GetVerbsEvent<TVerb> : EntityEventArgs where TVerb : Verb
{
	public readonly SortedSet<TVerb> Verbs = new SortedSet<TVerb>();

	public readonly List<VerbCategory> ExtraCategories;

	public readonly bool CanAccess;

	public readonly EntityUid Target;

	public readonly EntityUid User;

	public readonly bool CanInteract;

	public readonly bool CanComplexInteract;

	public readonly HandsComponent? Hands;

	public readonly EntityUid? Using;

	public GetVerbsEvent(EntityUid user, EntityUid target, EntityUid? @using, HandsComponent? hands, bool canInteract, bool canComplexInteract, bool canAccess, List<VerbCategory> extraCategories)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		User = user;
		Target = target;
		Using = @using;
		Hands = hands;
		CanAccess = canAccess;
		CanComplexInteract = canComplexInteract;
		CanInteract = canInteract;
		ExtraCategories = extraCategories;
	}
}
