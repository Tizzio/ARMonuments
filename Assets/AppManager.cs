﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif
using UnityEngine.UI;
using Vuforia;

public class AppManager : MonoBehaviour
{
    public Text debugLabel;

    //public Text label;
    //public GameObject prefab;
    //public Transform modelTarget;
    //public float force = 10;

    void Start ()
    {
        CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);

#if PLATFORM_ANDROID
        bool wait = false;
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            wait = true;
        }

        StartCoroutine(StartLocationService(wait));
#endif

    }


    private void Update ()
    {
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            //ThrowBall();
            //label.gameObject.SetActive(true);
        }
        
    }

    IEnumerator StartLocationService (bool wait)
    {
        if (wait)
        {
            debugLabel.text += "Waiting for authorization\n";
            yield return new WaitForSeconds(5.0f);
        }

        // First, check if user has location service enabled
        if (!Input.location.isEnabledByUser)
        {
            Debug.LogError("LocationService Not enabled");
            debugLabel.text += "LocationService Not enabled\n";
            //yield break;
        }
        else
        {
            Debug.Log("LocationService is enabled!");
            debugLabel.text += "LocationService is enabled!\n";
        }

        // Start service before querying location
        Input.location.Start();

        // Wait until service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            Debug.LogError("Timed out");
            debugLabel.text += "Timed out\n";
            //yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.LogError("Unable to determine device location");
            debugLabel.text += "Unable to determine device location\n";
            yield break;
        }
        else
        {
            while (true)
            {
                // Access granted and location value could be retrieved
                Debug.Log("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
                
                debugLabel.text = (Vector2.Distance( new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude), new Vector2(45.470765f, 10.893145f)) <00.00006f)? "fontana avis!": " non abbastanza vicino";
                yield return new WaitForSeconds(5.0f);
            }
        }

        // Stop service if there is no need to query location updates continuously
        Input.location.Stop();
    }
    //void ThrowBall()
    //{
    //    GameObject obj = Instantiate(prefab, transform.position, transform.rotation);
    //    obj.transform.SetParent(modelTarget, true);
    //    Rigidbody rb = obj.GetComponent<Rigidbody>();
    //    rb.AddForce(transform.forward * force, ForceMode.Impulse);
    //    Physics.gravity = -10 * modelTarget.transform.up;
    //}
}
