using System;
using System.Collections.Generic;
using System.Reflection;
using Robust.Shared.Reflection;

namespace Robust.Shared.Input;

public sealed class BoundKeyMap
{
	private readonly IReflectionManager reflectionManager;

	private readonly Dictionary<BoundKeyFunction, KeyFunctionId> KeyFunctionsMap = new Dictionary<BoundKeyFunction, KeyFunctionId>();

	private readonly List<BoundKeyFunction> KeyFunctionsList = new List<BoundKeyFunction>();

	internal IEnumerable<BoundKeyFunction> AllKeyFunctions => KeyFunctionsList;

	public BoundKeyMap(IReflectionManager reflectionManager)
	{
		this.reflectionManager = reflectionManager;
	}

	public void PopulateKeyFunctionsMap()
	{
		if (KeyFunctionsMap.Count != 0)
		{
			throw new InvalidOperationException("Cannot run this method twice.");
		}
		foreach (Type item in reflectionManager.FindTypesWithAttribute<KeyFunctionsAttribute>())
		{
			FieldInfo[] fields = item.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
			foreach (FieldInfo fieldInfo in fields)
			{
				if (!fieldInfo.IsLiteral && fieldInfo.IsInitOnly && !(fieldInfo.FieldType != typeof(BoundKeyFunction)))
				{
					KeyFunctionsList.Add((BoundKeyFunction)fieldInfo.GetValue(null));
				}
			}
		}
		KeyFunctionsList.Sort();
		for (int j = 0; j < KeyFunctionsList.Count; j++)
		{
			KeyFunctionsMap.Add(KeyFunctionsList[j], new KeyFunctionId(j));
		}
	}

	public bool FunctionExists(string name)
	{
		return KeyFunctionsMap.ContainsKey(new BoundKeyFunction(name));
	}

	public KeyFunctionId KeyFunctionID(BoundKeyFunction function)
	{
		return KeyFunctionsMap[function];
	}

	public BoundKeyFunction KeyFunctionName(KeyFunctionId function)
	{
		return KeyFunctionsList[(int)function];
	}

	public bool TryGetKeyFunction(KeyFunctionId funcId, out BoundKeyFunction func)
	{
		List<BoundKeyFunction> keyFunctionsList = KeyFunctionsList;
		int num = (int)funcId;
		if (0 > num || num >= keyFunctionsList.Count)
		{
			func = default(BoundKeyFunction);
			return false;
		}
		func = keyFunctionsList[num];
		return true;
	}
}
