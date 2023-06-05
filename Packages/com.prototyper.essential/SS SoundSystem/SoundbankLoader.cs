using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS
{
    public class SoundbankLoader : MonoBehaviour
    {
        [SerializeField] private string[] banks;
        [SerializeField] private bool localization = false;

        private void Awake()
        {
            string postfix = "";
            if (localization)
            {
                // Check current language
                postfix = TextTable.GetCurrentLanguage().ToString();
            }
                foreach (var bank in banks)
                {
                var path = $"{bank}_{postfix}";
                Debug.Log("Loading soundbank: " + path);
                GameObject sbObj = (GameObject)Resources.Load(path);
                if (sbObj)
                {
                    sbObj = Instantiate(sbObj);
                    sbObj.transform.SetParent(transform);
                }
            }
        }
    }

}