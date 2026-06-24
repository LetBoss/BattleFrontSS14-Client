// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.ResPathUtil
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Collections;
using System;
using System.Collections.Generic;
using System.Text;

#nullable enable
namespace Robust.Shared.Utility;

public static class ResPathUtil
{
  public static ResPath Clean(this ResPath path)
  {
    if (path.CanonPath == "")
      return ResPath.Empty;
    ValueList<string> valueList = new ValueList<string>();
    if (path.IsRooted)
      valueList.Add("/");
    foreach (string str in path.CanonPath.Split('/'))
    {
      switch (str)
      {
        case ".":
        case "":
          continue;
        case "..":
          if (valueList.Count > 0)
          {
            if (valueList.Count != 1 || !(valueList[0] == "/"))
            {
              int index = valueList.Count - 1;
              if (valueList[index] != "..")
              {
                valueList.RemoveAt(index);
                continue;
              }
              break;
            }
            continue;
          }
          break;
      }
      valueList.Add(str);
    }
    StringBuilder stringBuilder = new StringBuilder(path.CanonPath.Length);
    int num = !path.IsRooted || valueList.Count <= 1 ? 0 : 1;
    for (int index = 0; index < valueList.Count; ++index)
    {
      if (index > num)
        stringBuilder.Append('/');
      stringBuilder.Append(valueList[index]);
    }
    return stringBuilder.Length != 0 ? new ResPath(stringBuilder.ToString()) : ResPath.Self;
  }

  public static ResPath GetCommonSegments(this ResPath path, ResPath other)
  {
    string[] strArray1 = path.EnumerateSegments();
    string[] strArray2 = other.EnumerateSegments();
    int num = Math.Min(strArray1.Length, strArray2.Length);
    ValueList<string> values = new ValueList<string>();
    for (int index = 0; index < num && strArray1[index] == strArray2[index]; ++index)
      values.Add(strArray1[index]);
    return new ResPath(string.Join<string>('/', (IEnumerable<string>) values));
  }

  public static ResPath GetNextSegment(this ResPath path, ResPath other)
  {
    string[] strArray1 = path.EnumerateSegments();
    string[] strArray2 = other.EnumerateSegments();
    int num = Math.Min(strArray1.Length, strArray2.Length);
    int index1 = 0;
    string canonPath = string.Empty;
    for (int index2 = 0; index2 < num && strArray1[index2] == strArray2[index2]; ++index2)
    {
      canonPath = strArray1[index2];
      ++index1;
    }
    if (index1 < strArray1.Length)
      canonPath = strArray1[index1] + (index1 != strArray1.Length - 1 || path.Extension.Length == 0 ? "/" : string.Empty);
    return new ResPath(canonPath);
  }

  public static string[] EnumerateSegments(this ResPath path)
  {
    if (!path.IsRooted)
      return path.CanonPath.Split('/');
    string canonPath = path.CanonPath;
    return canonPath.Substring(1, canonPath.Length - 1).Split('/');
  }
}
