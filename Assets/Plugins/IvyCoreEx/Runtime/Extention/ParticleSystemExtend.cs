using System;
//using IvyCoreEx.Pool;
//using MoreMountains.NiceVibrations;
//using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
//using MoreMountains.NiceVibrations;
using Random = UnityEngine.Random;

namespace IvyCoreEx.Extention
{
    [ExecuteAlways,DisallowMultipleComponent]
    public class ParticleSystemExtend : MonoBehaviour //SerializedMonoBehaviour
    {
        //[LabelText("是否播放完毕释放"), OnValueChanged("CheckDestoryMode")]
        public bool isDestoryWhenStop_ = false;
        //[LabelText("是否播放完毕隐藏"),OnValueChanged("CheckInvisibleMode")]
        public bool isInvisibleWhenStop_=false;
        //[LabelText("是否播放完毕回收"), OnValueChanged("CheckPoolMode")]
        public bool isUsePoolMode_ = true;
        //[LabelText("初始设置为回调触发模式[建议勾选]")]
        public bool isForceUseCallBackStopActionMode_ = true;
        
        //[LabelText("设置随机种子同步的节点")]
        public List<List<ParticleSystem>> mKeepSameRandSeedParticleList;

        //[LabelText("控制对象节点"),Tooltip("控制对象不为空时,播放完毕隐藏,回收,释放都将针对控制对象")]
        public GameObject mPoolObject_;
        ParticleSystem mMainPS_ = null;
        bool mWasStopped_ = false;
        //[ShowInInspector,LabelText("事件列表"),ListDrawerSettings(ShowIndexLabels = true,ShowItemCount = true)]
        public List<PSE_EventBase> mEventList_;

        Dictionary<string, System.Action> mTriggerActionSaver_;
        [HideInInspector]
        public System.Action OnParticleSystemStoppedCallback;

        private void OnEnable()
        {
#if UNITY_EDITOR
            if (mMainPS_ == null)
                mMainPS_ = GetComponent<ParticleSystem>();
            if (mMainPS_ != null)
            {
                mMainPS_.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                mMainPS_.Play();
            }
#endif
        }

        void CheckDestoryMode()
        {
            if (isDestoryWhenStop_)
            {
                isInvisibleWhenStop_ = false;
                isUsePoolMode_ = false;
            }
        }
        void CheckPoolMode()
        {
            if (isUsePoolMode_)
            {
                isDestoryWhenStop_ = false;
                isInvisibleWhenStop_ = false;
            }
        }
        void CheckInvisibleMode()
        {
            if (isInvisibleWhenStop_)
            {
                isDestoryWhenStop_ = false;
                isUsePoolMode_ = false;
            }
        }
        
        public void AddTriggerAction(string name, System.Action action)
        {
            if (mTriggerActionSaver_ == null)
                mTriggerActionSaver_ = new Dictionary<string, System.Action>();
            mTriggerActionSaver_[name] = action;
        }

        public void TriggerAtion(string name)
        {
            if (mTriggerActionSaver_ == null)
                return;
            if (mTriggerActionSaver_.ContainsKey(name))
                mTriggerActionSaver_[name].Invoke();
        }

        public void RemoveTriggerAction(string name)
        {
            if (mTriggerActionSaver_ == null)
                return;
            if (mTriggerActionSaver_.ContainsKey(name))
                mTriggerActionSaver_.Remove(name);
        }

        public void RemoveAllTriggerAction()
        {
            if (mTriggerActionSaver_ == null)
                return;
            mTriggerActionSaver_.Clear();
        }

        public void Start()
        {
            if (mMainPS_ == null)
                mMainPS_ = GetComponent<ParticleSystem>();
            if (mMainPS_ != null)
            {
                if (isForceUseCallBackStopActionMode_)
                {
                    var main = mMainPS_.main;
                    // main.stopAction = ParticleSystemStopAction.Callback;
                    main.loop = false;
                }
            }
        }

        public void ResetEvents()
        {
            if (mMainPS_ != null)
            {
                // mMainPS_.time = 0;
                if (mEventList_ != null && mEventList_.Count > 0)
                {
                    for (var i = 0; i < mEventList_.Count; ++i)
                    {
                        mEventList_[i].Reset();
                    }
                }
            }
        }
        public void Update()
        {
#if UNITY_EDITOR
            if(mMainPS_!=null)
            {
                if (isForceUseCallBackStopActionMode_)
                {
                    if (!mMainPS_.IsAlive(true))
                    {
                        if (!mWasStopped_)
                        {
                            mWasStopped_ = true;
                            OnParticleSystemStopped();
                        }
                    }
                    else
                    {
                        mWasStopped_ = false;
                    }
                }

                if (mEventList_ != null && mEventList_.Count > 0)
                {
                    var localPos = this.transform.localPosition;
                    var worldPos = this.transform.position;
                    for (var i = 0; i < mEventList_.Count; ++i)
                    {
                        mEventList_[i].DoEvent(this,mMainPS_.time, worldPos, localPos);
                    }
                }
            }
#endif
        }

        public void OnParticleSystemStopped()
        {
            OnParticleSystemStoppedCallback?.Invoke();
#if UNITY_EDITOR
            if(isUsePoolMode_&&Application.isPlaying)
            {
                //PoolManager.GetInstance().RecyclePoolObject(mPoolObject_ != null ? mPoolObject_ : this.gameObject);
                if (mPoolObject_ != null) GameObject.Destroy(mPoolObject_); else GameObject.Destroy(this.gameObject);
                ResetEvents();
            }
            if(isInvisibleWhenStop_ && Application.isPlaying)
            {
                if(mMainPS_!=null)
                {
                    // mMainPS_.time = 0;
                }
                if(mPoolObject_ != null)
                {
                    mPoolObject_.SetActive(false);
                }
                else
                {
                    this.gameObject.SetActive(false);
                }
                ResetEvents();
            }
            if(isDestoryWhenStop_ && Application.isPlaying)
            {
                if (mPoolObject_ != null)
                {
                    GameObject.Destroy(mPoolObject_);
                }
                else
                {
                    GameObject.Destroy(gameObject);
                }
            }
#else
            if (isUsePoolMode_) {
                //PoolManager.GetInstance().StoreObject(mPoolObject_ != null ? mPoolObject_ : this.gameObject);
                if (mPoolObject_ != null) GameObject.Destroy(mPoolObject_); else GameObject.Destroy(this.gameObject);
                ResetEvents();
            }
            if(isInvisibleWhenStop_)
            {
                if(mMainPS_!=null)
                {
                    // mMainPS_.time = 0;
                }
                if(mPoolObject_ != null)
                {
                    mPoolObject_.SetActive(false);
                }
                else
                {
                    this.gameObject.SetActive(false);
                }
                ResetEvents();
            }
            if(isDestoryWhenStop_)
            {
                if (mPoolObject_ != null)
                {
                    GameObject.Destroy(mPoolObject_);
                }
                else
                {
                    GameObject.Destroy(gameObject);
                }
            }
#endif
        }
    }

    public class PSE_EventBase
    {
        public virtual void Reset() { }
        public virtual void DoEvent(ParticleSystemExtend pse,float curTime, Vector3 worldPos, Vector3 locaPos) { }
    }

    public class PSE_Event_StringTrigger:PSE_EventBase
    {
        //[LabelText("触发事件名称")]
        public string mTriggerName_;
        //[LabelText("触发事件"),SuffixLabel("秒")]
        public float mTriggerTime_;
        bool mHadTriggered_ = false;
        public override void Reset()
        {
            mHadTriggered_ = false;
        }
        public override void DoEvent(ParticleSystemExtend pse, float curTime, Vector3 worldPos, Vector3 locaPos)
        {
            if (!mHadTriggered_)
            {
                if (curTime >= mTriggerTime_)
                {
                    mHadTriggered_ = true;
                    pse.TriggerAtion(mTriggerName_);
                }
            }
        }
    }


    public class PSE_Event_PlaySound : PSE_EventBase
    {
        bool mPlayed_ = false;
        //[LabelText("播放时间"),SuffixLabel("秒")]
        public float mPlayTime_;
        //[LabelText("音效文件"),HideIf("@mUseRandomAudioClip_")]
        public AudioClip mAudioClip_;
        //[LabelText("震动文件"), HideIf("@mUseRandomAudioClip_")]
        //public MMNVAndroidWaveFormAsset mMNV_;
        //[LabelText("是否使用随机音效文件")]
        public bool mUseRandomAudioClip_;
        //[LabelText("随机音效文件列表"), ShowIf("@mUseRandomAudioClip_")]
        public List<AudioClip> mAudioClipRandom_;
        //[LabelText("随机震动文件列表"), ShowIf("@mUseRandomAudioClip_"),InfoBox("与随机音效保持index对应关系来保证最佳效果,未发现对应文件时将随机调用")]
        //public List<MMNVAndroidWaveFormAsset> mMNVRandom_;
        //[LabelText("音效不受TimeScale影响")]
        public bool mNotEffectByTimeScale_ = false;
        public override void Reset()
        {
            mPlayed_ = false;
        }
        public override void DoEvent(ParticleSystemExtend pse, float curTime, Vector3 worldPos, Vector3 locaPos)
        {
            if (!mPlayed_)
            {
                if (curTime >= mPlayTime_)
                {
                    mPlayed_ = true;
                    //var sm = Sound.SoundManager.GetInstance();
                    //if(sm!=null)
                    //{
                    //    if (mUseRandomAudioClip_ && mAudioClipRandom_ != null)
                    //    {
                    //        var index = Random.Range(0, mAudioClipRandom_.Count);
                    //        sm.PlaySound(mAudioClipRandom_[index], !mNotEffectByTimeScale_);
                    //        // if (mMNVRandom_ != null && mMNVRandom_.Count > 0)
                    //        // {
                    //        //     if (index >= 0 && index < mMNVRandom_.Count)
                    //        //     {
                    //        //         VibrationHelper.VibrationAdvance(mMNVRandom_[index].AHAPFile.text, mMNVRandom_[index].WaveForm.Pattern, mMNVRandom_[index].WaveForm.Amplitudes, -1, null, null, null, -1);
                    //        //     }
                    //        // }
                    //    }
                    //    else
                    //    {
                    //        sm.PlaySound(mAudioClip_, !mNotEffectByTimeScale_);
                    //        // if (mMNV_ != null)
                    //        // {
                    //        //     VibrationHelper.VibrationAdvance(mMNV_.AHAPFile.text, mMNV_.WaveForm.Pattern, mMNV_.WaveForm.Amplitudes, -1, null, null, null, -1);
                    //        // }
                    //    }
                    //}
//#if UNITY_EDITOR
//                    if(!Application.isPlaying&& sm!=null)
//                    {
//                        GameObject.DestroyImmediate(sm.gameObject);
//                    }
//#endif
                }
            }
        }
    }


    public class PSE_Event_CreatePrefab : PSE_EventBase
    {
        bool mPlayed_ = false;
        //[LabelText("创建时间"),SuffixLabel("秒")]
        public float mCreateTime_;
        //[LabelText("创建偏移坐标")]
        public Vector3 mRandomPosition_;
        //[LabelText("原型创建对象")]
        public GameObject mPrefabObject;
        public override void Reset()
        {
            mPlayed_ = false;
        }
        public override void DoEvent(ParticleSystemExtend pse, float curTime, Vector3 worldPos, Vector3 locaPos)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif
            if (!mPlayed_)
            {
                if (curTime >= mCreateTime_)
                {
                    mPlayed_ = true;
                    //var obj = IvyCoreEx.Pool.PoolManager.GetInstance().GetPoolObject(mPrefabObject);
                    var obj = mPrefabObject != null ? GameObject.Instantiate(mPrefabObject) : null;
                    if (obj != null)
                    {
                        //obj.transform.position = worldPos + mRandomPosition_.Random();
                        obj.transform.position = worldPos + mRandomPosition_; // Vector3 doesn't have a .Random() method by default unless there's an extension, which might be missing.
                    }
                }
            }
        }

    }
}