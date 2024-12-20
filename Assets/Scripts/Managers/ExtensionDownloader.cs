using System.IO;
using UnityEngine;

public class ExtensionDownloader : MonoBehaviour
{
    private string fileName = "ChromeExtension.zip"; 

    public void DownloadExtension()
    {
        //FOR ANY UPDAWGS LOOKING AT THIS FILE
        //streamingAssets is a special Unity Folder dont change name pls!
        string sourcePath = Path.Combine(Application.streamingAssetsPath, fileName);


        string destinationPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile), "Downloads", fileName);

        try
        {
            // Copy the file from StreamingAssets to the destination
            if (File.Exists(destinationPath))
            {
                Debug.Log("File already exists. Overwriting...");
                File.Delete(destinationPath);
            }

            // Handle different platforms
            if (sourcePath.Contains("://")) // Android or WebGL platforms
            {
                StartCoroutine(DownloadStreamingAssetsFile(sourcePath, destinationPath));
            }
            else
            {
                File.Copy(sourcePath, destinationPath);
                Debug.Log($"Extension file copied to: {destinationPath}");
                Application.OpenURL("file://" + Path.GetDirectoryName(destinationPath));
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to download the extension: {ex.Message}");
        }
    }
    private System.Collections.IEnumerator DownloadStreamingAssetsFile(string sourcePath, string destinationPath)
    {
        using (WWW www = new WWW(sourcePath))
        {
            yield return www;

            if (string.IsNullOrEmpty(www.error))
            {
                File.WriteAllBytes(destinationPath, www.bytes);
                Debug.Log($"Extension file copied to: {destinationPath}");
                Application.OpenURL("file://" + Path.GetDirectoryName(destinationPath));
            }
            else
            {
                Debug.LogError($"Error downloading file: {www.error}");
            }
        }
    }
}