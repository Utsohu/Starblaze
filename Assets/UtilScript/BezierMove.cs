using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ƶ����֮��Ļص�
/// </summary>
public delegate void BezierMoveCompleteDel();
public class BezierMove : Object /*MonoBehaviour*/
{

    /// <summary>
    /// ���������ߵ�·���������������Խ���õ�����Խ��������������Ҳ��Խ��
    /// </summary>
    private int _segmentNum = 100;


    /// <summary>
    /// ��ñ������������е�·����
    /// </summary>
    private Vector3[] _allposArry;

    /// <summary>
    /// �ƶ����ٶ�
    /// </summary>
    private float _speed = 10;
    /// <summary>
    /// ���е�·���������ֵ
    /// </summary>
    private int _index = 0;

    /// <summary>
    /// ��Ҫ�ƶ���Ŀ��
    /// </summary>
    private Transform _moveTargte;

    /// <summary>
    /// �Ƿ�����ƶ�
    /// </summary>
    private bool _isMoveComplete = false;
    /// <summary>
    ///  ��ʼ���������������е�·����
    /// </summary>
    /// <param name="startPos">��ʼ�ƶ���λ��</param>
    /// <param name="controlPos">�м���λ�ã�Ŀ���Ǹı������ƶ������߻���</param>
    /// <param name="endPos">�����ƶ���λ��</param>
    /// <param name="moveTarget">��Ҫ�ƶ���Ŀ������</param>
    /// <param name="moveSpeed">�ƶ��ٶ�</param>
    /// <param name="segmentNum">����֮��Bezier���������е������</param>
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
    ///  �ƶ�
    /// </summary>
    /// <param name="moveComplete">�ƶ���ɵĻص�</param>
    public void Move(BezierMoveCompleteDel moveComplete = null)
    {
        if (_allposArry == null || _allposArry.Length == 0)
        {
            Debug.LogError("û�г�ʼ�����������ߵ����е�·����");
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