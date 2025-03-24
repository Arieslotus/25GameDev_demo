using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;

public class L2gameController : MonoBehaviour
{
    [Header("��ת����")]
    public Transform Spawner;
    public Transform spawnerTarget; // Χ����ת��Ŀ�����
    public float spawnerRadius = 13f; // ��ת�뾶
    public float rotationSpeed; // ��ת�ٶȣ��Ƕ�/s��

    private float angle = 0f; // ��ǰ��ת�Ƕ�
    float startOffsetAngle = -90; //dont edit //��ʼ��ת��ƫ��90d
    float catRotationOffsetAngle;

    [Header("������������")]
    public GameObject tap; // ����Ԥ����
    public GameObject hold; // ����Ԥ����
    public GameObject head;
    //public float spawnInterval = 1f; // ����������ʱ����
    public float spawnInterval = 0.1f; // ���� Note ��ʱ����


    [Header("Ŀ������")]
    public Transform noteTarget; // Ŀ��㣨���ڴ��ݸ�������������
    public float accCircleRadius = 2f; // ԲȦ�İ뾶�����ڴ��ݸ�������������
    public float moveToAccTime = 2f; // �����ƶ���׼ȷ�ߵ�ʱ�䣨���ڴ��ݸ�������������

    [Header("è")]
    public CatController cat;

    [Header("����������")]
    public AudioSource musicPlayer;
    public TextAsset chart;
    private float[] timeStamps;
    private int[] noteType;//1tap, 2hold
    private float[] noteHoldTime;

    [Header("�����ж�����")]
    public float checkTimeRange = 0.16f;
    public float perfectCheckTimeRange = 0.08f;

    [HideInInspector]public float myTime = 0;//��ʱ��
    private int index = 0;

    [Header("����")]
    [HideInInspector] public int health;
    [HideInInspector] public int score;
    [HideInInspector] public int combo;
    [HideInInspector] public int totalScore;
    public GameObject UI_combo;
    public GameObject UI_health;
    public Text text_score;
    public Text text_health;
    public Text text_combo;
    public int scorePerPerfect;
    public int scorePerGood;
    public int scorePerMiss;
    public int numToShowCombo;

    [Header("�ж�")]
    public GameObject perfect;
    public GameObject good;
    public GameObject miss;
    public Transform spawnPos;
    //public float riseSpeed = 1f; // �ϸ��ٶ�
    public float fadeOutDuration = 1f; // ��������ʱ��
    public float destroyDelay = 1f; // �����ӳ�ʱ��
    public float riseDuration = 1f; // �ϸ�����ʱ��
    public float pauseDuration = 0.5f; // ͣ��ʱ��
    public float targetHeight = 2f; // ͣ��ʱ�ĸ߶�
    public float continueRiseSpeed = 2f;
    public bool startSpawn = false;

    [Header("��Ϸ����")]
    public GameObject gamefailed;
    public GameObject gameCompleted;
    public GameObject resultPage;
    public Image result;
    public Sprite S;
    public Sprite A;
    public Sprite B;
    [HideInInspector]
    public int perfectNum;
    public int goodNum;
    public int missNum;
    public int bestComboNum;







    private void Awake()
    {
        LoadChart();
        musicPlayer.Pause();
        StartCoroutine("GameStart");
        //������ʱ��-3+2��
        myTime = -3 + moveToAccTime;

        //rotate           
        catRotationOffsetAngle = -moveToAccTime * rotationSpeed;

        angle += startOffsetAngle;
        cat.offsetAngle = catRotationOffsetAngle;

        SpawnerRote();
    }

    void Start()
    {
        // ʹ�� InvokeRepeating ÿ��һ��ʱ����������
        //InvokeRepeating(nameof(SpawnNote), 0f, spawnInterval);

        //cat
        cat.rotationSpeed = rotationSpeed;
        score = 0;
        health = 3;
        UI_combo.SetActive(false);

        Debug.Log("musicPlayer.clip.length" + musicPlayer.clip.length);
    }


    void Update()
    {
        SpawnerRote();
        myTime += Time.deltaTime;
        if (index < timeStamps.Length)
        {
            if (myTime >= timeStamps[index])
            {
                //Debug.Log("noteType[index]" + noteType[index]);
                //Instantiate(tap, Spawner.position, Quaternion.identity);
                if (noteType[index] == 1)
                {
                    //Debug.Log("Spawn Tap");
                    SpawnNote();
                }
                else if (noteType[index] == 2)
                {
                    SpawnHoldNote(index);
                }

                index++;
            }

        }

        //test
        if (startSpawn)
        {
            if (myTime > 0.8)
            {
                Instantiate(perfect,spawnPos);
                myTime = 0;
            }
        }

        //if (health <= 0)
        //{
        //    ////ʧ����ʾ
        //    //StartCoroutine(EndWithDelay(false));
        //    //enabled = false; // ���� Update ���
        //}


        //���ֲ������
        //if (!musicPlayer.isPlaying)
            //Debug.Log("musicPlayer!IsPlaying");
        //Debug.Log("musicPlayer.time" + musicPlayer.time);
        if (musicPlayer.isPlaying && musicPlayer.time >= musicPlayer.clip.length-0.2)
        {
            //musicPlayer.clip.length
            Debug.Log("music over");
            //�ɹ���ʾ
            StartCoroutine(EndWithDelay(true));
            enabled = false; // ���� Update ���
        }
    }

    IEnumerator EndWithDelay(bool isCompleted)
    {
        yield return new WaitForSeconds(2); // �ȴ��ӳ�ʱ��

        resultPage.gameObject.SetActive(true);
        //if (isCompleted)
        //{
        //    result.sprite = resultImage_Win;
        //}
        //else
        //{
        //    result.sprite = resultImage_Failed;
        //}
    }

    IEnumerator GameStart()
    {
        yield return new WaitForSeconds(3);
        musicPlayer.Play();
    }

    void SpawnerRote()
    {
        // ������ת�Ƕ�(-˳ʱ��/+��ʱ��)
        angle += rotationSpeed * Time.deltaTime;
        // �����µ�λ��
        float x = spawnerTarget.position.x + Mathf.Cos(angle * Mathf.Deg2Rad) * spawnerRadius;//Mathf.Cos �� Mathf.Sin �����Ǻ����� Unity ��ֻ���ܻ�������Ϊ������
        float y = spawnerTarget.position.y + Mathf.Sin(angle * Mathf.Deg2Rad) * spawnerRadius;
        // ���µ�ǰ�����λ��
        Spawner.position = new Vector3(x, y, transform.position.z);
    }
    private void SpawnNote()
    {
        // ʵ��������Ԥ����
        GameObject note = Instantiate(tap, Spawner.position, Quaternion.identity);
    }

    void SpawnHoldNote(int index)
    {
        StartCoroutine(SpawnHoldNotesCoroutine(index));
    }
    private IEnumerator SpawnHoldNotesCoroutine(int index)
    {
        float elapsedTime = 0f; // �ѹ�ȥ��ʱ��

        while (elapsedTime < noteHoldTime[index]- spawnInterval)
        {
            //Debug.Log("startSpawnHold");
            //�׸�hold
            if (elapsedTime ==0f)
            {
               // Debug.Log("spawnHead");
                GameObject note = Instantiate(head, Spawner.position, Quaternion.identity);
                note.GetComponent<L2HoldController>().holdTime = noteHoldTime[index];
                note.GetComponent<L2HoldController>().isFirst = true;
            }
            else
            {
                //Debug.Log("spawnHold");
                GameObject note = Instantiate(hold, Spawner.position, Quaternion.identity);
                note.GetComponent<L2HoldController>().isFirst = false;
            }

            // �ȴ� spawnInterval ʱ��
            yield return new WaitForSeconds(spawnInterval);

            // �����ѹ�ȥ��ʱ��
            elapsedTime += spawnInterval;
        }
    }

    // �� Scene ��ͼ�л���ԲȦ��������ԣ�
    private void OnDrawGizmosSelected()
    {
        if (noteTarget != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(noteTarget.position, accCircleRadius);
        }
    }

    void LoadChart()
    {
        string[] eachLine = chart.text.Split("\n");
        timeStamps = new float[eachLine.Length];
        noteType = new int[eachLine.Length];
        noteHoldTime = new float[eachLine.Length];
        for(int i = 0; i < eachLine.Length; i++)
        {
            string[] eachPart = eachLine[i].Split(",");
            //tap
            if (eachPart.Length==2)
            {
                timeStamps[i] = Convert.ToSingle(eachPart[0]);
                noteType[i] = 1;
                totalScore += scorePerPerfect;
            }
            //hold
            else if (eachPart.Length==3)
            {
                timeStamps[i] = Convert.ToSingle(eachPart[0]);
                noteType[i] = 2;
                noteHoldTime[i] = Convert.ToSingle(eachPart[2]);
            }
            
            //noteType = Convert.ToSingle(eachPart[1]);
        }
    }

    public void JudgeNote(float hitTime)
    {
        hitTime = System.Math.Abs(hitTime);
        FindObjectOfType<L2CameraShake>().ShakeCamra();
        if (hitTime < perfectCheckTimeRange / 2) //perfect
        {
            score += scorePerPerfect;
            perfectNum++;
            if (L2CheckList.graderList != null && L2CheckList.graderList.Count > 2)
            {
                foreach (var grader in L2CheckList.graderList)
                {
                    if (grader != null)
                    {
                        Destroy(grader.gameObject);
                    }
                }

                // ����б�
                L2CheckList.graderList.Clear();
            }
            Instantiate(perfect,spawnPos);
        }
        else
        {
            score += scorePerGood;
            goodNum++;
            if (L2CheckList.graderList != null && L2CheckList.graderList.Count > 2)
            {
                foreach (var grader in L2CheckList.graderList)
                {
                    if (grader != null)
                    {
                        Destroy(grader.gameObject);
                    }
                }

                // ����б�
                L2CheckList.graderList.Clear();
            }
            Instantiate(good,spawnPos);
        }
        combo++;
        if (bestComboNum < combo)
        {
            bestComboNum = combo;
        }
        if (combo > numToShowCombo)
        {
            UI_combo.SetActive(true);
        }
        text_score.text = "" + score;
        text_combo.text = "" + combo;
    }

    public void MissNote()
    {
        combo = 0;
        score += scorePerMiss;
        missNum++;
        health--;
        if (UI_health.transform.childCount > 0)
        {
            // ��ȡ��һ��������
            Transform firstChild = UI_health.transform.GetChild(0);
            // ���ٵ�һ��������
            Destroy(firstChild.gameObject);
        }
        text_score.text = "" + score;
        text_health.text = "" + health;
        if (L2CheckList.graderList != null && L2CheckList.graderList.Count > 2)
        {
            foreach (var grader in L2CheckList.graderList)
            {
                if (grader != null)
                {
                    Destroy(grader.gameObject);
                }
            }

            // ����б�
            L2CheckList.graderList.Clear();
        }
        Instantiate(miss,spawnPos);
        UI_combo.SetActive(false);
    }

}
