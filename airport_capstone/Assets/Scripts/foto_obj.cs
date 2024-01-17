using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class foto_obj : MonoBehaviour
{
    // Variables for screenshot resolution, file number, camera and a list of objects 
    public GameObject[] obj;
    public Camera cam;
    int resWidth, resHeight, type;
    public static int filenumber = 1;

    void Start()
    {
        StartCoroutine(random());
    }

    // Loop to take a screenshot of each object 'nb_pics' time positionned randomly on the map
    IEnumerator random()
    {
        // Variables for the random position of the object, for the camera, and the number of screenshot per object 
        float x, y, z, xcam, ycam, zcam;
        Transform target;
        int nb_pics = 150;

        for (int j=0; j < nb_pics; j++)
        {
            for (int i = 0; i < obj.Length; i++)
            {
                // Object 'obj[i]' is positionned randomly on the map on the floor
                x = Random.Range(-30.0f, 430.0f);
                y = obj[i].transform.position.y;
                z = Random.Range(-240.0f, 150.0f);
                obj[i].transform.position = new Vector3(x, y, z);
                target = obj[i].transform;

                // camera placed randomly around the object
                xcam = Random.Range(-12.0f, 12.0f);
                ycam = Random.Range(1.0f, 14.0f);
                zcam = Random.Range(-12.0f, 12.0f);

                if (ycam < 4.0 && xcam < 6.0 && zcam < 6.0)
                {
                    xcam = 7.0f;
                    zcam = 7.0f;
                }

                xcam = x + xcam;
                zcam = z + zcam;

                cam.transform.position = new Vector3(xcam, ycam, zcam);

                // Camera is oriented on the center of the object 
                cam.transform.LookAt(target.position);

                // Take a screenshot and label it
                takePics(obj[i]);

                yield return null;
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

    // Generate a unique screenshot name based on file number
    public static string ScreenShotName()
    {
        filenumber++;
        return Application.dataPath + "/screenshots/" + filenumber.ToString() + ".png";

    }

    // Take a picture with camera 'cam' with a label file containing object 'obj'
    public void takePics(GameObject obj)
    {
        int multiplier = 4; // Adjust the multiplier as needed
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
        
        var renderers = obj.GetComponentsInChildren<Renderer>();

        if (renderers != null && renderers.Length > 0)
        {
            var bounds = renderers[0].bounds;
    
            for (var i = 1; i < renderers.Length; ++i)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }

            // Position of the object on the screenshot
            Vector3 center = bounds.center;
            Vector3 viewPos = cam.WorldToViewportPoint(center);
            Vector3 extents = bounds.extents;
        
            float x1 = center.x + extents.x;
            float x2 = center.x - extents.x;
            float y1 = center.y + extents.y;
            float y2 = center.y - extents.y;
            float z1 = center.z + extents.z;
            float z2 = center.z - extents.z;
            
            // Position of the vertices of the object in the scene
            Vector3 point1 = new Vector3(x1, y1, z1);
            Vector3 point2 = new Vector3(x1, y1, z2);
            Vector3 point3 = new Vector3(x1, y2, z1);
            Vector3 point4 = new Vector3(x1, y2, z2);
            Vector3 point5 = new Vector3(x2, y1, z1);
            Vector3 point6 = new Vector3(x2, y1, z2);
            Vector3 point7 = new Vector3(x2, y2, z1);
            Vector3 point8 = new Vector3(x2, y2, z2);

            // Position of vertices of the object on the screenshot
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

            // Add the object to the label according to its tag
            string labelname = Application.dataPath + "/labels/" + filenumber.ToString() + ".txt";

            if (obj.CompareTag("ULD_f"))
            {
                type = 0;
            }

            else if (obj.CompareTag("Door_f"))
            {
                type = 1;
            }

            else if (obj.CompareTag("EmptyDolly_f"))
            {
                type = 2;
            }

            else if (obj.CompareTag("Speedloader_f"))
            {
                type = 3;
            }

            else if (obj.CompareTag("Highloader_f"))
            {
                type = 4;
            }

            else if (obj.CompareTag("TUG_f"))
            {
                type = 5;
            }

            else if (obj.CompareTag("Pallet_f"))
            {
                type = 6;
            }

            else if (obj.CompareTag("HighloaderDown_f"))
            {
                type = 7;
            }

            // If the file does not exist, create it
            using (StreamWriter writer = new StreamWriter(labelname, false))
            {
                // Add the label of the object to the file
                string line = type.ToString() + " " + viewPos.x.ToString() + " " + (1-viewPos.y).ToString() + " " + final_width.ToString() + " " + final_height.ToString();
                writer.WriteLine(line);
            }
        }

        // Release resources and increment iteration
        cam.targetTexture = null;
        RenderTexture.active = null;
        rt.Release();
    }
}

