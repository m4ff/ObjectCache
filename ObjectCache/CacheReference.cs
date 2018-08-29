using UnityEngine;

public class CacheReference
{

    //public static readonly CacheReference None = new CacheReference(null, null);

    //internal ulong instanceId;
    internal GameObject reference;
    //internal ObjectCache.ObjectCacheScript script;

    public GameObject gameObject
    {
        get
        {
            if ((object)reference != null)
            {
                return reference;
            }
            else
            {
                throw new CacheReferenceExpiredExeption();
            }
        }
    }

    internal CacheReference(GameObject obj)//, MonoBehaviour s)
    {
        reference = obj;
        //script = (ObjectCache.ObjectCacheScript)s;
        //instanceId = s == null ? ulong.MaxValue : script.instanceId;
    }


    public static implicit operator bool(CacheReference r)
    {
        return (object)r != null && r.reference;
    }

    public static bool operator ==(CacheReference a, CacheReference b)
    {
        if ((object)b == null)
        {
            if ((object)a == null)
            {
                return true;
            }

            return a.reference;
        }
        else if ((object)a == null)
        {
            return b.reference;
        }
        else
        {
            return (object)a == (object)b;
        }

        //if (b == null)
        //{
        //    return !a;
        //}
        //else if (b is CacheReference)
        //{
        //    return a.instanceId == ((CacheReference)b).instanceId && a.reference == ((CacheReference)b).reference;
        //}
        //else
        //{
        //    return false;
        //}
    }

    public static bool operator !=(CacheReference a, CacheReference b)
    {
        return !(a == b);
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
        {
            return reference;
        }
        if (obj is CacheReference)
        {
            return (object)this == obj;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public void Activate()
    {
        if ((object)reference == null) throw new CacheReferenceExpiredExeption();
        reference.SetActive(true);
    }


    // Wrapping GameObject properties and methods

    public bool activeInHierarchy
    {
        get
        {
            if ((object)reference == null) throw new CacheReferenceExpiredExeption();
            return reference.activeInHierarchy;
        }
    }

    public bool activeSelf
    {
        get
        {
            if ((object)reference == null) throw new CacheReferenceExpiredExeption();
            return reference.activeSelf;
        }
    }

    public bool isStatic
    {
        get
        {
            if ((object)reference == null) throw new CacheReferenceExpiredExeption();
            return reference.isStatic;
        }
        set
        {
            if ((object)reference == null) throw new CacheReferenceExpiredExeption();
            reference.isStatic = value;
        }
    }

    public int layer
    {
        get
        {
            if ((object)reference == null) throw new CacheReferenceExpiredExeption();
            return reference.layer;
        }
        set
        {
            if ((object)reference == null) throw new CacheReferenceExpiredExeption();
            reference.layer = value;
        }
    }

    public UnityEngine.SceneManagement.Scene scene
    {
        get
        {
            if ((object)reference == null) throw new CacheReferenceExpiredExeption();
            return reference.scene;
        }
    }

    public string tag
    {
        get
        {
            if ((object)reference == null) throw new CacheReferenceExpiredExeption();
            return reference.tag;
        }
        set
        {
            if ((object)reference == null) throw new CacheReferenceExpiredExeption();
            reference.tag = value;
        }
    }

    public string name
    {
        get
        {
            if ((object)reference == null) throw new CacheReferenceExpiredExeption();
            return reference.name;
        }
        set
        {
            if ((object)reference == null) throw new CacheReferenceExpiredExeption();
            reference.name = value;
        }
    }

    public Transform transform
    {
        get
        {
            if ((object)reference == null) throw new CacheReferenceExpiredExeption();
            return reference.transform;
        }
    }

    public T GetComponent<T>()
    {
        if ((object)reference == null) throw new CacheReferenceExpiredExeption();
        return reference.GetComponent<T>();
    }

    public T GetComponentInChildren<T>()
    {
        if ((object)reference == null) throw new CacheReferenceExpiredExeption();
        return reference.GetComponentInChildren<T>();
    }
}
