using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SS;

public class AdventureTaskSystem : Singleton<AdventureTaskSystem> {
    [SerializeField] private List<AdventureTask> tasks = new List<AdventureTask>();
    [SerializeField] private List<ATCharacter> characters = new List<ATCharacter>();

    int taskIndex = 0;
    AdventureTask currTask = null;

    private void Start()
    {
        foreach (AdventureTask task in tasks)
        {
            if (task != null && task.gameObject != null)
            {
                task.gameObject.SetActive(false);
            }
        }
        SetNextTask(0);
    }

    private void SetNextTask(int index)
    {
        if (index >= 0 && tasks.Count > index && tasks[index] != null)
        {
            taskIndex = index;
            currTask = tasks[taskIndex];
            if (currTask)
            {
                currTask.Init();
                currTask.gameObject.SetActive(true);
            }
        }
    }

    private void Update()
    {
        if (taskIndex >= 0 && tasks.Count > taskIndex && tasks[taskIndex] != null)
        {
            if (currTask)
            {
                if (currTask.IsFinished)
                {
                    SetNextTask(taskIndex++);
                }
                else
                {
                    foreach (ATCharacter ch in characters)
                    {
                        ch.OnUpdate(currTask);
                    }
                }
            }
        }
    }
}
