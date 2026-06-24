// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.ILGeneratorExt
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System.Reflection.Emit;

#nullable enable
namespace Robust.Shared.Utility;

public static class ILGeneratorExt
{
  public static RobustILGenerator GetRobustGen(this DynamicMethod dynamicMethod)
  {
    return new RobustILGenerator(dynamicMethod.GetILGenerator());
  }

  public static RobustILGenerator GetRobustGen(this ILGenerator generator)
  {
    return new RobustILGenerator(generator);
  }
}
