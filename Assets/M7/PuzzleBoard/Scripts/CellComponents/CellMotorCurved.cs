/*
 * TileMotor.cs
 * Author: Cristjan Lazar
 * Date: Oct 19, 2018
 */

using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using DG.Tweening;

namespace M7.Match
{
    public class CellMotorCurved : CellMotor
    {


        //[SerializeField] LerpRoutineData _animData;
        //[SerializeField] AnimationCurve _animCurve;

        //Vector3 _lastDesiredPosition;
        //Vector3 _lastStartPosition;



        //float activeLerpStep { get { return _animCurve.Evaluate(_animData.lerpStep); } }
       


        //IEnumerator _activeCoroutine;


        //public override void Move_TowardsCell(Vector3 tilePosition)
        //{
       
        //    _lastStartPosition = transform.position;
        //    _lastDesiredPosition = tilePosition;

        //    NewMove();
           
        //}


        //void NewMove()
        //{
        //    if (_activeCoroutine != null)
        //        StopCoroutine(_activeCoroutine);

        //    _activeCoroutine = MoveCoroutine();
        //    StartCoroutine(_activeCoroutine);
        //}

        //IEnumerator MoveCoroutine()
        //{

        //    _animData.Reinit();

        //    while (_animData.lerpStep < 1)
        //    {
        //        _animData.UpdateValues();
        //        Vector3 newPos  = Vector3.Lerp(_lastStartPosition, _lastDesiredPosition, activeLerpStep);
        //        target.position = newPos;

        //        yield return new WaitForSeconds(Time.deltaTime);
        //    }


        //}

		
	}





}
