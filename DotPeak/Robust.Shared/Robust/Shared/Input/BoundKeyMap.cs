// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Input.BoundKeyMap
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;

#nullable enable
namespace Robust.Shared.Input;

public sealed class BoundKeyMap
{
  private readonly IReflectionManager reflectionManager;
  private readonly Dictionary<BoundKeyFunction, KeyFunctionId> KeyFunctionsMap = new Dictionary<BoundKeyFunction, KeyFunctionId>();
  private readonly List<BoundKeyFunction> KeyFunctionsList = new List<BoundKeyFunction>();

  public BoundKeyMap(IReflectionManager reflectionManager)
  {
    this.reflectionManager = reflectionManager;
  }

  internal IEnumerable<BoundKeyFunction> AllKeyFunctions
  {
    get => (IEnumerable<BoundKeyFunction>) this.KeyFunctionsList;
  }

  public void PopulateKeyFunctionsMap()
  {
    if (this.KeyFunctionsMap.Count != 0)
      throw new InvalidOperationException("Cannot run this method twice.");
    foreach (Type type in this.reflectionManager.FindTypesWithAttribute<KeyFunctionsAttribute>())
    {
      foreach (FieldInfo field in type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy))
      {
        if (!field.IsLiteral && field.IsInitOnly && !(field.FieldType != typeof (BoundKeyFunction)))
          this.KeyFunctionsList.Add((BoundKeyFunction) field.GetValue((object) null));
      }
    }
    this.KeyFunctionsList.Sort();
    for (int index = 0; index < this.KeyFunctionsList.Count; ++index)
      this.KeyFunctionsMap.Add(this.KeyFunctionsList[index], new KeyFunctionId(index));
  }

  public bool FunctionExists(string name)
  {
    return this.KeyFunctionsMap.ContainsKey(new BoundKeyFunction(name));
  }

  public KeyFunctionId KeyFunctionID(BoundKeyFunction function) => this.KeyFunctionsMap[function];

  public BoundKeyFunction KeyFunctionName(KeyFunctionId function)
  {
    return this.KeyFunctionsList[(int) function];
  }

  public bool TryGetKeyFunction(KeyFunctionId funcId, out BoundKeyFunction func)
  {
    List<BoundKeyFunction> keyFunctionsList = this.KeyFunctionsList;
    int index = (int) funcId;
    if (0 > index || index >= keyFunctionsList.Count)
    {
      func = new BoundKeyFunction();
      return false;
    }
    func = keyFunctionsList[index];
    return true;
  }
}
