using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PhotonAnimStateSync : StateMachineBehaviour
{
    //Sync animation states by placing this on animation state machines.
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        int animHash = GetNameHash(stateInfo);

        if (PunObjectSync._ObjectSync != null)
        {
            if (!PhotonNetwork.InRoom)
            {
                PhotonMasterSync.instance.AddAnimState(animator.ToString(), animHash);
            }
            else
            {
                PhotonMasterSync.instance.UpdateAnimState(animator.ToString(), animHash);
            }
        }
    }

    public int GetNameHash(AnimatorStateInfo animatorStateInfo)
    {
        return animatorStateInfo.fullPathHash;
    }

}
