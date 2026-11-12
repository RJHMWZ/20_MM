using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Match3.UI
{
    public class TargetUIItem : MonoBehaviour
    {
        public Image iconImage;
        public TextMeshProUGUI amountText;
        [Header("完成时的显示")]
        public GameObject completedImageObj; // 挂载完成后的图片对象

        private int requiredAmount;

        public void Setup(Sprite icon, int amount)
        {
            if (iconImage != null) iconImage.sprite = icon;
            requiredAmount = amount;
            
            if (completedImageObj != null)
            {
                completedImageObj.SetActive(false);
            }
            if (amountText != null)
            {
                amountText.gameObject.SetActive(true);
            }
            
            UpdateAmount(amount); // initially show full required amount
        }

        public void UpdateAmount(int currentRemaining)
        {
            if (currentRemaining <= 0)
            {
                if (completedImageObj != null)
                {
                    if (amountText != null) amountText.gameObject.SetActive(false);
                    completedImageObj.SetActive(true);
                }
                else if (amountText != null)
                {
                    amountText.text = "✓"; // Show checkmark when done
                    amountText.color = Color.green;
                }
            }
            else
            {
                if (completedImageObj != null)
                {
                    completedImageObj.SetActive(false);
                }
                if (amountText != null)
                {
                    amountText.gameObject.SetActive(true);
                    amountText.text = currentRemaining.ToString();
                }
            }
        }
    }
}