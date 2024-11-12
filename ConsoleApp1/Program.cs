using System;
using System.Collections.Generic;
using System.Linq;

namespace ApsTask
{
    public class Sorting
    {
        public static void SortTasks(WorkTask[] tasks)
        {
            var lookup = tasks.ToLookup(t => t.OpId);
            foreach (var doingTask in tasks.Where(task => task.doing))
                foreach (var opTask in lookup[doingTask.OpId])
                    opTask.doing = true;
            var workTasks = tasks.Where(task => !task.isAdjust).ToArray();
            if (workTasks.Length == 0) return;
            foreach (var ts in workTasks.GroupBy(t => t.batchId))
            {
                var arr = ts.OrderBy(t => t.BatchOpIndex).ToArray();
                for (int i = 0; i < arr.Length; i++)
                    arr[i].IndexInBatch = i;
            }
            foreach (var nextTask in tasks.Where(task => task.prevTaskId > 0))
                foreach (var prevTask in tasks.Where(t => t.Id == nextTask.prevTaskId && t.OpId != nextTask.OpId))
                {
                    foreach (var nextOpTask in tasks.Where(t => t.OpId == nextTask.OpId))
                        nextOpTask.PrevOpId = prevTask.OpId;
                    break;
                }
            foreach (var task in workTasks)
                foreach (var t in workTasks.Where(t => t != task && t.batchId == task.batchId && t.BatchOpIndex > task.BatchOpIndex && t.Pos < task.Pos))
                    t.Pos = task.Pos;
            var sortedTasks = new List<WorkTask>(workTasks.Length);
            while (sortedTasks.Count < workTasks.Length)
            {
                var task = workTasks.Where(t => !sortedTasks.Contains(t))
                    .OrderByDescending(t => t.mustFirst)
                    .ThenByDescending(t => t.doing)
                    .ThenByDescending(t => t.mustNext)
                    .ThenBy(t => t.IndexInBatch)
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
                        AddTask(task2);
                bool adjustSelector(WorkTask t) => t.adjust == task.adjust && t.IndexInBatch == 0;
                foreach (var task2 in workTasks.Where(t => t.PrevOpId == task.OpId).OrderByDescending(adjustSelector))
                    AddTask(task2);
                foreach (var t in workTasks.Where(adjustSelector))
                    AddTask(t);
                foreach (var task2 in workTasks)
                    if (task2.batchId == task.batchId)
                        AddTask(task2);
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
        public int IndexInBatch;
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
