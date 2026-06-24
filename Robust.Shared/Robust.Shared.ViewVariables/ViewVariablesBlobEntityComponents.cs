using System;
using System.Collections.Generic;
using Robust.Shared.Serialization;

namespace Robust.Shared.ViewVariables;

[Serializable]
[NetSerializable]
public sealed class ViewVariablesBlobEntityComponents : ViewVariablesBlob
{
	[Serializable]
	[NetSerializable]
	public sealed class Entry : IComparable<Entry>
	{
		public string FullName { get; set; }

		public string Stringified { get; set; }

		public string ComponentName { get; set; }

		public int CompareTo(Entry other)
		{
			if (this == other)
			{
				return 0;
			}
			if (other == null)
			{
				return 1;
			}
			return string.Compare(Stringified, other.Stringified, StringComparison.Ordinal);
		}
	}

	public List<Entry> ComponentTypes { get; set; } = new List<Entry>();
}
