﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CinematicEffects;
public class EffectManager : MonoBehaviour
{

    public static EffectManager instance;
    public Camera mainCam;


    public Animator cameraEyesAnimator;

    public Bloom bloomScript;
    private float bloomIncrement = 4;
    private float bloomDelay = .1f;
    private float bloomStep = 40;


    //public MotionBlur motionBlurScript;
    public CustomImageEffect customImageEffectScript;
    public FlipImageCameraEffect flipEffectScript;
    public CameraShake cameraShakeEffect;

    private float focalIncrement = 10;
    private float focalDelay = .01f;
    private float focalStep = 120;


    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(PlayFocalEffect());
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            TurnEffectByIndex(0);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            TurnEffectByIndex(1);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            TurnEffectByIndex(2);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            TurnEffectByIndex(3);
        }

    }


    public void InvokeBloom() {

        StartCoroutine(PlayBoomEffect());
        StartCoroutine(CloseEyes());
        StartCoroutine(PlayRandomEffect());
    }


    private float effectDelay = 3;
    private float effectDuration = 10;

    private int tunnelIndex = 0;
    private IEnumerator PlayRandomEffect() {


        int randomEffectIndex = Random.Range(0, 2);
        //randomEffectIndex = 3;
        yield return new WaitForSeconds(Random.Range(0,effectDelay));
        TurnEffectByIndex(tunnelIndex);
        yield return new WaitForSeconds(Random.Range(4, effectDuration));
        tunnelIndex++;
        tunnelIndex = tunnelIndex % 4;
        TurnEffectByIndex(3);


    }

    private IEnumerator CloseEyes() {

        cameraEyesAnimator.SetTrigger("Close");
        yield return new WaitForSeconds(.5f);
        cameraEyesAnimator.SetBool("Open", true);
        yield return new WaitForSeconds(.5f);
        cameraEyesAnimator.SetBool("Open", false);


    }
    private IEnumerator PlayBoomEffect()
    {
        Debug.Log(bloomStep / 2 + "  sdsdsd ");
        int direction = 1;
        float value=0;
        int duration = (int)(bloomStep / bloomIncrement);

        for (int i = 0; i< duration; i++ ) {


            if (i == (int)(duration / 2))
            {
               
                direction = -1;
            }
            value += (bloomIncrement * direction);
            bloomScript.settings.intensity = value;

            yield return new WaitForSeconds(bloomDelay);
        }

    }

    private void TurnEffectByIndex(int index) {

        switch (index) {

            case 0:
                //motionBlurScript.enabled = true;
                customImageEffectScript.enabled = false;
                flipEffectScript.enabled = false;
            break;

            case 1:
                //motionBlurScript.enabled = false;
                customImageEffectScript.enabled = true;
                flipEffectScript.enabled = false;
            break;

          

            case 2:
                //motionBlurScript.enabled = false;
                customImageEffectScript.enabled = false;
                flipEffectScript.enabled = false;
                cameraShakeEffect.SetTrauma();
            break;

            case 3:
                //motionBlurScript.enabled = false;
                customImageEffectScript.enabled = false;
                flipEffectScript.enabled = false;
                break;


        }
        

        
     }

    private IEnumerator PlayFocalEffect()
    {
        
        int direction = 1;
        float value = 60;
        int duration = (int)(2*focalStep / focalIncrement);
        mainCam.nearClipPlane = 1;
        for (int i = 0; i < duration; i++)
        {


         
            value += (focalIncrement * direction);
            mainCam.fieldOfView = value;

            yield return new WaitForSeconds(focalDelay);
        }
        yield return new WaitForSeconds(2.5f);
        mainCam.nearClipPlane = .1f;
        for (int i = 0; i < duration; i++)
        {



            value -= (focalIncrement * direction);
            mainCam.fieldOfView = value;

            yield return new WaitForSeconds(focalDelay);
        }
    }

}
