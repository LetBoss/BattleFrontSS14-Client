// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.InternalReflectionUtils
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Serialization.Manager.Definition;
using System;
using System.Reflection;
using System.Reflection.Emit;

#nullable enable
namespace Robust.Shared.Utility;

internal static class InternalReflectionUtils
{
  private static void EmitSetField(ILGenerator rGenerator, AbstractFieldInfo info)
  {
    switch (info)
    {
      case SpecificFieldInfo specificFieldInfo:
        rGenerator.Emit(OpCodes.Stfld, specificFieldInfo.FieldInfo);
        break;
      case SpecificPropertyInfo specificPropertyInfo:
        SpecificFieldInfo backingField = specificPropertyInfo.GetBackingField();
        if (backingField != null)
        {
          rGenerator.Emit(OpCodes.Stfld, backingField.FieldInfo);
          break;
        }
        MethodInfo meth = specificPropertyInfo.PropertyInfo.GetSetMethod(true) ?? throw new NullReferenceException();
        Type declaringType = info.DeclaringType;
        OpCode opcode = ((object) declaringType != null ? (declaringType.IsValueType ? 1 : 0) : 0) != 0 ? OpCodes.Call : OpCodes.Callvirt;
        rGenerator.Emit(opcode, meth);
        break;
    }
  }

  internal static object EmitFieldAccessor(Type obj, FieldDefinition fieldDefinition)
  {
    if (fieldDefinition.BackingField is SpecificFieldInfo backingField1)
      return (object) backingField1.FieldInfo;
    if (fieldDefinition.BackingField is SpecificPropertyInfo backingField2)
      return (object) (backingField2.PropertyInfo.GetGetMethod(true) ?? throw new InvalidOperationException("Property has no getter"));
    DynamicMethod dynamicMethod = new DynamicMethod("AccessField", fieldDefinition.BackingField.FieldType, new Type[1]
    {
      obj.MakeByRefType()
    }, true);
    dynamicMethod.DefineParameter(1, ParameterAttributes.Out, "target");
    ILGenerator ilGenerator = dynamicMethod.GetILGenerator();
    ilGenerator.Emit(OpCodes.Ldarg_0);
    if (!obj.IsValueType)
      ilGenerator.Emit(OpCodes.Ldind_Ref);
    switch (fieldDefinition.BackingField)
    {
      case SpecificFieldInfo specificFieldInfo:
        ilGenerator.Emit(OpCodes.Ldfld, specificFieldInfo.FieldInfo);
        break;
      case SpecificPropertyInfo specificPropertyInfo:
        MethodInfo meth = specificPropertyInfo.PropertyInfo.GetGetMethod(true) ?? throw new NullReferenceException();
        OpCode opcode = fieldDefinition.BackingField.FieldType.IsValueType ? OpCodes.Call : OpCodes.Callvirt;
        ilGenerator.Emit(opcode, meth);
        break;
    }
    ilGenerator.Emit(OpCodes.Ret);
    return (object) dynamicMethod.CreateDelegate(typeof (InternalReflectionUtils.AccessField<,>).MakeGenericType(obj, fieldDefinition.BackingField.FieldType));
  }

  internal static object EmitFieldAssigner(
    Type objType,
    AbstractFieldInfo backingField,
    bool boxing = false)
  {
    if (!boxing)
    {
      if (backingField is SpecificFieldInfo specificFieldInfo)
      {
        FieldInfo fieldInfo = specificFieldInfo.FieldInfo;
        if ((object) fieldInfo != null && !fieldInfo.IsInitOnly)
          return (object) specificFieldInfo.FieldInfo;
      }
      if (backingField is SpecificPropertyInfo specificPropertyInfo)
      {
        SpecificFieldInfo field;
        if (specificPropertyInfo.TryGetBackingField(out field) && !field.FieldInfo.IsInitOnly)
          return (object) field.FieldInfo;
        MethodInfo setMethod = specificPropertyInfo.PropertyInfo.GetSetMethod(true);
        if ((object) setMethod != null)
          return (object) setMethod;
      }
    }
    Type fieldType = backingField.FieldType;
    DynamicMethod dynamicMethod = new DynamicMethod("AssignField", typeof (void), new Type[2]
    {
      objType.MakeByRefType(),
      boxing ? typeof (object) : fieldType
    }, true);
    dynamicMethod.DefineParameter(1, ParameterAttributes.Out, "target");
    dynamicMethod.DefineParameter(2, ParameterAttributes.None, "value");
    ILGenerator ilGenerator = dynamicMethod.GetILGenerator();
    ilGenerator.Emit(OpCodes.Ldarg_0);
    if (!objType.IsValueType)
      ilGenerator.Emit(OpCodes.Ldind_Ref);
    ilGenerator.Emit(OpCodes.Ldarg_1);
    if (boxing)
      ilGenerator.Emit(fieldType.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, fieldType);
    InternalReflectionUtils.EmitSetField(ilGenerator, backingField);
    ilGenerator.Emit(OpCodes.Ret);
    return (object) dynamicMethod.CreateDelegate(typeof (InternalReflectionUtils.AssignField<,>).MakeGenericType(objType, boxing ? typeof (object) : fieldType));
  }

  internal delegate TValue AccessField<TTarget, TValue>(ref TTarget target);

  internal delegate void AssignField<TTarget, TValue>(ref TTarget target, TValue? value);
}
