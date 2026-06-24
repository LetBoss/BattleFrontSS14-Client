// Decompiled with JetBrains decompiler
// Type: Content.Shared.Administration.AdminCommandPermissions
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.RepresentationModel;

#nullable enable
namespace Content.Shared.Administration;

public sealed class AdminCommandPermissions
{
  public readonly HashSet<string> AnyCommands = new HashSet<string>();
  public readonly Dictionary<string, AdminFlags[]> AdminCommands = new Dictionary<string, AdminFlags[]>();

  public void LoadPermissionsFromStream(Stream fs)
  {
    using (StreamReader streamReader = new StreamReader(fs, EncodingHelpers.UTF8))
    {
      YamlStream yamlStream = new YamlStream();
      yamlStream.Load((TextReader) streamReader);
      foreach (YamlMappingNode yamlMappingNode in (YamlSequenceNode) yamlStream.Documents[0].RootNode)
      {
        IEnumerable<string> other = ((IEnumerable<YamlNode>) YamlHelpers.GetNode<YamlSequenceNode>(yamlMappingNode, "Commands")).Select<YamlNode, string>((Func<YamlNode, string>) (p => YamlHelpers.AsString(p)));
        YamlNode yamlNode;
        if (YamlHelpers.TryGetNode(yamlMappingNode, "Flags", ref yamlNode))
        {
          AdminFlags flags = AdminFlagsHelper.NamesToFlags((IEnumerable<string>) YamlHelpers.AsString(yamlNode).Split(",", StringSplitOptions.RemoveEmptyEntries));
          foreach (string key in other)
          {
            AdminFlags[] adminFlagsArray1;
            if (!this.AdminCommands.TryGetValue(key, out adminFlagsArray1))
            {
              this.AdminCommands.Add(key, new AdminFlags[1]
              {
                flags
              });
            }
            else
            {
              AdminFlags[] adminFlagsArray2 = new AdminFlags[adminFlagsArray1.Length + 1];
              adminFlagsArray1.CopyTo((Array) adminFlagsArray2, 0);
              AdminFlags[] adminFlagsArray3 = adminFlagsArray2;
              adminFlagsArray3[adminFlagsArray3.Length - 1] = flags;
              this.AdminCommands[key] = adminFlagsArray2;
            }
          }
        }
        else
          this.AnyCommands.UnionWith(other);
      }
    }
  }

  public bool CanCommand(string cmdName, AdminData? admin)
  {
    if (this.AnyCommands.Contains(cmdName))
      return true;
    AdminFlags[] adminFlagsArray;
    if (!this.AdminCommands.TryGetValue(cmdName, out adminFlagsArray) || admin == null)
      return false;
    foreach (AdminFlags flag in adminFlagsArray)
    {
      if (admin.HasFlag(flag))
        return true;
    }
    return false;
  }
}
