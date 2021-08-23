using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityEngine.SceneManagement;

public class Experiment : MonoBehaviour
{   
    public GameManager gameManager;
    public UIManager uIManager;

    private int[] testCamera;
    private int[,] cameraConfigurations;

    public int[] testTask;
    public int[] testLevel;
    public int[] testTrial;
    
    public int[] cameraIndices;

    public int[] taskIndices;
    public int[] levelIndices;
    public int[] trialIndices;

    private int experimentLength;
    private int currentIndex;
    
    private bool moved;

    // levelt

    void Start()
    {
        // Procedure marker
        currentIndex = 0;

        // Predifined full configuration
        cameraConfigurations = new int [,] 
                               {{1, 1, 1}, {0, 1, 1}, {1, 0, 1}, {1, 1, 0}};
    }

    void Update()
    {
        if (!moved &&
            (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) ||
             Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D)) )
            moved = true;
    }

    public void StartExperiment()
    {
        CreateIndicesArray();
        currentIndex = 0;

        gameManager.isExperimenting = true;
        int cameraConfigIndex = cameraIndices[currentIndex];
        gameManager.LoadSceneWithRobot(taskIndices[currentIndex], 
                                       levelIndices[currentIndex],
                                       cameraConfigurations[cameraConfigIndex, 0],
                                       cameraConfigurations[cameraConfigIndex, 1],
                                       cameraConfigurations[cameraConfigIndex, 2]);

        moved = false;
        StartCoroutine(StartRecordOnAction());
    }

    public void ReloadLevel()
    {
        StopCoroutine(StartRecordOnAction());
        if (gameManager.isRecording)
            gameManager.Record();

        gameManager.ReloadScene();

        moved = false;
        StartCoroutine(StartRecordOnAction());
    }

    public void NextLevel()
    {
        currentIndex += 1;
        if (currentIndex != experimentLength)
        {
            int cameraConfigIndex = cameraIndices[currentIndex];
            gameManager.LoadSceneWithRobot(taskIndices[currentIndex], 
                                           levelIndices[currentIndex],
                                           cameraConfigurations[cameraConfigIndex, 0],
                                           cameraConfigurations[cameraConfigIndex, 1],
                                           cameraConfigurations[cameraConfigIndex, 2]);
            moved = false;
            StartCoroutine(StartRecordOnAction());
        }
        else
        {
            uIManager.PopMessage("You have finished all the experiments!");
        }
    }
    private IEnumerator StartRecordOnAction()
    {
        yield return new WaitUntil(() => moved == true);
        gameManager.Record(currentIndex.ToString() + "- " + 
                           trialIndices[currentIndex].ToString() + "; ");
    }

    public void SetExperimentConditions(bool[] conditions)
    {
        // Task array
        testTask = ConditionToIndexArray(conditions.Skip(0).Take(5).ToArray());

        // Camera configuration array
        bool[] cameraConditionsTemp = new bool[4];
        cameraConditionsTemp[0] = true;
        conditions.Skip(5).Take(3).ToArray().CopyTo(cameraConditionsTemp, 1);

        testCamera = ConditionToIndexArray(cameraConditionsTemp);

        // Level array
        testLevel = ConditionToIndexArray(conditions.Skip(8).Take(3).ToArray());

        // Trial array
        testTrial = ConditionToIndexArray(conditions.Skip(11).Take(2).ToArray());
    }
    private int[] ConditionToIndexArray(bool[] conditions)
    {
        int[] indices;

        // Initialization
        int count = 0;
        foreach (bool condition in conditions)
            if (condition)
                count += 1;
        indices = new int[count];

        // Fill
        int i = 0;
        for (int c = 0; c < conditions.Length; ++c)
            if (conditions[c])
            {
                indices[i] = c;
                i += 1;
            }
        return indices;
    }
        
    private void CreateIndicesArray()
    {
        System.Random random = new System.Random();

        // Intialize indices array
        experimentLength = testCamera.Length * testTask.Length * 
                           testLevel.Length * testTrial.Length;

        cameraIndices = new int[experimentLength];

        taskIndices = new int[experimentLength];
        levelIndices = new int[experimentLength];
        trialIndices = new int[experimentLength];

        // Extend
        int count = 0;
        // level
        count = 0;
        for (int l = 0; l < testLevel.Length; ++l)
        {
            for (int s = 0; s < testTrial.Length * testCamera.Length * testTask.Length; ++s)
            {
                levelIndices[count] = testLevel[l];
                count += 1;
            }
        }
        // trial
        count = 0;
        for (int l = 0; l < testLevel.Length; ++l)
        {
            for (int tr = 0; tr < testTrial.Length; ++tr)
            {
                for (int s = 0; s < testCamera.Length * testTask.Length; ++s)
                {
                    trialIndices[count] = testTrial[tr];
                    count += 1;
                }
            }
        }
        // task
        count = 0;
        for (int l = 0; l < testLevel.Length; ++l)
        {
            for (int tr = 0; tr < testTrial.Length; ++tr)
            {
                int[] randomTestTask = testTask.OrderBy(x => random.Next()).ToArray();
                for (int ta = 0; ta < testTask.Length; ++ta)
                {
                    for (int s = 0; s < testCamera.Length; ++s)
                    {
                        taskIndices[count] = randomTestTask[ta];
                        count += 1;
                    }
                }
            }
        }
        // camera
        count = 0;
        for (int l = 0; l < testLevel.Length; ++l)
        {
            for (int tr = 0; tr < testTrial.Length; ++tr)
            {
                for (int ta = 0; ta < testTask.Length; ++ta)
                {
                    int[] randomTestCamera = 
                          testCamera.OrderBy(x => random.Next()).ToArray();
                    for (int s = 0; s < testCamera.Length; ++s)
                    {
                        cameraIndices[count] = randomTestCamera[s];
                        count += 1;
                    }
                }
            }
        }

        currentIndex = 0;
    }
}