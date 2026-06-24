using System.Collections.Generic;
using System.Linq;

namespace Robust.Shared.Serialization.Markdown.Validation;

public sealed class ValidatedMappingNode : ValidationNode
{
	public Dictionary<ValidationNode, ValidationNode> Mapping { get; }

	public override bool Valid => Mapping.All<KeyValuePair<ValidationNode, ValidationNode>>((KeyValuePair<ValidationNode, ValidationNode> p) => p.Key.Valid && p.Value.Valid);

	public ValidatedMappingNode(Dictionary<ValidationNode, ValidationNode> mapping)
	{
		Mapping = mapping;
	}

	public override IEnumerable<ErrorNode> GetErrors()
	{
		IEnumerable<KeyValuePair<ValidationNode, ValidationNode>> enumerable = Mapping.Where<KeyValuePair<ValidationNode, ValidationNode>>((KeyValuePair<ValidationNode, ValidationNode> p) => !p.Key.Valid || !p.Value.Valid);
		foreach (var (validationNode3, value) in enumerable)
		{
			foreach (ErrorNode error in validationNode3.GetErrors())
			{
				yield return error;
			}
			foreach (ErrorNode error2 in value.GetErrors())
			{
				yield return error2;
			}
		}
	}
}
