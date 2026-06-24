// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Prototypes.PrototypeInheritanceTree
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Prototypes;

public sealed class PrototypeInheritanceTree
{
  private Dictionary<string, HashSet<string>> _nodes = new Dictionary<string, HashSet<string>>();
  private Dictionary<string, HashSet<string>> _pendingParent = new Dictionary<string, HashSet<string>>();
  private HashSet<string> _baseNodes = new HashSet<string>();
  private Dictionary<string, string> _parents = new Dictionary<string, string>();

  public IReadOnlySet<string> BaseNodes => (IReadOnlySet<string>) this._baseNodes;

  public IReadOnlySet<string> Children(string id)
  {
    return this._nodes.ContainsKey(id) ? (IReadOnlySet<string>) this._nodes[id] : throw new ArgumentException($"ID {id} not present in InheritanceTree", nameof (id));
  }

  public string GetBaseNode(string id)
  {
    string key = this._nodes.ContainsKey(id) ? id : throw new ArgumentException($"ID {id} not present in InheritanceTree", nameof (id));
    string str;
    while (this._parents.TryGetValue(key, out str))
      key = str;
    return key;
  }

  public string? GetParent(string id) => this._parents.GetValueOrDefault<string, string>(id);

  public void AddId(string id, string? parent, bool overwrite = false)
  {
    if (overwrite && this.HasId(id))
      this.RemoveId(id);
    if (this._nodes.ContainsKey(id))
      throw new ArgumentException($"ID {id} already present in InheritanceTree", nameof (id));
    if (parent != null)
    {
      this._parents.Add(id, parent);
      HashSet<string> stringSet;
      if (this._nodes.TryGetValue(parent, out stringSet))
      {
        stringSet.Add(id);
      }
      else
      {
        if (!this._pendingParent.TryGetValue(parent, out HashSet<string> _))
          this._pendingParent[parent] = new HashSet<string>();
        this._pendingParent[parent].Add(id);
      }
      for (string key = parent; key != null; this._parents.TryGetValue(key, out key))
      {
        if (key == id)
          throw new InvalidOperationException($"Cycle detected when trying to add id {id} with parent {parent}");
      }
    }
    else
      this._baseNodes.Add(id);
    HashSet<string> stringSet1;
    if (!this._pendingParent.TryGetValue(id, out stringSet1))
      stringSet1 = new HashSet<string>();
    this._nodes.Add(id, stringSet1);
  }

  public bool HasId(string id) => this._nodes.ContainsKey(id);

  public void RemoveId(string id)
  {
    if (!this._nodes.ContainsKey(id))
      throw new ArgumentException($"ID {id} not present in InheritanceTree", nameof (id));
    this._nodes.Remove(id);
    foreach ((string _, HashSet<string> stringSet) in this._pendingParent)
      stringSet.Remove(id);
    this._baseNodes.Remove(id);
    this._parents.Remove(id);
  }
}
