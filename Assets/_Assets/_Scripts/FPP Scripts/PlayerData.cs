using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class PlayerData : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI primaryWeponBulletCount,secondaryWeponBulletCount/* ,meleWeponTotalAmount */;
    [SerializeField] private Image primeryWepon,secondaryWepon,meleWepon;
    [SerializeField] private PlayerType playerType;
    public enum PlayerType{
        Frindely,Enemy
    }
    public void SetBulletCount(int currentAmount,int maxAmount,Gun.WeponPositions weponType){
        switch(weponType){
            case Gun.WeponPositions.Primary:
                primaryWeponBulletCount.SetText(string.Concat(currentAmount,"/",maxAmount));
            break;
            case Gun.WeponPositions.Secondary:
                secondaryWeponBulletCount.SetText(string.Concat(currentAmount,"/",maxAmount));
            break;
            /* case Gun.WeponPositions.Mele:
                meleWeponTotalAmount?.SetText(string.Concat("-","/","-"));
            break; */
        }
    }
    public void SetWeponIcons(Sprite iconSprite,Gun.WeponPositions weponType){
        switch(weponType){
            case Gun.WeponPositions.Primary:
                primeryWepon.sprite = iconSprite;
            break;
            case Gun.WeponPositions.Secondary:
                secondaryWepon.sprite = iconSprite;
            break;
            case Gun.WeponPositions.Mele:
                meleWepon.sprite = iconSprite;
            break;
        }
    }
    
    
    public void SetPlayerType (PlayerType playerType){
        this.playerType = playerType;
    }
    public PlayerType GetPlayerType(){
        return playerType;
    }
}