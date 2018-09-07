using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Destroyable
{
    [SerializeField]
    private float AttackDistance = 80;
    [SerializeField]
    private float loopDuration = 30;
    [SerializeField]
    private float AnimeTime = 2;
    [SerializeField]
    private Transform MyMesh;
    [SerializeField]
    private List<Transform> path = new List<Transform>();
    [SerializeField]
    private List<Gun> guns = new List<Gun>();
    [SerializeField]
    private Transform looker;
    private enum Rot { parent, animParent, looker, animLooker, dead };
    private Rot curRot = Rot.parent;
    private float startTime = 0;

    public override void Destroy()
    {
        curRot = Rot.dead;
        Blackboard.EnemyExplosion.transform.position = transform.position;
        Blackboard.EnemyExplosion.Play();
        Blackboard.Sounds.PlaySound("Explosion");
        Blackboard.PlaneControls.Score += 10;
        Destroy(gameObject);
    }

    private void Start()
    {
        MoveAlongPath();
    }

    void LateUpdate ()
    {
        float dist = Vector3.Distance(Blackboard.PlaneControls.transform.position, transform.position);
        
        if (curRot == Rot.parent && dist < AttackDistance)
        {
            curRot = Rot.animLooker;
            startTime = Time.time;
            foreach (Gun g in guns)
            {
                g.StartFiring();
            }
        }
        else if (curRot == Rot.looker)
        {
            MyMesh.LookAt(Blackboard.PlaneControls.transform);
            if (dist > AttackDistance)
            {
                curRot = Rot.animParent;
                startTime = Time.time;
                foreach (Gun g in guns)
                {
                    g.StopFiring();
                }
            }
        }
        else if (curRot == Rot.animParent)
        {
            float t = (Time.time - startTime) / AnimeTime;
            MyMesh.rotation = Quaternion.Lerp(MyMesh.rotation, transform.rotation, t);
            if (t >= 1)
            {
                curRot = Rot.parent;
            }
        }
        else if (curRot == Rot.animLooker)
        {
            looker.LookAt(Blackboard.PlaneControls.transform);
            float t = (Time.time - startTime) / AnimeTime;
            MyMesh.rotation = Quaternion.Lerp(MyMesh.rotation, looker.rotation, t);
            if (t >= 1)
            {
                curRot = Rot.looker;
            }
        }
    }

    private void MoveAlongPath()
    {
        if (curRot != Rot.dead)
        {
            //iTween.MoveTo(gameObject, iTween.Hash("path", path.ToArray(), "orienttopath", true, "time", loopDuration, "easetype", iTween.EaseType.linear, "looktime", 0, "looptype", iTween.LoopType.loop));
        }
    }
}
