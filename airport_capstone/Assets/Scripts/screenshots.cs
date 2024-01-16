using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class screenshots : MonoBehaviour
{
    // Variables for screenshot resolution, file number, and timing
    int resWidth;
    int resHeight;
    public static int filenumber = 1; //
    public float video_end = 34.3f; // End of the screenshots in seconds
    public float video_start = 1.6f; // Start of the screenshots in seconds
    public float saveInterval = 0.5f; // Time between two screenshots in seconds
    int type;
    // List to store GameObjects with specific tags for labeling
    List<GameObject> objectsToLabel = new List<GameObject>(); 


    void Start()
    {
        // Tags of objects to include in labeling
        string[] tagsToInclude = new string[] { "ULD", "Door", "EmptyDolly", "Speedloader", "Highloader", "TUG", "Pallet", "HighloaderDown" };

        foreach (string tag in tagsToInclude)
        {
            GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(tag);
            objectsToLabel.AddRange(objectsWithTag);
        }

        StartCoroutine(SaveFile());
    }

    // Generate a unique screenshot name based on file number
    public static string ScreenShotName()
    {
        filenumber++;
        return Application.dataPath + "/screenshots/" + filenumber.ToString() + ".png";

    }

    // Draw Gizmos for labeled objects in the Scene view
    void OnDrawGizmos()
    {
        foreach (GameObject objectToLabel in objectsToLabel)
        {
            var renderers = objectToLabel.GetComponentsInChildren<Renderer>();

            if (renderers != null && renderers.Length > 0)
            {
                var bounds = renderers[0].bounds;

                for (var i = 1; i < renderers.Length; ++i)
                {
                    bounds.Encapsulate(renderers[i].bounds);
                }

                Gizmos.color = Color.red; // Set Gizmos color

                // Draw wire cube Gizmo around the bounding box
                Gizmos.DrawWireCube(bounds.center, bounds.size);

                // Draw a sphere at the center of the bounding box
                Gizmos.color = Color.green; // Set color for the center
                Gizmos.DrawSphere(bounds.center, 0.1f); // Adjust the radius as needed

                // Draw the ray from the camera to the object center
                GameObject taggedCamera = GameObject.FindWithTag("picCam");

                if (taggedCamera != null)
                {
                    Gizmos.color = Color.blue; // Set color for the ray
                    Gizmos.DrawLine(taggedCamera.transform.position, bounds.center);
                }
            }
        }
    }

    // Return the greatest value between actual_value and the distance between two points
    public float getHeightWidth(float actual_value, float point1, float point2)
    {
        float tmp_value = Mathf.Abs(point1 - point2);

        if (actual_value < tmp_value)
        {
            actual_value = tmp_value;
        }

        return actual_value;
    }

    // Coroutine to save screenshots and labels
    private IEnumerator SaveFile()
    {
        int it = 0;
        int nb_it = Mathf.FloorToInt((video_end - video_start) / saveInterval)+1; // Number of iteration to do
        yield return new WaitForSeconds(video_start); 
        while (it < nb_it)
        {
            foreach (GameObject go in GameObject.FindGameObjectsWithTag("picCam")) //for only one camera, comment this line
            {
                Camera cam = go.GetComponent<Camera>(); //for only one camera, comment this line
                //Camera cam = GetComponent<Camera>(); for  only one camera, decomment this line
                int multiplier = 4; // Quality of the screenshot (multiply the number of pixel in width and height)
                resWidth = cam.pixelWidth * multiplier;
                resHeight = cam.pixelHeight * multiplier;

                // Create a RenderTexture with higher bit depth for better quality
                RenderTexture rt = new RenderTexture(resWidth, resHeight, 32);
                cam.targetTexture = rt;
                cam.Render();

                // Create a higher-quality texture
                Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGBA32, false);

                // Read pixels from the RenderTexture
                RenderTexture.active = rt;
                screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
                screenShot.Apply();

                // Encode the texture to PNG
                byte[] bytes = screenShot.EncodeToPNG();

                // Save the image
                string screenname = ScreenShotName();
                System.IO.File.WriteAllBytes(screenname, bytes);

                // Identification of objects present in the screenshot
                foreach (GameObject objectToLabel in objectsToLabel)
                {
                    var renderers = objectToLabel.GetComponentsInChildren<Renderer>();

                    if (renderers != null && renderers.Length > 0)
                    {
                        var bounds = renderers[0].bounds;

                        for (var i = 1; i < renderers.Length; ++i)
                        {
                            bounds.Encapsulate(renderers[i].bounds);
                        }

                        // Position of the object on the screenshot
                        Vector3 object_center = bounds.center;
                        Vector3 viewPos = cam.WorldToViewportPoint(object_center);
                        Vector3 object_extents = bounds.extents;

                        float x1 = object_center.x + object_extents.x;
                        float x2 = object_center.x - object_extents.x;
                        float y1 = object_center.y + object_extents.y;
                        float y2 = object_center.y - object_extents.y;
                        float z1 = object_center.z + object_extents.z;
                        float z2 = object_center.z - object_extents.z;

                        Vector3 point1 = new Vector3(x1, y1, z1);
                        Vector3 point2 = new Vector3(x1, y1, z2);
                        Vector3 point3 = new Vector3(x1, y2, z1);
                        Vector3 point4 = new Vector3(x1, y2, z2);
                        Vector3 point5 = new Vector3(x2, y1, z1);
                        Vector3 point6 = new Vector3(x2, y1, z2);
                        Vector3 point7 = new Vector3(x2, y2, z1);
                        Vector3 point8 = new Vector3(x2, y2, z2);

                        // Position of vertex of the object bound on the screenshot
                        Vector3 screenPoint1 = cam.WorldToViewportPoint(point1);
                        Vector3 screenPoint2 = cam.WorldToViewportPoint(point2);
                        Vector3 screenPoint3 = cam.WorldToViewportPoint(point3);
                        Vector3 screenPoint4 = cam.WorldToViewportPoint(point4);
                        Vector3 screenPoint5 = cam.WorldToViewportPoint(point5);
                        Vector3 screenPoint6 = cam.WorldToViewportPoint(point6);
                        Vector3 screenPoint7 = cam.WorldToViewportPoint(point7);
                        Vector3 screenPoint8 = cam.WorldToViewportPoint(point8);

                        // Dimention of the object on the screenshot (compare the size between each vertex)
                        float final_width = Mathf.Abs(screenPoint1.x - screenPoint2.x);
                        float final_height = Mathf.Abs(screenPoint1.y - screenPoint2.y);
                        final_width = getHeightWidth(final_width, screenPoint1.x, screenPoint3.x);
                        final_height = getHeightWidth(final_height, screenPoint1.y, screenPoint3.y);
                        final_width = getHeightWidth(final_width, screenPoint1.x, screenPoint4.x);
                        final_height = getHeightWidth(final_height, screenPoint1.y, screenPoint4.y);
                        final_width = getHeightWidth(final_width, screenPoint1.x, screenPoint5.x);
                        final_height = getHeightWidth(final_height, screenPoint1.y, screenPoint5.y);
                        final_width = getHeightWidth(final_width, screenPoint1.x, screenPoint6.x);
                        final_height = getHeightWidth(final_height, screenPoint1.y, screenPoint6.y);
                        final_width = getHeightWidth(final_width, screenPoint1.x, screenPoint7.x);
                        final_height = getHeightWidth(final_height, screenPoint1.y, screenPoint7.y);
                        final_width = getHeightWidth(final_width, screenPoint1.x, screenPoint8.x);
                        final_height = getHeightWidth(final_height, screenPoint1.y, screenPoint8.y);
                        final_width = getHeightWidth(final_width, screenPoint2.x, screenPoint3.x);
                        final_height = getHeightWidth(final_height, screenPoint2.y, screenPoint3.y);
                        final_width = getHeightWidth(final_width, screenPoint2.x, screenPoint4.x);
                        final_height = getHeightWidth(final_height, screenPoint2.y, screenPoint4.y);
                        final_width = getHeightWidth(final_width, screenPoint2.x, screenPoint5.x);
                        final_height = getHeightWidth(final_height, screenPoint2.y, screenPoint5.y);
                        final_width = getHeightWidth(final_width, screenPoint2.x, screenPoint6.x);
                        final_height = getHeightWidth(final_height, screenPoint2.y, screenPoint6.y);
                        final_width = getHeightWidth(final_width, screenPoint2.x, screenPoint7.x);
                        final_height = getHeightWidth(final_height, screenPoint2.y, screenPoint7.y);
                        final_width = getHeightWidth(final_width, screenPoint2.x, screenPoint8.x);
                        final_height = getHeightWidth(final_height, screenPoint2.y, screenPoint8.y);
                        final_width = getHeightWidth(final_width, screenPoint3.x, screenPoint4.x);
                        final_height = getHeightWidth(final_height, screenPoint3.y, screenPoint4.y);
                        final_width = getHeightWidth(final_width, screenPoint3.x, screenPoint5.x);
                        final_height = getHeightWidth(final_height, screenPoint3.y, screenPoint5.y);
                        final_width = getHeightWidth(final_width, screenPoint3.x, screenPoint6.x);
                        final_height = getHeightWidth(final_height, screenPoint3.y, screenPoint6.y);
                        final_width = getHeightWidth(final_width, screenPoint3.x, screenPoint7.x);
                        final_height = getHeightWidth(final_height, screenPoint3.y, screenPoint7.y);
                        final_width = getHeightWidth(final_width, screenPoint3.x, screenPoint8.x);
                        final_height = getHeightWidth(final_height, screenPoint3.y, screenPoint8.y);
                        final_width = getHeightWidth(final_width, screenPoint4.x, screenPoint5.x);
                        final_height = getHeightWidth(final_height, screenPoint4.y, screenPoint5.y);
                        final_width = getHeightWidth(final_width, screenPoint4.x, screenPoint6.x);
                        final_height = getHeightWidth(final_height, screenPoint4.y, screenPoint6.y);
                        final_width = getHeightWidth(final_width, screenPoint4.x, screenPoint7.x);
                        final_height = getHeightWidth(final_height, screenPoint4.y, screenPoint7.y);
                        final_width = getHeightWidth(final_width, screenPoint4.x, screenPoint8.x);
                        final_height = getHeightWidth(final_height, screenPoint4.y, screenPoint8.y);
                        final_width = getHeightWidth(final_width, screenPoint5.x, screenPoint6.x);
                        final_height = getHeightWidth(final_height, screenPoint5.y, screenPoint6.y);
                        final_width = getHeightWidth(final_width, screenPoint5.x, screenPoint7.x);
                        final_height = getHeightWidth(final_height, screenPoint5.y, screenPoint7.y);
                        final_width = getHeightWidth(final_width, screenPoint5.x, screenPoint8.x);
                        final_height = getHeightWidth(final_height, screenPoint5.y, screenPoint8.y);
                        final_width = getHeightWidth(final_width, screenPoint6.x, screenPoint7.x);
                        final_height = getHeightWidth(final_height, screenPoint6.y, screenPoint7.y);
                        final_width = getHeightWidth(final_width, screenPoint6.x, screenPoint8.x);
                        final_height = getHeightWidth(final_height, screenPoint6.y, screenPoint8.y);
                        final_width = getHeightWidth(final_width, screenPoint7.x, screenPoint8.x);
                        final_height = getHeightWidth(final_height, screenPoint7.y, screenPoint8.y);
                        
                        // Check if the object is on the screenshot and great enough to label it
                        if (viewPos.x <= 1 && viewPos.x >= 0 && viewPos.y <= 1 && viewPos.y >= 0 && final_height >= 0.01 && final_width >= 0.01 )
                        {
                            bool done = false;
                            // Add the object to the label according to its tag
                            string labelname = Application.dataPath + "/labels/" + filenumber.ToString() + ".txt";

                            // Objects that cannot be hidden by the plane
                            if (objectToLabel.CompareTag("Highloader"))
                            {
                                type = 4;
                                done = true;
                            }
                            else if (objectToLabel.CompareTag("Door"))
                            {
                                type = 1;
                                done = true;
                            }
                            else if (objectToLabel.CompareTag("HighloaderDown"))
                            {
                                type = 7;
                                done = true;
                            }

                            if (done)
                            {
                                // If the file does not exist, create it
                                using (StreamWriter writer = new StreamWriter(labelname, true))
                                {
                                    string line = type + " " + viewPos.x.ToString() + " " + (1-viewPos.y).ToString() + " " + final_width.ToString() + " " + final_height.ToString();
                                    writer.WriteLine(line);
                                }
                            }
                            else
                            {
                                RaycastHit hit;

                                // Check if the object is hidden by the plane
                                if (Physics.Raycast(cam.transform.position, object_center - cam.transform.position, out hit))
                                {

                                    if (hit.collider.gameObject.tag != "Plane" && hit.collider.gameObject.tag != "Door")
                                    {
                                        if (objectToLabel.CompareTag("ULD"))
                                        {
                                            type = 0;
                                        }

                                        else if (objectToLabel.CompareTag("EmptyDolly"))
                                        {
                                            type = 2;
                                        }

                                        else if (objectToLabel.CompareTag("Speedloader"))
                                        {
                                            type = 3;
                                        }
                                    
                                        else if (objectToLabel.CompareTag("TUG"))
                                        {
                                            type = 5;
                                        }

                                        else if (objectToLabel.CompareTag("Pallet"))
                                        {
                                            type = 6;
                                        }
                                        
                                        // If the file does not exist, create it
                                        using (StreamWriter writer = new StreamWriter(labelname, true))
                                        {
                                            string line = type + " " + viewPos.x.ToString() + " " + (1-viewPos.y).ToString() + " " + final_width.ToString() + " " + final_height.ToString();
                                            writer.WriteLine(line);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                
                // Release resources and increment iteration
                cam.targetTexture = null;
                RenderTexture.active = null;
                rt.Release();
            }
            it = it + 1;
            yield return new WaitForSeconds(saveInterval);
        }
    }
}
   