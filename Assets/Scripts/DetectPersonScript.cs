

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.VR.WSA.WebCam;


public class DetectPersonScript : MonoBehaviour
{
    PhotoCapture _photoCaptureObject = null;
    public GameObject panelCurrentUser;
    private string filePath;
    private string imageString = "";
    string jsonResults;
    UserDtoCollection userDtoCollection;
    List<String> recognizedUsers;

    private string endpointForIdentification = "http://api-narato-kdg-stage-users-dev.azurewebsites.net/api/users/findUserHololens";
    private string endpointForLogging = "http://api-narato-kdg-stage-users-dev.azurewebsites.net/api/logs";

    // Use this for initialization
    void Start ()
    {
         filePath = System.IO.Path.Combine(Application.persistentDataPath, string.Format(@"user.jpg"));
        //This will call de analyse method every 10 seconds. Which lets us take a picture every 10 seconds.
        StartCoroutine(Analyse());
        recognizedUsers = new List<String>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator Analyse()
    {
        int secondsInterval = 10;
        while (true)
        {
            Initialize();
            yield return new WaitForSeconds(secondsInterval);
        }
    }


    private void Initialize()
    {
        PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);
    }

    //we store our object, set our parameters, and start Photo Mode
    void OnPhotoCaptureCreated(PhotoCapture captureObject)
    {
        _photoCaptureObject = captureObject;

        Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();

        CameraParameters c = new CameraParameters();
        c.hologramOpacity = 0.0f;
        c.cameraResolutionWidth = cameraResolution.width;
        c.cameraResolutionHeight = cameraResolution.height;
        c.pixelFormat = CapturePixelFormat.BGRA32;

        captureObject.StartPhotoModeAsync(c, OnPhotoModeStarted);
    }

    private void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result)
    {
        if (result.success)
        {
             _photoCaptureObject.TakePhotoAsync(filePath, PhotoCaptureFileOutputFormat.JPG, OnCapturedPhotoToDisk);   
        }
        else
        {
            Debug.LogError("Unable to start photo mode!");
        }
    }

    //save the photo, then get it back from the location where it was saved and send it via REST call to the discory API to get the photoID
    //With this photoid we can get the user to whom this photoid belongs by calling the hololensapi
    void OnCapturedPhotoToDisk(PhotoCapture.PhotoCaptureResult result)
    {
        if (result.success)
        {
            byte[] image  = File.ReadAllBytes(filePath);    
            GetUsers(image);
        }
        else
        {
            Debug.Log("Failed to save Photo to disk");
        }

        _photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
    }

    public void GetUsers(byte[] image)
    {
       StartCoroutine(UploadPhotoStringToBeDiscovered(image));
    }

    IEnumerator UploadPhotoStringToBeDiscovered(byte[] image)
    {
      
        imageString = Convert.ToBase64String(image);
        WWWForm form = new WWWForm();
        form.AddField("imageString", imageString);
        WWW www = new WWW(endpointForIdentification, form);
           yield return www;

        if (www.text.Equals(""))
        {
            gameObject.transform.SendMessage("UpdatePersonList", new UserDto[0]);
        }
		else
		{
			//jsonResults = "{ \"userdtos\": " + www.text + "}";
			userDtoCollection = JsonUtility.FromJson<UserDtoCollection>(www.text);
			gameObject.transform.SendMessage("UpdatePersonList", userDtoCollection.data); 

			foreach (UserDto user in userDtoCollection.data) {
				StartCoroutine(CreateLog(user.id));
			}
        }
    }

    IEnumerator CreateLog(string userId)
    {
        WWWForm form = new WWWForm();

        form.AddField("Timestamp", DateTime.Now.ToString());
        form.AddField("UserID", userId);

        WWW www = new WWW(endpointForLogging, form);
        yield return www;

        if (www.text.Equals(""))
        {
            //error logging
        }

    }

    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        _photoCaptureObject.Dispose();
        _photoCaptureObject = null;
    }
}
