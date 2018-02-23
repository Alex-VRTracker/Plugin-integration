using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* VR Tracker
 * This script is to be set on a Gameobject between the Camera and the Object to which the Headset Tag position is applied
 * 
 */

public class VRTrackerHeadsetRotation : MonoBehaviour
{

    public Camera camera;
    public VRTrackerTag tag;

    private Quaternion previousOffset;
    private Quaternion destinationOffset;

    private Vector3 newRotation;

    private float t;
    private float timeToReachTarget = 5.0f;

    [Tooltip("The minimum offset in degrees to blink instead of rotating.")]
    public float minOffsetToBLink = 30.0f;

    /*[Tooltip("The VRTK Headset Fade script to use when fading the headset. If this is left blank then the script will need to be applied to the same GameObject.")]
    public VRTK.VRTK_HeadsetFade headsetFade;
    */
    void OnEnable()
    {
        newRotation = Vector3.zero;

        //headsetFade = (headsetFade != null ? headsetFade : FindObjectOfType<VRTK.VRTK_HeadsetFade>());
        /*if (headsetFade == null)
        {
            //VRTK.VRTK_Logger.Error(VRTK.VRTK_Logger.GetCommonMessage(VRTK.VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_HeadsetCollisionFade", "VRTK_HeadsetFade", "the same or child"));
            return;
        }
        else
        {
            //headsetFade.HeadsetFadeComplete += HeadsetFadeCompleteHandler;
        }*/

        StartCoroutine(FixOffset());
        previousOffset = Quaternion.Euler(Vector3.zero);
        destinationOffset = Quaternion.Euler(Vector3.zero);

    }

    // Update is called once per frame
    void LateUpdate()
    {
        t += Time.deltaTime / timeToReachTarget;
        transform.localRotation = Quaternion.Lerp(previousOffset, destinationOffset, t);
    }

    IEnumerator FixOffset()
    {
        while (true)
        {
            if (VRTracker.instance != null)
            {
                if (tag == null)
                    tag = VRTracker.instance.getHeadsetTag();
                if (tag != null)
                {
                    Vector3 tagRotation = tag.getOrientation();
                    Vector3 cameraRotation = camera.transform.localEulerAngles;

                    newRotation.y = tagRotation.y - cameraRotation.y; 
                    previousOffset = destinationOffset;
                    destinationOffset = Quaternion.Euler(newRotation);
                    t = 0;
                }
                yield return new WaitForSeconds(5);
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    /*public void HeadsetFadeCompleteHandler(object sender, VRTK.HeadsetFadeEventArgs e)
    {
        previousOffset = destinationOffset;
        destinationOffset = Quaternion.Euler(newRotation);
        t = timeToReachTarget;
    }*/

}
