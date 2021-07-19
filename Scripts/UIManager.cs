using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    [SerializeField] private Text ammoText;
    [SerializeField] private Text lvlText;
    [SerializeField] private Text expText;
    [SerializeField] private Text modeText;
    [SerializeField] private Slider HPBar;
    public Text damageTip;

    public static UIManager Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<UIManager>();
            return _instance;
        }
    }

    //显示背包子弹数目
    public void UpdateAmmoText(int ammoInCartridge, int ammoRemain)
    {
        ammoText.text = ammoInCartridge + "/" +ammoRemain;
    }
    
    //显示经验值
    public void UpdateExpText(int exp)
    {
        expText.text = "exp " + exp;
    }
    
    //显示等级
    public void UpdateLvlText(int lvl)
    {
        lvlText.text = "lvl "+ lvl;
    }
    
    //显示血条
    public void UpdateHPBar(int hp)
    {
        HPBar.value = hp;
    }
    
    //显示射击模式
    public void UpdateModeText(string mode)
    {
        modeText.text = mode;
    }
    
    //显示攻击伤害
    public void showDamage(Vector3 loc, int damage)
    {
        damageTip.transform.gameObject.SetActive(true);
        damageTip.text = "-" + damage;
        damageTip.transform.position = loc;
        StartCoroutine(flowDamage());
    }

    //显示伤害跳字
    IEnumerator flowDamage()
    {
        for (int i = 0; i < 10; i++)
        {
            damageTip.transform.position = new Vector3(damageTip.transform.position.x,
                damageTip.transform.position.y+5,damageTip.transform.position.z);
            yield return new WaitForSeconds(0.1f);
        }
        damageTip.transform.gameObject.SetActive(false);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        damageTip.transform.gameObject.SetActive(false);
    }

}
