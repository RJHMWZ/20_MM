using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Sirenix.OdinInspector;
//using Sirenix.Serialization;
using System;
using DG.Tweening;

namespace IvyCoreEx
{
    [DisallowMultipleComponent]
    public class UI_Base : MonoBehaviour
    {
        public enum UIActionType
        {
            eEnter,
            eOut,
            eEnumCount
        }
        [HideInInspector]
        public Action OnDestroyCallBack;

        //[ToggleLeft, LabelText("生成成员变量")]
        public bool generateMember_;
        //[ValueDropdown("GetAllComponentsName"), ShowIf("IsShowSelectComponent"), LabelText("变量类型名称")]
        public string componentName_;
        //[LabelText("生成独立组件"), ShowIf("IsShowGenerateStandaloneComponent")]
        public bool generateStandaloneComponent_;
        //[ShowIf("generateMember_"), LabelText("变量说明"), MultiLineProperty(2)]
        public string componentDescription_;
        //[LabelText("初始未激活")]
        public bool notActive_ = false;
        [NonSerialized]
        private Tween enterAction_ = null;
        [NonSerialized]
        private Tween outAction_ = null;

        private Action mEnterActionPreCallBack_ = null;
        private Action mEnterActionEndCallBack_ = null;
        private Action mOutActionPreCallBack_ = null;
        private Action mOutActionEndCallBack_ = null;

        private Func<bool> mEnterActionCheckCallBack_ = null;
        private Func<bool> mOutActionCheckCallBack_ = null;
        /// <summary>
        /// 进场行为添加前置回调
        /// </summary>
        /// <param name="act"></param>
        public void SetEnterActionPreCallBack(Action act)
        {
            mEnterActionPreCallBack_ = act;
        }
        /// <summary>
        /// 进场行为添加后置回调
        /// </summary>
        /// <param name="act"></param>
        public void SetEnterActionEndCallBack(Action act)
        {
            mEnterActionEndCallBack_ = act;
        }
        /// <summary>
        /// 退场行为添加前置回调
        /// </summary>
        /// <param name="act"></param>
        public void SetOutActionPreCallBack(Action act)
        {
            mOutActionPreCallBack_ = act;
        }
        /// <summary>
        /// 退场行为添加后置回调
        /// </summary>
        /// <param name="act"></param>
        public void SetOutActionEndCallBack(Action act)
        {
            mOutActionEndCallBack_ = act;
        }

        public void SetEnterActionCheckCallBack(Func<bool> checkCallBack)
        {
            mEnterActionCheckCallBack_ = checkCallBack;
        }

        public void SetOutActionCheckCallBack(Func<bool> checkCallBack)
        {
            mOutActionCheckCallBack_ = checkCallBack;
        }

        public bool IsEnterActionRunning()
        {
            return enterAction_ != null && enterAction_.IsPlaying();
        }

        public bool IsOutActionRunning()
        {
            return outAction_ != null && outAction_.IsPlaying();
        }

        bool IsShowSelectComponent()
        {
            return generateMember_;
        }

        bool IsShowGenerateStandaloneComponent()
        {
            //return GetComponent<UI_Window>() == null && generateMember_;
            return generateMember_;
        }


        public List<string> GetAllComponentsName()
        {
            List<string> names = new List<string>();
            var list = this.GetComponents<Component>();
            names.Add("UnityEngine.GameObject");
            foreach (var t in list)
            {
                names.Add(t.GetType().ToString());
            }
            return names;
        }

        //[InfoBox("无效GameAction", InfoMessageType.Error, "isEnterActionInvaild")]
        //[LabelText("进场行为"), ValueDropdown("GetActionsDescription")]
        //[EditAndClearActionButton]
        public string enterActionDescription_ = string.Empty;
        //[LabelText("进场行为运行中是否允许覆盖")]
        public bool mIsEnterActionOverridable_ = false;

        //[LabelText("Enable时执行进场行为")]
        public bool mRunEnterActionWhenEnable_;
        //[LabelText("Enable时执行进场行为遍历子节点"),ShowIf("mRunEnterActionWhenEnable_")]
        public bool mRunEnterActionWhenEnableRecursionChildren_;
        
        bool isEnterActionInvaild()
        {
            if (string.IsNullOrEmpty(enterActionDescription_))
                return false;
            //return GameActionManager.GetInstance().IsActionDescriptionNotRegisted(enterActionDescription_);
            return false;
        }

        static List<string> GetActionsDescription()
        {
            //return GameActionManager.GetInstance().GetAllActionsDescription();
            return new List<string>();
        }
        //[InfoBox("无效GameAction", InfoMessageType.Error, "isOutActionVaild")]
        //[LabelText("出场行为"), ValueDropdown("GetActionsDescription")]
        //[EditAndClearActionButton]
        public string outActionDescription_ = string.Empty;
        //[LabelText("出场行为运行中是否允许覆盖")]
        public bool mIsOutActionOverridable_ = false;
        //[LabelText("Disable时执行出场行为")]
        public bool mRunOutActionWhenEnable_;
        //[LabelText("Disable时执行出场行为遍历子节点"),ShowIf("mRunOutActionWhenEnable_")]
        public bool mRunOutActionWhenEnableRecursionChildren_;
        

        public void OnEnable()
        {
            if (mRunEnterActionWhenEnable_)
            {
                RunEnterAction(mRunEnterActionWhenEnableRecursionChildren_);
            }
        }

        public void OnDisable()
        {
            if (mRunOutActionWhenEnable_)
            {
                RunOutAction(mRunOutActionWhenEnableRecursionChildren_);
            }
        }

        bool isOutActionVaild()
        {
            if (string.IsNullOrEmpty(outActionDescription_))
                return false;
            //return GameActionManager.GetInstance().IsActionDescriptionNotRegisted(outActionDescription_);
            return false;
        }

        public void RunEnterAction(bool recursionChildren = false)
        {
            if (mEnterActionCheckCallBack_ != null)
            {
                if (!mEnterActionCheckCallBack_())
                    return;
            }
            RunAction(UIActionType.eEnter, recursionChildren, mEnterActionPreCallBack_, mEnterActionEndCallBack_);
        }

        public void RunEnterActionWithoutOtherCallBack(bool recursionChildren = false)
        {
            if (mEnterActionCheckCallBack_ != null)
            {
                if (!mEnterActionCheckCallBack_())
                    return;
            }
            RunAction(UIActionType.eEnter, recursionChildren, null, null);
        }

        public void RunOutAction(bool recursionChildren = false)
        {
            if (mOutActionCheckCallBack_ != null)
            {
                if (!mOutActionCheckCallBack_())
                    return;
            }
            RunAction(UIActionType.eOut, recursionChildren, mOutActionPreCallBack_, mOutActionEndCallBack_);
        }
        
        public void RunOutActionWithoutOtherCallBack(bool recursionChildren = false)
        {
            if (mOutActionCheckCallBack_ != null)
            {
                if (!mOutActionCheckCallBack_())
                    return;
            }
            RunAction(UIActionType.eOut, recursionChildren, null, null);
        }


        protected void RunAction(UIActionType type, bool recursionChildren = false, Action preCallBack = null, Action endCallBack = null)
        {
            //var gam = GameActionManager.GetInstance();
            if (recursionChildren)
            {
                var baseTrans = transform;
                if (baseTrans != null)
                {
                    //不处理未激活对象
                    var childList = baseTrans.GetComponentsInChildren<UI_Base>();
                    for (var i = 0; i < childList.Length; ++i)
                    {
                        //ActionSequence seq = null;
                        switch (type)
                        {
                            case UIActionType.eEnter:
                                {
                                    //var enterAction = gam.GetActionByDescription(childList[i].enterActionDescription_);
                                    //if (enterAction != null)
                                    //{
                                    //    seq = enterAction.m_Action;
                                    //}
                                }
                                break;
                            case UIActionType.eOut:
                                {
                                    //var outAction = gam.GetActionByDescription(childList[i].outActionDescription_);
                                    //if (outAction != null)
                                    //{
                                    //    seq = outAction.m_Action;
                                    //}
                                }
                                break;
                            default:
                                break;
                        }

                        //if (seq != null && seq.getActionCount() > 0)
                        if (false)
                        {
                            //var gameObj = childList[i].gameObject;
                            //childList[i].ResetToInitPosition();
                            //seq.Run(gameObj, null, preCallBack, endCallBack);
                        }
                        else
                        {
                            if (preCallBack != null || endCallBack != null)
                            {
                                if (preCallBack != null)
                                    preCallBack();
                                if (endCallBack != null)
                                    endCallBack();
                            }
                        }
                    }
                }
            }
            else
            {
                //ActionSequence seq = null;
                switch (type)
                {
                    case UIActionType.eEnter:
                        {
                            if (!mIsEnterActionOverridable_)
                            {
                                if (enterAction_ != null && enterAction_.IsPlaying() && !enterAction_.IsComplete())
                                {
                                    return;
                                }
                            }
                            //var enterAction = gam.GetActionByDescription(enterActionDescription_);
                            //if (enterAction != null)
                            //{
                            //    seq = enterAction.m_Action;
                            //    if (seq != null && seq.getActionCount() > 0)
                            //    {
                            //        if (enterAction_ != null && enterAction_.IsPlaying() && !enterAction_.IsComplete())
                            //        {
                            //            enterAction_.Kill(true);
                            //        }
                            //        if (outAction_ != null && outAction_.IsPlaying() && !outAction_.IsComplete())
                            //        {
                            //            outAction_.Kill(true);
                            //        }
                            //    }
                            //}
                        }
                        break;
                    case UIActionType.eOut:
                        {
                            if (!mIsOutActionOverridable_)
                            {
                                if (outAction_ != null && outAction_.IsPlaying() && !outAction_.IsComplete())
                                {
                                    return;
                                }
                            }
                            //var outAction = gam.GetActionByDescription(outActionDescription_);
                            //if (outAction != null)
                            //{
                            //    seq = outAction.m_Action;
                            //    if (seq != null && seq.getActionCount() > 0)
                            //    {
                            //        if (outAction_ != null && outAction_.IsPlaying() && !outAction_.IsComplete())
                            //        {
                            //            outAction_.Kill(true);
                            //        }
                            //        if (enterAction_ != null && enterAction_.IsPlaying() && !enterAction_.IsComplete())
                            //        {
                            //            enterAction_.Kill(true);
                            //        }
                            //    }
                            //}
                        }
                        break;
                    default:
                        break;
                }
                //if (seq != null && seq.getActionCount() > 0)
                if (false)
                {

                    this.ResetToInitPosition();
                    //var tween = seq.Run(gameObject, null, preCallBack);
                    Tween tween = null;
                    switch (type)
                    {
                        case UIActionType.eEnter:
                            enterAction_ = tween;
                            //enterAction_.OnComplete(() =>
                            //{
                            //    if (endCallBack != null)
                            //        endCallBack();
                            //    enterAction_ = null;
                            //});
                            break;
                        case UIActionType.eOut:
                            outAction_ = tween;
                            //outAction_.OnComplete(() =>
                            //{
                            //    if (endCallBack != null)
                            //        endCallBack();
                            //    outAction_ = null;
                            //});
                            break;
                    }
                }
                else
                {
                    if (preCallBack != null || endCallBack != null)
                    {
                        if (preCallBack != null)
                            preCallBack();
                        if (endCallBack != null)
                            endCallBack();
                    }
                }
            }
        }

        protected void ResetToInitPosition()
        {
            this.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(m_initAnchoredPosition3D.x, m_initAnchoredPosition3D.y, m_initAnchoredPosition3D.z);
            //this.transform.position = new Vector3(m_initPosition.x, m_initPosition.y, m_initPosition.z);
            //this.transform.localPosition = new Vector3(m_initLocalPosition.x, m_initLocalPosition.y, m_initLocalPosition.z);
        }

        [HideInInspector]
        public Vector3 m_initPosition;
        [HideInInspector]
        public Vector3 m_initLocalPosition;
        [HideInInspector]
        public Vector3 m_initAnchoredPosition3D;

        public void Awake()
        {
            m_initPosition = this.transform.position;
            m_initLocalPosition = this.transform.localPosition;
            var rectTR_ = this.GetComponent<RectTransform>();
            if (rectTR_ != null)
                m_initAnchoredPosition3D = this.GetComponent<RectTransform>().anchoredPosition3D;
            else
            {
                m_initAnchoredPosition3D = this.transform.position;
            }
            if (notActive_)
            {
                this.gameObject.SetActive(false);
            }
        }

        public void OnDestroy()
        {
            //var uim = UI_Manager.GetInstance();
            //if (uim != null)
            //    uim.UnRegisterRefreshEvent(this);
            OnDestroyCallBack?.Invoke();
        }
#if UNITY_EDITOR
        static Vector3[] fourCorners = new Vector3[4];
        static Rect tempUseRect = new Rect();

        private void OnDrawGizmos()
        {
            /*var gam = GameActionManager.GetInstance();
            bool showError = false;
            bool showWarn = false;
            if (!string.IsNullOrEmpty(enterActionDescription_))
            {
                if (gam.IsActionDescriptionNotRegisted(enterActionDescription_))
                {
                    showError = true;
                }
                if (!showError)
                {
                    var action = gam.GetActionByDescription(enterActionDescription_);
                    showWarn = !action.m_Action.IsApplyToCreate(this.gameObject);
                }
            }
            if (!showError && !string.IsNullOrEmpty(outActionDescription_))
            {
                if (gam.IsActionDescriptionNotRegisted(outActionDescription_))
                {
                    showError = true;
                }
                if (!showError && !showWarn)
                {
                    var action = gam.GetActionByDescription(outActionDescription_);
                    showWarn = !action.m_Action.IsApplyToCreate(this.gameObject);
                }
            }
            if (showError || showWarn)
            {
                var rectCom = this.GetComponent<RectTransform>();
                if (rectCom)
                {
                    rectCom.GetWorldCorners(fourCorners);
                    var midX = fourCorners[0].x + (fourCorners[2].x - fourCorners[0].x) * 0.5f;
                    var midY = fourCorners[0].y + (fourCorners[2].y - fourCorners[0].y) * 0.5f;
                    tempUseRect.Set(midX - 100, midY - 100, 200, 200);
                    if (showError)
                    {
                        Gizmos.DrawGUITexture(tempUseRect, GlobleConfig.GetUnityBuildinIcon("console.erroricon") as Texture);
                    }
                    else if (showWarn)
                    {
                        Gizmos.DrawGUITexture(tempUseRect, GlobleConfig.GetUnityBuildinIcon("console.warnicon") as Texture);
                    } 
                }
            }*/
        }
#endif
    }

    //[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    //public class EditAndClearActionButtonAttribute : Attribute
    //{
    //    public EditAndClearActionButtonAttribute()
    //    {
    //    }
    //}
}
