// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Localization.LinguiniAdapter
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Linguini.Shared.Types.Bundle;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Robust.Shared.Localization;

internal static class LinguiniAdapter
{
  internal static ILocValue ToLocValue(this IFluentType arg)
  {
    switch (arg)
    {
      case FluentNone _:
        return (ILocValue) new LocValueNone("");
      case FluentNumber fluentNumber:
        return (ILocValue) new LocValueNumber(FluentNumber.op_Implicit(fluentNumber));
      case FluentString fluentString:
        return (ILocValue) new LocValueString(FluentString.op_Implicit(fluentString));
      case FluentLocWrapperType fluentLocWrapperType:
        return fluentLocWrapperType.WrappedValue;
      default:
        throw new ArgumentOutOfRangeException(nameof (arg));
    }
  }

  public static IFluentType FluentFromObject(this object obj, LocContext context)
  {
    IFluentType ifluentType;
    switch (obj)
    {
      case ILocValue wrappedValue:
        ifluentType = (IFluentType) new FluentLocWrapperType(wrappedValue, context);
        break;
      case EntityUid entityUid:
        ifluentType = (IFluentType) new FluentLocWrapperType((ILocValue) new LocValueEntity(entityUid), context);
        break;
      case IFluentEntityUid fluentEntityUid:
        ifluentType = (IFluentType) new FluentLocWrapperType((ILocValue) new LocValueEntity(fluentEntityUid.FluentOwner), context);
        break;
      case DateTime dateTime:
        ifluentType = (IFluentType) new FluentLocWrapperType((ILocValue) new LocValueDateTime(dateTime), context);
        break;
      case TimeSpan timeSpan:
        ifluentType = (IFluentType) new FluentLocWrapperType((ILocValue) new LocValueTimeSpan(timeSpan), context);
        break;
      case Color _:
        Color color = (Color) obj;
        ifluentType = (IFluentType) FluentString.op_Implicit(((Color) ref color).ToHex());
        break;
      case bool _:
      case Enum _:
        ifluentType = (IFluentType) FluentString.op_Implicit(obj.ToString().ToLowerInvariant());
        break;
      case string str:
        ifluentType = (IFluentType) FluentString.op_Implicit(str);
        break;
      case byte num1:
        ifluentType = (IFluentType) FluentNumber.op_Implicit((float) num1);
        break;
      case sbyte num2:
        ifluentType = (IFluentType) FluentNumber.op_Implicit((float) num2);
        break;
      case short num3:
        ifluentType = (IFluentType) FluentNumber.op_Implicit((float) num3);
        break;
      case ushort num4:
        ifluentType = (IFluentType) FluentNumber.op_Implicit((float) num4);
        break;
      case int num5:
        ifluentType = (IFluentType) FluentNumber.op_Implicit((float) num5);
        break;
      case uint num6:
        ifluentType = (IFluentType) FluentNumber.op_Implicit((float) num6);
        break;
      case long num7:
        ifluentType = (IFluentType) FluentNumber.op_Implicit((float) num7);
        break;
      case ulong num8:
        ifluentType = (IFluentType) FluentNumber.op_Implicit((float) num8);
        break;
      case double num9:
        ifluentType = (IFluentType) FluentNumber.op_Implicit(num9);
        break;
      case float num10:
        ifluentType = (IFluentType) FluentNumber.op_Implicit(num10);
        break;
      default:
        ifluentType = (IFluentType) FluentString.op_Implicit(obj.ToString());
        break;
    }
    return ifluentType;
  }

  public static IFluentType FluentFromVal(this ILocValue locValue, LocContext context)
  {
    IFluentType ifluentType;
    if ((object) (locValue as LocValueNone) == null)
    {
      LocValueNumber locValueNumber = locValue as LocValueNumber;
      if ((object) locValueNumber == null)
      {
        LocValueString locValueString = locValue as LocValueString;
        ifluentType = (object) locValueString != null ? (IFluentType) FluentString.op_Implicit(locValueString.Value) : (IFluentType) new FluentLocWrapperType(locValue, context);
      }
      else
        ifluentType = (IFluentType) FluentNumber.op_Implicit(locValueNumber.Value);
    }
    else
      ifluentType = (IFluentType) FluentNone.None;
    return ifluentType;
  }
}
