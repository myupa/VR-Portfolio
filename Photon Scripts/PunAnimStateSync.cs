using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PunAnimStateSync : StateMachineBehaviour
{
    //Sync Animation States
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        int animHash = GetNameHash(stateInfo);

        if (PunObjectSync._ObjectSync != null)
        {
            if (!PhotonNetwork.InRoom)
            {
                PunObjectSync._ObjectSync.AddAnimState(animator.ToString(), animHash);
            }
            else
            {
                PunObjectSync._ObjectSync.UpdateAnimState(animator.ToString(), animHash);
            }
        }
    }

    public int GetNameHash(AnimatorStateInfo animatorStateInfo)
    {
        return animatorStateInfo.fullPathHash;
    }

}
