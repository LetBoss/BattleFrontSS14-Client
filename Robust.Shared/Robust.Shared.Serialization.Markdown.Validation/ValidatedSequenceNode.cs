using System.Collections.Generic;
using System.Linq;

namespace Robust.Shared.Serialization.Markdown.Validation;

public sealed class ValidatedSequenceNode : ValidationNode
{
	public List<ValidationNode> Sequence { get; }

	public override bool Valid => Sequence.All((ValidationNode p) => p.Valid);

	public ValidatedSequenceNode(List<ValidationNode> sequence)
	{
		Sequence = sequence;
	}

	public override IEnumerable<ErrorNode> GetErrors()
	{
		foreach (ValidationNode item in Sequence)
		{
			foreach (ErrorNode error in item.GetErrors())
			{
				yield return error;
			}
		}
	}
}
