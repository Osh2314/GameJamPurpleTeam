using System.Collections;
using System.Collections.Generic;
using GhoseHouse.Object.Hunter;
using UnityEngine;

public class Hunter : HunterBase
{
    public override sealed void HunterIdle()
    {
        Debug.Log("Idle");
    }

    public override sealed void HunterPatrol()
    {
        Debug.Log("Patrol");
    }

    public override sealed void HunterTrack()
    {
        Debug.Log("Track");
    }

    public override sealed void HunterAttack()
    {
        Debug.Log("Attack");
    }

    public override sealed void HunterDefense()
    {
        Debug.Log("Defense");
    }
}