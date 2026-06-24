using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Content.Client.Guidebook.Richtext;
using Content.Shared.Kitchen;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;

namespace Content.Client.Guidebook.Controls;

public sealed class GuideMicrowaveGroupEmbed : BoxContainer, IDocumentTag
{
	[Dependency]
	private ILogManager _logManager;

	[Dependency]
	private IPrototypeManager _prototype;

	private readonly ISawmill _sawmill;

	public GuideMicrowaveGroupEmbed()
	{
		((BoxContainer)this).Orientation = (LayoutOrientation)1;
		IoCManager.InjectDependencies<GuideMicrowaveGroupEmbed>(this);
		_sawmill = _logManager.GetSawmill("guidebook.microwave_group");
		((Control)this).MouseFilter = (MouseFilterMode)0;
	}

	public GuideMicrowaveGroupEmbed(string group)
		: this()
	{
		CreateEntries(group);
	}

	public bool TryParseTag(Dictionary<string, string> args, [NotNullWhen(true)] out Control? control)
	{
		control = null;
		if (!args.TryGetValue("Group", out string value))
		{
			_sawmill.Error("Microwave group embed tag is missing group argument");
			return false;
		}
		CreateEntries(value);
		control = (Control?)(object)this;
		return true;
	}

	private void CreateEntries(string group)
	{
		foreach (FoodRecipePrototype item in from p in _prototype.EnumeratePrototypes<FoodRecipePrototype>()
			where p.Group.Equals(@group)
			orderby p.Name
			select p)
		{
			GuideMicrowaveEmbed guideMicrowaveEmbed = new GuideMicrowaveEmbed(item);
			((Control)this).AddChild((Control)(object)guideMicrowaveEmbed);
		}
	}
}
