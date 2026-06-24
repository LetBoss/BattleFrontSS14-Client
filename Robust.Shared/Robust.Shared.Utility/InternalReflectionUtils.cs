using System;
using System.Reflection;
using System.Reflection.Emit;
using Robust.Shared.Serialization.Manager.Definition;

namespace Robust.Shared.Utility;

internal static class InternalReflectionUtils
{
	internal delegate TValue AccessField<TTarget, TValue>(ref TTarget target);

	internal delegate void AssignField<TTarget, TValue>(ref TTarget target, TValue? value);

	private static void EmitSetField(ILGenerator rGenerator, AbstractFieldInfo info)
	{
		if (!(info is SpecificFieldInfo specificFieldInfo))
		{
			if (info is SpecificPropertyInfo specificPropertyInfo)
			{
				SpecificFieldInfo backingField = specificPropertyInfo.GetBackingField();
				if (backingField != null)
				{
					rGenerator.Emit(OpCodes.Stfld, backingField.FieldInfo);
					return;
				}
				MethodInfo meth = specificPropertyInfo.PropertyInfo.GetSetMethod(nonPublic: true) ?? throw new NullReferenceException();
				Type? declaringType = info.DeclaringType;
				OpCode opcode = (((object)declaringType != null && declaringType.IsValueType) ? OpCodes.Call : OpCodes.Callvirt);
				rGenerator.Emit(opcode, meth);
			}
		}
		else
		{
			rGenerator.Emit(OpCodes.Stfld, specificFieldInfo.FieldInfo);
		}
	}

	internal static object EmitFieldAccessor(Type obj, FieldDefinition fieldDefinition)
	{
		if (fieldDefinition.BackingField is SpecificFieldInfo specificFieldInfo)
		{
			return specificFieldInfo.FieldInfo;
		}
		if (fieldDefinition.BackingField is SpecificPropertyInfo specificPropertyInfo)
		{
			return specificPropertyInfo.PropertyInfo.GetGetMethod(nonPublic: true) ?? throw new InvalidOperationException("Property has no getter");
		}
		DynamicMethod dynamicMethod = new DynamicMethod("AccessField", fieldDefinition.BackingField.FieldType, new Type[1] { obj.MakeByRefType() }, restrictedSkipVisibility: true);
		dynamicMethod.DefineParameter(1, ParameterAttributes.Out, "target");
		ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
		iLGenerator.Emit(OpCodes.Ldarg_0);
		if (!obj.IsValueType)
		{
			iLGenerator.Emit(OpCodes.Ldind_Ref);
		}
		AbstractFieldInfo backingField = fieldDefinition.BackingField;
		if (!(backingField is SpecificFieldInfo specificFieldInfo2))
		{
			if (backingField is SpecificPropertyInfo specificPropertyInfo2)
			{
				MethodInfo meth = specificPropertyInfo2.PropertyInfo.GetGetMethod(nonPublic: true) ?? throw new NullReferenceException();
				OpCode opcode = (fieldDefinition.BackingField.FieldType.IsValueType ? OpCodes.Call : OpCodes.Callvirt);
				iLGenerator.Emit(opcode, meth);
			}
		}
		else
		{
			iLGenerator.Emit(OpCodes.Ldfld, specificFieldInfo2.FieldInfo);
		}
		iLGenerator.Emit(OpCodes.Ret);
		return dynamicMethod.CreateDelegate(typeof(AccessField<, >).MakeGenericType(obj, fieldDefinition.BackingField.FieldType));
	}

	internal static object EmitFieldAssigner(Type objType, AbstractFieldInfo backingField, bool boxing = false)
	{
		if (!boxing)
		{
			if (backingField is SpecificFieldInfo specificFieldInfo)
			{
				FieldInfo fieldInfo = specificFieldInfo.FieldInfo;
				if ((object)fieldInfo != null && !fieldInfo.IsInitOnly)
				{
					return specificFieldInfo.FieldInfo;
				}
			}
			if (backingField is SpecificPropertyInfo specificPropertyInfo)
			{
				if (specificPropertyInfo.TryGetBackingField(out SpecificFieldInfo field) && !field.FieldInfo.IsInitOnly)
				{
					return field.FieldInfo;
				}
				MethodInfo setMethod = specificPropertyInfo.PropertyInfo.GetSetMethod(nonPublic: true);
				if ((object)setMethod != null)
				{
					return setMethod;
				}
			}
		}
		Type fieldType = backingField.FieldType;
		DynamicMethod dynamicMethod = new DynamicMethod("AssignField", typeof(void), new Type[2]
		{
			objType.MakeByRefType(),
			boxing ? typeof(object) : fieldType
		}, restrictedSkipVisibility: true);
		dynamicMethod.DefineParameter(1, ParameterAttributes.Out, "target");
		dynamicMethod.DefineParameter(2, ParameterAttributes.None, "value");
		ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
		iLGenerator.Emit(OpCodes.Ldarg_0);
		if (!objType.IsValueType)
		{
			iLGenerator.Emit(OpCodes.Ldind_Ref);
		}
		iLGenerator.Emit(OpCodes.Ldarg_1);
		if (boxing)
		{
			iLGenerator.Emit(fieldType.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, fieldType);
		}
		EmitSetField(iLGenerator, backingField);
		iLGenerator.Emit(OpCodes.Ret);
		return dynamicMethod.CreateDelegate(typeof(AssignField<, >).MakeGenericType(objType, boxing ? typeof(object) : fieldType));
	}
}
