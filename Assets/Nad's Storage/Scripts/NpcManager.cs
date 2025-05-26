using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events; 

public class NpcManager : MonoBehaviour
{
    private NpcScript[] npcs;
    [Header("Unity Events")]
    public UnityEvent keyObtained;
    public UnityEvent keyStolen;    

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
        keyObtained.Invoke(); 
    }
    public void NPC_DisableChasePlayer()
    {
        foreach (NpcScript npc in npcs)
        {
            if (npc != null)
            {
                npc.DisableChase();
            }
        }
        keyStolen.Invoke(); 
    }

}