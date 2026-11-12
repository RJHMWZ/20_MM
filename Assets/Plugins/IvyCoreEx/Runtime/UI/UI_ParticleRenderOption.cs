//using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IvyCoreEx
{
    // [TypeInfoBox("<b><color=#00ff00>此组件主要使用于UI界面中,粒子对象的显示层级关系动态调整.</color></b>\n" +
    //              "调整逻辑:\n" +
    //              "1)当指定Canvas不为空时,根据指定canvas对粒子系统进行调整\n" +
    //              "2)当指定Canvas为空时,判断UIWindow节点是否为空,为空时将自动向上级节点查找UIWindow节点,调整层级依据当前查找到的Canvas为基准\n" +
    //              "3)部分界面如果是放置在场景中自动注册的,需要延迟处理粒子层级")]
    
    public class UI_ParticleRenderOption : MonoBehaviour
    {
        //[LabelText("UIWindow节点"), ShowIf("@mDestCanvas_==null&&!mFindingClosestCanvas_")]
        //public UI_Window mWindowFindNode_ = null;
        //[LabelText("指定Canvas"),HideIf("mFindingClosestCanvas_")]
        public Canvas mDestCanvas_ = null;
        //[LabelText("查找最近的Canvas组件")]
        public bool mFindingClosestCanvas_ = false;
        //[LabelText("指定ParticleSystem")]
        public ParticleSystem mDestPS_;
        ParticleSystem mPS_;
        //[LabelText("是否延迟执行设置逻辑")]
        public bool mDelayDoLogic_ = false;
        //[LabelText("延迟执行设置逻辑帧数"),ShowIf("mDelayDoLogic_")]
        public int mDelayDoLogicFrames_ = 1;
        public void DoOptionLogic()
        {
            var canvas = mDestCanvas_;
            if (mPS_ == null)
            {
                return;
            }
            if (canvas != null)
            {
                var renderList = mPS_.GetComponentsInChildren<ParticleSystemRenderer>(true);
                for (var i = 0; i < renderList.Length; ++i)
                {
                    var srcOrder = renderList[i].sortingOrder;
                    renderList[i].gameObject.layer = canvas.gameObject.layer;
                    renderList[i].sortingOrder = canvas.sortingOrder;
                    renderList[i].sortingLayerID = canvas.sortingLayerID;
                    renderList[i].sortingFudge = -(float)srcOrder * 0.1f + renderList[i].sortingFudge;
                }
            }
        }
        IEnumerator DoOptionCoroutineLogic()
        {
            var val = mDelayDoLogicFrames_;
            while (val > 0)
            {
                --val;
                yield return null;
            }
            DoOptionLogic();
        }
        private void Start()
        {
            CheckCavansAndRoot();
        }
        //[Button("编辑器模式下预览设置")]
        public void CheckCavansAndRoot()
        {
            if (mFindingClosestCanvas_)
            {
                mDestCanvas_ =  this.GetComponentInParent<Canvas>();
            }
            if (mDestCanvas_ == null)
            {
                //if (mWindowFindNode_ == null)
                //    mWindowFindNode_ = this.GetComponentInParent<UI_Window>();
                //if (mWindowFindNode_ != null)
                //{
                //     mWindowFindNode_.TryGetComponent(out mDestCanvas_);
                //}
            }
            if (mDestPS_ != null)
                mPS_ = mDestPS_;
            else
                mPS_ = this.GetComponent<ParticleSystem>();

            if (mDestCanvas_ != null)
            {
                if (Application.isPlaying)
                {
                    if (mDelayDoLogic_)
                    {
                        StartCoroutine(DoOptionCoroutineLogic());
                    }
                    else
                    {
                        DoOptionLogic();
                    }
                }
                else
                {
                    DoOptionLogic();
                }
                
            }
            
            /*if (mDestCanvas_ != null)
            {
                ParticleSystemRenderer[] renderList = null;
                if(mDestPS_!=null)
                    renderList = mDestPS_.gameObject.GetComponentsInChildren<ParticleSystemRenderer>(true);
                else
                    renderList = mDestCanvas_.gameObject.GetComponentsInChildren<ParticleSystemRenderer>(true);
                for (var i = 0; i < renderList.Length; ++i)
                {
                    var srcOrder = renderList[i].sortingOrder;
                    renderList[i].sortingOrder = mDestCanvas_.sortingOrder;
                    renderList[i].sortingLayerID = mDestCanvas_.sortingLayerID;
                    renderList[i].sortingFudge = -(float)srcOrder * 0.1f + renderList[i].sortingFudge;
                }
            }
            else
            {
                if (mDestPS_ != null)
                    mPS_ = mDestPS_;
                else
                    mPS_ = this.GetComponent<ParticleSystem>();
                if (mWindowFindNode_ == null)
                    mWindowFindNode_ = this.GetComponentInParent<UI_Window>();
                if (mWindowFindNode_ != null && mPS_ != null)
                {
                    if (mWindowFindNode_.autoRegister_)
                        StartCoroutine(DoOptionCoroutineLogic());
                    else
                        DoOptionLogic();
                }
            }*/
        }
    }
}