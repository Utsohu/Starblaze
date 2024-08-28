using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 移动完成之后的回调
/// </summary>
public delegate void BezierMoveCompleteDel();
public class BezierMove : Object /*MonoBehaviour*/
{

    /// <summary>
    /// 贝赛尔曲线的路径点的数量，数量越多获得的曲线越完美，但是消耗也就越大
    /// </summary>
    private int _segmentNum = 100;


    /// <summary>
    /// 获得贝赛尔曲线所有的路径点
    /// </summary>
    private Vector3[] _allposArry;

    /// <summary>
    /// 移动的速度
    /// </summary>
    private float _speed = 10;
    /// <summary>
    /// 所有的路径点的索引值
    /// </summary>
    private int _index = 0;

    /// <summary>
    /// 需要移动的目标
    /// </summary>
    private Transform _moveTargte;

    /// <summary>
    /// 是否完成移动
    /// </summary>
    private bool _isMoveComplete = false;
    /// <summary>
    ///  初始化贝赛尔曲线所有的路径点
    /// </summary>
    /// <param name="startPos">开始移动的位置</param>
    /// <param name="controlPos">中间点的位置，目的是改变物体移动的曲线弧度</param>
    /// <param name="endPos">最终移动的位置</param>
    /// <param name="moveTarget">需要移动的目标物体</param>
    /// <param name="moveSpeed">移动速度</param>
    /// <param name="segmentNum">三点之间Bezier曲线上所有点的数量</param>
    public void Init(Vector3 startPos, Vector3 controlPos, Vector3 endPos, Transform moveTarget, float moveSpeed, int segmentNum = 100)
    {
        _moveTargte = moveTarget;
        _speed = moveSpeed;
        _allposArry = new Vector3[segmentNum];
        _segmentNum = segmentNum;
        _allposArry = BezierUtils.GetBeizerList(startPos, controlPos, endPos, _segmentNum);
        _isMoveComplete = false;
    }

    /// <summary>
    ///  移动
    /// </summary>
    /// <param name="moveComplete">移动完成的回调</param>
    public void Move(BezierMoveCompleteDel moveComplete = null)
    {
        if (_allposArry == null || _allposArry.Length == 0)
        {
            Debug.LogError("没有初始化贝赛尔曲线的所有的路径点");
            return;
        }
        if (!_isMoveComplete)
        {
            if (Vector3.Distance(_moveTargte.position, _allposArry[_index]) <= 0.1f)
            {
                _index++;
                if (_index > _segmentNum - 1)
                {
                    _index = _segmentNum - 1;
                    if (moveComplete != null)
                    {
                        moveComplete();

                    }
                    _isMoveComplete = true;
                }
            }
            else
            {
                _moveTargte.position = Vector3.Slerp(_moveTargte.position, _allposArry[_index], _speed * Time.deltaTime);
                int n = _index;
                if (_index < _segmentNum - 1) n = _index + 1;
                float x = _allposArry[n].x - _moveTargte.position.x;
                float y = _allposArry[n].y - _moveTargte.position.y;
                float delta = Mathf.Atan(x / y) * Mathf.Rad2Deg * -1;
                if (/*x > 0 && */y < 0) 
                    delta += 180;

                _moveTargte.rotation = Quaternion.Euler(0f, 0.0f, delta);
            }
        }

    }
}