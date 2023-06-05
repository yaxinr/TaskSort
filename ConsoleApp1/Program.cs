using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApsTask
{
    public class Sorting
    {
        public static void SortTasks(WorkTask[] tasks)
        {
            var workTasks = tasks.Where(task => !task.isAdjust).ToArray();
            if (workTasks.Length == 0) return;

            foreach (var task in tasks.Where(task => task.prevTaskId > 0))
                foreach (var t in tasks.Where(t => t.Id == task.prevTaskId && t.OpId != task.OpId))
                {
                    foreach (var opTask in tasks)
                        if (opTask.OpId == task.OpId)
                        {
                            opTask.PrevOpId = t.OpId;
                        }
                    break;
                }
            foreach (var task in workTasks)
                foreach (var t in workTasks.Where(t => t != task && t.batchId == task.batchId && t.BatchOpIndex > task.BatchOpIndex && t.Pos < task.Pos))
                    t.Pos = task.Pos;
            List<WorkTask> sortedTasks = new List<WorkTask>(workTasks.Length);
            while (sortedTasks.Count < workTasks.Length)
            {
                var task = workTasks.Where(t => !sortedTasks.Contains(t))
                    .OrderByDescending(t => t.mustFirst)
                    .ThenByDescending(t => t.doing)
                    .ThenByDescending(t => t.mustNext)
                    .ThenBy(t => t.Pos)
                    .ThenBy(t => t.BatchOpIndex)
                    .FirstOrDefault();
                if (task == null) break;
                AddTask(task);
            }
            for (int i = 0; i < sortedTasks.Count; i++)
                sortedTasks[i].NewPos = (i + 1) * 10 + 1;
            foreach (var task in tasks)
                if (task.isAdjust)
                    foreach (var opTask in sortedTasks.Where(t => t.OpId == task.OpId))
                        task.NewPos = opTask.NewPos - 1;
            Array.Sort(tasks, (a, b) => a.NewPos - b.NewPos);
            for (int i = 0; i < tasks.Length; i++)
                tasks[i].NewPos = i + 1;
            return;

            void AddRelatedTasks(WorkTask task)
            {
                foreach (var task2 in workTasks)
                    if (task2.mustNext)
                    {
                        AddTask(task2);
                    }
                foreach (var task2 in workTasks)
                    if (task2.PrevOpId == task.OpId)
                    {
                        AddTask(task2);
                    }
                foreach (var task2 in workTasks)
                    if (task2.adjust == task.adjust)
                    {
                        AddTask(task2);
                    }
                foreach (var task2 in workTasks)
                    if (task2.batchId == task.batchId)
                    {
                        AddTask(task2);
                    }
            }
            void AddTask(WorkTask task)
            {
                if (!sortedTasks.Contains(task))
                {
                    sortedTasks.Add(task);
                    AddRelatedTasks(task);
                }
            }
        }
    }
    public class WorkTask
    {
        public int Pos;
        public int Id;
        public int OpId;
        public int NewPos;
        public string adjust;
        public int opIndexInBatch;
        public bool doing;
        public bool mustFirst;
        public bool mustNext;
        public bool isAdjust;
        public int batchId;
        public string nop;
        public int BatchOpIndex => int.TryParse(nop, out int i) ? i : 0;
        public int prevTaskId;
        public int PrevOpId;
        public WorkTask(int id, int opId, int pos, int batchId, string adjust, bool isAdjust = false, bool mustFirst = false, bool doing = false)
        {
            Id = id;
            OpId = opId;
            Pos = pos;
            //newPos = pos;
            this.adjust = adjust;
            this.mustFirst = mustFirst;
            this.isAdjust = isAdjust;
            this.doing = doing;
            this.batchId = batchId;
        }
        override public string ToString() => $"id={Id} {Pos}->{NewPos} {adjust}  mk{batchId} nop{nop} {(doing ? "doing" : "")} {(mustFirst ? "mustFirst" : "")} {(isAdjust ? "adjust" : "")} mks{OpId} prevOp={PrevOpId}";
    }
}
