using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

//This is not my code, this was public code I'm reposting.   Along with php to receive the json

public class ClientLogon : MonoBehaviour
{

    public string User= "";
    public string Pass = "";
    public string Email = "";
    public string CreateUserURL = "https://yourwebsite.com/yourlogonscript.php";
    // Start is called before the first frame update
    void Start()
    {
        
    }

    IEnumerator Upload(string u, string p, string e)
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("field1=foo&field2=bar"));
        formData.Add(new MultipartFormFileSection("my file data", "myfile.txt"));

        UnityWebRequest www = UnityWebRequest.Post(CreateUserURL, formData);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Form upload complete!");
        }
    }


// Update is called once per frame
void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) { 
            StartCoroutine(Upload(User, Pass, Email));
        }
    }
}
