namespace Content.Client._RMC14.Xenonids.Construction.Tunnel;

public struct TunnelPathfindingConfig
{
	public double DirectDistanceWeight { get; set; }

	public double TunnelHopPenalty { get; set; }

	public double BacktrackingPenalty { get; set; }

	public double MaxConnectionDistance { get; set; }

	public int MaxIntermediateTunnels { get; set; }

	public static TunnelPathfindingConfig Default => new TunnelPathfindingConfig
	{
		DirectDistanceWeight = 1.0,
		TunnelHopPenalty = 0.3,
		BacktrackingPenalty = 5.0,
		MaxConnectionDistance = 800.0,
		MaxIntermediateTunnels = 1
	};
}
