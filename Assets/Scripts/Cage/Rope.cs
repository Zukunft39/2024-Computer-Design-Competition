using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State
{
    Stretch,//伸长
    Shorten,//缩短
    None,//静止
}
public class Rope : MonoBehaviour
{
    [SerializeField] private State state;
    private Transform rope;
    private Transform hook;
    private float hookScale;
    [SerializeField] private float currentMaxLength;
    [Tooltip("绳子长度")]
    [SerializeField] private float length;
    [Tooltip("绳子拉伸速度")]
    [SerializeField] private float speed;
    [Tooltip("第一阶段最长长度")]
    [SerializeField] private float maxLength1;//轻
    [Tooltip("第二阶段最长长度")]
    [SerializeField] private float maxLength2;//重
    [Tooltip("旋转角度")]
    [SerializeField] private float minAngle;

    CageManager manager;
    ObjectPooler pooler;

    public State GetState
    {
        set { state = value; }
        get { return state; }
    }

    private void Start()
    {
        rope = transform;
        hook = rope.GetChild(0);
        length = 1;
        state = State.None;
        currentMaxLength = maxLength1;
        hookScale = hook.localScale.y;
        manager = FindObjectOfType<CageManager>();
        pooler = ObjectPooler.Instance;
    }
    private void Update()
    {

        if (state == State.Shorten) Shorten();
        else if(state == State.None)
        {
            if (Input.GetKeyUp(KeyCode.Space)){
                state = State.Stretch;
                MainAudioManager.AudioManagerInstance.PlaySFXScene("Rope");
            }
            Rotate();
            Switch();
        }
        else if(state == State.Stretch) Stretch();
    }
    //旋转
    private void Rotate()
    {
        float rotate = Input.GetAxisRaw("Horizontal");
        Vector3 vector = new Vector3(0, 0, -rotate);
        Quaternion ropeRotation = rope.rotation;
        float currentAngle = ropeRotation.eulerAngles.z;
        if (Mathf.Abs(currentAngle - 180f) <= minAngle)
        {
            rope.Rotate(vector * 60 * Time.deltaTime);
        }
        else
        {
            Quaternion temp = transform.rotation;
            if (currentAngle - 180f > minAngle) temp.eulerAngles = new Vector3(temp.eulerAngles.x, temp.eulerAngles.y, minAngle + 180f);
            else if (currentAngle - 180f < -minAngle) temp.eulerAngles = new Vector3(temp.eulerAngles.x, temp.eulerAngles.y, -minAngle + 180f);
            transform.rotation = temp;
        }
    }

    //拉伸
    private void Stretch()
    {   
        if (length >= currentMaxLength)
        {
            state = State.Shorten;
            return;
        }
        length += Time.deltaTime * speed;
        rope.localScale = new Vector3(rope.localScale.x, length, rope.localScale.z);
        hook.localScale = new Vector3(hook.localScale.x, hookScale / length, hook.localScale.z);
    }

    //缩短
    private void Shorten()
    {
        if (length <= 1)
        {
            length = 1;
            state = State.None;
            if (hook.childCount != 0)
            {
                manager.AddNum(hook.GetChild(0).tag);
                //Debug.Log(hook.GetChild(0).tag);
                //摧毁子物体
                Move move = hook.GetChild(0).GetComponent<Move>();
#if UNITY_EDITOR
                if (move == null) Debug.LogError("" + hook.GetChild(0).name + "have no Move Script");
#endif
                //笼子生成目标
                manager.Catch(hook.GetChild(0).tag);
                //回收子物体
                pooler.Recover(hook.GetChild(0).gameObject, hook.GetChild(0).tag);
                hook.GetChild(0).gameObject.transform.SetParent(move.GetTransform(), false);
                move.enabled = true;
                move.jumpTimer = 0;
            }
            hook.GetComponent<Collider2D>().enabled = true;
            rope.localScale = new Vector3(rope.localScale.x, length, rope.localScale.z);
            hook.localScale = new Vector3(hook.localScale.x, hookScale / length, hook.localScale.z);
            return;
        }
        length -= Time.deltaTime * speed;
        rope.localScale = new Vector3(rope.localScale.x, length, rope.localScale.z);
        hook.localScale = new Vector3(hook.localScale.x, hookScale / length, hook.localScale.z);

    }

    //切换
    private void Switch()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (currentMaxLength == maxLength1)
            {
                currentMaxLength = maxLength2;
                manager.arrowIma.transform.position = new Vector3(manager.heavyIma.transform.position.x, manager.arrowIma.transform.position.y, manager.arrowIma.transform.position.z);
            }
            else if (currentMaxLength == maxLength2)
            {
                currentMaxLength = maxLength1;
                manager.arrowIma.transform.position = new Vector3(manager.lightIma.transform.position.x, manager.arrowIma.transform.position.y, manager.arrowIma.transform.position.z);
            }
        }
    }

    public bool CheckLength()
    {
        if (currentMaxLength == maxLength1) return true;
        return false;
    }
}
