using System;
using System.Collections.Generic;

namespace Robust.Shared.Prototypes;

public sealed class PrototypeInheritanceTree
{
	private Dictionary<string, HashSet<string>> _nodes = new Dictionary<string, HashSet<string>>();

	private Dictionary<string, HashSet<string>> _pendingParent = new Dictionary<string, HashSet<string>>();

	private HashSet<string> _baseNodes = new HashSet<string>();

	private Dictionary<string, string> _parents = new Dictionary<string, string>();

	public IReadOnlySet<string> BaseNodes => _baseNodes;

	public IReadOnlySet<string> Children(string id)
	{
		if (!_nodes.ContainsKey(id))
		{
			throw new ArgumentException("ID " + id + " not present in InheritanceTree", "id");
		}
		return _nodes[id];
	}

	public string GetBaseNode(string id)
	{
		if (!_nodes.ContainsKey(id))
		{
			throw new ArgumentException("ID " + id + " not present in InheritanceTree", "id");
		}
		string text = id;
		string value;
		while (_parents.TryGetValue(text, out value))
		{
			text = value;
		}
		return text;
	}

	public string? GetParent(string id)
	{
		return _parents.GetValueOrDefault(id);
	}

	public void AddId(string id, string? parent, bool overwrite = false)
	{
		if (overwrite && HasId(id))
		{
			RemoveId(id);
		}
		if (_nodes.ContainsKey(id))
		{
			throw new ArgumentException("ID " + id + " already present in InheritanceTree", "id");
		}
		if (parent != null)
		{
			_parents.Add(id, parent);
			if (_nodes.TryGetValue(parent, out HashSet<string> value))
			{
				value.Add(id);
			}
			else
			{
				if (!_pendingParent.TryGetValue(parent, out HashSet<string> _))
				{
					_pendingParent[parent] = new HashSet<string>();
				}
				_pendingParent[parent].Add(id);
			}
			string value3 = parent;
			while (value3 != null)
			{
				if (value3 == id)
				{
					throw new InvalidOperationException("Cycle detected when trying to add id " + id + " with parent " + parent);
				}
				_parents.TryGetValue(value3, out value3);
			}
		}
		else
		{
			_baseNodes.Add(id);
		}
		if (!_pendingParent.TryGetValue(id, out HashSet<string> value4))
		{
			value4 = new HashSet<string>();
		}
		_nodes.Add(id, value4);
	}

	public bool HasId(string id)
	{
		return _nodes.ContainsKey(id);
	}

	public void RemoveId(string id)
	{
		if (!_nodes.ContainsKey(id))
		{
			throw new ArgumentException("ID " + id + " not present in InheritanceTree", "id");
		}
		_nodes.Remove(id);
		foreach (KeyValuePair<string, HashSet<string>> item in _pendingParent)
		{
			item.Deconstruct(out var _, out var value);
			value.Remove(id);
		}
		_baseNodes.Remove(id);
		_parents.Remove(id);
	}
}
