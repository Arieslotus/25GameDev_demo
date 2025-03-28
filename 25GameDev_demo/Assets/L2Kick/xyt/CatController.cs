using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;



public class CatController : MonoBehaviour
{
    [HideInInspector] public float rotationSpeed; // 旋转速度（度/秒）
    float startAngle = 0;
    float startOffsetAngle = 0;
    [HideInInspector] public float offsetAngle;

    public Transform footRTrans; // 右脚
    public Transform footLTrans; // 左脚
    public GameObject trailR1, trailR2, trailL1, trailL2;
    int kickNum = 0;

    public float popOutDistance = 0.5f; // 弹出距离
    public float popTime = 0.1f; // 弹出时间（秒）
    public float retractTime = 0.2f; // 收回时间（秒）
    public float judgeSpaceTime = 0.2f;//界定短按和长按的时间区分
    float currentSpaceTime = 0;
    bool isCalculatingSpaceTime = false;


    //dot
    [Header("dotween")]
    public float duration = 0.3f; // 振动持续时间
    public float strength = 0.2f; // 变形强度

    private Vector3 originalScale;

    [Header("miss")]
    public SpriteRenderer body_spriteRenderer;
    public Material catMiss_mat; // 要切换的材质
    private Material bodyOriginalMaterial; // 存储原始材质
    public float materialChangeDuration = 0.2f; // 材质更换持续时间

    public SpriteRenderer face_spriteRenderer;
    public Sprite normal, kick, miss;
    private Sprite faceOriginalSprite;
    bool isMissing = false;

    [Header("good")]
    private Material bracketsOriginalMaterial; // 存储原始材质
    public Material catGood_mat;
    public SpriteRenderer smallBracket_sp, bigBracket_sp;

    [Header("perfect")]
    public Material catPerfect_mat;

    //audio
    public AudioSource catSoundTap, catSoundHold;
    public AudioClip tap, hold;

    public enum KickType
    {
        Tap,
        Hold,
        None
    }

    class Foot
    {
        public int Id;
        public Transform footTrans;
        public Vector3 originalLocalPosition;
        public float animationProgress;
        public bool isPopping;//正在出腿
        public bool isExtended;//正在伸着腿
        public bool isRetracting;//正在收脚

        public KickType currentKickType;
      
    }
    Foot footR, footL;
    Foot currentControlFoot;
    [HideInInspector]
    public bool isOutRFoot,isOutLFoot;

    public GameObject perfectHoldL,perfectHoldR;
    public ParticleSystem[] perfectHoldEffect;



    //public float arcHeight = 0.2f; // 运动的弧度高度

    //处于一个有问题的代码但是能跑

    private void Awake()
    {
        // 设置初始旋转角度
        transform.eulerAngles = new Vector3(0, 0, offsetAngle);

        // init
        //r foot
        footR = new Foot();
        footR.Id = 2;
        footR.footTrans = footRTrans;
        footR.originalLocalPosition = footRTrans.localPosition;
        footR.animationProgress = 0;
        footR.isPopping = footR.isExtended = footR.isRetracting = false;
        footR.currentKickType = KickType.None;
        //l foot
        footL = new Foot();
        footL.Id = 1;
        footL.footTrans = footLTrans;
        footL.originalLocalPosition = footLTrans.localPosition;
        footL.animationProgress = 0;
        footL.isPopping = footL.isExtended = footL.isRetracting = false;
        footL.currentKickType= KickType.None;

    }

    void Start()
    {
        currentControlFoot = footL;

        //turn off trail
        //fx

        
            trailR1.GetComponent<TrailRenderer>().emitting = false;
            trailR1.GetComponent<TrailRenderer>().enabled = false;

            trailR2.GetComponent<TrailRenderer>().emitting = false;
            trailR2.GetComponent<TrailRenderer>().enabled = false;




        
            trailL1.GetComponent<TrailRenderer>().enabled = false;
            trailL1.GetComponent<TrailRenderer>().emitting = false;
            trailL2.GetComponent<TrailRenderer>().enabled = false;
            trailL2.GetComponent<TrailRenderer>().emitting = false;

        //dot
        originalScale = transform.localScale; // 记录初始大小

        //miss fx

        if (body_spriteRenderer != null)
        {
            bodyOriginalMaterial = body_spriteRenderer.material; // 记录原始材质
        }
        if (face_spriteRenderer != null)
        {
            faceOriginalSprite = face_spriteRenderer.sprite; // 记录原始材质
        }
        if(smallBracket_sp != null)
        {
            bracketsOriginalMaterial = smallBracket_sp.material;
        }

    }

    void Update()
    {
        // 旋转
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        //bool
        isOutLFoot = footL.isExtended && footL.currentKickType == KickType.Hold;
        isOutRFoot = footR.isExtended && footR.currentKickType == KickType.Hold;


        // 按下空格键触发踢脚动画
        if (Input.GetKeyDown(KeyCode.Space))
        {
            currentSpaceTime = 0;
            isCalculatingSpaceTime=true;

            if ( !currentControlFoot.isExtended)
            {
                if(currentControlFoot.isRetracting)
                {
                    // change current control foot
                    kickNum++;
                    currentControlFoot = (kickNum % 2 == 0) ? footL : footR;

                }
                else
                {                    
                    kickNum++;
                    currentControlFoot = (kickNum % 2 == 0) ? footL : footR;

                }
                currentControlFoot.isRetracting = false;
                currentControlFoot.isExtended = false;
                currentControlFoot.isPopping = true;
                currentControlFoot.animationProgress = 0f;

                currentControlFoot.currentKickType = KickType.Tap;//先假设是tap
            }

            //dot
            DoJellyEffect();
            //audio
            //catSound.PlayOneShot(tap);
            catSoundTap.PlayOneShot(tap);

        }

        if (isCalculatingSpaceTime)
        {
            currentSpaceTime += Time.deltaTime;

            //Debug.LogError($"current{currentSpaceTime} jude{judgeSpaceTime}");

            if (currentSpaceTime >= judgeSpaceTime)
            {
                //Debug.LogError($"{currentControlFoot.Id} holding fx");
                //Debug.LogError("...");
                currentControlFoot.isExtended = true;
                currentControlFoot.currentKickType = KickType.Hold;
            }
        }

        // 长按保持伸出
        if (Input.GetKey(KeyCode.Space) && currentControlFoot.isExtended)
        {
           // Debug.LogError($"foot{currentControlFoot.Id} is extended, fx , tyep{currentControlFoot.currentKickType}");
            
            if(currentControlFoot.currentKickType == KickType.Hold)
            {
                //fx
                if (currentControlFoot.Id == 2)
                {
                    trailR1.GetComponent<TrailRenderer>().enabled = true;
                    trailR1.GetComponent<TrailRenderer>().emitting = true;

                    trailR2.GetComponent<TrailRenderer>().enabled = true;
                    trailR2.GetComponent<TrailRenderer>().emitting = true;
                    if(L2CheckList.headCheckList.Count > 0)
                    {
                        if (L2CheckList.headCheckList[0].myFootStatus == footStatus.perfect)
                        {
                            //perfect
                            //Debug.Log("footStatus.perfect");
                            perfectHoldR.SetActive(true);
                            //PlayPerfectEffect(perfectHoldR);
                        }
                        if (L2CheckList.headCheckList[0].myFootStatus == footStatus.good)
                        {
                            //good
                        }
                    }

                    //audio
                    catSoundHold.Play();

                }
                else if (currentControlFoot.Id == 1)
                {

                    trailL1.GetComponent<TrailRenderer>().enabled = true;
                    trailL1.GetComponent<TrailRenderer>().emitting = true;

                    trailL2.GetComponent<TrailRenderer>().enabled = true;
                    trailL2.GetComponent<TrailRenderer>().emitting = true;

                    if (L2CheckList.headCheckList.Count > 0)
                    {
                        if (L2CheckList.headCheckList[0].myFootStatus == footStatus.perfect)
                        {
                            //perfect
                            //Debug.Log("footStatus.perfect");
                            perfectHoldL.SetActive(true);
                            //PlayPerfectEffect(perfectHoldL);
                        }
                        if (L2CheckList.headCheckList[0].myFootStatus == footStatus.good)
                        {
                            //good
                        }
                    }

                    //audio
                    catSoundHold.Play();
                }

            }
            else { 
                // tap
            }

            //cat face
            face_spriteRenderer.sprite = kick;


            return; // 如果已经伸出并按住空格，则不执行收回逻辑
        }
        else
        {
            //fx
            if (currentControlFoot.Id == 2)
            {
                trailR1.GetComponent<TrailRenderer>().emitting = false;
                trailR1.GetComponent<TrailRenderer>().enabled = false;

                trailR2.GetComponent<TrailRenderer>().emitting = false;
                trailR2.GetComponent<TrailRenderer>().enabled = false;

                perfectHoldR.SetActive(false);
                if (L2CheckList.headCheckList.Count > 0)
                {
                    if (L2CheckList.headCheckList[0].myFootStatus == footStatus.none)
                    {
                        perfectHoldL.SetActive(false);
                    }
                }
                else
                {
                    perfectHoldL.SetActive(false);
                }
                //audio
                catSoundHold.Stop();

            }
            else if (currentControlFoot.Id == 1)
            {
                trailL1.GetComponent<TrailRenderer>().enabled = false;
                trailL1.GetComponent<TrailRenderer>().emitting = false;
                trailL2.GetComponent<TrailRenderer>().enabled = false;
                trailL2.GetComponent<TrailRenderer>().emitting = false;

                perfectHoldL.SetActive(false);
                if (L2CheckList.headCheckList.Count > 0)
                {
                    if (L2CheckList.headCheckList[0].myFootStatus == footStatus.none)
                    {
                        perfectHoldL.SetActive(false);
                    }
                }
                else
                {
                    perfectHoldL.SetActive(false);
                }
                //audio
                catSoundHold.Stop();
            }

            //cat face
            if(!isMissing)
            {
                face_spriteRenderer.sprite = normal;
            }

            
        }






        // 松开空格键时收回
        if (Input.GetKeyUp(KeyCode.Space))
        {
            isCalculatingSpaceTime = false;
            //Debug.LogError($"time:{currentSpaceTime}");
            if (currentSpaceTime >= judgeSpaceTime)
            {

                currentControlFoot.isExtended = false;
                currentControlFoot.isPopping = false;
                currentControlFoot.isRetracting = true;
                currentControlFoot.animationProgress = 0f;

                // Debug.LogError("hold");
            }


            //currentControlFoot.currentKickType = KickType.None;
        }











        // 控制脚的弹出和收回
        AnimateFoot( footL);
        AnimateFoot( footR);

        //Debug.LogError($"footL animat p{footL.animationProgress}");
        //Debug.LogError($"footR animat p{footR.animationProgress}");


    }

    void AnimateFoot( Foot foot)
    {
        bool isAnimating = foot.isPopping || foot.isRetracting || foot.isExtended;
        if (isAnimating)
        {


            if (foot.isExtended)
            {


                if (foot.currentKickType == KickType.Hold)
                {
                    //if (foot.Id == 2)
                    //{
                    //    footRTrans.localPosition = foot.originalLocalPosition + Vector3.down * popOutDistance;

                    //}
                    //else if (foot.Id == 1)
                    //{
                    //    footLTrans.localPosition =  foot.originalLocalPosition + Vector3.down * popOutDistance;
                    //}
                    return;
                }
                else if (foot.currentKickType == KickType.Tap)
                {
                    foot.isExtended = false;
                    foot.isPopping = false;
                    foot.isRetracting = true;
                    foot.animationProgress = 0f;
                }


            }


            foot.animationProgress += Time.deltaTime / (foot.isPopping ? popTime : retractTime);
            //float t = animationProgress;
            //t = t * t * (3f - 2f * t); // 平滑曲线（更有力的踢出）

            if (foot.isPopping)
            {
                //// 计算抛物线轨迹（贝塞尔曲线）
                //Vector3 start = footOriginalLocalPosition;
                //Vector3 end = footOriginalLocalPosition + Vector3.down * popOutDistance;

                //// **动态计算控制点**
                //Vector3 direction = (end - start).normalized; // 获取方向
                //Vector3 perpDirection = new Vector3(-direction.y, direction.x, 0); // 获取垂直方向
                ////Vector3 control = (start + end) * 0.5f + perpDirection * arcHeight * Mathf.Clamp01(popOutDistance / 0.5f);
                //Vector3 control = start + Vector3.up * arcHeight + Vector3.right * (kickNum % 2 == 0 ? -arcHeight : arcHeight);
                //// 计算贝塞尔曲线位置
                //Vector3 bezierPos = (1 - t) * (1 - t) * start + 2 * (1 - t) * t * control + t * t * end;

                ////foot.localPosition = bezierPos;

                //foot.footTrans.localPosition = Vector3.Lerp(foot.originalLocalPosition, foot.originalLocalPosition + Vector3.down * popOutDistance, foot.animationProgress);
                
                if(foot.Id == 2)
                {
                    footRTrans.localPosition = Vector3.Lerp(foot.originalLocalPosition, foot.originalLocalPosition + Vector3.down * popOutDistance, foot.animationProgress);

                }
                else if(foot.Id == 1)
                {
                    footLTrans.localPosition = Vector3.Lerp(foot.originalLocalPosition, foot.originalLocalPosition + Vector3.down * popOutDistance, foot.animationProgress);
                }


                if (foot.animationProgress >= 1f)
                {
                    foot.isPopping = false;
                    foot.isRetracting = false;
                    foot.isExtended = true;
                    foot.animationProgress = 0f;

                }
            }

            if (foot.isRetracting)
            {
                if (foot.Id == 2)
                {
                    footRTrans.localPosition = Vector3.Lerp(foot.originalLocalPosition + Vector3.down * popOutDistance, foot.originalLocalPosition, foot.animationProgress);


                }
                else if (foot.Id == 1)
                {
                    footLTrans.localPosition = Vector3.Lerp(foot.originalLocalPosition + Vector3.down * popOutDistance, foot.originalLocalPosition, foot.animationProgress);
                }
                
                if (foot.animationProgress >= 1f)
                {
                    foot.isPopping = false;
                    foot.isExtended = false;
                    foot.isRetracting = false;
                    foot.animationProgress = 0f;
                }
            }
        }

    }

    void DoJellyEffect()
    {
        transform.DOKill(); // 先清除之前的动画，避免冲突

        transform.DOScale(new Vector3(originalScale.x * (1 + strength), originalScale.y * (1 - strength), originalScale.z), duration / 2)
            .SetEase(Ease.OutQuad) // 先放大一点
            .OnComplete(() =>
            {
                transform.DOScale(originalScale, duration / 2).SetEase(Ease.OutBounce); // 然后回弹
            });
    }

    //used in note cotroller
    public void MissEffect()
    {
        DoJellyEffect();
        if (catMiss_mat != null)
        {
            StartCoroutine(ChangeBodyMaterialTemporarily(catMiss_mat));
        }
    }

    //used in game cotroller
    public void GoodEffect()
    {
        if (catGood_mat != null)
        {
            StartCoroutine(ChangeBracketsMaterialTemporarily(catGood_mat));
        }
    }

    //used in game cotroller
    public void PerfectEffect()
    {
        if (catPerfect_mat != null)
        {
            StartCoroutine(ChangeBracketsMaterialTemporarily(catPerfect_mat));
        }
    }

    IEnumerator ChangeBodyMaterialTemporarily(Material newMat)
    {
        if (body_spriteRenderer != null)
        {
            body_spriteRenderer.sharedMaterial = newMat; // 更换材质
            face_spriteRenderer.sprite = miss;
            isMissing = true;
            yield return new WaitForSeconds(materialChangeDuration); // 等待
            body_spriteRenderer.sharedMaterial = bodyOriginalMaterial; // 恢复原材质
            face_spriteRenderer.sprite = faceOriginalSprite;
            isMissing = false;
        }
    }
    IEnumerator ChangeBracketsMaterialTemporarily(Material newMat)
    {
        if (smallBracket_sp != null && bigBracket_sp != null)
        {
            smallBracket_sp.sharedMaterial = newMat; // 更换材质
            bigBracket_sp.sharedMaterial = newMat; // 更换材质

            yield return new WaitForSeconds(materialChangeDuration); // 等待
            smallBracket_sp.sharedMaterial = bracketsOriginalMaterial; // 恢复原材质
            bigBracket_sp.sharedMaterial = bracketsOriginalMaterial; // 恢复原材质


        }
    }

    //void PlayPerfectEffect(GameObject eff)
    //{
    //    Debug.Log("ins hold effect");
    //    perfectHoldEffect = eff.GetComponentsInChildren<ParticleSystem>();
    //    foreach (ParticleSystem ps in perfectHoldEffect)
    //    {
    //        ps.Play();
    //    }
    //    //Destroy(smoke, 2f);
    //}

}