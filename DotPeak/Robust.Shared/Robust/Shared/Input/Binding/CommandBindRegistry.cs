// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Input.Binding.CommandBindRegistry
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Log;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Robust.Shared.Input.Binding;

public sealed class CommandBindRegistry : ICommandBindRegistry
{
  private readonly ISawmill _sawmill;
  private List<CommandBindRegistry.TypedCommandBind> _bindings = new List<CommandBindRegistry.TypedCommandBind>();
  private Dictionary<BoundKeyFunction, List<InputCmdHandler>> _bindingsForKey = new Dictionary<BoundKeyFunction, List<InputCmdHandler>>();
  private bool _graphDirty;

  public CommandBindRegistry(ISawmill sawmill) => this._sawmill = sawmill;

  public void Register<TOwner>(CommandBinds commandBinds)
  {
    this.Register(commandBinds, typeof (TOwner));
  }

  public void Register(CommandBinds commandBinds, Type owner)
  {
    if (this._bindings.Any<CommandBindRegistry.TypedCommandBind>((Func<CommandBindRegistry.TypedCommandBind, bool>) (existing => existing.ForType == owner)))
      this._sawmill.Warning("Command binds already registered for type {0}, but you are trying to register more. This may be a programming error. Did you register these under the wrong type, or did you forget to unregister these bindings when your system / manager is shutdown?", (object) owner.Name);
    foreach (CommandBind binding in commandBinds.Bindings)
      this._bindings.Add(new CommandBindRegistry.TypedCommandBind(owner, binding));
    this._graphDirty = true;
  }

  public IEnumerable<InputCmdHandler> GetHandlers(BoundKeyFunction function)
  {
    if (this._graphDirty)
      this.RebuildGraph();
    List<InputCmdHandler> inputCmdHandlerList;
    return this._bindingsForKey.TryGetValue(function, out inputCmdHandlerList) ? (IEnumerable<InputCmdHandler>) inputCmdHandlerList : Enumerable.Empty<InputCmdHandler>();
  }

  public void Unregister(Type owner)
  {
    this._bindings.RemoveAll((Predicate<CommandBindRegistry.TypedCommandBind>) (binding => binding.ForType == owner));
    this._graphDirty = true;
  }

  public void Unregister<TOwner>() => this.Unregister(typeof (TOwner));

  internal void RebuildGraph()
  {
    this._bindingsForKey.Clear();
    foreach (KeyValuePair<BoundKeyFunction, List<CommandBindRegistry.TypedCommandBind>> binding in this.FunctionToBindings())
      this._bindingsForKey[binding.Key] = this.ResolveDependencies(binding.Key, binding.Value);
    this._graphDirty = false;
  }

  private Dictionary<BoundKeyFunction, List<CommandBindRegistry.TypedCommandBind>> FunctionToBindings()
  {
    Dictionary<BoundKeyFunction, List<CommandBindRegistry.TypedCommandBind>> bindings = new Dictionary<BoundKeyFunction, List<CommandBindRegistry.TypedCommandBind>>();
    foreach (CommandBindRegistry.TypedCommandBind binding in this._bindings)
    {
      if (!bindings.ContainsKey(binding.CommandBind.BoundKeyFunction))
        bindings[binding.CommandBind.BoundKeyFunction] = new List<CommandBindRegistry.TypedCommandBind>();
      bindings[binding.CommandBind.BoundKeyFunction].Add(binding);
    }
    return bindings;
  }

  private List<InputCmdHandler> ResolveDependencies(
    BoundKeyFunction function,
    List<CommandBindRegistry.TypedCommandBind> bindingsForFunction)
  {
    List<TopologicalSort.GraphNode<CommandBindRegistry.TypedCommandBind>> nodes = new List<TopologicalSort.GraphNode<CommandBindRegistry.TypedCommandBind>>();
    Dictionary<Type, List<TopologicalSort.GraphNode<CommandBindRegistry.TypedCommandBind>>> dictionary = new Dictionary<Type, List<TopologicalSort.GraphNode<CommandBindRegistry.TypedCommandBind>>>();
    foreach (CommandBindRegistry.TypedCommandBind typedCommandBind in bindingsForFunction)
    {
      if (!dictionary.ContainsKey(typedCommandBind.ForType))
        dictionary[typedCommandBind.ForType] = new List<TopologicalSort.GraphNode<CommandBindRegistry.TypedCommandBind>>();
      TopologicalSort.GraphNode<CommandBindRegistry.TypedCommandBind> graphNode = new TopologicalSort.GraphNode<CommandBindRegistry.TypedCommandBind>(typedCommandBind);
      dictionary[typedCommandBind.ForType].Add(graphNode);
      nodes.Add(graphNode);
    }
    foreach (TopologicalSort.GraphNode<CommandBindRegistry.TypedCommandBind> graphNode1 in nodes)
    {
      foreach (Type key in graphNode1.Value.CommandBind.After)
      {
        List<TopologicalSort.GraphNode<CommandBindRegistry.TypedCommandBind>> graphNodeList;
        if (dictionary.TryGetValue(key, out graphNodeList))
        {
          foreach (TopologicalSort.GraphNode<CommandBindRegistry.TypedCommandBind> graphNode2 in graphNodeList)
            graphNode2.Dependant.Add(graphNode1);
        }
      }
      foreach (Type key in graphNode1.Value.CommandBind.Before)
      {
        List<TopologicalSort.GraphNode<CommandBindRegistry.TypedCommandBind>> graphNodeList;
        if (dictionary.TryGetValue(key, out graphNodeList))
        {
          foreach (TopologicalSort.GraphNode<CommandBindRegistry.TypedCommandBind> graphNode3 in graphNodeList)
            graphNode1.Dependant.Add(graphNode3);
        }
      }
    }
    return TopologicalSort.Sort<CommandBindRegistry.TypedCommandBind>((IEnumerable<TopologicalSort.GraphNode<CommandBindRegistry.TypedCommandBind>>) nodes).Select<CommandBindRegistry.TypedCommandBind, InputCmdHandler>((Func<CommandBindRegistry.TypedCommandBind, InputCmdHandler>) (c => c.CommandBind.Handler)).ToList<InputCmdHandler>();
  }

  private sealed class TypedCommandBind
  {
    public readonly Type ForType;
    public readonly CommandBind CommandBind;

    public TypedCommandBind(Type forType, CommandBind commandBind)
    {
      this.ForType = forType;
      this.CommandBind = commandBind;
    }
  }
}
