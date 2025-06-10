using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NpcManager : MonoBehaviour
{
    private NpcScript[] npcs;
    [Header("Unity Events")]
    public UnityEvent keyObtained;
    public UnityEvent keyStolen;
    [Header("Camera")]
    public CameraFollow mainCamera;

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
    public void bignpc_jump()
    {
        mainCamera.Shake(0.1f, 0.1f); 
    }

}