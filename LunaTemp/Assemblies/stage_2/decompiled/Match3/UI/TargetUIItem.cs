using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.UI
{
	public class TargetUIItem : MonoBehaviour
	{
		public Image iconImage;

		public TextMeshProUGUI amountText;

		[Header("完成时的显示")]
		public GameObject completedImageObj;

		private int requiredAmount;

		public void Setup(Sprite icon, int amount)
		{
			if (iconImage != null)
			{
				iconImage.sprite = icon;
			}
			requiredAmount = amount;
			if (completedImageObj != null)
			{
				completedImageObj.SetActive(false);
			}
			if (amountText != null)
			{
				amountText.gameObject.SetActive(true);
			}
			UpdateAmount(amount);
		}

		public void UpdateAmount(int currentRemaining)
		{
			if (currentRemaining <= 0)
			{
				if (completedImageObj != null)
				{
					if (amountText != null)
					{
						amountText.gameObject.SetActive(false);
					}
					completedImageObj.SetActive(true);
				}
				else if (amountText != null)
				{
					amountText.text = "✓";
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
