using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.Serialization;

namespace Robust.Shared.ViewVariables;

[Serializable]
[NetSerializable]
public sealed class ViewVariablesBlobMembers : ViewVariablesBlob
{
	[Serializable]
	[NetSerializable]
	[Virtual]
	public class ReferenceToken
	{
		public string Stringified { get; set; }

		public override string ToString()
		{
			return Stringified;
		}
	}

	[Serializable]
	[NetSerializable]
	public sealed class PrototypeReferenceToken : ReferenceToken
	{
		public string ID { get; set; }

		public string Variant { get; set; }

		public override string ToString()
		{
			return $"{base.Stringified} Prototype: {Variant} ID: {ID}";
		}
	}

	[Serializable]
	[NetSerializable]
	public sealed class ServerValueTypeToken
	{
		public string Stringified { get; set; }

		public override string ToString()
		{
			return Stringified;
		}
	}

	[Serializable]
	[NetSerializable]
	public sealed class ServerKeyValuePairToken
	{
		public object Key { get; set; }

		public object Value { get; set; }
	}

	[Serializable]
	[NetSerializable]
	public sealed class ServerTupleToken : ITuple
	{
		public object[] Items { get; set; }

		public object this[int index] => Items[index];

		public int Length => Items.Length;
	}

	[Serializable]
	[NetSerializable]
	public sealed class MemberData
	{
		public bool Editable { get; set; }

		public string Type { get; set; }

		public string TypePretty { get; set; }

		public string Name { get; set; }

		public int PropertyIndex { get; set; }

		public object Value { get; set; }
	}

	public List<(string groupName, List<MemberData> groupMembers)> MemberGroups { get; set; } = new List<(string, List<MemberData>)>();
}
