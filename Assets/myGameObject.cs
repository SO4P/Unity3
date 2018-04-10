using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class myGameObject : MonoBehaviour, ISceneController, IUserAction
{
    public SSActionManager actionManager { get; set; }
    Vector3[] positions = new Vector3[] {new Vector3(6.5F,1.25F,0), new Vector3(7.5F,1.25F,0), new Vector3(8.5F,1.25F,0),
                new Vector3(9.5F,1.25F,0), new Vector3(10.5F,1.25F,0), new Vector3(11.5F,1.25F,0),
                new Vector3(12.5F,1.25F,0), new Vector3(13.5F,1.25F,0), new Vector3(14.5F,1.25F,0),
                new Vector3(15.5F,1.25F,0), new Vector3(16.5F,1.25F,0), new Vector3(17.5F,1.25F,0)};  //character在岸上的位置
    Vector3 waterPosition = new Vector3(0, -2.75f, 0);
    Vector3 from_pos = new Vector3(12, -2, 0);  //开始coast的位置
    Vector3 to_pos = new Vector3(-12, -2, 0);   //终点coast的位置
    public Vector3[] From_positions = new Vector3[] { new Vector3(4.5F, 0.5F, 0), new Vector3(5.5F, 0.5F, 0) };
    public Vector3[] To_positions = new Vector3[] { new Vector3(-5.5F, 0.5F, 0), new Vector3(-4.5F, 0.5F, 0) };
    public Vector3 BoatPosition = new Vector3(5, 0, 0);
    public int priests = 3;
    public int devils = 3;
    int priest = 3; //记录更新后的priest数
    int devil = 3;  //记录更新后的devil数
    public int status = 0;  //记录游戏状态 1->win -1->lose 
    public string Pri = "priest";
    public string Dev = "devil";

    SSDirector director;
    public GameObject startCoast;
    public GameObject endCoast;
    public GameObject boat;
    public GameObject[] characters;
    public GameObject water;

    public int boatCapacity = 2;    //船的运载能力
    public int boatSide = 1;    //船的位置
    public int find = 0;
    public GameObject[] BoatItem = new GameObject[2];
    public GameObject[] startItem;
    public GameObject[] endItem;
    public int count = 0;

    public void loadResources()
    {
        GameObject temp = Instantiate(Resources.Load("Perfabs/Water", typeof(GameObject)), waterPosition, Quaternion.identity, null) as GameObject;
        water = temp;
        water.name = "water";
        startCoast = Instantiate(Resources.Load("Perfabs/Stone", typeof(GameObject)), from_pos, Quaternion.identity, null) as GameObject;
        startCoast.name = "start";
        endCoast = Instantiate(Resources.Load("Perfabs/Stone", typeof(GameObject)), to_pos, Quaternion.identity, null) as GameObject;
        endCoast.name = "end";
        boat = Instantiate(Resources.Load("Perfabs/Boat", typeof(GameObject)), BoatPosition, Quaternion.identity, null) as GameObject;
        boat.name = "boat";

        characters = new GameObject[priests + devils];
        startItem = new GameObject[priests + devils];
        endItem = new GameObject[priests + devils];

        for (int i = 0; i < priests; i++)
        {
            GameObject Pri = Instantiate(Resources.Load("Perfabs/Priest", typeof(GameObject)), Vector3.zero, Quaternion.identity, null) as GameObject;
            Pri.name = "priest" + i;
            Pri.transform.position = positions[i];
            characters[i] = Pri;
            startItem[i] = Pri;
        }

        for (int i = 0; i < devils; i++)
        {
            GameObject Dev = Instantiate(Resources.Load("Perfabs/Devil", typeof(GameObject)), Vector3.zero, Quaternion.identity, null) as GameObject;
            Dev.name = "devil" + i;
            Dev.transform.position = positions[i + priests];
            characters[i + priests] = Dev;
            startItem[i + priests] = Dev;
        }

        
    }

    public void reStart()
    {
        count = 0;
        boat.transform.position = BoatPosition;
        for (int i = 0; i < priests + devils; i++)
        {
            characters[i].transform.position = positions[i];
            startItem[i] = characters[i];
            endItem[i] = null;
        }
        boatCapacity = 2;
        boatSide = 1;
        for(int i = 0;i < 2; i++)
        {
            if (BoatItem[i] != null)
            {
                BoatItem[i].transform.parent = null;
                BoatItem[i] = null;
            }
        }
        find = 0;
    }

    public void reLoad(int pri, int dev)
    {
        reStart();
        for(int i = 0;i < priests + devils; i++)
        {
            Destroy(characters[i]);
        }
        characters = new GameObject[pri + dev];
        startItem = new GameObject[pri + dev];
        endItem = new GameObject[pri + dev];
        priests = pri;
        devils = dev;
        for (int i = 0; i < priests; i++)
        {
            GameObject Pri = Instantiate(Resources.Load("Perfabs/Priest", typeof(GameObject)), Vector3.zero, Quaternion.identity, null) as GameObject;
            Pri.name = "priest" + i;
            Pri.transform.position = positions[i];
            characters[i] = Pri;
            startItem[i] = Pri;
        }

        for (int i = 0; i < devils; i++)
        {
            GameObject Dev = Instantiate(Resources.Load("Perfabs/Devil", typeof(GameObject)), Vector3.zero, Quaternion.identity, null) as GameObject;
            Dev.name = "devil" + i;
            Dev.transform.position = positions[i + priests];
            characters[i + priests] = Dev;
            startItem[i + priests] = Dev;
        }
        
    }

    // Use this for initialization
    void Awake () {
        director = SSDirector.getInstance();
        director.currentSceneController = this;
        loadResources();
    }

    void OnGUI()
    {
        if (status == -1)
        {
            GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 85, 100, 50), "Gameover!");
        }
        else if (status == 1)
        {
            GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 85, 100, 50), "You win!");
        }
        if (GUI.Button(new Rect(Screen.width / 2 - 30, 2 * Screen.height / 3, 60, 60), "Restart"))
        {
            status = 0;
            reStart();
        }
        Pri = GUI.TextField(new Rect(80, 0, 60, 30), Pri);
        Dev = GUI.TextField(new Rect(80, 40, 60, 30), Dev);
        if (GUI.Button(new Rect(0, 0, 60, 60), "Change"))
        {
            int temp1 = 0;
            if (Pri != "priest" && int.TryParse(Pri, out temp1))
            {
                priest = temp1;
            }
            int temp2 = 0;
            if (Dev != "devil" && int.TryParse(Dev, out temp2))
            {
                devil = temp2;
            }
            status = 0;
            if (priest < devil)
                Debug.Log("Devils are more than Priests.");
            else if (priest + devil <= 12)
                reLoad(priest, devil);
            else
                Debug.Log("Too many characters.The sum of character must smaller than 13.");
        }
    }

    public void gameStatus()
    {
        int Ps = 0;
        int Pe = 0;
        int Ds = 0;
        int De = 0;

        for(int i = 0;i < priests + devils; i++)
        {
            if (i < priests) {
                if (characters[i].transform.position.x > 0)
                    Ps++;
                else
                    Pe++;
            }
            else
            {
                if (characters[i].transform.position.x > 0)
                    Ds++;
                else
                    De++;
            }
        }

        if(count == priests + devils)
        {
            status = 1;
            return;
        }

        if (Ps < Ds && Ps > 0)
        {
            status = -1;
            return;
        }
        else if (Pe < De && Pe > 0)
        {
            status = -1;
            return;
        }
        status =  0;
    }

    public Vector3 GetEmptyIndex()  //获得coast上的空位
    {
        if (boatSide == 1)
        {
            for (int i = 0; i < priests + devils; i++)
            {
                if (startItem[i] == null)
                {
                    return positions[i];
                }
            }
        }
        else
        {
            for (int i = 0; i < priests + devils; i++)
            {
                if (endItem[i] == null)
                {
                    Vector3 temp = positions[i];
                    temp.x = - temp.x;
                    return temp;
                }
            }
        }
        return new Vector3(0, 0, 0);
    }

}
