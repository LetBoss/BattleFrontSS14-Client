// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.PrototypeReloadSystem
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Robust.Shared.GameObjects;

internal sealed class PrototypeReloadSystem : EntitySystem
{
  [Dependency]
  private readonly IPrototypeManager _prototypes;
  [Dependency]
  private readonly IComponentFactory _componentFactory;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<PrototypesReloadedEventArgs>(new EntityEventHandler<PrototypesReloadedEventArgs>(this.OnPrototypesReloaded));
  }

  private void OnPrototypesReloaded(PrototypesReloadedEventArgs eventArgs)
  {
    PrototypesReloadedEventArgs.PrototypeChangeSet prototypeChangeSet;
    if (!eventArgs.ByType.TryGetValue(typeof (EntityPrototype), out prototypeChangeSet))
      return;
    AllEntityQueryEnumerator<MetaDataComponent> entityQueryEnumerator = this.AllEntityQuery<MetaDataComponent>();
    EntityUid uid;
    MetaDataComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      string id = comp1.EntityPrototype?.ID;
      if (id != null && prototypeChangeSet.Modified.ContainsKey(id))
      {
        EntityPrototype newPrototype = this._prototypes.Index<EntityPrototype>(id);
        this.UpdateEntity(uid, comp1, newPrototype);
      }
    }
  }

  private void UpdateEntity(
    EntityUid entity,
    MetaDataComponent metaData,
    EntityPrototype newPrototype)
  {
    EntityPrototype entityPrototype = metaData.EntityPrototype;
    List<(string, Type)> valueTupleList = (entityPrototype != null ? entityPrototype.Components.Keys.Where<string>((Func<string, bool>) (n => n != "Transform" && n != "MetaData")).Select<string, (string, Type)>((Func<string, (string, Type)>) (name => (name, this._componentFactory.GetRegistration(name).Type))).ToList<(string, Type)>() : (List<(string, Type)>) null) ?? new List<(string, Type)>();
    List<(string, Type)> list = newPrototype.Components.Keys.Where<string>((Func<string, bool>) (n => n != "Transform" && n != "MetaData")).Select<string, (string, Type)>((Func<string, (string, Type)>) (name => (name, this._componentFactory.GetRegistration(name).Type))).ToList<(string, Type)>();
    List<string> ignoredComponents = new List<string>();
    foreach ((string key, Type type) in valueTupleList.Except<(string, Type)>((IEnumerable<(string, Type)>) list))
    {
      if (newPrototype.Components.ContainsKey(key))
        ignoredComponents.Add(key);
      else
        this.RemComp(entity, type);
    }
    this.EntityManager.CullRemovedComponents();
    foreach ((string str, Type _) in list.Where<(string, Type)>((Func<(string, Type), bool>) (t => !ignoredComponents.Contains(t.name))).Except<(string, Type)>((IEnumerable<(string, Type)>) valueTupleList))
    {
      EntityPrototype.ComponentRegistryEntry component1 = newPrototype.Components[str];
      IComponent component2 = this._componentFactory.GetComponent(str);
      if (!this.HasComp(entity, component2.GetType()))
        this.AddComp<IComponent>(entity, component2);
    }
    metaData.EntityPrototype = newPrototype;
  }
}
