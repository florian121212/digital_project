using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class screenshots : MonoBehaviour
{
    int resWidth;
    int resHeight;
    public static int fname;

    void Start()
    {
        fname = 157;
    }

    public static string ScreenShotName(int width, int height)
    {
        fname++;
        return Application.dataPath + "/screenshots/" + fname.ToString() + ".png";

    }

    void Update()
    {
        if (Input.GetKeyUp("k"))
        {
            gameObject.GetComponent<Camera>().enabled = false;
            takePics();
        }
    }

    public float getHeightWidth(float actual_value, float point1, float point2)
    {
        float tmp_value = Mathf.Abs(point1 - point2);


        if (actual_value < tmp_value)
        {
            actual_value = tmp_value;
        }

        return actual_value;
    }

    

    public void takePics()
    {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("picCam"))
        {
            Camera cam = go.GetComponent<Camera>();

            int multiplier = 2; // Adjust the multiplier as needed
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

            // Reset the RenderTexture and camera targetTexture
            RenderTexture.active = null;
            cam.targetTexture = null;

            // Encode the texture to PNG
            byte[] bytes = screenShot.EncodeToPNG();

            // Save the image
            string filename = ScreenShotName(resWidth, resHeight);
            System.IO.File.WriteAllBytes(filename, bytes);

            // Get the label of one ULD
            GameObject[] objectsToLabel = GameObject.FindGameObjectsWithTag("ULD"); ;

            foreach(GameObject objectToLabel in objectsToLabel)
            {
                var renderer = objectToLabel.GetComponent<Renderer>();

                Vector3 ULD_center = renderer.bounds.center;
                

                //Debug.DrawRay(cam.transform.position, (ULD_center - cam.transform.position), Color.yellow);

                Vector3 viewPos = cam.WorldToViewportPoint(ULD_center);
                //print(viewPos);

                Vector3 ULD_extents = renderer.bounds.extents;

                float x1 = ULD_center.x + ULD_extents.x;
                float x2 = ULD_center.x - ULD_extents.x;
                float y1 = ULD_center.y + ULD_extents.y;
                float y2 = ULD_center.y - ULD_extents.y;
                float z1 = ULD_center.z + ULD_extents.z;
                float z2 = ULD_center.z - ULD_extents.z;

                Vector3 point1 = new Vector3(x1, y1, z1);
                Vector3 point2 = new Vector3(x1, y1, z2);
                Vector3 point3 = new Vector3(x1, y2, z1);
                Vector3 point4 = new Vector3(x1, y2, z2);
                Vector3 point5 = new Vector3(x2, y1, z1);
                Vector3 point6 = new Vector3(x2, y1, z2);
                Vector3 point7 = new Vector3(x2, y2, z1);
                Vector3 point8 = new Vector3(x2, y2, z2);

                Vector3 screenPoint1 = cam.WorldToViewportPoint(point1);
                Vector3 screenPoint2 = cam.WorldToViewportPoint(point2);
                Vector3 screenPoint3 = cam.WorldToViewportPoint(point3);
                Vector3 screenPoint4 = cam.WorldToViewportPoint(point4);
                Vector3 screenPoint5 = cam.WorldToViewportPoint(point5);
                Vector3 screenPoint6 = cam.WorldToViewportPoint(point6);
                Vector3 screenPoint7 = cam.WorldToViewportPoint(point7);
                Vector3 screenPoint8 = cam.WorldToViewportPoint(point8);

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


                //print(final_height);
                //print(final_width);


                if (viewPos.x <= 1 && viewPos.x >= 0 && viewPos.y <= 1 && viewPos.y >= 0 && final_height >= 0.01 && final_width >= 0.01)
                {

                    // Create a ray from the camera to the object's position
                    RaycastHit hit;

                    // Perform the raycast
                    if (Physics.Raycast(cam.transform.position, ULD_center - cam.transform.position, out hit))
                    {
                        if(hit.transform.gameObject.tag == "ULD")
                        {
                            string labelname = Application.dataPath + "/labels/" + fname.ToString() + ".txt";

                            using (StreamWriter writer = new StreamWriter(labelname, true))
                            {
                                string line = "0 " + viewPos.x.ToString() + " " + (1 - viewPos.y).ToString() + " " + final_width.ToString() + " " + final_height.ToString();
                                writer.WriteLine(line);
                            }
                        }
                    }

                    
                }

            }

            /*Vector3 screenMin = cam.WorldToViewportPoint(renderer.bounds.min);
            Vector3 screenMax = cam.WorldToViewportPoint(renderer.bounds.max);

            float screenWidth = screenMax.x - screenMin.x;
            float screenHeight = screenMax.y - screenMin.y;*/


            cam.targetTexture = null;
            RenderTexture.active = null;
            rt.Release();
        }
    }
}