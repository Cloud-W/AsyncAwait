using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    [Serializable]
    public class CustomTask
    {
        [NonSerialized] public CustomTaskGameObject GameObject;
        public string Name;
        public int TaskLifeTime;
        public int Step;
        public int WaitTime;
    }

    [SerializeField] private CustomTaskGameObject _taskPrefab;
    [SerializeField] private List<CustomTask> _customTasks = new List<CustomTask>();
    [SerializeField] private Text TaskFinished;

    async void Start()
    {
        for (var index = 0; index < _customTasks.Count; index++)
        {
            var task = _customTasks[index];
            var taskGameObject = Instantiate(_taskPrefab);
            taskGameObject.transform.position =
                new Vector3((index - _customTasks.Count / 2) * 2 + _customTasks.Count % 2, 0, 0);
            taskGameObject.TextTaskName.text = task.Name;
            taskGameObject.TextTaskTime.text = task.TaskLifeTime.ToString();
            taskGameObject.TextWaitTime.text = task.WaitTime.ToString();
            task.GameObject = taskGameObject;
        }

        ExecuteTasks();
    }
    
    async void ExecuteTasks()
    {
        int step = 0;

        int totalStep = _customTasks.Last().Step;
        while (step <= totalStep)
        {
            var customTasks = _customTasks.FindAll(x => x.Step == step);
            List<Task> tasks = new List<Task>();
            foreach (var ct in customTasks)
            {
                var task = ExecuteTask(ct);
                tasks.Add(task);
                ct.GameObject.AnimatorTask.SetTrigger("Running");
                await task;
               
                if (ct.WaitTime > 0)
                {
                    tasks.Add(ExecuteWaitTask(ct));
                }
            }

            await Task.WhenAll(tasks);
            step++;
        }


        Debug.Log("开喝！");
    }


    async Task ExecuteTask(CustomTask customTask)
    {
        try
        {
            while (customTask.TaskLifeTime > 0)
            {
                await Task.Delay(1000);
                customTask.TaskLifeTime--;
                customTask.GameObject.TextTaskTime.text = customTask.TaskLifeTime.ToString();
            }
            
            customTask.GameObject.AnimatorTask.SetTrigger("Finished");
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        
    }

    async Task ExecuteWaitTask(CustomTask customTask)
    {
        try
        {
            while (customTask.WaitTime > 0)
            {
                await Task.Delay(1000);
                customTask.WaitTime--;
                if (customTask.GameObject) customTask.GameObject.TextWaitTime.text = customTask.WaitTime.ToString();
            }
            
            
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
}