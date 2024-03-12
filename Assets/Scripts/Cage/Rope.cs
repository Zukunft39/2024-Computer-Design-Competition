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

    private void Start()
    {
        rope = transform;
        hook = rope.GetChild(0);
        length = 1;
        state = State.None;
        currentMaxLength = maxLength1;
        hookScale = hook.localScale.y;
        manager = FindObjectOfType<CageManager>();
    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space)) state = State.Stretch;


        if (state == State.Shorten) Shorten();
        else if(state == State.None) Rotate();
        else if(state == State.Stretch) Stretch();
        Switch();
    }
    //旋转
    private void Rotate()
    { 
        float rotate = Input.GetAxisRaw("Horizontal");
        Vector3 vector = new Vector3(0, 0, -rotate);
        if (Mathf.Abs(rope.localRotation.z) >= minAngle)
        {
            rope.Rotate(vector * 60 * Time.deltaTime);

        }
        else
        {
            Quaternion temp = transform.localRotation;
            if (rope.localRotation.z < minAngle) temp.z = minAngle;
            else if (rope.localRotation.z > -minAngle) temp.z = -minAngle;
            transform.localRotation = temp;
        }
    }

    //拉伸
    private void Stretch()
    {
#if UNITY_EDITOR
        Debug.Log("拉伸");
#endif
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
            if (hook.childCount != 0)
            {
                manager.AddNum(hook.GetChild(0).tag);
            }
            hook.localScale = new Vector3(hook.localScale.x, hookScale / length , hook.localScale.z);
            state = State.None;
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
            if (currentMaxLength == maxLength1) currentMaxLength = maxLength2;
            else if (currentMaxLength == maxLength2) currentMaxLength = maxLength1;
        }
    }
}
