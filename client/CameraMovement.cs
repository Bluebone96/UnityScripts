
using UnityEngine;
using System.Collections;
 
public class CameraMovement : MonoBehaviour {
 
    public float smooth = 1.5f;
    public Transform player;
    public Vector3 relCameraPos; //相对位置摄像机对人物
    private float relCameraPosMag; //摄像机和人物的距离
    private Vector3 newPos;  //摄像机试着抵达的位置
    
    void Awake()
    {
        //player = gameObject.transform.parent;
        player = GameObject.FindGameObjectWithTag("MainPlayer").transform;
        //摄像机相对位置 = 摄像机位置 - 玩家位置
        // relCameraPos = transform.position - player.position + new Vector3(10, 5, 10);
        relCameraPos = new Vector3(0, 2.5f, -6);
        //实际向量长度-0.5 小一点
        relCameraPosMag = relCameraPos.magnitude - 0.5f;
 
    }
 
    void FixedUpdate()
    {
 
        //摄像机的初始位置 = 玩家位置 + 相对位置
        Vector3 standardPos = player.position + relCameraPos;
        //摄像机的俯视位置 = 玩家位置 + 玩家正上方 * 相对位置向量长度
        Vector3 abovePos = player.position + Vector3.up * relCameraPosMag;
 
        Vector3[] checkPoints = new Vector3[5];
 
        checkPoints[0] = standardPos;
 
        checkPoints[1] = Vector3.Lerp(standardPos, abovePos, 0.25f);
        checkPoints[2] = Vector3.Lerp(standardPos, abovePos, 0.5f);
        checkPoints[3] = Vector3.Lerp(standardPos, abovePos, 0.75f);
 
 
        checkPoints[4] = abovePos;
 
        for (int i = 0;i<checkPoints.Length;i++)
        {
            if (ViewingPosCheck(checkPoints[i]))
            {
                break;
            }
        }
 
        transform.position = Vector3.Lerp(transform.position, newPos, smooth * Time.deltaTime);
 
        SmoothLookAt();
    }
 
 
    //使用光线投射的方法检测摄像机能否投射碰撞到玩家 
    bool ViewingPosCheck(Vector3 checkPos)
    {
 
        RaycastHit hit;
 
        if (Physics.Raycast(checkPos,player.position-checkPos,out hit,relCameraPosMag))
        {
            if (hit.transform!=player)
            {
                return false;
            }
        }
 
        newPos = checkPos;
        return true;
    }
    //使摄像机函数在移动过程中始终面对玩家
    void SmoothLookAt()
    {
        Vector3 relPlayPosition = player.position - transform.position;
 
        Quaternion lookAtRotation = Quaternion.LookRotation(relPlayPosition, Vector3.up);
 
        transform.rotation = Quaternion.Lerp(transform.rotation, lookAtRotation, smooth * Time.deltaTime);
    }
}
 
