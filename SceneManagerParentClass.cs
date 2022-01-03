using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum Zone
{
    Underwater,
    Abovewater,
    Forest,
    Tunnel,
    Cave
}

public class SceneManagerParentClass : MonoBehaviour
{

    public InputActionReference pressAnyButton;

    public virtual void SetZone(Zone zone)
    {
            switch (zone)
            {
                case Zone.Cave:
                    break;
                case Zone.Tunnel:
                    break;
                case Zone.Forest:
                    break;
                case Zone.Underwater:
                    break;
                case Zone.Abovewater:
                    break;
                default:
                    break;
            }
    }

}
