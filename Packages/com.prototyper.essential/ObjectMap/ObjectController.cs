using UnityEngine;

namespace SS
{
    [RequireComponent(typeof(ObjectID))]
    public class ObjectController : MonoBehaviour
    {
        [SerializeField] private bool defaultActiveState = true;
        [SerializeField] private bool addDebugObjectToggle = false;
        [SerializeField] private bool addDebugGroupToggle = false;

        private const string debugGroup = "ObjectMap";
        public string debugObjectName => $"{objectID.objectID}{objectID.gameObject.GetInstanceID()}";

        private ObjectID _objectID;

        public ObjectID objectID
        {
            get
            {
                if (_objectID == null)
                {
                    _objectID = GetComponent<ObjectID>();
                }
                return _objectID;
            }
        }

        private void Awake()
        {
            if (objectID == null)
                return;

            objectID.gameObject.SetActive(defaultActiveState);

            if (addDebugObjectToggle)
            {
                DebugMenu.AddToggle(debugGroup, debugObjectName,
                    () => { return objectID.gameObject.activeSelf; },
                    (active) => { objectID.gameObject.SetActive(active); }
                );
            }
            if (addDebugGroupToggle && !string.IsNullOrEmpty(objectID.groupID))
            {
                ObjectMap.EnableDebugGroup(objectID.groupID);
            }
        }

        private void OnDestroy()
        {
            if (addDebugObjectToggle)
            {
                DebugMenu.Remove(debugObjectName);
            }
        }
    }
}