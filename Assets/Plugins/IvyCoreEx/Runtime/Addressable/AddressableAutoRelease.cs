using System;
using System.Collections;
using System.Collections.Generic;
using IvyCoreEx;
//using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Object = UnityEngine.Object;

public class AddressableAutoRelease : MonoBehaviour
{
    //[ShowInInspector,ReadOnly,LabelText("自动释放键值")]
    public string mKey_;
#if IVY_ADDRESSABLE_ENABLE
    private void Awake()
    {
        // 试玩广告(Playworks)环境下，直接禁用Addressables的引用计数管理，防止引起空指针异常或导致粒子特效无法播放
        // if(!string.IsNullOrEmpty(mKey_))
        //     AddressableManager.GetInstance().IncreaseRefCountByKey(mKey_);
    }

    private void OnDestroy()
    {
        // if(!string.IsNullOrEmpty(mKey_))
        //     AddressableManager.GetInstance()?.Release(mKey_);
#if UNITY_EDITOR
        if (this != null)
        {
            var go = this.gameObject;
            if (go != null)
            {
                EditorUtility.ClearDirty(go);
            }
        }
#endif
    }
#endif

    public string Key
    {
        get => mKey_;
        set => mKey_ = value;
    }
}
