using System.Collections.Generic;
using UnityEngine;

public class NpcManager : MonoBehaviour
{
    private NpcScript[] npcs;

    void Start()
    {
        npcs = GetComponentsInChildren<NpcScript>();
    }
    
    public void NPC_ChasePlayer()
    {
        foreach (NpcScript npc in npcs)
        {
            if (npc != null)
            {
                npc.SetToChase(); 
            }
        }
    }

}