using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCGetOnBoat : SSAction
{
    public myGameObject sceneController;
    bool move = false;
    int index = 0;

    public static CCGetOnBoat GetSSAction()
    {
        CCGetOnBoat action = ScriptableObject.CreateInstance<CCGetOnBoat>();
        return action;
    }

    public override void Start()
    {
        sceneController = (myGameObject)SSDirector.getInstance().currentSceneController;
    }

    public override void Update()
    {
        if (move)
        {
            if (sceneController.boatSide == 1 && sceneController.find == 1)
            {
                if (this.transform.position != sceneController.From_positions[index])
                    this.transform.position = Vector3.MoveTowards(this.transform.position, sceneController.From_positions[index], 20 * Time.deltaTime);
                else
                    move = false;
            }
            else if (sceneController.boatSide == -1 && sceneController.find == -1)
            {
                if (this.transform.position != sceneController.To_positions[index])
                    this.transform.position = Vector3.MoveTowards(this.transform.position, sceneController.To_positions[index], 20 * Time.deltaTime);
                else
                    move = false;
            }
            if (!move)
            {
                sceneController.find = 0;
                sceneController.gameStatus();
                this.destroy = true;
                this.callback.SSActionEvent(this);
            }
        }
        else
        {
            if (sceneController.boatCapacity != 0)
            {
                if (sceneController.boatSide == 1)
                {
                    for (int i = 0; i < sceneController.priests + sceneController.devils; i++)
                    {
                        if (sceneController.startItem[i] == gameobject)
                        {
                            sceneController.find = 1;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < sceneController.priests + sceneController.devils; i++)
                    {
                        if (sceneController.endItem[i] == gameobject)
                        {
                            sceneController.find = -1;
                        }
                    }
                }
                if (sceneController.find != 0)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        if (sceneController.BoatItem[i] == null)
                        {
                            index = i;
                            sceneController.BoatItem[i] = gameobject;
                            break;
                        }
                    }
                    gameobject.transform.parent = sceneController.boat.transform;
                    sceneController.boatCapacity--;
                    move = true;

                    if (sceneController.boatSide == 1)
                    {
                        for (int i = 0; i < sceneController.priests + sceneController.devils; i++)
                        {
                            if (sceneController.startItem[i] == gameobject)
                            {
                                sceneController.startItem[i] = null;
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < sceneController.priests + sceneController.devils; i++)
                        {
                            if (sceneController.endItem[i] == gameobject)
                            {
                                sceneController.endItem[i] = null;
                                sceneController.count--;
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}

public class CCBoatMove : SSAction
{
    public myGameObject sceneController;
    bool move = true;

    public static CCBoatMove GetSSAction()
    {
        CCBoatMove action = ScriptableObject.CreateInstance<CCBoatMove>();
        return action;
    }
    // Use this for initialization  
    public override void Start()
    {
        sceneController = (myGameObject)SSDirector.getInstance().currentSceneController;
    }

    public override void Update()
    {
        if (move)
        {
            if (this.transform.position.x != -sceneController.boatSide * sceneController.BoatPosition.x)
            {
                Vector3 dest = sceneController.BoatPosition;
                dest.x = -sceneController.boatSide * dest.x;
                this.transform.position = Vector3.MoveTowards(this.transform.position, dest, 20 * Time.deltaTime);
            }
            else
            {
                sceneController.boatSide = -sceneController.boatSide;
                move = false;
            }
        }
        else
        {
            this.destroy = true;
            this.callback.SSActionEvent(this);
        }
    }
}

public class CCGetOffBoat : SSAction
{
    public myGameObject sceneController;
    bool move = false;

    public static CCGetOffBoat GetSSAction()
    {
        CCGetOffBoat action = ScriptableObject.CreateInstance<CCGetOffBoat>();
        return action;
    }
    // Use this for initialization  
    public override void Start()
    {
        sceneController = (myGameObject)SSDirector.getInstance().currentSceneController;
    }

    public override void Update()
    {
        if (move)
        {
            if (this.transform.position != sceneController.GetEmptyIndex())
            {
                this.transform.position = Vector3.MoveTowards(this.transform.position, sceneController.GetEmptyIndex(), 20 * Time.deltaTime);
            }
            else
            {
                move = false;

                if (sceneController.boatSide == 1)
                {
                    for (int i = 0; i < sceneController.priests + sceneController.devils; i++)
                    {
                        if (sceneController.startItem[i] == null)
                        {
                            sceneController.startItem[i] = gameobject;
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < sceneController.priests + sceneController.devils; i++)
                    {
                        if (sceneController.endItem[i] == null)
                        {
                            sceneController.endItem[i] = gameobject;
                            sceneController.count++;
                            break;
                        }
                    }
                }
            

                sceneController.find = 0;
                sceneController.gameStatus();
                this.destroy = true;
                this.callback.SSActionEvent(this);
            }
        }
        else
        {
            for (int i = 0; i < 2; i++)
            {
                if (sceneController.BoatItem[i] == gameobject)
                    sceneController.find = 1;
            }

            if (sceneController.find == 1)
            {
                for (int i = 0; i < 2; i++)
                {
                    if (sceneController.BoatItem[i] == gameobject)
                    {
                        sceneController.BoatItem[i] = null;
                        break;
                    }
                }
                gameobject.transform.parent = null;
                sceneController.boatCapacity++;
                move = true;
            }
        }
    }
}