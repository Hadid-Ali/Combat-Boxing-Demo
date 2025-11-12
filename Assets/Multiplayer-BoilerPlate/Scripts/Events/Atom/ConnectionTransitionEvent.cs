using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;
using UnityAtoms;

public struct RegionConfig
{
    public List<Region> Availableregions;
    public Region BestRegion;
}

[EditorIcon("atom-icon-cherry")]
[CreateAssetMenu(menuName = "Unity Atoms/Events/Custom/Networking/ConnectionTransitionEvent", fileName = "ConnectionEvent")]
public sealed class ConnectionTransitionEvent : AtomEvent<RegionConfig>
{
    
}
