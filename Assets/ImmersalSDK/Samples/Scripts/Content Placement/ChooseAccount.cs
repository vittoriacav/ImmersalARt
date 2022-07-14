using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Immersal.Samples.Account
{
    public class ChooseAccount : MonoBehaviour
    {
        public GameObject developerUI;
        public GameObject userUI;
        public GameObject devButton;
        public GameObject userButton;
        public GameObject ARcamera;
        // Start is called before the first frame update
        void Start()
        {
            developerUI.SetActive(false);
            userUI.SetActive(false);
        }

        public void chosenUser()
        {
            userUI.SetActive(true);
            devButton.SetActive(false);
            userButton.SetActive(false);
            //ARcamera.GetComponent<UnityEngine.XR.ARFoundation.AROcclusionManager>().enabled = false;
        }

        public void chosenDev()
        {
            developerUI.SetActive(true);
            devButton.SetActive(false);
            userButton.SetActive(false);
            //ARcamera.GetComponent<UnityEngine.XR.ARFoundation.AROcclusionManager>().enabled = true;

        }
    }
}

