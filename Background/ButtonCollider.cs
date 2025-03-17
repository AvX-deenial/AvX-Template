using Photon.Pun;
using UnityEngine;
using static AVTemp.Menu.WristMenu;

namespace AVTemp.Background
{
	internal class Button : MonoBehaviour
	{
		public string relatedText;

		public static float buttonCooldown = 0f;
		
		public void OnTriggerEnter(Collider collider)
		{
			if (Time.time > buttonCooldown && collider == buttonCollider && wristMenu != null)
			{
				buttonCooldown = Time.time + 0.2f;
				GorillaTagger.Instance.StartVibration(rightHandMenu, GorillaTagger.Instance.tagHapticStrength / 2f, GorillaTagger.Instance.tagHapticDuration / 2f);
                GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(buttonSound, rightHandMenu, 0.7f);
                Toggle(this.relatedText);
            }
		}
	}
}
