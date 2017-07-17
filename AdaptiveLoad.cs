using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//作者：遗忘的银灵
//FGO英灵动画读取器——通用版（适用于大部分fgo其它.unity3d单位？）
//比专用版适用性更强，可以读取魔神柱等等敌人的动作和模型
//不过还是只能在编辑器里使用，无法导出为exe独立文件...因为这些assetbundle都是有限制的。。。
//操作:按j键可以切换动画（为防止一次跳过数个动画，每次按键有效间隔1秒）
//注释就不全写了，请看customLoad.cs
public class AdaptiveLoad : MonoBehaviour
{
    GameObject ab;
    public string assetName = "/Jean.unity3d";//读取的包的名字，建议在unity editor的loadadaptive里修改
    public string prefabName = "chr";
    public int controlNum = 1;
    string[] clipName = new string[100];
    AnimationClip[] acm = new AnimationClip[25];
    double timeCD = 1;
    int j = 0;
    // Use this for initialization  
    void Start()
    {

        StartCoroutine(LoadScene());
        
    }

    // Update is called once per frame  
    void Update()
    {
        
        if (ab == null)
        {
            return;
        }
        Animation animation = ab.GetComponent<Animation>();
        int ik = animation.GetClipCount();

        int idx = 0;

        //自动读取animationClip
        foreach (AnimationState anim in animation)
        {
            // 显示有哪些animation,debug用
            //Debug.Log("Animation (" + idx + "): " + anim.name);
            clipName[idx] = anim.name;
            acm[idx] = anim.clip;
            idx++;
        }
        ik = idx + 1;
        timeCD -= Time.deltaTime;
        if (Input.GetKey("j") && timeCD < 0)
        {
            timeCD = 2;

            animation.Play(clipName[j]);
            j++;
            if (j >= ik)
            {
                j = 0;
            }
        }

        if (Input.GetKey("1") && controlNum == 1)
        {
            animation.Play("attack_a");
        }
        if (Input.GetKey("2") && controlNum == 2)
        {
            animation.Play("treasureArms2_a");
        }
    }

    IEnumerator LoadScene()
    {
        WWW www;
        //读取的文件路径，也就是.unity3d的那个  
        //www = new WWW("file:///" + Application.dataPath + assetName);
        www = WWW.LoadFromCacheOrDownload("file:///" + Application.dataPath + assetName, 1);
        yield return www;
        AssetBundle asb = www.assetBundle;


        UnityEngine.Object ao = asb.LoadAsset(prefabName);

        ab = (GameObject)Instantiate(ao);
        ab.transform.position = this.transform.position;
        ab.transform.parent = this.transform;
        ab.transform.localPosition = new Vector3(0, 0, 0);
        ab.transform.localRotation = Quaternion.identity;
        

        Transform tmp;
        Renderer rend;
        //这儿用的shader是Transparent Cutout，但不是适用于每个单位，因为透明度只有0和1的分别，
        // 有些带半透明的单位就很难办，比如魔神柱子的眼睛...
        Shader shader2 = Shader.Find("Custom/Transparent Cutout");
        //所以也可以用以下的代码改变定义的shader,反注释掉就好
        shader2 = Shader.Find("Legacy Shaders/Transparent/Cutout/Soft Edge Unlit");



        try {
            tmp = ab.transform.Find("body_level_1");
            rend = tmp.GetComponent<Renderer>();
            rend.material.shader = shader2;
        }
        catch (Exception e) {

        }


        try
        {
            rend = ab.transform.Find("body_level_2").GetComponent<Renderer>();
            rend.material.shader = shader2;
            //ab.transform.Find("body_level_2").gameObject.SetActive(false);
        }
        catch (Exception e) { }


        try
        {
            rend = ab.transform.Find("body_level_3").GetComponent<Renderer>();
            rend.material.shader = shader2;
            //ab.transform.Find("body_level_3").gameObject.SetActive(false);
        }
        catch (Exception e) { }

        

        //因为是适用版，不一定有武器，所以一下代码有可能会报错。到时候注释掉就行。
        rend = ab.transform.Find("weapon_level_1_2_3").GetComponent<Renderer>();
        rend.material.shader = shader2;


        //ab.transform.Find("weapon_level_1_2_3").gameObject.SetActive(false);


        /*foreach (Transform child in transform)
        {
            Debug.Log(child.gameObject.name);
        }*/


    }
}