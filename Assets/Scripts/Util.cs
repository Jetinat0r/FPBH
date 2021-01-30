using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util
{
    public static void SetLayerRecursively(GameObject _obj, int _newLayer)
    {
        if (_obj == null)
        {
            return;
        }

        _obj.layer = _newLayer;

        foreach (Transform _child in _obj.transform)
        {
            if (_child == null)
            {
                continue;
            }

            SetLayerRecursively(_child.gameObject, _newLayer);
        }
    }

    public static void SetTagRecursively(GameObject _obj, string _newTag)
    {
        if (_obj == null)
        {
            return;
        }

        _obj.tag = _newTag;

        foreach (Transform _child in _obj.transform)
        {
            if (_child == null)
            {
                continue;
            }

            SetTagRecursively(_child.gameObject, _newTag);
        }
    }
}
