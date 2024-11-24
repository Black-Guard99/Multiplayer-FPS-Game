using UnityEngine;
[CreateAssetMenu(menuName = "Configs/AmmoConfig", fileName = "AmmoConfig")]
public class AmmoConfig : StatSO {
    public int maxAmmo = 300;
    public int clipSize = 30;
    public int currentMaxAmmoo = 120;
    public int currentClipAmmo = 30;
    public void Reload(){
        int maxReloadAmount = Mathf.Min(clipSize,currentMaxAmmoo);
        int availableBulletsinCurrentClip = clipSize - currentClipAmmo;
        int reloadAmount = Mathf.Min(maxReloadAmount,availableBulletsinCurrentClip);
        currentClipAmmo += reloadAmount;
        currentMaxAmmoo -= reloadAmount;
    }

    public bool CanReload() {
        return currentClipAmmo < clipSize && currentMaxAmmoo > 0;
    }
}