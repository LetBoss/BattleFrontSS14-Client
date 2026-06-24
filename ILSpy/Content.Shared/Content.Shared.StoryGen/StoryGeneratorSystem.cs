using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Content.Shared.Dataset;
using Robust.Shared.Collections;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared.StoryGen;

public sealed class StoryGeneratorSystem : EntitySystem
{
	[Dependency]
	private IPrototypeManager _protoMan;

	[Dependency]
	private IRobustRandom _random;

	public bool TryGenerateStoryFromTemplate(ProtoId<StoryTemplatePrototype> template, [NotNullWhen(true)] out string? story, int? seed = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		StoryTemplatePrototype templateProto = default(StoryTemplatePrototype);
		if (!_protoMan.TryIndex<StoryTemplatePrototype>(template, ref templateProto))
		{
			story = null;
			return false;
		}
		if (seed.HasValue)
		{
			_random.SetSeed(seed.Value);
		}
		ValueList<(string, object)> variables = default(ValueList<(string, object)>);
		variables._002Ector(templateProto.Variables.Count);
		LocalizedDatasetPrototype listProto = default(LocalizedDatasetPrototype);
		foreach (var (name, list) in templateProto.Variables)
		{
			if (_protoMan.TryIndex<LocalizedDatasetPrototype>(list, ref listProto))
			{
				string chosenWord = base.Loc.GetString(RandomExtensions.Pick<string>(_random, (IReadOnlyList<string>)listProto.Values));
				variables.Add((name, (object)chosenWord));
			}
		}
		story = base.Loc.GetString(LocId.op_Implicit(templateProto.LocId), variables.ToArray());
		return true;
	}
}
