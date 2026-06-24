using System;
using System.Linq;
using System.Reflection;
using Robust.Shared.Collections;

namespace Robust.Shared.ContentPack;

public static class AppDomainExt
{
	public static Assembly GetAssemblyByName(this AppDomain domain, string name)
	{
		ValueList<Assembly> valueList = new ValueList<Assembly>(1);
		Assembly[] assemblies = domain.GetAssemblies();
		foreach (Assembly assembly in assemblies)
		{
			if (!(assembly.GetName().Name != name))
			{
				valueList.Add(assembly);
			}
		}
		if (valueList.Count != 1)
		{
			string value = string.Join(" ", valueList.Select((Assembly o) => o.GetName().Name));
			throw new InvalidOperationException($"Expected 1 assembly for {name}, found {valueList.Count}. Found {value}");
		}
		return valueList[0];
	}
}
