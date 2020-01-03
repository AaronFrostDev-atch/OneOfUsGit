
using Bolt;
using UnityEngine;




[BoltGlobalBehaviour("Prototyping")]
public class NetworkCallbacks : Bolt.GlobalEventListener
{

    GameObject cam;
    GameObject player;
    Vector3 spawnPos = GameData.SpawnPositions.spawns[0];


    public override void SceneLoadLocalDone(string scene)
    {

        // Instantiate Player
        player = BoltNetwork.Instantiate(BoltPrefabs.MyPlayer, spawnPos, Quaternion.identity);



        // Take the non-networked camera in the scene, and attach it to the player
        cam = GameObject.FindWithTag("MainCamera");
        cam.transform.SetParent(player.transform);
        cam.transform.localPosition = new Vector3(0, .9f, 0);



        // Tell the Render Effects manager that we have spawned in
        GameObject.Find("Scene Settings").GetComponent<TimedEffects>().spawnedIn = true;


    }

    
}


[BoltGlobalBehaviour()]
public class ObjectMoveBehavior : Bolt.GlobalEventListener, IObjectMovedListener
{

    InteractiveObjects i;

    public override void OnEvent(ObjectMoved evnt)
    {
        if (evnt.Entity.IsOwner)
        {


            // Interactive objects respond to events

            if (evnt.Entity.GetComponent<InteractiveObjects>())
            {
                i = evnt.Entity.GetComponent<InteractiveObjects>();
            }

            if (evnt.Push)
            {
                i.SetVelocity(evnt.Velocity);
                evnt.Entity.GetState<IDynamicObjectState>().Held = false;
            }
            else
            {
                if (evnt.Hold)
                {
                    i.Hold(evnt.Position, evnt.Rotation);
                    evnt.Entity.GetState<IDynamicObjectState>().Held = true;
                }
                else 
                {
                    i.Drop(evnt.Velocity);
                    evnt.Entity.GetState<IDynamicObjectState>().Held = false;
                }

            }
            
                
            

            

            


        }

    }
}
