using System;
using System.Collections.Generic;
using System.Linq;

namespace ApsTask
{
    public class Sorting
    {
        /*
        public static WorkTask[] GetSortedTasks(WorkTask[] tasks)
        {
            var workTasks = tasks.Where(task => !task.isAdjust).ToArray();
            foreach (var g in workTasks.GroupBy(t => t.batchId))
                if (g.Count() > 1)
                {
                    var ops = g.OrderBy(t => t.nop).ThenBy(t => t.Pos).ToArray();
                    for (int i = 0; i < ops.Length; i++)
                    {
                        WorkTask task = ops[i];
                        task.opIndexInBatch = i;
                        if (i > 0 && task.Pos <= ops[i - 1].Pos)
                            task.newPos = ops[i - 1].Pos + 1;
                    }
                }

            PrintList(workTasks, "sorted input:");
            List<WorkTask> sortedTasks = new List<WorkTask>();
            var tasksById = workTasks.ToDictionary(t => t.Id);

            foreach (var task in workTasks)
                if (task.prevTaskId > 0)
                {
                    if (tasksById.TryGetValue(task.prevTaskId, out WorkTask prevTask))
                        task.newPos = prevTask.newPos + 1;
                }

            foreach (var task in workTasks.Where(t => t.mustFirst))
                AddTask(task);
            //PrintList(sortedTasks, "sorted must first tasks:");
            foreach (var task in workTasks.Where(t => t.doing))
                AddTask(task);
            //PrintList(sortedTasks, "sorted doing tasks:");
            foreach (var task in workTasks.Where(t => t.mustNext))
                AddTask(task);
            foreach (var task in workTasks.OrderBy(t => t.newPos))
                AddTask(task);

            for (int i = 0; i < sortedTasks.Count; i++)
                sortedTasks[i].newPos = (i + 1) * 10 + 1;
            foreach (var task in tasks)
                if (task.isAdjust)
                    foreach (var opTask in sortedTasks.Where(t => t.opId == task.opId))
                        task.newPos = opTask.newPos - 1;
            //PrintList(sortedTasks, "sorted work tasks:");
            Array.Sort(tasks, (a, b) => a.newPos - b.newPos);
            //PrintList(tasks, "sorted tasks:");
            for (int i = 0; i < tasks.Length; i++)
                tasks[i].newPos = i + 1;
            return tasks;

            void AddRelatedTasks(WorkTask task)
            {
                foreach (var task2 in workTasks)
                    if (task2.adjust == task.adjust)
                    {
                        if (!sortedTasks.Contains(task2))
                            sortedTasks.Add(task2);
                    }
                foreach (var task2 in workTasks)
                    if (task2.batchId == task.batchId)
                    {
                        if (!sortedTasks.Contains(task2))
                            sortedTasks.Add(task2);
                    }
                foreach (var task2 in workTasks)
                    if (task2.prevTaskId == task.Id)
                    {
                        if (!sortedTasks.Contains(task2))
                            sortedTasks.Add(task2);
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
        */
        public static void SortTasks(WorkTask[] tasks)
        {
            var workTasks = tasks.Where(task => !task.isAdjust).ToArray();
            if (workTasks.Length == 0) return;
            List<WorkTask> sortedTasks = new List<WorkTask>();

            foreach (var task in workTasks.Where(t => t.mustFirst))
                AddTask(task);
            foreach (var task in workTasks.Where(t => t.doing))
                AddTask(task);
            foreach (var task in workTasks.Where(t => t.mustNext))
                AddTask(task);
            foreach (var task in workTasks.OrderBy(t => t.NewPos))
                AddTask(task);

            for (int i = 0; i < sortedTasks.Count; i++)
                sortedTasks[i].NewPos = (i + 1) * 10 + 1;
            foreach (var task in tasks)
                if (task.isAdjust)
                    foreach (var opTask in sortedTasks.Where(t => t.opId == task.opId))
                        task.NewPos = opTask.NewPos - 1;
            Array.Sort(tasks, (a, b) => a.NewPos - b.NewPos);
            for (int i = 0; i < tasks.Length; i++)
                tasks[i].NewPos = i + 1;
            return;

            void AddRelatedTasks(WorkTask task)
            {
                foreach (var task2 in workTasks)
                    if (task2.prevTaskId == task.Id)
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
        private int pos;
        public int Pos => pos;
        public int Id;
        public int opId;
        public int NewPos;
        public string adjust;
        public int opIndexInBatch;
        public bool doing;
        public bool mustFirst;
        public bool mustNext;
        public bool isAdjust;
        public int batchId;
        public string nop;
        public int prevTaskId;

        public WorkTask(int id, int opId, int pos, int batchId, string adjust, bool isAdjust = false, bool mustFirst = false, bool doing = false)
        {
            Id = id;
            this.opId = opId;
            this.pos = pos;
            //newPos = pos;
            this.adjust = adjust;
            this.mustFirst = mustFirst;
            this.isAdjust = isAdjust;
            this.doing = doing;
            this.batchId = batchId;
        }
        override public string ToString() => $"id={Id} {pos}->{NewPos} {adjust}  mk{batchId} nop{nop} {(doing ? "doing" : "")} {(mustFirst ? "mustFirst" : "")} {(isAdjust ? "adjust" : "")} mks{opId} prev={prevTaskId}";
    }
}
