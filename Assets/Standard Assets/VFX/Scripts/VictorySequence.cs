using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class VictorySequence : MonoBehaviour
{


    public static VictorySequence Instance;


    public System.Action onAnyInput;


    
    [Header("Object References")]
    [SerializeField] ParticleSystem _victorySequence_Burst;
    [SerializeField] ParticleSystem _victorySequence_Loop;

    [SerializeField] GameObject _worldOverlay;
    [SerializeField] GameObject _victorySequence_StaticObjects;

    [SerializeField] Animation _animation;

    [Header("Delay Time")]
    [SerializeField] float _delayBeforeLoop;
    bool isCoroutineActive;

    IEnumerator activeCoroutine;


    void Awake()
    {
        Instance = this;
    }

    [Button]
    public void PlaySequence()
    {
        PlayNewCoroutine();
    }


    public void StopSequence()
    {

        isCoroutineActive = false;


        StopAllCoroutines();

        _animation.Stop();
        _victorySequence_Burst.gameObject.SetActive(false);
        _victorySequence_Loop.gameObject.SetActive(false);
        _victorySequence_StaticObjects.SetActive(false);
        _worldOverlay.SetActive(false);
    }

    void Update()
    {
        ListenForInput();
    }


    void ListenForInput()
    {

        if (!isCoroutineActive)
            return;

        if (Input.anyKeyDown)
        {
            if (onAnyInput != null)
            {
                onAnyInput();
                //Debug.Log("[OnAnyInput]");

                onAnyInput = null;
            }

            OnAnyInput_Default();
        }
    }
    void OnAnyInput_Default()
    {
        StopSequence();
       //Debug.Log("[OnAnyInputDefault]");
    }



    void PlayNewCoroutine()
    {



       
        StopSequence();


    

        isCoroutineActive = true;
        if (activeCoroutine != null)
            StopCoroutine(activeCoroutine);
        activeCoroutine = VictorySequenceCoroutine();
        StartCoroutine(activeCoroutine);
    }

    IEnumerator VictorySequenceCoroutine()
    {
        _worldOverlay.SetActive(true);
        _animation.Play("VictorySequencePopAnimation");
        _victorySequence_StaticObjects.SetActive(true);


        yield return new WaitForSeconds(_delayBeforeLoop);

        _victorySequence_Burst.gameObject.SetActive(true);
        _victorySequence_Burst.Play();
        //_victorySequence_Loop.gameObject.SetActive(true);
        //_victorySequence_Loop.Play();

        yield return new WaitWhile(() => _animation.isPlaying);
        _animation.Play("VictorySequenceIdleAnimation");

        yield return new WaitForSeconds(3);
        if (onAnyInput != null)
        {
            onAnyInput();
            //Debug.Log("[OnAnyInput]");

            onAnyInput = null;
        }

        OnAnyInput_Default();
    }


    void OnDestroy()
    {
        //Debug.Log("Victory Sequence was destroyed");
    }
}
