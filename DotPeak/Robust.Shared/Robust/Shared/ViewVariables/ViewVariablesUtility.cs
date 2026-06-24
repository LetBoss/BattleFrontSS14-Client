// Decompiled with JetBrains decompiler
// Type: Robust.Shared.ViewVariables.ViewVariablesUtility
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

#nullable enable
namespace Robust.Shared.ViewVariables;

public static class ViewVariablesUtility
{
  public static bool TypeHasVisibleMembers(Type type)
  {
    return type.GetAllFields().Cast<MemberInfo>().Concat<MemberInfo>((IEnumerable<MemberInfo>) type.GetAllProperties()).Any<MemberInfo>((Func<MemberInfo, bool>) (f => ViewVariablesUtility.TryGetViewVariablesAccess(f, out VVAccess? _)));
  }

  public static bool TryGetViewVariablesAccess(MemberInfo info, [NotNullWhen(true)] out VVAccess? access)
  {
    ViewVariablesAttribute attribute;
    if (info.TryGetCustomAttribute<ViewVariablesAttribute>(out attribute))
    {
      access = new VVAccess?(attribute.Access);
      return true;
    }
    if (info.HasCustomAttribute<DataFieldAttribute>() || info.HasCustomAttribute<IncludeDataFieldAttribute>())
    {
      access = new VVAccess?(VVAccess.ReadWrite);
      return true;
    }
    access = new VVAccess?();
    return false;
  }
}
