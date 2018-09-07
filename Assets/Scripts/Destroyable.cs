using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Destroyable : MonoBehaviour
{
    [SerializeField]
    protected int hP = 100;
    public int HP
    {
        get
        {
            return hP;
        }
    }
    
    public void Damage(int damage)
    {
        hP -= damage;
        if (hP <= 0)
        {
            Destroy();
        }
    }

    public abstract void Destroy();
}
