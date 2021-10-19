using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private ArticulationBody verticalJoint;

    [SerializeField]
    private ArticulationBody horizontalJoint;

    [SerializeField]
    private float sensitivity = 1.0f;

    [SerializeField]
    private List<GameObject> rings;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        foreach (GameObject ring in rings)
        {
            Image ringImage = ring.GetComponent<Image>();
            RectTransform ringTransform = ring.GetComponent<RectTransform>();
            Vector2 ringSize = ringTransform.sizeDelta;
            Texture2D tex = new Texture2D((int) ringSize.x, (int)ringSize.y);
            Rect rect = new Rect(0, 0, tex.width, tex.height);
            Sprite sprite = Sprite.Create(tex, rect, new Vector2(0.5f, 0.5f));
            ringImage.sprite = sprite;

            Color fillColor = Color.white;
            Color[] pixels =  tex.GetPixels();
            
            for (var i = 0; i < pixels.Length; ++i)
            {
                int row  = i / tex.width;
                int col = i % tex.width;

                // Draw a circle
                // Calculate the distance from the center of the circle, but cheaply without sqrt
                float dist = (row - tex.width / 2) * (row - tex.width / 2) + (col - tex.height / 2) * (col - tex.height / 2);
                float radius = tex.width / 2;
                float thickness = 4;
                float radiusSubtract = radius - thickness;
                if (dist > radiusSubtract * radiusSubtract && dist < radius * radius) {
                    pixels[i] = fillColor;
                } else {
                    pixels[i] = Color.clear;
                }
            }
            
            tex.SetPixels( pixels );
            tex.Apply();
        }
    }

    bool DrawCircle(float dist, Color fillColor, float radius, float thickness) {
        float radiusSubtract = radius - thickness;
        return dist > radiusSubtract * radiusSubtract && dist < radius * radius;
    }

    // Update is called once per frame
    void Update()
    {
        float verticalLook = Input.GetAxis("Mouse Y") * sensitivity;
        ArticulationDrive verticalDrive = verticalJoint.xDrive;
        verticalDrive.target += verticalLook;
        verticalDrive.damping = 1000;
        verticalDrive.forceLimit = 0.5f;
        //verticalDrive.target = Mathf.Clamp(verticalDrive.target, verticalDrive.lowerLimit, verticalDrive.upperLimit);
        verticalJoint.xDrive = verticalDrive;


        float horizontalLook = Input.GetAxis("Mouse X") * sensitivity;
        ArticulationDrive horizontalDrive = horizontalJoint.xDrive;
        horizontalDrive.target -= horizontalLook;
        horizontalDrive.damping = 1000;
        horizontalDrive.forceLimit = 0.5f;
        //horizontalDrive.target = Mathf.Clamp(horizontalDrive.target, horizontalDrive.lowerLimit, horizontalDrive.upperLimit);
        horizontalJoint.xDrive = horizontalDrive;

        float horizontalRemap = horizontalDrive.target; // - horizontalDrive.lowerLimit) / (horizontalDrive.upperLimit - horizontalDrive.lowerLimit);
        float verticalRemap = verticalDrive.target; // - verticalDrive.lowerLimit) / (verticalDrive.upperLimit - verticalDrive.lowerLimit);
        

        Vector2 cursorPosition = new Vector2(-horizontalRemap, verticalRemap);
        float size = rings[0].GetComponent<RectTransform>().sizeDelta.x;
        //cursorPosition *= size;
        //cursorPosition += new Vector2(size / 2.0f, -size / 2.0f);

        // Move the rings towards the cursor position, with each ring getting closer than the previous
        for (int i = 0; i < rings.Count; i++)
        {
            GameObject ring = rings[i];
            RectTransform ringTransform = ring.GetComponent<RectTransform>();
            ringTransform.localPosition = Vector2.Lerp(Vector2.zero, cursorPosition, i / (float) rings.Count);
        }
    }
}
