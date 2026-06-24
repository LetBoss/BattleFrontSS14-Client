using System;
using System.Text;
using Robust.Shared.Collections;

namespace Robust.Shared.Utility;

public static class ResPathUtil
{
	public static ResPath Clean(this ResPath path)
	{
		if (path.CanonPath == "")
		{
			return ResPath.Empty;
		}
		ValueList<string> valueList = default(ValueList<string>);
		if (path.IsRooted)
		{
			valueList.Add("/");
		}
		string[] array = path.CanonPath.Split('/');
		foreach (string text in array)
		{
			switch (text)
			{
			case "..":
				if (valueList.Count > 0)
				{
					if (valueList.Count == 1 && valueList[0] == "/")
					{
						continue;
					}
					int index = valueList.Count - 1;
					if (valueList[index] != "..")
					{
						valueList.RemoveAt(index);
						continue;
					}
				}
				break;
			case ".":
			case "":
				continue;
			}
			valueList.Add(text);
		}
		StringBuilder stringBuilder = new StringBuilder(path.CanonPath.Length);
		int num = ((path.IsRooted && valueList.Count > 1) ? 1 : 0);
		for (int j = 0; j < valueList.Count; j++)
		{
			if (j > num)
			{
				stringBuilder.Append('/');
			}
			stringBuilder.Append(valueList[j]);
		}
		if (stringBuilder.Length != 0)
		{
			return new ResPath(stringBuilder.ToString());
		}
		return ResPath.Self;
	}

	public static ResPath GetCommonSegments(this ResPath path, ResPath other)
	{
		string[] array = path.EnumerateSegments();
		string[] array2 = other.EnumerateSegments();
		int num = Math.Min(array.Length, array2.Length);
		ValueList<string> valueList = default(ValueList<string>);
		for (int i = 0; i < num && array[i] == array2[i]; i++)
		{
			valueList.Add(array[i]);
		}
		return new ResPath(string.Join('/', valueList));
	}

	public static ResPath GetNextSegment(this ResPath path, ResPath other)
	{
		string[] array = path.EnumerateSegments();
		string[] array2 = other.EnumerateSegments();
		int num = Math.Min(array.Length, array2.Length);
		int num2 = 0;
		string canonPath = string.Empty;
		for (int i = 0; i < num && array[i] == array2[i]; i++)
		{
			canonPath = array[i];
			num2++;
		}
		if (num2 < array.Length)
		{
			canonPath = array[num2] + ((num2 != array.Length - 1 || path.Extension.Length == 0) ? "/" : string.Empty);
		}
		return new ResPath(canonPath);
	}

	public static string[] EnumerateSegments(this ResPath path)
	{
		if (!path.IsRooted)
		{
			return path.CanonPath.Split('/');
		}
		string canonPath = path.CanonPath;
		return canonPath.Substring(1, canonPath.Length - 1).Split('/');
	}
}
