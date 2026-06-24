// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Xenonids.Construction.Tunnel.TunnelPathfindingConfig
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

#nullable disable
namespace Content.Client._RMC14.Xenonids.Construction.Tunnel;

public struct TunnelPathfindingConfig
{
  public double DirectDistanceWeight { get; set; }

  public double TunnelHopPenalty { get; set; }

  public double BacktrackingPenalty { get; set; }

  public double MaxConnectionDistance { get; set; }

  public int MaxIntermediateTunnels { get; set; }

  public static TunnelPathfindingConfig Default
  {
    get
    {
      return new TunnelPathfindingConfig()
      {
        DirectDistanceWeight = 1.0,
        TunnelHopPenalty = 0.3,
        BacktrackingPenalty = 5.0,
        MaxConnectionDistance = 800.0,
        MaxIntermediateTunnels = 1
      };
    }
  }
}
