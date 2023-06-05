using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SS
{
    public class TMPHyperLink : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        private TMP_Text pTextMeshPro;

        void Awake()
        {

            if (!pTextMeshPro)
                pTextMeshPro = GetComponent<TMP_Text>();

        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Camera camera = null;
            var canvas = GetComponentInParent<Canvas>();
            if (canvas)
            {
                camera = canvas.worldCamera;
            }

            int linkIndex = TMP_TextUtilities.FindIntersectingLink(pTextMeshPro, eventData.position, camera);
            if (linkIndex >= 0)
            {
                TMP_LinkInfo linkInfo = pTextMeshPro.textInfo.linkInfo[linkIndex];
                var linkID = linkInfo.GetLinkID();
                if (linkID.Contains(@"://"))
                    Application.OpenURL(linkInfo.GetLinkID());
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
        }
    }
}
