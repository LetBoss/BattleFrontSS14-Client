using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Admin;
using Content.Shared.Database;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared.Verbs;

[Serializable]
[NetSerializable]
[Virtual]
public class Verb : IComparable
{
	public static string DefaultTextStyleClass = "Verb";

	public string TextStyleClass = DefaultTextStyleClass;

	[NonSerialized]
	public Action? Act;

	[NonSerialized]
	public object? ExecutionEventArgs;

	[NonSerialized]
	public EntityUid EventTarget = EntityUid.Invalid;

	[NonSerialized]
	public bool ClientExclusive;

	public string Text = string.Empty;

	public SpriteSpecifier? Icon;

	public VerbCategory? Category;

	public bool Disabled;

	public string? Message;

	public int Priority;

	public NetEntity? IconEntity;

	public bool? CloseMenu;

	public LogImpact Impact = LogImpact.Low;

	public bool ConfirmationPopup;

	public bool? DoContactInteraction;

	public static List<Type> VerbTypes = new List<Type>
	{
		typeof(Verb),
		typeof(VvVerb),
		typeof(InteractionVerb),
		typeof(UtilityVerb),
		typeof(InnateVerb),
		typeof(AlternativeVerb),
		typeof(ActivationVerb),
		typeof(ExamineVerb),
		typeof(EquipmentVerb),
		typeof(RMCAdminVerb)
	};

	public virtual int TypePriority => 0;

	public virtual bool CloseMenuDefault => true;

	public virtual bool DefaultDoContactInteraction => false;

	public int CompareTo(object? obj)
	{
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		if (!(obj is Verb otherVerb))
		{
			return -1;
		}
		if (TypePriority != otherVerb.TypePriority)
		{
			return otherVerb.TypePriority - TypePriority;
		}
		if (Priority != otherVerb.Priority)
		{
			return otherVerb.Priority - Priority;
		}
		if (Category?.Text != otherVerb.Category?.Text)
		{
			return string.Compare(Category?.Text, otherVerb.Category?.Text, StringComparison.CurrentCulture);
		}
		if (Text != otherVerb.Text)
		{
			return string.Compare(Text, otherVerb.Text, StringComparison.CurrentCulture);
		}
		NetEntity? iconEntity = IconEntity;
		NetEntity? iconEntity2 = otherVerb.IconEntity;
		if (iconEntity.HasValue != iconEntity2.HasValue || (iconEntity.HasValue && iconEntity.GetValueOrDefault() != iconEntity2.GetValueOrDefault()))
		{
			if (!IconEntity.HasValue)
			{
				return -1;
			}
			if (!otherVerb.IconEntity.HasValue)
			{
				return 1;
			}
			NetEntity value = IconEntity.Value;
			return ((NetEntity)(ref value)).CompareTo(otherVerb.IconEntity.Value);
		}
		return string.Compare(((object)Icon)?.ToString(), ((object)otherVerb.Icon)?.ToString(), StringComparison.CurrentCulture);
	}
}
