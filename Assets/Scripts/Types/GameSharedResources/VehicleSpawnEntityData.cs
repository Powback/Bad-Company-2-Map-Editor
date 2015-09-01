using UnityEngine;
using System.Collections;
using System.Collections;
using System.Collections.Generic;

public class VehicleSpawnEntityData : MonoBehaviour {
	public string GUID;
	public List<BC2Array> Components;
	public float Enumeration;
	public string Name;
	public bool Enabled;
	public string Team;
	public int Amount;
	public float SpawnDelay;
	public float InitializedSpawnDelay;
	public bool AllowMulipleSpawn;
	public bool Immortal;
	public bool FakeImmortal;
	public string Vehicle;
	public List<Complex> Vehicles;

	public int TakeControlEntryIndex;
	public float RotationYaw;
	public float RotationPitch;
	public float RotationRoll;
	public float Throttle;
	public float Brake;
	public float RespawnRange;
	public float SpawnAreaRadius;
	public float TimeUntilAbandoned;
	public float TimeUntilAbandonedIsDestroyed;
	public float BotBailWhenHealthBelow;
	public float BotBailOutDelay;
	public float KeepAliveRadius;
	public float WreckDuration;
	public int MaxVehicles;
	public int MaxVehiclesPerMap;
	public int MeshShaderSetNumber;
	public bool AutoSpawn;
	public bool ApplyDamageToAbandondedVehicles;
	public bool SnapToSurface;
	public bool SpawnVehicleFromBeginning;
	public bool DestroyVehiclesOnDisable;
	public bool SetTeamOnSpawn;
	public bool EffectedByImpulse;
	public float LodDistance;
	public string EnterRestriction;
	public bool OnlySendEventForHumanPlayers;
	public bool SendWeaponEvents;
	
}
