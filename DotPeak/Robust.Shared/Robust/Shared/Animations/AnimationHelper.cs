// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Animations.AnimationHelper
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Reflection;

#nullable enable
namespace Robust.Shared.Animations;

public static class AnimationHelper
{
  public static void SetAnimatableProperty(object target, string name, object value)
  {
    PropertyInfo property = target.GetType().GetProperty(name);
    if (property == (PropertyInfo) null)
      throw new ArgumentException($"Animatable property with name '{name}' does not exist.");
    if (!Attribute.IsDefined((MemberInfo) property, typeof (AnimatableAttribute)))
      throw new ArgumentException($"Animatable property with name '{name}' does not exist.");
    property.SetValue(target, value);
  }

  public static object? GetAnimatableProperty(object target, string name)
  {
    PropertyInfo property = target.GetType().GetProperty(name);
    if (property == (PropertyInfo) null)
      throw new ArgumentException($"Animatable property with name '{name}' does not exist.");
    return Attribute.IsDefined((MemberInfo) property, typeof (AnimatableAttribute)) ? property.GetValue(target) : throw new ArgumentException($"Animatable property with name '{name}' does not exist.");
  }

  public static void CallAnimatableMethod(object target, string name, object[] arguments)
  {
    MethodInfo method = target.GetType().GetMethod(name);
    if (method == (MethodInfo) null)
      throw new ArgumentException($"Animatable method with name '{name}' does not exist.");
    if (!Attribute.IsDefined((MemberInfo) method, typeof (AnimatableAttribute)))
      throw new ArgumentException($"Animatable method with name '{name}' does not exist.");
    method.Invoke(target, arguments);
  }
}
