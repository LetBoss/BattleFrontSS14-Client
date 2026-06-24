using System;
using System.Collections.Frozen;

namespace Robust.Shared.GameObjects;

public sealed class ComponentRegistration
{
	public string[] NetworkedFields = Array.Empty<string>();

	public FrozenDictionary<string, int> NetworkedFieldLookup = FrozenDictionary<string, int>.Empty;

	public string Name { get; }

	public CompIdx Idx { get; }

	public bool Unsaved { get; }

	public ushort? NetID { get; internal set; }

	public Type Type { get; }

	internal ComponentRegistration(string name, Type type, CompIdx idx, bool unsaved = false)
	{
		Name = name;
		Type = type;
		Idx = idx;
		Unsaved = unsaved;
	}

	public override string ToString()
	{
		return $"ComponentRegistration({Name}: {Type})";
	}
}
