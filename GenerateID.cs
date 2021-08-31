using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;



public class GenerateID : MonoBehaviour
{
    public string IP;
    public GameObject Pass1;
    public GameObject Pass2;
    public GameObject Email;
    public GameObject SubmitButton;
    public Text Insts;

    public bool ReadyToSubmit = false;


    public string android_id;
    // Start is called before the first frame update


    public void CreateAccount()
    {
        SubmitButton.GetComponentInChildren<Button>().interactable = false;
        SubmitButton.GetComponentInChildren<Text>().text = "sending...";
        //get new json object
        CancelInvoke("checkInput");
        // loginObject = JsonUtility.FromJson<CreateAccount>(json);
       // Debug.Log("Attempting To upload");
        StartCoroutine(UploadInfo());
        Instructions = "Connecting to Server...";
    }

     IEnumerator UploadInfo()
    {

  
       JsonObjectData loginObject = new JsonObjectData();
        loginObject.email = Email.GetComponentInChildren<Text>().text.ToString();
        loginObject.password = Pass1.GetComponentInParent<InputField>().text;
        loginObject.androidID = android_id; //if available for mobile android phones.
        loginObject.logon = "1";
        loginObject.iP = IP; //the IP of client
        loginObject.pid = 0;  //recieved by server after success
        loginObject.error = "";
        loginObject.gameState = 0;

        //create a json string.


        string json = JsonUtility.ToJson(loginObject);

        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        
       // formData.Add(new MultipartFormDataSection("logon=1&json="+json));

       // formData.Add(new MultipartFormFileSection("my file data", "myfile.txt"));

        UnityWebRequest www = new UnityWebRequest("https://yourwebsite.com/yourfile.php", "POST");
        Debug.Log("json data is "+json); //comment out when finished...
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
           
            Debug.Log("There was a problemo "+www.error); 
            Instructions = "Could Not Contact the Server, Check your Internet Connection";
            SubmitButton.GetComponentInChildren<Button>().interactable = false;
            InvokeRepeating("checkInput", 0.7f, 0.5f);
        }
        else
        {
            
            Debug.Log("Status Code: " + www.responseCode);
            Debug.Log("Response" + www.downloadHandler.text);

            //Get Json Data and change value of the logonOjects variables replaced with this new info.
            loginObject = JsonUtility.FromJson<JsonObjectData>(www.downloadHandler.text);
    



            if (loginObject.error != "") //There was an error...
            {
                Instructions = loginObject.error;
                Email.GetComponentInParent<InputField>().text = "";
                SubmitButton.GetComponentInChildren<Button>().interactable = false;
              
                InvokeRepeating("checkInput", 1.0f, 0.2f);
               
            }
            if(loginObject.pid != 0)
            {

                CancelInvoke("checkInput");
                Instructions = "Success!";
                SubmitButton.GetComponentInChildren<Button>().interactable = false;
                //We're successful now go to the next screen.


            }
           
            // Debug.Log("Upload Complete new player ID is");
        }
        

    }

    //is a valid email?
    public bool IsValidEmail(string email)
    {
        const string pattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|" + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)" + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";

        var regex = new Regex(pattern, RegexOptions.IgnoreCase);

        //Debug.Log("Regexing Email" + email);

        return regex.IsMatch(email);
    }

    public string Instructions = "Enter a password over 6 characters long";

    public void checkInput()
    {
   
        if (IsValidEmail(Email.GetComponentInChildren<Text>().text.ToString()))
        {
      
            if (Pass1.GetComponentInChildren<Text>().text.Length > 6)
            {


                if (Pass1.GetComponentInChildren<Text>().text == Pass2.GetComponentInChildren<Text>().text)
                {

                    //EVERYTHING IS AWESOME
                    Instructions = "Passwords Match";
                    SubmitButton.GetComponentInChildren<Button>().interactable = true;
                    SubmitButton.GetComponentInChildren<Text>().text = "Create Account";
                

                }
                else
                {
                    Instructions = "Enter the password again";
                    SubmitButton.GetComponentInChildren<Button>().interactable = false;
                    SubmitButton.GetComponentInChildren<Text>().text = "Waiting on Input";
                }


            }
            else
            {

                SubmitButton.GetComponentInChildren<Button>().interactable = false;
                SubmitButton.GetComponentInChildren<Text>().text = "Waiting on Input";

                Instructions = "Enter a password over 6 characters long";
                SubmitButton.GetComponentInChildren<Button>().interactable = false;
            }





        }
        else
        {

            SubmitButton.GetComponentInChildren<Button>().interactable = false;
            SubmitButton.GetComponentInChildren<Text>().text = "Waiting on Input";

            SubmitButton.GetComponentInChildren<Button>().interactable = false;
            Instructions = "Enter a valid email address";

        }

        

    }



    void Start()
    {
      
        //tries to get an IP address...
        try
        {
            IP = NetworkManager.singleton.networkAddress;
            AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = up.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject contentResolver = currentActivity.Call<AndroidJavaObject>("getContentResolver");
            AndroidJavaClass secure = new AndroidJavaClass("android.provider.Settings$Secure");
            android_id = secure.CallStatic<string>("getString", contentResolver, "android_id");
        }catch
        {
            //We failed so just create dummy data..
            IP = "0.0.0.0";
            android_id = "0";

        }

        InvokeRepeating("checkInput", 0.4f, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {

        Insts.text = Instructions;
        
      //  string json = JsonUtility.ToJson(loginObject);

    }
}



[Serializable]
public class JsonObjectData
{
    public string email;
    public string password;
    public string androidID;
    public string logon;
    public int pid;
    public string skey;
    public string iP;
    public string ActiveServer;
    public string error;
    public int gameState;
   
}
