using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetAccess : MonoBehaviour {
    public void UpdateAccessKey(string text) {
        AWSManager.accessKey = text;
    }

    public void UpdateSecretKey(string text) {
        AWSManager.accessKeySecret = text;
    } 
}
