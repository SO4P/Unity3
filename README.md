# 1、操作与总结
+ 参考 Fantasy Skybox FREE 构建自己的游戏场景
    + 在Assert Store下载资源，并利用地形工具进行修改
    ![scene](https://github.com/SO4P/Unity3/blob/master/3.2.png)
+ 写一个简单的总结，总结游戏对象的使用
    + 游戏对象：
        + Empty：
        最常用却不显示的对象
        + Audio：
        声音
        + Camera：
        玩家观察的摄像机
        + Light：
        光源
        + Object：
        用于构建场景的物体
# 2、编程实践
+ 在上次作业的基础上，参考了老师提供的博客的代码进行更改，维持了原有的功能：
    + 游戏结束后不接受用户输入，除了restart
    + 可以自定义priest和devil的数量，数量和最大为12
    + 物体移动可见移动轨迹
+ UML类图：
![UML](https://github.com/SO4P/Unity3/blob/master/3.1.png)
+ 代码：
    + 动作的基类和事件回调接口：

            public enum SSActionEventType : int { Started, Competeted }

            public interface ISSActionCallback
            {
                void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Competeted,
                int intParam = 0, string strParam = null, Object objectParam = null);
            }

            public class SSAction : ScriptableObject
            {
                public bool enable = true;
                public bool destroy = false;

                public GameObject gameobject { get; set; }
                public Transform transform { get; set; }
                public ISSActionCallback callback { get; set; }

                public virtual void Start()
                {
                    throw new System.NotImplementedException();
                }

                public virtual void Update()
                {
                    throw new System.NotImplementedException();
                }
            }
    + 具体动作：
        
            public class CCGetOnBoat : SSAction
            {   //上船动作
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
            {   //开船动作
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
                        sceneController.gameStatus();
                        this.destroy = true;
                        this.callback.SSActionEvent(this);
                    }
                }
            }

            public class CCGetOffBoat : SSAction
            {   //下船动作
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
    + 动作管理者基类

                public class SSActionManager : MonoBehaviour {
                private Dictionary<int, SSAction> actions = new Dictionary<int, SSAction>();
                private List<SSAction> waitingAdd = new List<SSAction>();
                private List<int> waitingDelete = new List<int>();

                // Use this for initialization
                void Start () {
		
	            }

                // Update is called once per frame
                protected void Update()
                {
                    foreach (SSAction ac in waitingAdd) actions[ac.GetInstanceID()] = ac;
                    waitingAdd.Clear();

                    foreach (KeyValuePair<int, SSAction> kv in actions)
                    {
                        SSAction ac = kv.Value;
                        if (ac.destroy)
                        {
                            waitingDelete.Add(ac.GetInstanceID());
                        }
                        else if (ac.enable)
                        {
                            ac.Update();
                        }
                    }

                    foreach (int key in waitingDelete)
                    {
                        SSAction ac = actions[key]; actions.Remove(key); DestroyObject(ac);
                    }
                    waitingDelete.Clear();
                }

                public void RunAction(GameObject gameobject, SSAction action, ISSActionCallback manager)
                {
                    action.gameobject = gameobject;
                    action.transform = gameobject.transform;
                    action.callback = manager;
                    waitingAdd.Add(action);
                    action.Start();
                }
            }
    + 管理具体过河动作派生类

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
    + GameObject管理类

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
    + 演示视频
    由于大小关系进行了压缩，解压名为“演示视频.zip”即可
    	
