using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ObjectCache
{

    public static bool dontDestroyOnlLoad = false;
    public static int numPrefabs = 20;
    public static int maxInstancesPerPrefab = 20;

    private static Dictionary<int, LinkedList<GameObject>> cache;

    private static Transform cacheParent;
    private static MonoBehaviour cacheScript;
    //private static ulong instanceIdCounter;

    private static WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();


    public static CacheReference Instantiate(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        Init();

        LinkedList<GameObject> objGroup = null;
        GameObject newObj = null;

        var prefabId = prefab.GetInstanceID();

        if (cache.TryGetValue(prefabId, out objGroup))
        {
            var node = objGroup.First;

            while (node != null)
            {
                var obj = node.Value;

                if (obj == null)
                {
                    Debug.LogError("ObjectCache: cached GameObject '" + prefab.name + "' was destroyed by an external script.");
                    node = node.Next;
                    objGroup.Remove(node.Previous);
                }
                else if (obj.GetComponent<ObjectCacheScript>().cached)
                {
                    var t = obj.transform;
                    t.position = position;
                    t.rotation = rotation;

                    t.localScale = prefab.transform.localScale;

                    t.parent = parent;

                    newObj = obj;
                    break;
                }
                else
                {
                    node = node.Next;
                }
            }
        }

        if (objGroup == null)
        {
            objGroup = new LinkedList<GameObject>();
            cache[prefabId] = objGroup;
        }

        ObjectCacheScript script = null;

        if (newObj == null)
        {
            newObj = Object.Instantiate(prefab, position, rotation, cacheParent);

            newObj.SetActive(false);

            newObj.transform.parent = parent;

            // FIX: this line creates unnecessay garbage
            //newObj.name = prefab.name;

            if (maxInstancesPerPrefab == 0 || objGroup.Count < maxInstancesPerPrefab)
            {
                script = newObj.AddComponent<ObjectCacheScript>();
                objGroup.AddLast(newObj);
            }
        }
        else
        {
            script = newObj.GetComponent<ObjectCacheScript>();
        }

        if (script != null)
        {
            script.cached = false;
            //script.instanceId = instanceIdCounter++;
        }

        var cacheReference = new CacheReference(newObj);

        cacheScript.StartCoroutine(CoActivate(newObj, script));

        return cacheReference;
    }

    public static void Recycle(CacheReference cacheReference)
    {
        var reference = cacheReference.reference;

        if ((object)reference == null)
        {
            Debug.LogError("ObjectCache: GameObject '" + reference.name + "' was already recycled.");
            return;
        }
        
        if(!reference)
        {
            Debug.LogError("ObjectCache: GameObject '" + reference.name + "' was destroyed by an external script and couldn't be recycled.");
        }

        var script = reference.GetComponent<ObjectCacheScript>();

        if (cacheParent == null || (object)(script) == null)
        {
            Object.Destroy(reference);
            return;
        }
        
        script.cached = true;

        reference.transform.parent = cacheParent;
        reference.SetActive(false);

        cacheReference.reference = null;
    }

    [System.Obsolete]
    public static void Recycle(CacheReference cacheReference, Vector3 resetScale)
    {
        GameObject t = cacheReference.reference;

        Recycle(cacheReference);

        if(t)
        {
            t.transform.localScale = resetScale;
        }
    }

    private static void Init()
    {
        if (cacheParent == null)
        {
            cache = new Dictionary<int, LinkedList<GameObject>>(numPrefabs);

            var cacheObject = new GameObject("ObjectCache");
            cacheScript = cacheObject.AddComponent<CacheScript>();

            cacheParent = new GameObject("CacheParent").transform;
            cacheParent.parent = cacheObject.transform;
            cacheParent.gameObject.SetActive(false);

            if (dontDestroyOnlLoad)
            {
                Object.DontDestroyOnLoad(cacheObject);
            }

            //instanceIdCounter = 1;
        }
    }

    private class CacheScript : MonoBehaviour
    {
        private void OnDestroy()
        {
            cache.Clear();
            cacheParent = null;
            cacheScript = null;
        }
    }

    private static IEnumerator CoActivate(GameObject gameObject, ObjectCacheScript script)
    {
        yield return waitForEndOfFrame;

        if (script != null)
        {
            if (!script.cached)
            {
                gameObject.SetActive(true);
            }
        }
    }

    internal class ObjectCacheScript : MonoBehaviour
    {

        internal bool cached;
        //private bool quitting;
        //public ulong instanceId { get; set; }

        private void OnEnable()
        {
            if (cached)
            {
                Debug.LogError("ObjectCahce: GameObject '" + gameObject.name + "' cannot be activated because it was recycled.");
                gameObject.SetActive(false);
            }
        }

        //private void OnApplicationQuit()
        //{
        //    quitting = true;
        //}

        private void OnDisable()
        {
            //if(quitting)
            //{
            //    return;
            //}

            enabled = true;

            //if(!cached && !gameObject.activeInHierarchy && cacheParent)
            //{
            //    Debug.LogError("ObjectCache: GameObject '" + gameObject.name + "' was deactivated without calling ObjectCache.Recycle. This violates the contract for using the ObjectCache and can cause unexpected behaviour");
            //}
        }
    }

}
