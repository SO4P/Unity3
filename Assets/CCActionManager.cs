using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCActionManager : SSActionManager, ISSActionCallback
{
    public myGameObject sceneController;
    public CCGetOnBoat getOn;
    public CCGetOffBoat getOff;
    public CCBoatMove boatMove;

    // Use this for initialization
    void Start () {
        sceneController = (myGameObject)SSDirector.getInstance().currentSceneController;
        sceneController.actionManager = this;
    }

    // Update is called once per frame
    protected new void Update()
    {
        if (Input.GetMouseButtonDown(0) && sceneController.status == 0)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if(hit.transform.tag == "Devil" || hit.transform.tag == "Priest")
                {
                    if (hit.transform.parent == sceneController.boat.transform)
                    {
                        getOff = CCGetOffBoat.GetSSAction();
                        this.RunAction(hit.collider.gameObject, getOff, this);
                    }
                    else
                    {
                        getOn = CCGetOnBoat.GetSSAction();
                        this.RunAction(hit.collider.gameObject, getOn, this);
                    }
                }
                else if(hit.transform.tag == "Boat" && sceneController.boatCapacity != 2)
                {
                    boatMove = CCBoatMove.GetSSAction();
                    this.RunAction(hit.collider.gameObject, boatMove, this);
                }
            }
        }
        base.Update();
    }

    public void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Competeted,
        int intParam = 0, string strParam = null, Object objectParam = null)
    {
        //  
    }
}
