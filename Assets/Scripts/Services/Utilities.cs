using System.Collections.Generic;
using UnityEngine;
#if UNITY_WEBGL
    using Task = Cysharp.Threading.Tasks.UniTask;
    using TaskBool = Cysharp.Threading.Tasks.UniTask<bool>;
#else
    using Task = System.Threading.Tasks.Task;
    using TaskBool = System.Threading.Tasks.Task<bool>;
#endif


public static class Utilities
{
#if UNITY_WEBGL
    public static async Task Wait(int Time = 300)
    {
        await Task.Delay(Time, true);
    }
#else 
    public static async Task Wait(int Time = 300)
    {
        await  Task.Delay(Time);
    }
#endif

    public static async TaskBool IsWaitEndsFailure(int Time = 300)
    {
        await Wait(Time);
        if (!Application.isPlaying) return true;
        return false;
    }

    public static string GetObjectHierarchy(GameObject Target)
    {
        List<string> AllNames = new List<string>();
        AllNames.Add(Target.name);
        Transform TargetParent = Target.transform.parent;
        while (TargetParent != null)
        {
            AllNames.Add(TargetParent.name);
            TargetParent = TargetParent.parent;
        }
        var Builder = new System.Text.StringBuilder();
        for (int i = AllNames.Count; i >= 0; i --)
        {
            Builder.Append(AllNames[i]);
            if (i > 0) Builder.Append("=>");
        }
        return Builder.ToString();
    }
}